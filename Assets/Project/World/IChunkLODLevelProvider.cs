using Project.World.Generation.Chunks;

namespace Project.World
{
    public interface IChunkLODLevelProvider
    {
        ChunkLOD GetLevel(ChunkPosition chunkPosition);
    }
}
