﻿using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

using BetterCms.Configuration;
using BetterCms.Core.DataAccess;
using BetterCms.Core.DataAccess.DataContext;
using BetterCms.Core.DataAccess.DataContext.Fetching;
using BetterCms.Core.Exceptions;
using BetterCms.Core.Security;
using BetterCms.Core.Services;
using BetterCms.Core.Services.Caching;
using BetterCms.Core.Services.Storage;
using BetterCms.Core.Web;

using BetterCms.Module.MediaManager.Models;
using BetterCms.Module.Root.Models;
using BetterCms.Module.Root.Mvc;

using Common.Logging;

using NHibernate;

namespace BetterCms.Module.MediaManager.Services
{
    internal class DefaultMediaFileService : IMediaFileService
    {
        private readonly IStorageService storageService;
        
        private readonly IRepository repository;

        private readonly IUnitOfWork unitOfWork;

        private readonly ICmsConfiguration configuration;

        private readonly IHttpContextAccessor httpContextAccessor;

        private readonly ISessionFactoryProvider sessionFactoryProvider;

        private readonly IMediaFileUrlResolver mediaFileUrlResolver;

        private readonly ISecurityService securityService;
        
        private readonly ICacheService cacheService;

        private readonly IAccessControlService accessControlService;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultMediaFileService" /> class.
        /// </summary>
        /// <param name="storageService">The storage service.</param>
        /// <param name="repository">The repository.</param>
        /// <param name="unitOfWork">The unit of work.</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="httpContextAccessor">The HTTP context accessor.</param>
        /// <param name="sessionFactoryProvider">The session factory provider.</param>
        /// <param name="mediaFileUrlResolver">The media file URL resolver.</param>
        /// <param name="securityService">The security service.</param>
        /// <param name="cacheService">The cache service.</param>
        /// <param name="accessControlService">The access control service.</param>
        public DefaultMediaFileService(IStorageService storageService, IRepository repository, IUnitOfWork unitOfWork,
            ICmsConfiguration configuration, IHttpContextAccessor httpContextAccessor, ISessionFactoryProvider sessionFactoryProvider,
            IMediaFileUrlResolver mediaFileUrlResolver, ISecurityService securityService, ICacheService cacheService,
            IAccessControlService accessControlService)
        {
            this.sessionFactoryProvider = sessionFactoryProvider;
            this.httpContextAccessor = httpContextAccessor;
            this.configuration = configuration;
            this.unitOfWork = unitOfWork;
            this.storageService = storageService;
            this.repository = repository;
            this.mediaFileUrlResolver = mediaFileUrlResolver;
            this.securityService = securityService;
            this.cacheService = cacheService;
            this.accessControlService = accessControlService;
        }

        public virtual void RemoveFile(Guid fileId, int version, bool doNotCheckVersion = false)
        {
            var file = repository
                .AsQueryable<MediaFile>(f => f.Id == fileId)
                .FetchMany(f => f.AccessRules)
                .FirstOrDefault();

            if (file == null)
            {
                throw new CmsException(string.Format("A file was not found by given id={0}", fileId));
            }

            try
            {
                if (file.IsUploaded.HasValue && file.IsUploaded.Value)
                {
                    Task.Factory
                        .StartNew(() => {})
                        .ContinueWith(task =>
                            {
                                storageService.RemoveObject(file.FileUri);
                            })
                        .ContinueWith(task =>
                            {
                                storageService.RemoveFolder(file.FileUri);
                            });
                }
            }
            finally
            {
                if (!doNotCheckVersion)
                {
                    file.Version = version;
                }
                file.AccessRules.ToList().ForEach(file.RemoveRule);
                repository.Delete(file);
                unitOfWork.Commit();   
            }
        }

