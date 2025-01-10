using Project.World.Generation.Chunks;

namespace Project.World
{
    public struct LODChunk 
    {
        public readonly Chunk Chunk;
        public ChunkLOD LOD;

        public LODChunk(Chunk chunk, ChunkLOD lod)
        {
            Chunk = chunk;
            LOD = lod;
        }
        
        public void GenerateBlocks() =>
            Chunk.GenerateBlocks(LOD);

        public void GenerateMesh() =>
            Chunk.GenerateMesh(LOD);
    }
}
