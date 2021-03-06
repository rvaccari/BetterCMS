﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18449
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BetterCms.Module.MediaManager.Views.History
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Web;
    using System.Web.Helpers;
    using System.Web.Mvc;
    using System.Web.Mvc.Ajax;
    using System.Web.Mvc.Html;
    using System.Web.Routing;
    using System.Web.Security;
    using System.Web.UI;
    using System.Web.WebPages;
    
    #line 1 "..\..\Views\History\MediaHistory.cshtml"
    using BetterCms.Module.MediaManager.Command.History.GetMediaHistory;
    
    #line default
    #line hidden
    
    #line 2 "..\..\Views\History\MediaHistory.cshtml"
    using BetterCms.Module.MediaManager.Content.Resources;
    
    #line default
    #line hidden
    
    #line 3 "..\..\Views\History\MediaHistory.cshtml"
    using BetterCms.Module.MediaManager.Controllers;
    
    #line default
    #line hidden
    
    #line 4 "..\..\Views\History\MediaHistory.cshtml"
    using BetterCms.Module.MediaManager.Models.Enum;
    
    #line default
    #line hidden
    
    #line 5 "..\..\Views\History\MediaHistory.cshtml"
    using BetterCms.Module.MediaManager.ViewModels.History;
    
    #line default
    #line hidden
    
    #line 6 "..\..\Views\History\MediaHistory.cshtml"
    using BetterCms.Module.Root.Content.Resources;
    
    #line default
    #line hidden
    
    #line 7 "..\..\Views\History\MediaHistory.cshtml"
    using BetterCms.Module.Root.Mvc;
    
    #line default
    #line hidden
    
    #line 8 "..\..\Views\History\MediaHistory.cshtml"
    using BetterCms.Module.Root.Mvc.Grids.Extensions;
    
    #line default
    #line hidden
    
    #line 9 "..\..\Views\History\MediaHistory.cshtml"
    using BetterCms.Module.Root.Mvc.Grids.TableRenderers;
    
    #line default
    #line hidden
    
    #line 10 "..\..\Views\History\MediaHistory.cshtml"
    using BetterCms.Module.Root.Mvc.Helpers;
    
    #line default
    #line hidden
    
    #line 11 "..\..\Views\History\MediaHistory.cshtml"
    using Microsoft.Web.Mvc;
    
    #line default
    #line hidden
    
    #line 12 "..\..\Views\History\MediaHistory.cshtml"
    using MvcContrib.UI.Grid;
    
    #line default
    #line hidden
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/History/MediaHistory.cshtml")]
    public partial class MediaHistory : System.Web.Mvc.WebViewPage<MediaHistoryViewModel>
    {

#line 15 "..\..\Views\History\MediaHistory.cshtml"
public System.Web.WebPages.HelperResult PreviewLink(MediaHistoryItem item)
{
#line default
#line hidden
return new System.Web.WebPages.HelperResult(__razor_helper_writer => {

#line 16 "..\..\Views\History\MediaHistory.cshtml"
 


#line default
#line hidden
WriteLiteralTo(__razor_helper_writer, "    <a");

WriteLiteralTo(__razor_helper_writer, " class=\"bcms-icn-preview\"");

WriteLiteralTo(__razor_helper_writer, " data-id=\"");


#line 17 "..\..\Views\History\MediaHistory.cshtml"
           WriteTo(__razor_helper_writer, item.Id);


#line default
#line hidden
WriteLiteralTo(__razor_helper_writer, "\"");

WriteLiteralTo(__razor_helper_writer, ">");


#line 17 "..\..\Views\History\MediaHistory.cshtml"
                     WriteTo(__razor_helper_writer, RootGlobalization.Button_Preview);


#line default
#line hidden
WriteLiteralTo(__razor_helper_writer, "</a>\r\n");


#line 18 "..\..\Views\History\MediaHistory.cshtml"


#line default
#line hidden
});

#line 18 "..\..\Views\History\MediaHistory.cshtml"
}
#line default
#line hidden

#line 20 "..\..\Views\History\MediaHistory.cshtml"
public System.Web.WebPages.HelperResult DownloadLink(MediaHistoryItem item)
{
#line default
#line hidden
return new System.Web.WebPages.HelperResult(__razor_helper_writer => {

#line 21 "..\..\Views\History\MediaHistory.cshtml"
 


#line default
#line hidden
WriteLiteralTo(__razor_helper_writer, "    <a");

WriteLiteralTo(__razor_helper_writer, " class=\"bcms-icn-download\"");

WriteLiteralTo(__razor_helper_writer, " data-id=\"");


#line 22 "..\..\Views\History\MediaHistory.cshtml"
            WriteTo(__razor_helper_writer, item.Id);


#line default
#line hidden
WriteLiteralTo(__razor_helper_writer, "\"");

WriteLiteralTo(__razor_helper_writer, " data-version=\"");


#line 22 "..\..\Views\History\MediaHistory.cshtml"
                                    WriteTo(__razor_helper_writer, item.Version);


#line default
#line hidden
WriteLiteralTo(__razor_helper_writer, "\"");

WriteLiteralTo(__razor_helper_writer, ">");


#line 22 "..\..\Views\History\MediaHistory.cshtml"
                                                   WriteTo(__razor_helper_writer, MediaGlobalization.MediaManager_ButtonDownload);


#line default
#line hidden
WriteLiteralTo(__razor_helper_writer, "</a>\r\n");


#line 23 "..\..\Views\History\MediaHistory.cshtml"


#line default
#line hidden
});

#line 23 "..\..\Views\History\MediaHistory.cshtml"
}
#line default
#line hidden

#line 25 "..\..\Views\History\MediaHistory.cshtml"
public System.Web.WebPages.HelperResult RestoreLink(MediaHistoryItem item)
{
#line default
#line hidden
return new System.Web.WebPages.HelperResult(__razor_helper_writer => {

#line 26 "..\..\Views\History\MediaHistory.cshtml"
 
    if (item.CanCurrentUserRestoreIt)
    {


#line default
#line hidden
WriteLiteralTo(__razor_helper_writer, "    <a");

WriteLiteralTo(__razor_helper_writer, " class=\"bcms-icn-restore\"");

WriteLiteralTo(__razor_helper_writer, " data-id=\"");


#line 29 "..\..\Views\History\MediaHistory.cshtml"
           WriteTo(__razor_helper_writer, item.Id);


#line default
#line hidden
WriteLiteralTo(__razor_helper_writer, "\"");

WriteLiteralTo(__razor_helper_writer, " data-version=\"");


#line 29 "..\..\Views\History\MediaHistory.cshtml"
                                   WriteTo(__razor_helper_writer, item.Version);


#line default
#line hidden
WriteLiteralTo(__razor_helper_writer, "\"");

WriteLiteralTo(__razor_helper_writer, ">");


#line 29 "..\..\Views\History\MediaHistory.cshtml"
                                                  WriteTo(__razor_helper_writer, RootGlobalization.Button_Restore);


#line default
#line hidden
WriteLiteralTo(__razor_helper_writer, "</a>\r\n");


#line 30 "..\..\Views\History\MediaHistory.cshtml"
    }
    else
    {


#line default
#line hidden
WriteLiteralTo(__razor_helper_writer, "    ");

WriteLiteralTo(__razor_helper_writer, "&nbsp;\r\n");


#line 34 "..\..\Views\History\MediaHistory.cshtml"
    }


#line default
#line hidden
});

#line 35 "..\..\Views\History\MediaHistory.cshtml"
}
#line default
#line hidden

#line 37 "..\..\Views\History\MediaHistory.cshtml"
public System.Web.WebPages.HelperResult DisplayStatus(string statusName, MediaHistoryStatus status)
{
#line default
#line hidden
return new System.Web.WebPages.HelperResult(__razor_helper_writer => {

#line 38 "..\..\Views\History\MediaHistory.cshtml"
 
    switch (status)
    {
        case MediaHistoryStatus.Archived:


#line default
#line hidden
WriteLiteralTo(__razor_helper_writer, "    ");

WriteLiteralTo(__razor_helper_writer, "<div class=\"bcms-archived-state\">");


#line 42 "..\..\Views\History\MediaHistory.cshtml"
         WriteTo(__razor_helper_writer, statusName);


#line default
#line hidden
WriteLiteralTo(__razor_helper_writer, "</div>\r\n");


#line 43 "..\..\Views\History\MediaHistory.cshtml"

            break;

        case MediaHistoryStatus.Active:


#line default
#line hidden
WriteLiteralTo(__razor_helper_writer, "    ");

WriteLiteralTo(__razor_helper_writer, "<div class=\"bcms-published-state\">");


#line 47 "..\..\Views\History\MediaHistory.cshtml"
          WriteTo(__razor_helper_writer, statusName);


#line default
#line hidden
WriteLiteralTo(__razor_helper_writer, "</div>\r\n");


#line 48 "..\..\Views\History\MediaHistory.cshtml"
        
            break;

        default:


#line default
#line hidden
WriteLiteralTo(__razor_helper_writer, "    ");

WriteLiteralTo(__razor_helper_writer, "statusName\r\n");


#line 53 "..\..\Views\History\MediaHistory.cshtml"
            break;
    }


#line default
#line hidden
});

#line 55 "..\..\Views\History\MediaHistory.cshtml"
}
#line default
#line hidden

        public MediaHistory()
        {
        }
        public override void Execute()
        {
WriteLiteral("\r\n");

WriteLiteral("\r\n");

WriteLiteral("\r\n");

WriteLiteral("\r\n");

WriteLiteral("\r\n");

            
            #line 57 "..\..\Views\History\MediaHistory.cshtml"
  
    Action<ColumnBuilder<MediaHistoryItem>> columns = column =>
    {
        column.For(f => PreviewLink(f))
               .Encode(false)
               .Named("&nbsp;")
               .Sortable(false)
               .HeaderAttributes(@style => "width: 40px; padding: 10px;", @class => "bcms-tables-nohover");

        column.For(m => m.PublishedOn.ToFormattedDateString())
               .Named(MediaGlobalization.MediaHistory_Column_PublishedOn)
               .SortColumnName("PublishedOn")
               .HeaderAttributes(@style => "width: 95px;")
               .Attributes(@style => "width: 100px;");

        column.For(m => m.PublishedByUser)
               .Named(MediaGlobalization.MediaHistory_Column_PublishedByUser)
               .SortColumnName("PublishedByUser")
               .HeaderAttributes(@style => "width: 200px;")
               .Attributes(@style => "width: 200px;");

        column.For(m => m.ArchivedOn.ToFormattedDateString())
               .Named(MediaGlobalization.MediaHistory_Column_ArchivedOn)
               .SortColumnName("ArchivedOn")
               .HeaderAttributes(@style => "width: 100px;")
               .Attributes(@style => "width: 100px;");

        column.For(m => m.DisplayedFor.ToFormatedTimeString())
               .Named(MediaGlobalization.MediaHistory_Column_DisplayedFor)
               .SortColumnName("DisplayedFor")
               .HeaderAttributes(@style => "width: 90px;")
               .Attributes(@style => "width: 90px;");

        column.For(m => DisplayStatus(m.StatusName, m.Status))
               .Named(MediaGlobalization.MediaHistory_Column_Status)
               .SortColumnName("StatusName")
               .Encode(false)
               .HeaderAttributes(@style => "width: 90px;")
               .Attributes(@style => "width: 90px;");

        column.For(f => DownloadLink(f))
               .Encode(false)
               .Named("&nbsp;")
               .Sortable(false)
               .HeaderAttributes(@style => "width: 57px;", @class => "bcms-tables-nohover")
               .Attributes(@style => "width: 57px;");

        column.For(f => RestoreLink(f))
               .Encode(false)
               .Named("&nbsp;")
               .Sortable(false)
               .HeaderAttributes(@style => "width: 115px;", @class => "bcms-tables-nohover")
               .Attributes(@style => "width: 89px;");
    };

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n<div");

WriteLiteral(" class=\"bcms-scroll-window\"");

WriteLiteral(">\r\n\r\n    <div");

WriteLiteral(" class=\"bcms-history-preview-holder\"");

WriteLiteral(">\r\n        <div");

WriteLiteral(" class=\"bcms-history-preview\"");

WriteLiteral(">\r\n            <div");

WriteLiteral(" id=\"bcms-history-preview\"");

WriteLiteral(" style=\"height: 100%\"");

WriteLiteral(">\r\n");

WriteLiteral("                ");

            
            #line 118 "..\..\Views\History\MediaHistory.cshtml"
           Write(Html.TabbedContentMessagesBox());

            
            #line default
            #line hidden
WriteLiteral("\r\n                <div");

WriteLiteral(" class=\"bcms-history-info\"");

WriteLiteral(" style=\"display: block;\"");

WriteLiteral(">");

            
            #line 119 "..\..\Views\History\MediaHistory.cshtml"
                                                                  Write(MediaGlobalization.MediaHistory_SelectVersionToPreviewMessage);

            
            #line default
            #line hidden
WriteLiteral("</div>\r\n                <div");

WriteLiteral(" class=\"bcms-history-content\"");

WriteLiteral("></div>\r\n            </div>\r\n        </div>\r\n    </div>\r\n\r\n    <div");

WriteLiteral(" class=\"bcms-history-table-holder\"");

WriteLiteral(">\r\n\r\n");

            
            #line 127 "..\..\Views\History\MediaHistory.cshtml"
        
            
            #line default
            #line hidden
            
            #line 127 "..\..\Views\History\MediaHistory.cshtml"
         using (Html.BeginForm<HistoryController>(f => f.MediaHistory((GetMediaHistoryRequest)null), FormMethod.Post, new { @id = "bcms-pagecontenthistory-form", @class = "bcms-ajax-form" }))
        {                        
            
            
            #line default
            #line hidden
            
            #line 129 "..\..\Views\History\MediaHistory.cshtml"
       Write(Html.HiddenGridOptions(Model.GridOptions));

            
            #line default
            #line hidden
            
            #line 129 "..\..\Views\History\MediaHistory.cshtml"
                                                      
            
            
            #line default
            #line hidden
            
            #line 130 "..\..\Views\History\MediaHistory.cshtml"
       Write(Html.HiddenFor(model => model.MediaId));

            
            #line default
            #line hidden
            
            #line 130 "..\..\Views\History\MediaHistory.cshtml"
                                                   


            
            #line default
            #line hidden
WriteLiteral("            <div");

WriteLiteral(" class=\"bcms-history-table-top\"");

WriteLiteral(">\r\n                <div");

WriteLiteral(" class=\"bcms-large-titles\"");

WriteLiteral(">");

            
            #line 133 "..\..\Views\History\MediaHistory.cshtml"
                                          Write(MediaGlobalization.MediaHistory_OlderVersions);

            
            #line default
            #line hidden
WriteLiteral("</div>\r\n                <div");

WriteLiteral(" class=\"bcms-search-block\"");

WriteLiteral(">\r\n");

WriteLiteral("                    ");

            
            #line 135 "..\..\Views\History\MediaHistory.cshtml"
               Write(Html.TextBoxFor(m => m.SearchQuery, new { @class = "bcms-editor-field-box", @placeholder = RootGlobalization.WaterMark_Search }));

            
            #line default
            #line hidden
WriteLiteral("\r\n                    <div");

WriteLiteral(" class=\"bcms-btn-search\"");

WriteLiteral(" id=\"bcms-pagecontenthistory-search-btn\"");

WriteLiteral(">");

            
            #line 136 "..\..\Views\History\MediaHistory.cshtml"
                                                                                    Write(RootGlobalization.Button_Search);

            
            #line default
            #line hidden
WriteLiteral("</div>\r\n                </div>\r\n            </div>                        \r\n");

            
            #line 139 "..\..\Views\History\MediaHistory.cshtml"
            
            
            #line default
            #line hidden
            
            #line 139 "..\..\Views\History\MediaHistory.cshtml"
       Write(Html.Grid(Model.Items).Sort(Model.GridOptions).Columns(columns).Attributes(@class => "bcms-tables bcms-history-grid").RenderUsing(new ScrollableEditableHtmlTableGridRenderer<MediaHistoryItem>()
                                                                                                                                 {
                                                                                                                                     InternalTableCssClass = "bcms-history-cell"
                                                                                                                                 }));

            
            #line default
            #line hidden
            
            #line 142 "..\..\Views\History\MediaHistory.cshtml"
                                                                                                                                   
            
            
            #line default
            #line hidden
            
            #line 143 "..\..\Views\History\MediaHistory.cshtml"
       Write(Html.HiddenSubmit());

            
            #line default
            #line hidden
            
            #line 143 "..\..\Views\History\MediaHistory.cshtml"
                                
        }

            
            #line default
            #line hidden
WriteLiteral("    </div>\r\n</div>\r\n\r\n<script");

WriteLiteral(" type=\"text/html\"");

WriteLiteral(" id=\"bcms-history-preview-template\"");

WriteLiteral(">\r\n    <iframe src=\"\" style=\"width: 100%; height: 100%; border: 0px;\"></iframe>\r\n" +
"</script>\r\n");

        }
    }
}
#pragma warning restore 1591
