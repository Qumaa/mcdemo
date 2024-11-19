namespace Project.World.Generation.Blocks
{
    public interface IBlockMeshProvider
    {
        BlockMesh GetBlockMesh(BlockType blockType);
    }
}
