using Project.World.Generation.Chunks;

namespace Project.World
{
    public interface IChunksIterator
    {
        bool TryGetNextChunk(ChunkPosition position, FaceDirection direction, out IChunk chunk);
    }
}
