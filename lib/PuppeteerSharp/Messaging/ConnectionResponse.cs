using Newtonsoft.Json.Linq;

namespace CefSharp.Dom.Messaging
{
    internal class ConnectionResponse
    {
        public int? Id { get; set; }

        public ConnectionError Error { get; set; }

        public JObject Result { get; set; }

        public string Method { get; set; }

        public JToken Params { get; set; }
    }
}
