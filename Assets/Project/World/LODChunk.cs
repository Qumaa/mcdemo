using Project.World.Generation.Chunks;

namespace Project.World
{
    public readonly struct LODChunk
    {
        public readonly Chunk Chunk;
        public readonly ChunkLOD LOD;
        
        public LODChunk(Chunk chunk, ChunkLOD lod)
        {
            Chunk = chunk;
            LOD = lod;
        }
    }
}
