using Project.World.Generation.Blocks;
using UnityEngine;

namespace Project.World.Generation.Terrain
{
    public class DummyBlockGenerator : IBlockGenerator
    {
        private readonly BlockType _notEmpty = new(0);
        private readonly BlockType _empty = null;
        
        public Block GetBlock(int x, int y, int z) =>
            y <= GetHeight(x, z) ? new Block(_notEmpty) : new Block(_empty);

        private int GetHeight(int x, int z)
        {
            const float scale = 10f;
            
            float perlin = Mathf.PerlinNoise(x / scale, z / scale);

            return 6 + (int) (4f * perlin);
        }
    }
}
