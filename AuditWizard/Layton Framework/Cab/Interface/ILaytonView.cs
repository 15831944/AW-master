namespace Layton.Cab.Interface
{
    public interface ILaytonView
    {
        LaytonWorkItem WorkItem { get; }
        void RefreshView();
    }
}
