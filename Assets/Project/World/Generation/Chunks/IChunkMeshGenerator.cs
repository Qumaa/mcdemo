using Project.World.Generation.Blocks;

namespace Project.World.Generation.Chunks
{
    public interface IChunkMeshGenerator
    {
        ChunkMesh Generate(IChunk chunk);
    }
}
