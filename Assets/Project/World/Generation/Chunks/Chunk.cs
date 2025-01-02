using Project.World.Generation.Blocks;
using Project.World.Generation.Terrain;
using UnityEngine;

namespace Project.World.Generation.Chunks
{
    public class Chunk : IChunk
    {
        public const int STANDARD_SIZE = 16;

        private IBlocksIterator _blocks;
        private readonly IChunkMeshGenerator _chunkMeshGenerator;
        private readonly IBlockMeshProvider _blockMeshProvider;
        private readonly IBlockGenerator _blockGenerator;
        private readonly IBlocksIteratorProvider _blocksIteratorProvider;
        private readonly IChunkMeshSetter _meshSetter;
        private readonly Vector3Int _position;

        public Chunk(Vector3Int position, IChunkMeshGenerator chunkMeshGenerator,
            IBlocksIteratorProvider blocksIteratorProvider, IChunkMeshSetter meshSetter)
        {
            _position = position;
            _chunkMeshGenerator = chunkMeshGenerator;
            _blocksIteratorProvider = blocksIteratorProvider;
            _meshSetter = meshSetter;
        }

        public void GenerateMesh(ChunkLOD lod)
        {
            _blocks = _blocksIteratorProvider.GetBlockIterator(_position, lod, _blocks);
            _meshSetter.SetMesh(_chunkMeshGenerator.Generate(_blocks));
        }
    }
}
