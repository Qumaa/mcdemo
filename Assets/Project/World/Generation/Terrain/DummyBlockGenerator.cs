using System;
using Project.World.Generation.Blocks;
using UnityEngine;

namespace Project.World.Generation.Terrain
{
    public class DummyBlockGenerator : IBlockGenerator
    {
        private readonly BlockType _notEmpty = new(0);
        private readonly BlockType _empty = null;

        public Block GenerateBlock(int x, int y, int z)
        {
            if (y <= 22 && y <= GetHeight(x, z))
                return new(_notEmpty);
            
            return new(_empty);
        }

        private static int GetHeight(int x, int z)
        {
            const float scale = 50f;
            const int offset = 10000;

            float perlin = Mathf.PerlinNoise((x + offset) / scale, (z + offset) / scale);

            return (int) (22f * perlin);
        }
    }
}
