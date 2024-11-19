﻿namespace Project.World.Generation.Blocks
{
    public interface IBlockIterator
    {
        BlockType this[int x, int y, int z] { get; }
    }
}