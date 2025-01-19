using System;
using Project.World.Generation.Blocks;
using UnityEngine;

namespace Project.World.Generation.Chunks
{
    public ref struct ChunkMeshGenerationContext
    {
        public readonly IChunk Chunk;
        public readonly IBlockMeshProvider BlockMeshProvider;
        public readonly ITransparencyTester TransparencyTester;
        private readonly IChunksIterator _chunksIterator;
            
        public FlatIndexHandle Handle;
            
        public ChunkMeshGenerationContext(IChunk chunk, IChunksIterator chunksIterator,
            IBlockMeshProvider blockMeshProvider, ITransparencyTester transparencyTester)
        {
            Chunk = chunk;
            BlockMeshProvider = blockMeshProvider;
            TransparencyTester = transparencyTester;
            _chunksIterator = chunksIterator;
                
            Handle = default;
        }

        public BlockFaceInfo FetchFaceInfo(FaceDirection faceDirection) =>
            Chunk.Blocks.TryGetNextBlock(Handle, faceDirection, out Block nextBlock) ?
                new(CoveredByNextBlock(nextBlock, faceDirection), false) :
                new(IsCoveredByAdjacentChunk(faceDirection), true);

        private bool IsCoveredByAdjacentChunk(FaceDirection direction)
        {
            if (!_chunksIterator.TryGetNextChunk(Chunk.Position, direction, out IChunk adjacentChunk))
                return direction is not FaceDirection.Up and not FaceDirection.Down;

            direction.Negate();
            IBlocksIterator blocks = Chunk.Blocks;
            IBlocksIterator adjacentBlocks = adjacentChunk.Blocks;
            Vector3Int position = Handle.ToVectorInt() + direction.ToVectorInt(blocks.Size - 1);

            if (adjacentBlocks.Size > blocks.Size)
                return IsCoveredByLargerChunk(adjacentBlocks, position, direction);

            position /= blocks.Size / adjacentBlocks.Size;
            return CoveredByNextBlock(adjacentBlocks.GetBlock(position), direction);
        }

        private bool IsCoveredByLargerChunk(IBlocksIterator adjacentBlocks, Vector3Int adjacentPosition,
            FaceDirection direction)
        {
            int adjacentBlocksNumber = adjacentBlocks.Size / Chunk.Blocks.Size;
            int offset = adjacentBlocksNumber - 1;
            adjacentPosition *= adjacentBlocksNumber;

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

            for (; axis1 < adjacentBlocksNumber; axis1++)
            {
                for (; axis2 < adjacentBlocksNumber; axis2++)
                    if (!CoveredByNextBlock(
                            adjacentBlocks.GetBlock(
                                adjacentPosition.x + x,
                                adjacentPosition.y + y,
                                adjacentPosition.z + z
                            ),
                            direction
                        ))
                        return false;

                axis2 -= adjacentBlocksNumber;
            }

            return true;
        }

        private bool CoveredByNextBlock(Block block, FaceDirection incomingDirection)
        {
            BlockType adjacentBlockType = block.Type;

            if (adjacentBlockType is null)
                return false;

            BlockMesh adjacentBlockMesh = BlockMeshProvider.GetBlockMesh(adjacentBlockType);

            return !TransparencyTester.IsTransparent(adjacentBlockMesh.Faces[incomingDirection.Opposite()]);
        }
    }
}