        public virtual MediaFile UploadFile(MediaType type, Guid rootFolderId, string fileName, long fileLength, Stream fileStream, bool isTemporary = true, 
            string title = "", string description = "")
        {
            string folderName = CreateRandomFolderName();
            MediaFile file = new MediaFile();
            if (!rootFolderId.HasDefaultValue())
            {
                file.Folder = repository.AsProxy<MediaFolder>(rootFolderId);
            }
            file.Title = !string.IsNullOrEmpty(title) ? title : Path.GetFileName(fileName);
            if (!string.IsNullOrEmpty(description))
            {
                file.Description = description;
            }
            file.Type = type;
            file.OriginalFileName = fileName;
            file.OriginalFileExtension = Path.GetExtension(fileName);           
            file.Size = fileLength;
            file.FileUri = GetFileUri(type, folderName, fileName);
            file.PublicUrl = GetPublicFileUrl(type, folderName, fileName);
            file.IsTemporary = isTemporary;
            file.IsCanceled = false;
            file.IsUploaded = null;
            if (configuration.Security.AccessControlEnabled)
            {
                file.AddRule(new AccessRule { AccessLevel = AccessLevel.ReadWrite, Identity = securityService.CurrentPrincipalName });
            }

            unitOfWork.BeginTransaction();
            repository.Save(file);
            unitOfWork.Commit();

            Task fileUploadTask = UploadMediaFileToStorageAsync<MediaFile>(fileStream, file.FileUri, file.Id, media => { media.IsUploaded = true; }, media => { media.IsUploaded = false; }, false);
            fileUploadTask.ContinueWith(
                task =>
                    {
                        // During uploading progress Cancel action can by executed. Need to remove uploaded files from the storage.
                        ExecuteActionOnThreadSeparatedSessionWithNoConcurrencyTracking(
                            session =>
                                {
                                    var media = session.Get<MediaFile>(file.Id);
                                    if (media != null)
                                    {
                                        if (media.IsCanceled && media.IsUploaded.HasValue && media.IsUploaded.Value)
                                        {
                                            RemoveFile(media.Id, media.Version);
                                        }
                                    }
                                });
                    });

            fileUploadTask.Start();

            return file;
        }

        public MediaFile UploadFileWithStream(
            MediaType type,
            Guid rootFolderId,
            string fileName,
            long fileLength,
            Stream fileStream,
            bool waitForUploadResult = false,
            string title = "",
            string description = "")
        {
            string folderName = CreateRandomFolderName();
            MediaFile file = new MediaFile();
            if (!rootFolderId.HasDefaultValue())
            {
                file.Folder = repository.AsProxy<MediaFolder>(rootFolderId);
            }
            file.Title = !string.IsNullOrEmpty(title) ? title : Path.GetFileName(fileName);
            if (!string.IsNullOrEmpty(description))
            {
                file.Description = description;
            }
            file.Type = type;
            file.OriginalFileName = fileName;
            file.OriginalFileExtension = Path.GetExtension(fileName);
            file.Size = fileLength;
            file.FileUri = GetFileUri(type, folderName, fileName);
            file.PublicUrl = GetPublicFileUrl(type, folderName, fileName);
            file.IsTemporary = false;
            file.IsCanceled = false;
            file.IsUploaded = null;
            if (configuration.Security.AccessControlEnabled)
            {
                file.AddRule(new AccessRule { AccessLevel = AccessLevel.ReadWrite, Identity = securityService.CurrentPrincipalName });
            }

            unitOfWork.BeginTransaction();
            repository.Save(file);

            if (!waitForUploadResult)
            {
                unitOfWork.Commit();
            }

            if (waitForUploadResult)
            {
                UploadMediaFileToStorageSync(fileStream, file.FileUri, file, media => { media.IsUploaded = true; }, media => { media.IsUploaded = false; }, false);

                unitOfWork.Commit();
                Events.MediaManagerEvents.Instance.OnMediaFileUpdated(file);
            }
            else
            {
                Task fileUploadTask = UploadMediaFileToStorageAsync<MediaFile>(fileStream, file.FileUri, file.Id, media => { media.IsUploaded = true; }, media => { media.IsUploaded = false; }, false);
                fileUploadTask.ContinueWith(
                    task =>
                    {
                        // During uploading progress Cancel action can by executed. Need to remove uploaded files from the storage.
                        ExecuteActionOnThreadSeparatedSessionWithNoConcurrencyTracking(
                            session =>
                            {
                                var media = session.Get<MediaFile>(file.Id);
                                if (media != null)
                                {
                                    if (media.IsCanceled && media.IsUploaded.HasValue && media.IsUploaded.Value)
                                    {
                                        RemoveFile(media.Id, media.Version);
                                    }
                                }
                            });
                    });

                fileUploadTask.Start();
            }

            return file;
        }

        public virtual string CreateRandomFolderName()
        {
            return Guid.NewGuid().ToString().Replace("-", string.Empty);
        }

        public virtual Uri GetFileUri(MediaType type, string folderName, string fileName)
        {
            string contentRoot;
            if (configuration.Security.AccessControlEnabled && type == MediaType.File)
            {
                contentRoot = configuration.Storage.SecuredContentRoot;
            }
            else
            {
                contentRoot = configuration.Storage.ContentRoot;
            }
            return new Uri(Path.Combine(GetContentRoot(contentRoot), type.ToString().ToLower(), folderName, fileName));
        }

        public virtual string GetPublicFileUrl(MediaType type, string folderName, string fileName)
        {
            string fullPath = Path.Combine(
                GetContentPublicRoot(configuration.Storage.PublicContentUrlRoot),
                Path.Combine(type.ToString().ToLower(), folderName, fileName));

            string absoluteUri = new Uri(fullPath).AbsoluteUri;

            return HttpUtility.UrlDecode(absoluteUri);
        }

