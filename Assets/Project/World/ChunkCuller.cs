using UnityEngine;

namespace Project.World
{
    public readonly ref struct ChunkCuller
    {
        private readonly ChunkPosition _center;

        public ChunkCuller(ChunkPosition center)
        {
            _center = center;
        }

        public ChunkCullingFlags GetFlags(ChunkPosition position)
        {
            Vector3Int difference = _center.SignedDifference(position);

            int x = difference.x;
            int y = difference.y;
            int z = difference.z;

            return new(
                y <= 0,
                y >= 0,
                x <= 0,
                x >= 0,
                z <= 0,
                z >= 0
                );
        }
    }
}
