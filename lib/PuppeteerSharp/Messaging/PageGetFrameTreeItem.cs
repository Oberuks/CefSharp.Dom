namespace CefSharp.Dom.Messaging
{
    internal class PageGetFrameTreeItem
    {
        public PageGetFrameTreeItemInfo Frame { get; set; }

        public PageGetFrameTreeItem[] ChildFrames { get; set; }
    }
}
