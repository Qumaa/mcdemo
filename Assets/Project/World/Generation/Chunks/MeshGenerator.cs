using System.Collections.Generic;
using Project.World.Generation.Blocks;
using UnityEngine;

namespace Project.World.Generation.Chunks
{
    public class MeshGenerator : IMeshGenerator
    {
        private readonly MeshBuilder _meshBuilder;

        public MeshGenerator(IBlockMeshProvider blockMeshProvider)
        {
            _meshBuilder = new MeshBuilder(blockMeshProvider);
        }

        public IMeshBuilder Start(IBlockIterator blockIterator) =>
            _meshBuilder.Start(blockIterator);

        private class MeshBuilder : IMeshBuilder
        {
            private static readonly DirectionVectorPair[] _directionVectorPairs =
            {
                DirectionVectorPair.Up,
                DirectionVectorPair.Down,
                DirectionVectorPair.Left,
                DirectionVectorPair.Right,
                DirectionVectorPair.Back,
                DirectionVectorPair.Forward
            };
            
            private readonly IBlockMeshProvider _blockMeshProvider;
            private IBlockIterator _blockIterator;

            private Mesh _generatedMesh;
            private List<Vector3> _vertices = new();
            private List<Vector3> _normals = new();
            private List<int> _triangles = new();
            
            public MeshBuilder(IBlockMeshProvider blockMeshProvider) {
                _blockMeshProvider = blockMeshProvider;
            }

            public IMeshBuilder Start(IBlockIterator blockIterator)
            {
                _blockIterator = blockIterator;
                RefreshMesh();
                return this;
            }

            private void RefreshMesh() =>
                _generatedMesh = new Mesh {name = "Chunk"};

            public void SetBlock(int x, int y, int z, BlockType blockType)
            {
                BlockMesh mesh = _blockMeshProvider.GetBlockMesh(blockType);
                
                if (mesh is null)
                    return;

                foreach (DirectionVectorPair pair in _directionVectorPairs)
                    if (!FaceIsCovered(x, y, z, pair))
                        AddFaceToMesh(x, y, z, mesh[pair.Direction]);
            }

            private bool FaceIsCovered(int x, int y, int z, DirectionVectorPair pair)
            {
                BlockType adjacentBlockType = _blockIterator[x + pair.Vector.x, y + pair.Vector.y, z + pair.Vector.z];
                BlockMesh adjacentBlockMesh = _blockMeshProvider.GetBlockMesh(adjacentBlockType);

                bool ret = adjacentBlockMesh is not null && adjacentBlockMesh.Opposite(pair.Direction).CoversAdjacentFace;
                return ret;
            }

            private void AddFaceToMesh(int x, int y, int z, BlockFace face)
            {
                int verticesOffset = _vertices.Count;
                Vector3 position = new(x, y, z);
                
                foreach (Vector3 vertex in face.Vertices)
                    _vertices.Add(position + vertex);

                foreach (int vertexIndex in face.Triangles)
                    _triangles.Add(verticesOffset + vertexIndex);

                foreach (Vector3 normal in face.Normals)
                    _normals.Add(normal);
            }

            public Mesh Finish()
            {
                _generatedMesh.vertices = _vertices.ToArray();
                _generatedMesh.triangles = _triangles.ToArray();
                _generatedMesh.normals = _normals.ToArray();
                
                Mesh buffer = _generatedMesh;
                _generatedMesh = null;
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
