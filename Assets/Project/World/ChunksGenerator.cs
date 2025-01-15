using Project.World.Generation.Chunks;

namespace Project.World
{
    public class ChunksGenerator
    {
        private readonly IChunkMeshGenerator _meshGenerator;
        private readonly IBlocksIteratorProvider _blocksProvider;
        private readonly IChunkLODProvider _lodProvider;
        private readonly ChunkViewFactory _factory;
        
        public ChunksGenerator(IChunkMeshGenerator meshGenerator, IBlocksIteratorProvider blocksProvider, IChunkLODProvider lodProvider, ChunkViewFactory factory)
        {
            _meshGenerator = meshGenerator;
            _blocksProvider = blocksProvider;
            _lodProvider = lodProvider;
            _factory = factory;
        }

        public Chunks Generate(ChunkPosition center, int loadDistance) =>
            new GenerationCapture(this, center, loadDistance).Generate();

        private readonly ref struct GenerationCapture
        {
            private readonly ChunksGenerator _context;
            private readonly Chunks _chunks;
            
            public GenerationCapture(ChunksGenerator context, ChunkPosition center, int loadDistance)
            {
                _context = context;

                _chunks = new(loadDistance, center);
            }

            public Chunks Generate()
            {
                int worldSize = _chunks.GetSize();

                ChunkPosition start = _chunks.Center.OffsetCopy(-_chunks.Extent);
            
                foreach (FlatIndexXYZ index in FlatIndexXYZ.Enumerate(worldSize))
                {
                    ChunkPosition position = start.OffsetCopy(index.x, index.y, index.z);

                    LODChunk lodChunk = CreateChunk(position);
                    lodChunk.GenerateBlocks();
                
                    _chunks.Set(position, lodChunk);
                }

                GenerateAndCullMeshes();

                return _chunks;
            }

            private LODChunk CreateChunk(ChunkPosition position)
            {
                IChunkView view =  _context._factory.Create(position);
                ChunkLOD lod = _context._lodProvider.GetLevel(_chunks.Center, position);
                // todo: remove _chunks parameter (remove chunk's responsibility to delegate blocks and mesh generation)
                Chunk chunk = new(position, _context._meshGenerator, _context._blocksProvider, _chunks, view);
                return new(chunk, lod);
            }

            private void GenerateAndCullMeshes()
            {
                ChunkMeshCuller culler = new(_chunks.Center);
                
                foreach (LODChunk lodChunk in _chunks.Values)
                {
                    lodChunk.GenerateMesh();
                    culler.Cull(lodChunk.Chunk.View.Faces, lodChunk.Chunk.Position);
                }
            }
        }
    }
}
