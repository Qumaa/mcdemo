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
            GenerationCapture capture = new(this, chunk, chunksIterator);

            foreach (FlatIndexHandle handle in FlatIndexHandle.Enumerate(blocks.Size))
                capture.AddBlock(handle);

            return capture.Build();
        }

        private ref struct GenerationCapture
        {
            private const int _CHUNK_SIZE = Chunk.STANDARD_SIZE;
            
            private readonly LODChunkMeshGenerator _context;
            private readonly int _verticesScaler;
            private readonly IChunk _chunk;
            private readonly IChunksIterator _chunksIterator;
            private FlatIndexHandle _handle;

            public GenerationCapture(LODChunkMeshGenerator context, IChunk chunk, IChunksIterator chunksIterator)
            {
                _context = context;
                _chunk = chunk;
                _chunksIterator = chunksIterator;

                _verticesScaler = _CHUNK_SIZE / chunk.Blocks.Size;
                _handle = default;
            }

            private IBlockMeshProvider _blockMeshProvider => _context._blockMeshProvider;
            private SixFaces<ChunkFaceBuilder> _faceBuilders => _context._faceBuilders;

            private IBlocksIterator _blocks => _chunk.Blocks;

            public void AddBlock(FlatIndexHandle handle)
            {
                _handle = handle;
                
                BlockMesh blockMesh = GetBlockMesh();

                if (blockMesh is not null)
                    AddBlockFaces(blockMesh);
            }

            public ChunkMesh Build()
            {
                SixFacesBuilder<ChunkFace> builder = new();

                foreach (FaceDirection direction in FaceDirections.Array)
                {
                    ChunkFaceBuilder faceBuilder = _faceBuilders[direction];

                    builder.AppendFace(new(faceBuilder.BuildMesh(), direction));
                    faceBuilder.Clear();
                }

                return new(builder.Build());
            }

            private void AddBlockFaces(BlockMesh blockMesh)
            {
                foreach (FaceDirection direction in FaceDirections.Array)
                {
                    if (FaceIsCovered(_handle, direction, out bool onEdge))
                        continue;

                    Vector3 position = _handle.ToVector();
                    BlockFace face = blockMesh.Faces[direction];
                    bool transparentOnEdge = onEdge && !face.CoversAdjacentFace;
                    
                    _faceBuilders[direction].AddBlockFace(position, face, _verticesScaler, transparentOnEdge);
                }
            }

            private BlockMesh GetBlockMesh()
            {
                Block block = _blocks[_handle.FlatIndex];
                BlockMesh mesh = _blockMeshProvider.GetBlockMesh(block.Type);
                return mesh;
            }

            private bool FaceIsCovered(FlatIndexHandle handle, FaceDirection faceDirection, out bool onEdge)
            {
                onEdge = false;

                if (_blocks.TryGetNextBlock(handle, faceDirection, out Block nextBlock))
                    CoversAdjacentFace(nextBlock, faceDirection);

                onEdge = true;
                return FaceIsCoveredByAdjacentChunk(handle, faceDirection);
            }

            private bool FaceIsCoveredByAdjacentChunk(FlatIndexHandle handle, FaceDirection direction)
            {
                if (!_chunksIterator.TryGetNextChunk(_chunk.Position, direction, out IChunk adjacentChunk))
                    return direction is not FaceDirection.Up and not FaceDirection.Down;

                direction.Negate();
                IBlocksIterator adjacentBlocks = adjacentChunk.Blocks;
                Vector3Int position = handle.ToVectorInt() + direction.ToVector(_blocks.Size - 1);

                if (adjacentBlocks.Size > _blocks.Size)
                    return FaceIsCoveredByLargerChunk(adjacentBlocks, position, direction);

                position /= _blocks.Size / adjacentBlocks.Size;
                return CoversAdjacentFace(adjacentBlocks.GetBlock(position), direction);
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
                                adjacentBlocks.GetBlock(
                                    adjacentPosition.x + x,
                                    adjacentPosition.y + y,
                                    adjacentPosition.z + z
                                ),
                                direction
                            ))
                            return false;

                    axis2 -= adjacentBlocksPerBlock;
                }

                return true;
            }

            private bool CoversAdjacentFace(Block block, FaceDirection direction)
            {
                BlockType adjacentBlockType = block.Type;

                if (adjacentBlockType is null)
                    return false;

                BlockMesh adjacentBlockMesh = _blockMeshProvider.GetBlockMesh(adjacentBlockType);

                return adjacentBlockMesh.Faces[direction].CoversAdjacentFace;
            }
        }

        private class ChunkFaceBuilder
        {
            private readonly List<Vector3> _vertices = new(1024);
            private readonly List<Vector3> _normals = new(1024);
            private readonly List<int> _triangles = new(1536);
            private int _transparentEdgeCounter = 0;

            public void AddBlockFace(Vector3 position, BlockFace face, float verticesScaler, bool transparentOnEdge)
            {
                int verticesOffset = _vertices.Count;
                position *= verticesScaler;
                
                if (transparentOnEdge)
                    AddTransparentBlockFace();

                foreach (Vector3 vertex in face.Vertices)
                    _vertices.Add(position + (vertex * verticesScaler));

                foreach (int vertexIndex in face.Triangles)
                    _triangles.Add(verticesOffset + vertexIndex);

                foreach (Vector3 normal in face.Normals)
                    _normals.Add(normal);
            }

            public void AddTransparentBlockFace() =>
                _transparentEdgeCounter++;

            public void Clear()
            {
                _vertices.Clear();
                _normals.Clear();
                _triangles.Clear();
                _transparentEdgeCounter = 0;
            }

            public ChunkFace BuildMesh()
            {
                if (_vertices.Count is 0)
                    return null;
                
                Mesh mesh = new()
                {
                    vertices = _vertices.ToArray(), normals = _normals.ToArray(), triangles = _triangles.ToArray()
                };

                mesh.UploadMeshData(true);

                return new(mesh, new(_transparentEdgeCounter));
            }
        }
    }
}
