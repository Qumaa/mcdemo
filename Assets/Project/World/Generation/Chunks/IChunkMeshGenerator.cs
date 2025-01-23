namespace Project.World.Generation.Chunks
{
    public interface IChunkMeshGenerator
    {
        ChunkMesh Generate(IChunk chunk, IChunksIterator chunksIterator);
    }

    public static class ChunkMeshGeneratorExtensions
    {
        public static ChunkMesh Generate(this IChunkMeshGenerator generator, IChunk chunk, IChunksIterator chunksIterator) =>
            generator.Generate(chunk, chunksIterator);
    }
}
