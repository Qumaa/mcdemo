namespace Project.World
{
    public class EdgeOpacity
    {
        private int _transparentBlocksCount;

        public EdgeOpacity(int transparentBlocksCount) 
        {
            _transparentBlocksCount = transparentBlocksCount;
        }

        public bool IsOpaque() =>
            _transparentBlocksCount is 0;

        public void NotifyTransparentBlocksAdded(int added) =>
            _transparentBlocksCount += added;

        public void NotifyTransparentBlocksRemoved(int removed)
        {
            if (removed > _transparentBlocksCount)
            {
                _transparentBlocksCount = 0;
                return;
            }

            _transparentBlocksCount -= removed;
        }

        public void Reset() =>
            _transparentBlocksCount = 0;
    }
}
