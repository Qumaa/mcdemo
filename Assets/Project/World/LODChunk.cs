using Project.World.Generation.Chunks;

namespace Project.World
{
    public struct LODChunk 
    {
        public readonly IChunk Chunk;
        public ChunkLOD LOD;

        public LODChunk(IChunk chunk, ChunkLOD lod)
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
