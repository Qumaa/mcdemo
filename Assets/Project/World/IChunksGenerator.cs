using Project.World.Generation.Chunks;

namespace Project.World
{
    public interface IChunksGenerator
    {
        Chunks Generate(ChunkPosition center, int loadDistance);
    }

    public class IncrementalChunksGenerator : IChunksGenerator
    {
        private readonly IChunkMeshGenerator _meshGenerator;
        private readonly IChunkLODProvider _lodProvider;
        private readonly IBlocksIteratorProvider _blocksProvider;
        private readonly ChunkViewFactory _factory;
        
        public IncrementalChunksGenerator(IChunkMeshGenerator meshGenerator, IBlocksIteratorProvider blocksProvider, IChunkLODProvider lodProvider, ChunkViewFactory factory)
        {
            _meshGenerator = meshGenerator;
            _blocksProvider = blocksProvider;
            _lodProvider = lodProvider;
            _factory = factory;
        }
        
        public Chunks Generate(ChunkPosition center, int loadDistance) =>
            throw new System.NotImplementedException();
    }
}
