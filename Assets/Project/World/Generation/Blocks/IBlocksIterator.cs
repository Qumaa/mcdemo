﻿using Project.World.Generation.Chunks;

namespace Project.World.Generation.Blocks
{
    public interface IBlocksIterator
    {
        public int Size { get; }
        Block this[int x, int y, int z] { get; set; }
        bool TryGetNextBlock(int x, int y, int z, FaceDirection faceDirection, out Block block);
    }
}
