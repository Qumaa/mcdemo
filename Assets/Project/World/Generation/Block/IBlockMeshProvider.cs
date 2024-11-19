namespace Project.World.Generation.Block
{
    public interface IBlockMeshProvider
    {
        BlockMesh GetBlockMesh(BlockType blockType);
    }
}
