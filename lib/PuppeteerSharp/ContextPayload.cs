using Newtonsoft.Json;

namespace CefSharp.Dom
{
    internal class ContextPayload
    {
        public int Id { get; set; }

        public ContextPayloadAuxData AuxData { get; set; }

        public string Name { get; set; }
    }
}
