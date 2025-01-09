using Project.World.Generation.Chunks;

namespace Project.World
{
    public class ChunksIteratorKostyl : IChunksIterator
    {
        public World World;
        public QueryResult<IChunk> QueryNextChunk(ChunkPosition position, FaceDirection direction) =>
            World.QueryNextChunk(position, direction);
    }
}
