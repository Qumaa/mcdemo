using Project.World.Generation.Blocks;
using Project.World.Generation.Terrain;

namespace Project.World.Generation.Chunks
{
    public class Chunk : IChunk
    {
        public const int STANDARD_SIZE = 16;

        public IChunkView View { get; }
        public ChunkPosition Position { get; set; }
        public IBlocksIterator Blocks { get; set; }

        public Chunk(ChunkPosition position, IChunkView view)
        {
            Position = position;
            View = view;
        }
    }
}
