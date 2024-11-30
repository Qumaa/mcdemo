namespace Project.World.Generation.Blocks
{
    public interface IBlocksIterator
    {
        public int Size { get; }
        Block this[int x, int y, int z] { get; }
        bool TryGetNext(int x, int y, int z, Direction direction, out Block block);
    }
}
