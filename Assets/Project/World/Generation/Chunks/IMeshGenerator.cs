using Project.World.Generation.Blocks;

namespace Project.World.Generation.Chunks
{
    public interface IMeshGenerator
    {
        IChunkMeshBuilder Start(IBlocksIterator iterator);
    }
}
