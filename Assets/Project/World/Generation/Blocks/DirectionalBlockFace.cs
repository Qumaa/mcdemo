namespace Project.World.Generation.Blocks
{
    public readonly struct DirectionalBlockFace
    {
        public readonly BlockFace Face;
        public readonly FaceDirection FaceDirection;

        public DirectionalBlockFace(BlockFace face, FaceDirection faceDirection)
        {
            Face = face;
            FaceDirection = faceDirection;
        }
    }
}
