using Project.World.Generation.Blocks;

namespace Project.World.Generation.Chunks
{
    public interface ITransparencyTester
    {
        bool IsTransparent(BlockFace face);
    }
}
