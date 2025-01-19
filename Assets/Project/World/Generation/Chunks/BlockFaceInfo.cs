namespace Project.World.Generation.Chunks
{
    public readonly ref struct BlockFaceInfo
    {
        public readonly bool IsCovered;
        public readonly bool IsOnEdge;
        
        public BlockFaceInfo(bool isCovered, bool isOnEdge)
        {
            IsCovered = isCovered;
            IsOnEdge = isOnEdge;
        }
    }
}
