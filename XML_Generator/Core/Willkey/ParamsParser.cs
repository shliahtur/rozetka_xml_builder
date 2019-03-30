
using AngleSharp.Html.Dom;
using System;
using System.Collections.Generic;

namespace Parser.Core.Habra
{
    class ParamsParser : IParser<List<Tuple<string, string>>>
    {
        public List<Tuple<string, string>> Parse(IHtmlDocument document)
        {
            List<Tuple<string, string>> paramPairs = new List<Tuple<string, string>>();

            string prop = "";
            string value = "";

            var pairs = document.QuerySelectorAll(".product-details-table td");

            for (int i = 0; i < pairs.Length; i += 2)
            {
                if (i % 2 == 0)
                {
                    prop = pairs[i].TextContent;
                    value = pairs[i + 1].TextContent;
                }


                Tuple<string, string> paramPair = Tuple.Create(prop, value);
                paramPairs.Add(paramPair);
            }

            return paramPairs;
        }
    }
}
