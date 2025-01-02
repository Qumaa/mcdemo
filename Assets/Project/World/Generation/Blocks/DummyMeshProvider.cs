using UnityEngine;

namespace Project.World.Generation.Blocks
{
    public class DummyMeshProvider : IBlockMeshProvider
    {
        private static readonly BlockMesh _cubeMesh;

        static DummyMeshProvider()
        {
            _cubeMesh = CreateCubeMesh();
        }

        public BlockMesh GetBlockMesh(BlockType blockType) =>
            blockType is null ? null : _cubeMesh;

        private static BlockMesh CreateCubeMesh() =>
            new(
                CreateTopFace(),
                CreateBottomFace(),
                CreateRightFace(),
                CreateLeftFace(),
                CreateForwardFace(),
                CreateBackFace()
            );

        private static Directional<BlockFace> CreateTopFace()
        {
            BlockFace face = new(
                new[]
                {
                    Vector3.up,
                    Vector3.up + Vector3.right,
                    Vector3.up + Vector3.forward,
                    Vector3.up + Vector3.forward + Vector3.right
                },
                new[] { Vector3.up, Vector3.up, Vector3.up, Vector3.up },
                new[] { 0, 2, 1, 1, 2, 3 },
                true
            );

            return new(face, FaceDirection.Up);
        }

        private static Directional<BlockFace> CreateBottomFace()
        {
            BlockFace face = new(
                new[] { Vector3.zero, Vector3.right, Vector3.forward, Vector3.forward + Vector3.right },
                new[] { Vector3.down, Vector3.down, Vector3.down, Vector3.down },
                new[] { 0, 1, 2, 1, 3, 2 },
                true
            );

            return new(face, FaceDirection.Down);
        }

        private static Directional<BlockFace> CreateRightFace()
        {
            BlockFace face = new(
                new[]
                {
                    Vector3.right,
                    Vector3.right + Vector3.up,
                    Vector3.right + Vector3.forward,
                    Vector3.right + Vector3.forward + Vector3.up
                },
                new[] { Vector3.right, Vector3.right, Vector3.right, Vector3.right },
                new[] { 0, 1, 2, 1, 3, 2 },
                true
            );

            return new(face, FaceDirection.Right);
        }

        private static Directional<BlockFace> CreateLeftFace()
        {
            BlockFace face = new(
                new[] { Vector3.zero, Vector3.up, Vector3.forward, Vector3.forward + Vector3.up },
                new[] { Vector3.right, Vector3.right, Vector3.right, Vector3.right },
                new[] { 0, 2, 1, 1, 2, 3 },
                true
            );

            return new(face, FaceDirection.Left);
        }

        private static Directional<BlockFace> CreateForwardFace()
        {
            BlockFace face = new(
                new[]
                {
                    Vector3.forward,
                    Vector3.forward + Vector3.up,
                    Vector3.forward + Vector3.right,
                    Vector3.forward + Vector3.right + Vector3.up
                },
                new[] { Vector3.forward, Vector3.forward, Vector3.forward, Vector3.forward },
                new[] { 0, 2, 1, 1, 2, 3 },
                true
            );

            return new(face, FaceDirection.Forward);
        }

        private static Directional<BlockFace> CreateBackFace()
        {
            BlockFace face = new(
                new[] { Vector3.zero, Vector3.up, Vector3.right, Vector3.right + Vector3.up },
                new[] { Vector3.back, Vector3.back, Vector3.back, Vector3.back },
                new[] { 0, 1, 2, 1, 3, 2 },
                true
            );

            return new(face, FaceDirection.Back);
        }
    }
}
