using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics;

namespace WhatIEatAPI.Helpers
{
    public static class HTMLHelpers
    {
        public static IHtmlContent ProcessName(this IHtmlHelper htmlHelper)
            => new HtmlString(Process.GetCurrentProcess().ProcessName);

        public static IHtmlContent HelloWorldHTMLString(this IHtmlHelper htmlHelper)
            => new HtmlString("<strong>Hello World</strong>");

        public static IHtmlContent IsDebug(this IHtmlHelper htmlHelper)
        {
            #if DEBUG
                return new HtmlString("DEBUG - IISExpress");
            #else
                return new HtmlString("RELEASE - IIS");
            #endif
        }
    }
}
