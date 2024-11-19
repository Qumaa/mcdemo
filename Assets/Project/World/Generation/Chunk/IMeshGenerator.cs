using Project.World.Generation.Block;

namespace Project.World.Generation.Chunk
{
    public interface IMeshGenerator
    {
        IMeshBuilder Start(IBlockIterator iterator);
    }
}
