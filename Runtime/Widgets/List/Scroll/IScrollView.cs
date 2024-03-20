namespace UIBinding
{
    public interface IScrollView
    {
        void JumpToIndex(int index);
        void ScrollToIndex(int index, float duration);
        void JumpToHead();
        void JumpToTail();
        void ScrollToHead(float duration);
        void ScrollToTail(float duration);
        void SetScrollEnabled(bool enabled);
    }
}
