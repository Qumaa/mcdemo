using Project.World.Generation.Chunks;

namespace Project.World
{
    public class ChunksIteratorKostyl : IChunksIterator
    {
        public World World;
        public bool TryGetNextChunk(ChunkPosition position, FaceDirection direction, out IChunk chunk) =>
            World.TryGetNextChunk(position, direction, out chunk);
    }
}
