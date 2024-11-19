namespace Project.World
{
    public interface IBlockIterator
    {
        BlockType this[int x, int y, int z] { get; }
    }
}
