using Project.World.Generation.Blocks;
using Project.World.Generation.Terrain;

namespace Project.World.Generation.Chunks
{
    public class Chunk : IChunk
    {
        private IBlockIterator _blocks;
        private readonly IMeshGenerator _meshGenerator;
        private readonly IBlockMeshProvider _blockMeshProvider;
        private readonly IBlockGenerator _blockGenerator;
        private readonly IBlockIteratorProvider _blockIteratorProvider;

        public Chunk(IMeshGenerator meshGenerator, IBlockIteratorProvider blockIteratorProvider)
        {
            _meshGenerator = meshGenerator;
            _blockIteratorProvider = blockIteratorProvider;
        }

        public ChunkMesh GenerateMesh(ChunkLOD lod)
        {
            _blocks = _blockIteratorProvider.GetBlockIterator(lod);
            return _meshGenerator.Start(_blocks).Finish();
        }
    }
}


