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

namespace BetterCms.Module.Root.Views.Sidebar
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
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/Sidebar/Container.cshtml")]
    public partial class Container : System.Web.Mvc.WebViewPage<BetterCms.Module.Root.Models.Sidebar.SidebarContainerViewModel>
    {
        public Container()
        {
        }
        public override void Execute()
        {
WriteLiteral("<div");

WriteLiteral(" id=\"bcms-sidemenu\"");

WriteLiteral(" class=\"bcms-sidemenu bcms-layer\"");

WriteLiteral(" style=\"display: none\"");

WriteLiteral(">\r\n");

WriteLiteral("    ");

            
            #line 3 "..\..\Views\Sidebar\Container.cshtml"
Write(Html.Partial("Side", Model.SideProjections));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("    ");

            
            #line 4 "..\..\Views\Sidebar\Container.cshtml"
Write(Html.Partial("Header", Model.HeaderProjections));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("    ");

            
            #line 5 "..\..\Views\Sidebar\Container.cshtml"
Write(Html.Partial("Body", Model.BodyProjections));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("    ");

            
            #line 6 "..\..\Views\Sidebar\Container.cshtml"
Write(Html.Partial("Footer", Model.Version));

            
            #line default
            #line hidden
WriteLiteral("\r\n</div>\r\n");

        }
    }
}
#pragma warning restore 1591
