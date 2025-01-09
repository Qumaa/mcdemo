using Project.World.Generation.Blocks;

namespace Project.World.Generation.Chunks
{
    public interface IChunk
    {
        ChunkMesh Mesh { get; }
        ChunkPosition Position { get; }
        IBlocksIterator Blocks { get; }
        void GenerateBlocks(ChunkLOD lod);
        void GenerateMesh(ChunkLOD lod);
    }
}
