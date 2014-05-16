﻿using System;
using System.Collections.Generic;
using System.Linq;

using BetterCms.Core.DataAccess;
using BetterCms.Core.DataAccess.DataContext;
using BetterCms.Core.DataContracts;
using BetterCms.Core.Exceptions.Api;
using BetterCms.Core.Exceptions.DataTier;
using BetterCms.Module.MediaManager.Models;
using BetterCms.Module.Root.Mvc;

using NHibernate.Linq;

using ServiceStack.ServiceInterface;

namespace BetterCms.Module.Api.Operations.MediaManager.Folders.Folder
{
    /// <summary>
    /// Default folder CRUD service.
    /// </summary>
    public class FolderService : Service, IFolderService
    {
        /// <summary>
        /// The repository.
        /// </summary>
        private readonly IRepository repository;

        /// <summary>
        /// The unit of work.
        /// </summary>
        private readonly IUnitOfWork unitOfWork;

        /// <summary>
        /// Initializes a new instance of the <see cref="FolderService" /> class.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="unitOfWork">The unit of work.</param>
        public FolderService(IRepository repository, IUnitOfWork unitOfWork)
        {
            this.repository = repository;
            this.unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Gets the specified folder.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>
        ///   <c>GetFolderRequest</c> with an folder.
        /// </returns>
        public GetFolderResponse Get(GetFolderRequest request)
        {
            var model =
                repository.AsQueryable<MediaFolder>(media => media.Id == request.FolderId && media.ContentType == Module.MediaManager.Models.MediaContentType.Folder)
                    .Select(
                        media =>
                        new FolderModel
                            {
                                Id = media.Id,
                                Version = media.Version,
                                CreatedBy = media.CreatedByUser,
                                CreatedOn = media.CreatedOn,
                                LastModifiedBy = media.ModifiedByUser,
                                LastModifiedOn = media.ModifiedOn,
                                Title = media.Title,
                                IsArchived = media.IsArchived,
                            })
                    .FirstOne();

            return new GetFolderResponse { Data = model };
        }

        /// <summary>
        /// Replaces the folder or if it doesn't exist, creates it.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>
        ///   <c>PutFolderResponse</c> with a folder id.
        /// </returns>
        public PutFolderResponse Put(PutFolderRequest request)
        {
            IEnumerable<MediaFolder> parentFolderFuture = null;
            if (request.Data.ParentFolderId.HasValue)
            {
                parentFolderFuture = repository.AsQueryable<MediaFolder>()
                    .Where(c => c.Id == request.Data.ParentFolderId.Value && !c.IsDeleted)
                    .ToFuture();
            }

            var mediaFolder = repository.AsQueryable<MediaFolder>()
                .Fetch(media => media.Folder)
                .Distinct()
                .ToFuture()
                .FirstOrDefault(folder => folder.Id == request.FolderId);

            MediaFolder parentFolder = null;
            if (parentFolderFuture != null)
            {
                parentFolder = parentFolderFuture.First();
                if (parentFolder.Type != (Module.MediaManager.Models.MediaType)(int)request.Data.Type)
                {
                    throw new CmsApiValidationException("Parent folder type does not match to this folder type.");
                }
            }

            var createFolder = mediaFolder == null;
            if (createFolder)
            {
                mediaFolder = new MediaFolder
                                  {
                                      Id = request.FolderId.GetValueOrDefault(),
                                      ContentType = Module.MediaManager.Models.MediaContentType.Folder,
                                      Type = (Module.MediaManager.Models.MediaType)(int)request.Data.Type
                                  };
            }
            else if (request.Data.Version > 0)
            {
                mediaFolder.Version = request.Data.Version;
            }

            unitOfWork.BeginTransaction();

            mediaFolder.Title = request.Data.Title;
            mediaFolder.Folder = parentFolder;

            mediaFolder.PublishedOn = DateTime.Now;
            mediaFolder.IsArchived = request.Data.IsArchived;

            var archivedMedias = new List<Media> { mediaFolder };
            var unarchivedMedias = new List<Media> { mediaFolder };
            if (request.Data.IsArchived)
            {
                ArchiveSubMedias(mediaFolder, archivedMedias);
            }
            else
            {
                UnarchiveSubMedias(mediaFolder, unarchivedMedias);
            }

            repository.Save(mediaFolder);

            unitOfWork.Commit();

            // Fire events.
            foreach (var archivedMedia in archivedMedias.Distinct())
            {
                Events.MediaManagerEvents.Instance.OnMediaArchived(archivedMedia);
            }

            foreach (var archivedMedia in unarchivedMedias.Distinct())
            {
                Events.MediaManagerEvents.Instance.OnMediaUnarchived(archivedMedia);
            }

            if (createFolder)
            {
                Events.MediaManagerEvents.Instance.OnMediaFolderCreated(mediaFolder);
            }
            else
            {
                Events.MediaManagerEvents.Instance.OnMediaFolderUpdated(mediaFolder);
            }

            return new PutFolderResponse { Data = mediaFolder.Id };
        }

        /// <summary>
        /// Deletes the specified folder.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>
        ///   <c>DeleteFolderResponse</c> with success status.
        /// </returns>
        public DeleteFolderResponse Delete(DeleteFolderRequest request)
        {
            if (request.Data == null || request.FolderId.HasDefaultValue())
            {
                return new DeleteFolderResponse { Data = false };
            }

            var itemToDelete = repository.AsQueryable<MediaFolder>().Where(p => p.Id == request.FolderId).FirstOne();

            if (request.Data.Version > 0 && itemToDelete.Version != request.Data.Version)
            {
                throw new ConcurrentDataException(itemToDelete);
            }

            DeleteMedias(itemToDelete);

            unitOfWork.Commit();

            Events.MediaManagerEvents.Instance.OnMediaFolderDeleted(itemToDelete);

            return new DeleteFolderResponse { Data = true };
        }

        /// <summary>
        /// Deletes the medias.
        /// </summary>
        /// <param name="media">The media.</param>
        private void DeleteMedias(Media media)
        {
            if (media.MediaTags != null)
            {
                foreach (var mediaTag in media.MediaTags)
                {
                    repository.Delete(mediaTag);
                }
            }

            if (media is MediaFile)
            {
                MediaFile file = (MediaFile)media;
                if (file.AccessRules != null)
                {
                    var rules = file.AccessRules.ToList();
                    rules.ForEach(file.RemoveRule);
                }
            }

            repository.Delete(media);

            var subItems = repository.AsQueryable<Media>().Where(m => !m.IsDeleted && m.Folder != null && m.Folder.Id == media.Id).ToList();
            foreach (var item in subItems)
            {
                DeleteMedias(item);
            }
        }

        /// <summary>
        /// Archives the sub medias.
        /// </summary>
        /// <param name="media">The media.</param>
        /// <param name="archivedMedias">The archived medias.</param>
        private void ArchiveSubMedias(IEntity media, List<Media> archivedMedias)
        {
            var subItems = repository.AsQueryable<Media>().Where(m => m.Folder != null && m.Folder.Id == media.Id).ToList();
            foreach (var subItem in subItems)
            {
                if (!subItem.IsArchived)
                {
                    subItem.IsArchived = true;
                    archivedMedias.Add(subItem);

                    repository.Save(subItem);
                }

                ArchiveSubMedias(subItem, archivedMedias);
            }
        }

        /// <summary>
        /// Unarchives the sub medias.
        /// </summary>
        /// <param name="media">The media.</param>
        /// <param name="unarchivedMedias">The unarchived medias.</param>
        private void UnarchiveSubMedias(IEntity media, List<Media> unarchivedMedias)
        {
            var subItems = repository.AsQueryable<Media>().Where(m => m.Folder != null && m.Folder.Id == media.Id).ToList();
            foreach (var subItem in subItems)
            {
                if (subItem.IsArchived)
                {
                    subItem.IsArchived = false;
                    unarchivedMedias.Add(subItem);

                    repository.Save(subItem);
                }

                UnarchiveSubMedias(subItem, unarchivedMedias);
            }
        }
    }
}