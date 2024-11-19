namespace Project.World.Generation.Block
{
    public interface IBlockIterator
    {
        BlockType this[int x, int y, int z] { get; }
    }
}
