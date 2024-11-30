using System.Collections.Generic;
using Project.World.Generation.Blocks;
using UnityEngine;

namespace Project.World.Generation.Chunks
{
    public class LODMeshGenerator : IMeshGenerator
    {
        private readonly LODChunkMeshBuilder _lodChunkMeshBuilder;

        public LODMeshGenerator(IBlockMeshProvider blockMeshProvider, int chunkDimensions)
        {
            _lodChunkMeshBuilder = new LODChunkMeshBuilder(blockMeshProvider, chunkDimensions);
        }

        public IChunkMeshBuilder Start(IBlocksIterator blocksIterator) =>
            _lodChunkMeshBuilder.Start(blocksIterator);

        private class LODChunkMeshBuilder : IChunkMeshBuilder
        {
            private static readonly Direction[] _directions =
            {
                Direction.Up,
                Direction.Down,
                Direction.Left,
                Direction.Right,
                Direction.Back,
                Direction.Forward
            };
            
            private readonly int _chunkDimensions;
            private readonly IBlockMeshProvider _blockMeshProvider;
            private IBlocksIterator _blocksIterator;

            private Mesh _generatedMesh;
            private List<Vector3> _vertices = new();
            private List<Vector3> _normals = new();
            private List<int> _triangles = new();
            private int _verticesScaler;
            
            public LODChunkMeshBuilder(IBlockMeshProvider blockMeshProvider, int chunkDimensions)
            {
                _blockMeshProvider = blockMeshProvider;
                _chunkDimensions = chunkDimensions;
            }

            public IChunkMeshBuilder Start(IBlocksIterator blocksIterator)
            {
                _blocksIterator = blocksIterator;
                _verticesScaler = _chunkDimensions / blocksIterator.Size;
                RefreshMesh();
                
                int size = blocksIterator.Size;
                for (var x = 0; x < size; x++)
                for (var y = 0; y < size; y++)
                for (var z = 0; z < size; z++)
                    AddBlock(x, y, z, _blocksIterator[x, y, z]);
                
                return this;
            }

            private void RefreshMesh() =>
                _generatedMesh = new Mesh {name = "Chunk"};

            public void AddBlock(int x, int y, int z, Block block)
            {
                BlockMesh mesh = _blockMeshProvider.GetBlockMesh(block.Type);
                
                if (mesh is null)
                    return;

                foreach (Direction direction in _directions)
                    if (!FaceIsCovered(x, y, z, direction))
                        AddFaceToMesh(x, y, z, mesh[direction]);
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

            public ChunkMesh Finish()
            {
                _generatedMesh.vertices = _vertices.ToArray();
                _generatedMesh.triangles = _triangles.ToArray();
                _generatedMesh.normals = _normals.ToArray();
                _generatedMesh.Optimize();
                _generatedMesh.UploadMeshData(true);
                
                Mesh buffer = _generatedMesh;
                _generatedMesh = null;
                _vertices.Clear();
                _triangles.Clear();
                _normals.Clear();
                
                return buffer;
            }

            private readonly struct DirectionVectorPair
            {
                public readonly Direction Direction;
                public readonly Vector3Int Vector;
                
                public DirectionVectorPair(Direction direction, Vector3Int vector)
                {
                    Direction = direction;
                    Vector = vector;
                }

                public static DirectionVectorPair Up => new(Direction.Up, Vector3Int.up);
                public static DirectionVectorPair Down => new(Direction.Down, Vector3Int.down);
                public static DirectionVectorPair Left => new(Direction.Left, Vector3Int.left);
                public static DirectionVectorPair Right => new(Direction.Right, Vector3Int.right);
                public static DirectionVectorPair Back => new(Direction.Back, Vector3Int.back);
                public static DirectionVectorPair Forward => new(Direction.Forward, Vector3Int.forward);
            }
        }
    }
}
