using Project.World.Generation.Chunks;
using UnityEngine;

namespace Project.World.Generation.Blocks
{
    public interface IBlocksIterator
    {
        public int Size { get; }
        Block this[FlatIndex index] { get; set; }
        bool TryGetNextBlock(FlatIndexHandle handle, FaceDirection faceDirection, out Block block);
    }

    public static class BlocksIteratorExtensions
    {
        public static Block GetBlock(this IBlocksIterator iterator, int x, int y, int z) =>
            iterator[FlatIndex.FromXYZ(iterator.Size, x, y, z)];
        public static Block GetBlock(this IBlocksIterator iterator, Vector3Int xyz) =>
            iterator.GetBlock(xyz.x, xyz.y, xyz.z);
    }
}
