namespace Project.World.Generation.Blocks
{
    public class BlockMesh
    {
        public SixFaceData<BlockFace> Faces { get; }

        public BlockMesh(Directional<BlockFace> top, Directional<BlockFace> bottom, Directional<BlockFace> right,
            Directional<BlockFace> left, Directional<BlockFace> front, Directional<BlockFace> back)
        {
            Faces = new(top, bottom, right, left, front, back);
        }
    }
}
