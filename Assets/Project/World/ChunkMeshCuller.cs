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

        public void Cull(ChunkFaceViews faceViews, ChunkPosition position)
        {
            Vector3Int difference = _center.SignedDifference(position);

            CullRightLeft(faceViews, difference.x);
            CullTopBottom(faceViews, difference.y);
            CullFrontBack(faceViews, difference.z);
        }

        private static void CullRightLeft(ChunkFaceViews faceViews, int x)
        {
            if (x is 0)
                return;

            bool positive = x > 0;
            faceViews.Right.SetVisibility(!positive);
            faceViews.Left.SetVisibility(positive);
        }

        private static void CullTopBottom(ChunkFaceViews faceViews, int y)
        {
            if (y is 0)
                return;

            bool positive = y > 0;
            faceViews.Top.SetVisibility(!positive);
            faceViews.Bottom.SetVisibility(positive);
        }

        private static void CullFrontBack(ChunkFaceViews faceViews, int z)
        {
            if (z is 0)
                return;

            bool positive = z > 0;
            faceViews.Front.SetVisibility(!positive);
            faceViews.Back.SetVisibility(positive);
        }
    }
}
