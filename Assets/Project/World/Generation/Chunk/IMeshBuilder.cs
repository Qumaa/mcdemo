using Project.World.Generation.Block;
using UnityEngine;

namespace Project.World.Generation.Chunk
{
    public interface IMeshBuilder
    {
        void SetBlock(int x, int y, int z, BlockType blockType);
        Mesh Finish();
    }
}
