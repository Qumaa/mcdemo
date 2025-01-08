using System.Collections.Generic;
using Project.World.Generation.Blocks;
using UnityEngine;

namespace Project.World.Generation.Chunks
{
    public class LODChunkMeshGenerator : IChunkMeshGenerator
    {
        private readonly IBlockMeshProvider _blockMeshProvider;

        private readonly SixFaces<ChunkFaceBuilder> _faceBuilders;

        public LODChunkMeshGenerator(IBlockMeshProvider blockMeshProvider)
        {
            _blockMeshProvider = blockMeshProvider;

            _faceBuilders = SixFaces.Empty<ChunkFaceBuilder>();
        }

        public ChunkMesh Generate(IBlocksIterator blocks)
        {
            int size = blocks.Size;
            MeshBuilder meshBuilder = new(this, blocks);

            for (int x = 0; x < size; x++)
            for (int y = 0; y < size; y++)
            for (int z = 0; z < size; z++)
                meshBuilder.AddBlock(x, y, z);

            return meshBuilder.Build();
        }

        private readonly ref struct MeshBuilder
        {
            private const int _CHUNK_SIZE = Chunk.STANDARD_SIZE;
            
            private readonly LODChunkMeshGenerator _generator;
            private readonly int _verticesScaler;
            private readonly IBlocksIterator _blocksIterator;

            public MeshBuilder(LODChunkMeshGenerator generator, IBlocksIterator blocksIterator)
            {
                _generator = generator;
                _blocksIterator = blocksIterator;

                _verticesScaler = _CHUNK_SIZE / blocksIterator.Size;
            }

            private IBlockMeshProvider _blockMeshProvider => _generator._blockMeshProvider;
            private SixFaces<ChunkFaceBuilder> _faceBuilders => _generator._faceBuilders;

            public void AddBlock(int x, int y, int z)
            {
                Block block = _blocksIterator[x, y, z];
                BlockMesh mesh = _blockMeshProvider.GetBlockMesh(block.Type);

                if (mesh is null)
                    return;

                foreach (FaceDirection direction in FaceDirections.Array)
                    if (!FaceIsCovered(x, y, z, direction))
                        _faceBuilders[direction].AddBlockFace(x, y, z, mesh.Faces[direction], _verticesScaler);
            }

            public ChunkMesh Build()
            {
                SixFacesBuilder<Mesh> builder = new();

                foreach (FaceDirection direction in FaceDirections.Array)
                {
                    ChunkFaceBuilder faceBuilder = _faceBuilders[direction];

                    builder.AppendFace(new(faceBuilder.BuildMesh(), direction));
                    faceBuilder.Clear();
                }

                return new(builder.Build());
            }

            private bool FaceIsCovered(int x, int y, int z, FaceDirection faceDirection)
            {
                if (!_blocksIterator.TryGetNext(x, y, z, faceDirection, out Block block))
                    return false;

                BlockType adjacentBlockType = block.Type;

                if (adjacentBlockType is null)
                    return false;

                BlockMesh adjacentBlockMesh = _blockMeshProvider.GetBlockMesh(adjacentBlockType);

                return adjacentBlockMesh.Faces.Opposite(faceDirection).CoversAdjacentFace;
            }
        }

        private class ChunkFaceBuilder
        {
            private readonly List<Vector3> _vertices = new();
            private readonly List<Vector3> _normals = new();
            private readonly List<int> _triangles = new();

            public void AddBlockFace(int x, int y, int z, BlockFace face, float verticesScaler)
            {
                int verticesOffset = _vertices.Count;
                Vector3 position = new Vector3(x, y, z) * verticesScaler;

                foreach (Vector3 vertex in face.Vertices)
                    _vertices.Add(position + (vertex * verticesScaler));

                foreach (int vertexIndex in face.Triangles)
                    _triangles.Add(verticesOffset + vertexIndex);

                foreach (Vector3 normal in face.Normals)
                    _normals.Add(normal);
            }

            public void Clear()
            {
                _vertices.Clear();
                _normals.Clear();
                _triangles.Clear();
            }

            public Mesh BuildMesh()
            {
                Mesh mesh = new()
                {
                    vertices = _vertices.ToArray(), normals = _normals.ToArray(), triangles = _triangles.ToArray()
                };

                mesh.UploadMeshData(true);

                return mesh;
            }
        }
    }
}
