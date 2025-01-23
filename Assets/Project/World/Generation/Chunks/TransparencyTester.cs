using Project.World.Generation.Blocks;

namespace Project.World.Generation.Chunks
{
    public class TransparencyTester : ITransparencyTester
    {
        public bool IsTransparent(BlockFace face) =>
            face is null || !face.CoversAdjacentFace;
    }
}
