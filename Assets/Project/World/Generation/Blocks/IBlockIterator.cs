namespace Project.World.Generation.Blocks
{
    public interface IBlockIterator
    {
        public int Dimensions { get; }
        Block this[int x, int y, int z] { get; }
        bool TryGetNext(int x, int y, int z, Direction direction, out Block block);
    }
}
