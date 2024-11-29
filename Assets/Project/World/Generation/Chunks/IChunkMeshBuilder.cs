using Project.World.Generation.Blocks;
using UnityEngine;

namespace Project.World.Generation.Chunks
{
    public interface IChunkMeshBuilder
    {
        void AddBlock(int x, int y, int z, Block block);
        ChunkMesh Finish();
    }
}
