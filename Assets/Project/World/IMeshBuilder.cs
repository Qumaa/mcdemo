using UnityEngine;

namespace Project.World
{
    public interface IMeshBuilder
    {
        void SetBlock(int x, int y, int z, BlockType blockType);
        Mesh Finish();
    }
}
