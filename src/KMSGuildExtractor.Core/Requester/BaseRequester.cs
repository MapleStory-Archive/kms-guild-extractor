using System.Net.Http;

using HtmlAgilityPack;

namespace KMSGuildExtractor.Core.Requester
{
    public class BaseRequester
    {
        protected static readonly HtmlWeb s_web = new HtmlWeb();
        protected static readonly HttpClient s_client = new HttpClient();
    }
}
