using UnityEngine;

namespace Project.World
{
    public class BlockFace
    {
        public readonly Vector3[] Vertices;
        public readonly Vector3[] Normals;
        public readonly int[] Triangles;
        public readonly bool CoversAdjacentFace;

        public BlockFace(Vector3[] vertices, Vector3[] normals, int[] triangles, bool coversAdjacentFace)
        {
            Vertices = vertices;
            Normals = normals;
            Triangles = triangles;
            CoversAdjacentFace = coversAdjacentFace;
        }
    }
}
