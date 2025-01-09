using Project.World.Generation.Chunks;

namespace Project.World
{
    public interface IChunksIterator
    {
        public QueryResult<IChunk> QueryNextChunk(ChunkPosition position, FaceDirection direction);
    }
}
