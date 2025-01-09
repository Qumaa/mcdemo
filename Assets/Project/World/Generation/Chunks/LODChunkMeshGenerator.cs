using System.Collections.Generic;
using Project.World.Generation.Blocks;
using UnityEngine;

namespace Project.World.Generation.Chunks
{
    public class LODChunkMeshGenerator : IChunkMeshGenerator
    {
        private readonly IBlockMeshProvider _blockMeshProvider;
        private readonly IChunksIterator _chunksIterator;

        private readonly SixFaces<ChunkFaceBuilder> _faceBuilders;

        public LODChunkMeshGenerator(IBlockMeshProvider blockMeshProvider, IChunksIterator chunksIterator)
        {
            _blockMeshProvider = blockMeshProvider;
            _chunksIterator = chunksIterator;

            _faceBuilders = SixFaces.Empty<ChunkFaceBuilder>();
        }

        public ChunkMesh Generate(IChunk chunk)
        {
            IBlocksIterator blocks = chunk.Blocks;
            int size = blocks.Size;
            MeshBuilder meshBuilder = new(this, chunk);

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
            private readonly IChunk _chunk;

            public MeshBuilder(LODChunkMeshGenerator generator, IChunk chunk)
            {
                _generator = generator;
                _chunk = chunk;

                _verticesScaler = _CHUNK_SIZE / chunk.Blocks.Size;
            }

            private IBlockMeshProvider _blockMeshProvider => _generator._blockMeshProvider;
            private IChunksIterator _chunksIterator => _generator._chunksIterator;
            private SixFaces<ChunkFaceBuilder> _faceBuilders => _generator._faceBuilders;

            private IBlocksIterator _blocks => _chunk.Blocks;

            public void AddBlock(int x, int y, int z)
            {
                Block block = _blocks[x, y, z];
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
                QueryResult<Block> result = _blocks.QueryNextBlock(x, y, z, faceDirection);

                return result.Status is QueryStatus.Failed ?
                    FaceIsCoveredByAdjacentChunk(x, y, z, faceDirection) :
                    CoversAdjacentFace(result.Value, faceDirection);
            }

            private bool FaceIsCoveredByAdjacentChunk(int x, int y, int z, FaceDirection direction)
            {
                QueryResult<IChunk> result = _chunksIterator.QueryNextChunk(_chunk.Position, direction);

                if (result.Status is QueryStatus.Failed)
                    return false;

                IBlocksIterator nextBlocks = result.Value.Blocks;

                Vector3Int position = new Vector3Int(x, y, z) - (direction.ToVector() * (_blocks.Size - 1));
                
                if (_blocks.Size >= nextBlocks.Size)
                {
                    position /= _blocks.Size / nextBlocks.Size;
                    return CoversAdjacentFace(nextBlocks[position.x, position.y, position.z], direction);
                }
                
                return false;
            }

            private bool CoversAdjacentFace(Block block, FaceDirection incomingDirection)
            {
                BlockType adjacentBlockType = block.Type;

                if (adjacentBlockType is null)
                    return false;

                BlockMesh adjacentBlockMesh = _blockMeshProvider.GetBlockMesh(adjacentBlockType);

                return adjacentBlockMesh.Faces.Opposite(incomingDirection).CoversAdjacentFace;
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
