using Project.World.Generation.Blocks;

namespace Project.World.Generation.Chunks
{
    public interface IChunk
    {
        IChunkView View { get; }
        ChunkPosition Position { get; }
        IBlocksIterator Blocks { get; }
    }
}
