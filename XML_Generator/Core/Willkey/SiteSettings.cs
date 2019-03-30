
namespace Parser.Core.Habra
{
    class SiteSettings : IParserSettings
    {

        public string BaseUrl { get; set; }

        public SiteSettings(string link)
        {
            BaseUrl = link;
        }

    }
}
