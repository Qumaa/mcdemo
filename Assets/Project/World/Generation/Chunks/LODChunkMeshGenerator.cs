using System;
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

        public ChunkMesh Generate(IChunk chunk, IChunksIterator chunksIterator)
        {
            IBlocksIterator blocks = chunk.Blocks;
            int size = blocks.Size;
            MeshBuilder meshBuilder = new(this, chunk, chunksIterator);

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
            private readonly IChunksIterator _chunksIterator;

            public MeshBuilder(LODChunkMeshGenerator generator, IChunk chunk, IChunksIterator chunksIterator)
            {
                _generator = generator;
                _chunk = chunk;
                _chunksIterator = chunksIterator;

                _verticesScaler = _CHUNK_SIZE / chunk.Blocks.Size;
            }

            private IBlockMeshProvider _blockMeshProvider => _generator._blockMeshProvider;
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

            private bool FaceIsCovered(int x, int y, int z, FaceDirection faceDirection) =>
                _blocks.TryGetNextBlock(x, y, z, faceDirection, out Block block) ?
                    CoversAdjacentFace(block, faceDirection) :
                    FaceIsCoveredByAdjacentChunk(x, y, z, faceDirection);

            private bool FaceIsCoveredByAdjacentChunk(int x, int y, int z, FaceDirection direction)
            {
                if (!_chunksIterator.TryGetNextChunk(_chunk.Position, direction, out IChunk adjacentChunk))
                    return direction is not FaceDirection.Up and not FaceDirection.Down;

                direction.Negate();
                IBlocksIterator adjacentBlocks = adjacentChunk.Blocks;
                Vector3Int position = new Vector3Int(x, y, z) + direction.ToVector(_blocks.Size - 1);

                // todo: case when next chunk is larger
                if (adjacentBlocks.Size > _blocks.Size)
                    return FaceIsCoveredByLargerChunk(adjacentBlocks, position, direction);

                position /= _blocks.Size / adjacentBlocks.Size;
                return CoversAdjacentFace(adjacentBlocks[position.x, position.y, position.z], direction);
            }

            private bool FaceIsCoveredByLargerChunk(IBlocksIterator adjacentBlocks, Vector3Int adjacentPosition,
                FaceDirection direction)
            {
                int adjacentBlocksPerBlock = adjacentBlocks.Size / _blocks.Size;
                int offset = adjacentBlocksPerBlock - 1;
                adjacentPosition *= adjacentBlocksPerBlock;
                
                int x = 0, y = 0, z = 0;
                ref int axis1 = ref x, axis2 = ref x;

                switch (direction)
                {
                    case FaceDirection.Up:
                        y += offset;
                        goto case FaceDirection.Down;
                    case FaceDirection.Down:
                        axis2 = ref z;
                        break;
                    
                    case FaceDirection.Right:
                        x += offset;
                        goto case FaceDirection.Left;
                    case FaceDirection.Left:
                        axis1 = ref y;
                        axis2 = ref z;
                        break;
                        
                    case FaceDirection.Forward:
                        z += offset;
                        goto case FaceDirection.Back;
                    case FaceDirection.Back:
                        axis2 = ref y;
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
                }

                for (; axis1 < adjacentBlocksPerBlock; axis1++)
                {
                    for (; axis2 < adjacentBlocksPerBlock; axis2++)
                        if (!CoversAdjacentFace(
                                adjacentBlocks[adjacentPosition.x + x, adjacentPosition.y + y, adjacentPosition.z + z],
                                direction
                            ))
                            return false;

                    axis2 -= adjacentBlocksPerBlock;
                }

                return true;
            }

            private bool CoversAdjacentFace(Block block, FaceDirection incomingDirection)
            {
                BlockType adjacentBlockType = block.Type;

                if (adjacentBlockType is null)
                    return false;

                BlockMesh adjacentBlockMesh = _blockMeshProvider.GetBlockMesh(adjacentBlockType);

                return adjacentBlockMesh.Faces[incomingDirection].CoversAdjacentFace;
            }
        }

        private class ChunkFaceBuilder
        {
            private readonly List<Vector3> _vertices = new(1024);
            private readonly List<Vector3> _normals = new(1024);
            private readonly List<int> _triangles = new(1536);

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
