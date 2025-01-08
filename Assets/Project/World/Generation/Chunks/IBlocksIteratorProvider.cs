using Project.World.Generation.Blocks;
using UnityEngine;

namespace Project.World.Generation.Chunks
{
    public interface IBlocksIteratorProvider
    {
        IBlocksIterator GetBlockIterator(ChunkPosition position, ChunkLOD lod, IBlocksIterator previous);
    }
}
