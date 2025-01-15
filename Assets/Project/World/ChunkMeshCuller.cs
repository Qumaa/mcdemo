using Project.World.Generation.Blocks;
using UnityEngine;

namespace Project.World
{
    public readonly ref struct ChunkMeshCuller
    {
        private readonly ChunkPosition _center;

        public ChunkMeshCuller(ChunkPosition center)
        {
            _center = center;
        }

        public void Cull(ChunkFaces faces, ChunkPosition position)
        {
            Vector3Int difference = _center.SignedDifference(position);

            CullRightLeft(faces, difference.x);
            CullTopBottom(faces, difference.y);
            CullFrontBack(faces, difference.z);
        }

        private static void CullRightLeft(ChunkFaces faces, int x)
        {
            if (x is 0)
                return;

            bool positive = x > 0;
            faces.Right.SetVisibility(!positive);
            faces.Left.SetVisibility(positive);
        }

        private static void CullTopBottom(ChunkFaces faces, int y)
        {
            if (y is 0)
                return;

            bool positive = y > 0;
            faces.Top.SetVisibility(!positive);
            faces.Bottom.SetVisibility(positive);
        }

        private static void CullFrontBack(ChunkFaces faces, int z)
        {
            if (z is 0)
                return;

            bool positive = z > 0;
            faces.Front.SetVisibility(!positive);
            faces.Back.SetVisibility(positive);
        }
    }
}
