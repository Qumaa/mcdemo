namespace Project.World.Generation.Blocks
{
    public sealed class BlockFaces : SixFaces<BlockFace>
    {
        public BlockFaces(Directional<BlockFace> face1, Directional<BlockFace> face2, Directional<BlockFace> face3,
            Directional<BlockFace> face4, Directional<BlockFace> face5, Directional<BlockFace> face6) : base(
            face1,
            face2,
            face3,
            face4,
            face5,
            face6
        ) { }
    }
}
