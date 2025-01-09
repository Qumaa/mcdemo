using Project.World.Generation.Chunks;

namespace Project.World
{
    public interface IChunkLODProvider
    {
        ChunkLOD GetLevel(ChunkPosition basePosition, ChunkPosition chunkPosition);
    }
}
