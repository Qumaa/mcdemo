using Project.World.Generation.Blocks;

namespace Project.World.Generation.Terrain
{
    public interface IBlockGenerator
    {
        Block GenerateBlock(int x, int y, int z);
    }
}
