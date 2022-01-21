using CefSharp.Puppeteer.Input;

namespace CefSharp.Puppeteer.Messaging
{
    internal class InputDispatchDragEventRequest
    {
        public DragEventType Type { get; set; }

        public decimal X { get; set; }

        public decimal Y { get; set; }

        public int Modifiers { get; set; }

        public DragData Data { get; set; }
    }
}