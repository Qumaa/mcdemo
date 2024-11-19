using Project.World.Generation.Blocks;
using UnityEngine;

namespace Project.World.Generation.Chunks
{
    public interface IMeshBuilder
    {
        void SetBlock(int x, int y, int z, BlockType blockType);
        Mesh Finish();
    }
}
