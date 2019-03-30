
using AngleSharp.Html.Dom;
using System.Collections.Generic;
using System.Linq;

namespace Parser.Core.Habra
{
    class PicturesParser : IParser<string[]>
    {
        public string[] Parse(IHtmlDocument document)
        {
            var list = new List<string>();
            var items = document.QuerySelectorAll(".woocommerce-product-gallery__wrapper a").OfType<IHtmlAnchorElement>(); ;

            foreach (var item in items)
            {
                list.Add(item.Href);
            }

            return list.ToArray();
        }
    }
}
