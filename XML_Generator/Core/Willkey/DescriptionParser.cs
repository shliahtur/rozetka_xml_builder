
using AngleSharp.Html.Dom;

namespace Parser.Core.Habra
{
    class DescriptionParser : IParser<string>
    {
        public string Parse(IHtmlDocument document)
        {
           
            var upload = document.GetElementsByClassName("woocommerce-product-details__short-description");

            string content = "error";
            if (upload.Length > 0)
            {
                content = upload[0].InnerHtml;
            }

            return content;
        }
    }
}