        public void UploadMediaFileToStorageSync<TMedia>(
            Stream sourceStream,
            Uri fileUri,
            TMedia media,
            Action<TMedia> updateMediaAfterUpload,
            Action<TMedia> updateMediaAfterFail,
            bool ignoreAccessControl) where TMedia : MediaFile
        {
            using (var stream = new MemoryStream())
            {
                sourceStream.Seek(0, SeekOrigin.Begin);
                sourceStream.CopyTo(stream);

                try
                {
                    ExecuteFileUpload(fileUri, stream, ignoreAccessControl);
                    updateMediaAfterUpload(media);
                }
                catch (Exception exc)
                {
                    updateMediaAfterFail(media);

                    // Log exception
                    LogManager.GetCurrentClassLogger().Error("Failed to upload file.",  exc);
                }
                finally
                {
                    stream.Close();
                    stream.Dispose();
                }
            }
        }

        public Task UploadMediaFileToStorageAsync<TMedia>(Stream sourceStream,
            Uri fileUri,
            Guid mediaId,
            Action<TMedia> updateMediaAfterUpload,
            Action<TMedia> updateMediaAfterFail,
            bool ignoreAccessControl)
            where TMedia : MediaFile
        {

            var stream = new MemoryStream();

            sourceStream.Seek(0, SeekOrigin.Begin);
            sourceStream.CopyTo(stream);

            Action<ISession> failedResultAction = session =>
            {
                var media = session.Get<TMedia>(mediaId);
                if (media != null)
                {
                    updateMediaAfterFail(media);
                    session.SaveOrUpdate(media);
                    session.Flush();
                    Events.MediaManagerEvents.Instance.OnMediaFileUpdated(media);
                }
            };

            Action<ISession> completedAction = session =>
            {
                var media = session.Get<TMedia>(mediaId);
                if (media != null)
                {
                    updateMediaAfterUpload(media);
                    session.SaveOrUpdate(media);
                    session.Flush();
                    Events.MediaManagerEvents.Instance.OnMediaFileUpdated(media);
                }
            };

            Action finalAction = () =>
            {
                stream.Close();
                stream.Dispose();
            };

            var task = new Task(() => ExecuteFileUpload(fileUri, stream, ignoreAccessControl));
            task
             .ContinueWith(
                t =>
                {
                    if (t.Exception == null)
                    {
                        ExecuteActionOnThreadSeparatedSessionWithNoConcurrencyTracking(completedAction);
                    }
                    else
                    {
                        ExecuteActionOnThreadSeparatedSessionWithNoConcurrencyTracking(failedResultAction);

                        // Log exception
                        LogManager.GetCurrentClassLogger().Error("Failed to upload file.", t.Exception.Flatten());
                    }
                })
             .ContinueWith(t => finalAction());

            return task;
        }

        private void ExecuteFileUpload(Uri fileUri, Stream stream, bool ignoreAccessControl)
        {
            var upload = new UploadRequest();
            upload.CreateDirectory = true;
            upload.Uri = fileUri;
            upload.InputStream = stream;
            upload.IgnoreAccessControl = ignoreAccessControl;

            storageService.UploadObject(upload);
        }

        private void ExecuteActionOnThreadSeparatedSessionWithNoConcurrencyTracking(Action<ISession> work)
        {
            using (var session = sessionFactoryProvider.OpenSession(false))
            {
                try
                {
                    lock (this)
                    {
                        work(session);
                    }
                }
                finally
                {
                    session.Close();
                }
            }
        }

        /// <summary>
        /// Gets the content root.
        /// </summary>
        /// <returns>Root path.</returns>
        private string GetContentRoot(string rootPath)
        {
            if (configuration.Storage.ServiceType == StorageServiceType.FileSystem && VirtualPathUtility.IsAppRelative(rootPath))
            {
                return httpContextAccessor.MapPath(rootPath);
            }

            return rootPath;
        }

        private string GetContentPublicRoot(string rootPath)
        {
            if (configuration.Storage.ServiceType == StorageServiceType.FileSystem && VirtualPathUtility.IsAppRelative(rootPath))
            {
                return httpContextAccessor.MapPublicPath(rootPath);
            }

            return rootPath;
        }

        public string GetDownloadFileUrl(MediaType type, Guid id, string fileUrl)
        {
            if (type == MediaType.Image || !configuration.Security.AccessControlEnabled || !storageService.SecuredUrlsEnabled)
            {
                return fileUrl;
            }

            return mediaFileUrlResolver.GetMediaFileFullUrl(id, fileUrl);
        }

        public void SaveMediaFile(MediaFile file)
        {
            unitOfWork.BeginTransaction();
            repository.Save(file);
            unitOfWork.Commit();
        }
    }
}