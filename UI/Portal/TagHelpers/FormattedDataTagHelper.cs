using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.Runtime.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Portal.TagHelpers {
    // You may need to install the Microsoft.AspNetCore.Razor.Runtime package into your project
    [HtmlTargetElement("formatted-data")]
    public class FormattedDataTagHelper : TagHelper {

        [HtmlAttributeName("asp-for")]
        public ModelExpression Data { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output) {
            output.TagName = "p";
            output.Content.SetHtmlContent(Data.Model?.ToString().Trim().Replace("\r", "<br />") ?? "");
        }
    }

    [HtmlTargetElement("display-for")]
    public class DisplayForTagHelper : TagHelper {

        [HtmlAttributeName("asp-for")]
        public ModelExpression Data { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output) {
            output.TagName = "";
            output.Content.SetHtmlContent(Data.Model?.ToString().Trim() ?? "");
        }
    }
}
