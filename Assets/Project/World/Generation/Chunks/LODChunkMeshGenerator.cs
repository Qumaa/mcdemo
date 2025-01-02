using System.Collections.Generic;
using Project.World.Generation.Blocks;
using UnityEngine;

namespace Project.World.Generation.Chunks
{
    public class LODChunkMeshGenerator : IChunkMeshGenerator
    {
        private readonly IBlockMeshProvider _blockMeshProvider;

        private readonly SixFaceData<ChunkFaceBuffer> _faceBuffers;

        public LODChunkMeshGenerator(IBlockMeshProvider blockMeshProvider)
        {
            _blockMeshProvider = blockMeshProvider;

            _faceBuffers = SixFaceData.Empty<ChunkFaceBuffer>();
        }

        public ChunkMesh Generate(IBlocksIterator blocks)
        {
            int size = blocks.Size;
            Builder builder = new(this, blocks, Chunk.STANDARD_SIZE);

            for (int x = 0; x < size; x++)
            for (int y = 0; y < size; y++)
            for (int z = 0; z < size; z++)
                builder.AddBlock(x, y, z, blocks[x, y, z]);

            return builder.Finish();
        }

        private readonly ref struct Builder
        {
            private readonly LODChunkMeshGenerator _generator;
            private readonly int _verticesScaler;
            private readonly IBlocksIterator _blocksIterator;

            public Builder(LODChunkMeshGenerator generator, IBlocksIterator blocksIterator, int standardChunkSize)
            {
                _generator = generator;
                _blocksIterator = blocksIterator;

                _verticesScaler = standardChunkSize / blocksIterator.Size;
            }

            private IBlockMeshProvider _blockMeshProvider => _generator._blockMeshProvider;
            private SixFaceData<ChunkFaceBuffer> _faceBuffers => _generator._faceBuffers;

            public void AddBlock(int x, int y, int z, Block block)
            {
                BlockMesh mesh = _blockMeshProvider.GetBlockMesh(block.Type);

                if (mesh is null)
                    return;

                foreach (FaceDirection direction in FaceDirections.Array)
                    if (!FaceIsCovered(x, y, z, direction))
                        _faceBuffers[direction].AddBlockFace(x, y, z, mesh.Faces[direction], _verticesScaler);
            }

            public ChunkMesh Finish()
            {
                Directional<Mesh>[] meshes = new Directional<Mesh>[6];

                for (int i = 0; i < FaceDirections.Array.Length; i++)
                {
                    FaceDirection direction = FaceDirections.Array[i];
                    ChunkFaceBuffer buffer = _faceBuffers[direction];

                    meshes[i] = new(buffer.Compile(), direction);
                    buffer.Clear();
                }

                return new(meshes);
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

        private class ChunkFaceBuffer
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

            public Mesh Compile()
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
