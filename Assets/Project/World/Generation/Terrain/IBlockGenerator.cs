using Project.World.Generation.Blocks;

namespace Project.World.Generation.Terrain
{
    public interface IBlockGenerator
    {
        Block GetBlock(int x, int y, int z);
    }
}
