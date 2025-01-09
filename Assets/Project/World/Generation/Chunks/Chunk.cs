using Project.World.Generation.Blocks;
using Project.World.Generation.Terrain;

namespace Project.World.Generation.Chunks
{
    public class Chunk : IChunk
    {
        public const int STANDARD_SIZE = 16;

        private readonly IChunkMeshGenerator _chunkMeshGenerator;
        private readonly IBlocksIteratorProvider _blocksIteratorProvider;
        private readonly IChunkView _view;
        public ChunkMesh Mesh => _view.Mesh;
        public ChunkPosition Position { get; }
        public IBlocksIterator Blocks { get; private set; }

        public Chunk(ChunkPosition position, IChunkMeshGenerator chunkMeshGenerator,
            IBlocksIteratorProvider blocksIteratorProvider, IChunkView view)
        {
            Position = position;
            _chunkMeshGenerator = chunkMeshGenerator;
            _blocksIteratorProvider = blocksIteratorProvider;
            _view = view;
        }

        public void GenerateBlocks(ChunkLOD lod) =>
            Blocks = _blocksIteratorProvider.GetBlockIterator(Position, lod, Blocks);

        public void GenerateMesh(ChunkLOD lod) =>
            _view.SetMesh(_chunkMeshGenerator.Generate(Blocks));
    }
}
