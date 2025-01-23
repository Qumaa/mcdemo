using System;
using Project.World.Generation.Blocks;
using UnityEngine;

namespace Project.World.Generation.Terrain
{
    public class DummyBlockGenerator : IBlockGenerator
    {
        private const int _UNINITIALIZED_HEIGHT = int.MinValue;
        private readonly BlockType _notEmpty = new(0);
        private readonly BlockType _empty = null;

        private int lastX, lastZ, lastHeight = _UNINITIALIZED_HEIGHT;

        public Block GenerateBlock(int x, int y, int z)
        {
            if (x != lastX || z != lastZ || lastHeight is _UNINITIALIZED_HEIGHT)
                lastHeight = GetHeight(x, z);
            
            return y <= GetHeight(x, z) ? new(_notEmpty) : new(_empty);
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
