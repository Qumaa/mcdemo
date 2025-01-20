using Project.World.Generation.Chunks;

namespace Project.World
{
    public class ChunksGenerator : IChunksGenerator
    {
        private readonly IChunkMeshGenerator _meshGenerator;
        private readonly IChunkLODProvider _lodProvider;
        private readonly IBlocksIteratorProvider _blocksProvider;
        private readonly ChunkViewFactory _factory;
        
        public ChunksGenerator(IChunkMeshGenerator meshGenerator, IBlocksIteratorProvider blocksProvider, IChunkLODProvider lodProvider, ChunkViewFactory factory)
        {
            _meshGenerator = meshGenerator;
            _blocksProvider = blocksProvider;
            _lodProvider = lodProvider;
            _factory = factory;
        }

        public Chunks Generate(ChunkPosition center, int loadDistance) =>
            new GenerationScope(this, center, loadDistance).Generate();

        private readonly ref struct GenerationScope
        {
            private readonly ChunksGenerator _context;
            private readonly Chunks _chunks;
            
            public GenerationScope(ChunksGenerator context, ChunkPosition center, int loadDistance)
            {
                _context = context;

                _chunks = new(loadDistance, center);
            }

            public Chunks Generate()
            {
                int worldSize = _chunks.GetSize();

                ChunkPosition start = _chunks.Center.OffsetCopy(-_chunks.Extent);

                // todo: generate chunks starting from center and around only when current chunk doesn't hide the adjacent one
                foreach (FlatIndexXYZ index in FlatIndexXYZ.Enumerate(worldSize))
                {
                    ChunkPosition position = start.OffsetCopy(index.x, index.y, index.z);

                    LODChunk lodChunk = CreateChunk(position);
                    GenerateBlocks(lodChunk);
                    
                
                    _chunks.SetDirect(index.Flat, lodChunk);
                }

                GenerateAndCullMeshes();

                return _chunks;
            }

            private LODChunk CreateChunk(ChunkPosition position)
            {
                IChunkView view =  _context._factory.Create(position);
                Chunk chunk = new(position, view);
                
                ChunkLOD lod = _context._lodProvider.GetLevel(_chunks.Center, position);
                
                return new(chunk, lod);
            }

            private void GenerateBlocks(LODChunk lodChunk) =>
                lodChunk.Chunk.Blocks = _context._blocksProvider.GetBlockIterator(
                    lodChunk.Chunk.Position,
                    lodChunk.LOD,
                    lodChunk.Chunk.Blocks
                );

            private void GenerateAndCullMeshes()
            {
                ChunkCuller culler = new(_chunks.Center);
                
                foreach (LODChunk lodChunk in _chunks.Values)
                {
                    Chunk chunk = lodChunk.Chunk;

                    chunk.View.SetMesh(
                        _context._meshGenerator.Generate(chunk, _chunks, culler.GetFlags(chunk.Position))
                    );
                }
            }
        }
    }
}
