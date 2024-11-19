namespace Project.World
{
    public interface IBlockMeshProvider
    {
        BlockMesh GetBlockMesh(BlockType blockType);
    }
}
