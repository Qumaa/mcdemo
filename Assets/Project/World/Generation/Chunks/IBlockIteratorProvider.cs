using Project.World.Generation.Blocks;

namespace Project.World.Generation.Chunks
{
    public interface IBlockIteratorProvider
    {
        IBlockIterator GetBlockIterator(ChunkLOD lod);
    }
}
