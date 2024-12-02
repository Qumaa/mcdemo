using System.Collections.Generic;
using Project.World.Generation.Blocks;
using UnityEngine;

namespace Project.World.Generation.Chunks
{
    public class LODChunkMeshGenerator : IChunkMeshGenerator
    {
        private readonly IBlockMeshProvider _blockMeshProvider;
        
        private readonly List<Vector3> _vertices = new();
        private readonly List<Vector3> _normals = new();
        private readonly List<int> _triangles = new();

        public LODChunkMeshGenerator(IBlockMeshProvider blockMeshProvider)
        {
            _blockMeshProvider = blockMeshProvider;
        }

        public ChunkMesh Generate(IBlocksIterator blocks)
        {
            int size = blocks.Size;
            var builder = new Builder(this, blocks, Chunk.STANDARD_SIZE);
                
            for (var x = 0; x < size; x++)
            for (var y = 0; y < size; y++)
            for (var z = 0; z < size; z++)
                builder.AddBlock(x, y, z, blocks[x, y, z]);

            return builder.Finish();
        }

        private readonly ref struct Builder
        {
            private readonly LODChunkMeshGenerator _generator;
            private readonly int _verticesScaler;
            private readonly Mesh _generatedMesh;
            private readonly IBlocksIterator _blocksIterator;
            
            public Builder(LODChunkMeshGenerator generator, IBlocksIterator blocksIterator, int standardChunkSize)
            {
                _generator = generator;
                _blocksIterator = blocksIterator;
                
                _verticesScaler = standardChunkSize / blocksIterator.Size;
                _generatedMesh = new Mesh { name = "Chunk" };
            }

            private static readonly Direction[] _directions =
            {
                Direction.Up,
                Direction.Down,
                Direction.Left,
                Direction.Right,
                Direction.Back,
                Direction.Forward
            };

            private IBlockMeshProvider _blockMeshProvider => _generator._blockMeshProvider;
            private List<Vector3> _vertices => _generator._vertices;
            private List<Vector3> _normals => _generator._normals;
            private List<int> _triangles => _generator._triangles;

            public void AddBlock(int x, int y, int z, Block block)
            {
                BlockMesh mesh = _blockMeshProvider.GetBlockMesh(block.Type);
                
                if (mesh is null)
                    return;

                foreach (Direction direction in _directions)
                    if (!FaceIsCovered(x, y, z, direction))
                        AddFaceToMesh(x, y, z, mesh[direction]);
            }
            
            public ChunkMesh Finish()
            {
                _generatedMesh.vertices = _vertices.ToArray();
                _generatedMesh.triangles = _triangles.ToArray();
                _generatedMesh.normals = _normals.ToArray();
                _generatedMesh.Optimize();
                _generatedMesh.UploadMeshData(true);

                _vertices.Clear();
                _triangles.Clear();
                _normals.Clear();
                
                return _generatedMesh;
            }

            private bool FaceIsCovered(int x, int y, int z, Direction direction)
            {
                if (!_blocksIterator.TryGetNext(x, y, z, direction, out Block block))
                    return false;
                
                BlockType adjacentBlockType = block.Type;

                if (adjacentBlockType is null)
                    return false;
                
                BlockMesh adjacentBlockMesh = _blockMeshProvider.GetBlockMesh(adjacentBlockType);

                return adjacentBlockMesh.Opposite(direction).CoversAdjacentFace;
            }

            private void AddFaceToMesh(int x, int y, int z, BlockFace face)
            {
                int verticesOffset = _vertices.Count;
                Vector3 position = new Vector3(x, y, z) * _verticesScaler;
                
                foreach (Vector3 vertex in face.Vertices)
                    _vertices.Add(position + vertex * _verticesScaler);

                foreach (int vertexIndex in face.Triangles)
                    _triangles.Add(verticesOffset + vertexIndex);

                foreach (Vector3 normal in face.Normals)
                    _normals.Add(normal);
            }
        }
    }
}
