using Project.World.Generation.Chunks;

namespace Project.World
{
    public interface IChunksIterator
    {
        public bool TryGetNextChunk(ChunkPosition position, FaceDirection direction, out IChunk chunk);
    }
}
