using Project.World.Generation.Blocks;
using Project.World.Generation.Terrain;
using UnityEngine;

namespace Project.World.Generation.Chunks
{
    public class Chunk : IChunk
    {
        private IBlocksIterator _blocks;
        private readonly IMeshGenerator _meshGenerator;
        private readonly IBlockMeshProvider _blockMeshProvider;
        private readonly IBlockGenerator _blockGenerator;
        private readonly IBlocksIteratorProvider _blocksIteratorProvider;

        public Chunk(IMeshGenerator meshGenerator, IBlocksIteratorProvider blocksIteratorProvider)
        {
            _meshGenerator = meshGenerator;
            _blocksIteratorProvider = blocksIteratorProvider;
        }

        public ChunkMesh GenerateMesh(ChunkLOD lod)
        {
            _blocks = _blocksIteratorProvider.GetBlockIterator(Vector3Int.zero, lod, _blocks);
            return _meshGenerator.Start(_blocks).Finish();
        }
    }
}


