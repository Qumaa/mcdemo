using Project.World.Generation.Blocks;
using UnityEngine;

namespace Project.World.Generation.Chunks
{
    public class LODChunkMeshGenerator : IChunkMeshGenerator
    {
        private readonly IBlockMeshProvider _blockMeshProvider;
        private readonly ITransparencyTester _transparencyTester;

        private readonly SixFaces<ChunkFaceBuilder> _faceBuilders;

        public LODChunkMeshGenerator(IBlockMeshProvider blockMeshProvider,
            ITransparencyTester transparencyTester)
        {
            _blockMeshProvider = blockMeshProvider;
            _transparencyTester = transparencyTester;

            _faceBuilders = SixFaces.Empty<ChunkFaceBuilder>();
        }

        public ChunkMesh Generate(IChunk chunk, IChunksIterator chunksIterator)
        {
            IBlocksIterator blocks = chunk.Blocks;
            GenerationScope scope = new(_faceBuilders, chunk, chunksIterator, _blockMeshProvider, _transparencyTester);

            foreach (FlatIndexHandle handle in FlatIndexHandle.Enumerate(blocks.Size))
                scope.AddBlock(handle);

            return scope.BuildMesh();
        }

        private ref struct GenerationScope
        {
            private const int _CHUNK_SIZE = Chunk.STANDARD_SIZE;

            private readonly int _verticesScaler;
            private readonly SixFaces<ChunkFaceBuilder> _faceBuilders;
            private ChunkMeshGenerationContext _context;

            public GenerationScope(SixFaces<ChunkFaceBuilder> faceBuilders, IChunk chunk,
                IChunksIterator chunksIterator, IBlockMeshProvider blockMeshProvider,
                ITransparencyTester transparencyTester)
            {
                _faceBuilders = faceBuilders;
                _context = new(chunk, chunksIterator, blockMeshProvider, transparencyTester);

                _verticesScaler = _CHUNK_SIZE / chunk.Blocks.Size;
            }

            public void AddBlock(FlatIndexHandle handle)
            {
                _context.Handle = handle;

                BlockMesh blockMesh = GetBlockMesh();

                if (blockMesh is not null)
                    ProcessBlockFaces(blockMesh);
            }

            public ChunkMesh BuildMesh()
            {
                SixFacesBuilder<ChunkFace> builder = new();

                foreach (FaceDirection direction in FaceDirections.Array)
                {
                    ChunkFaceBuilder faceBuilder = _faceBuilders[direction];

                    builder.AppendFace(new(faceBuilder.BuildMesh(), direction));
                    faceBuilder.Clear();
                }

                return new(builder.Build());
            }

            private void ProcessBlockFaces(BlockMesh blockMesh)
            {
                foreach (FaceDirection direction in FaceDirections.Array)
                {
                    BlockFaceInfo info = _context.FetchFaceInfo(direction);
                    
                    if (info.IsCovered)
                        continue;

                    Vector3 position = _context.Handle.ToVector();
                    BlockFace face = blockMesh.Faces[direction];
                    bool transparentOnEdge = info.IsOnEdge && _context.TransparencyTester.IsTransparent(face);

                    ChunkFaceBuilder faceBuilder = _faceBuilders[direction];

                    faceBuilder.AddBlockFace(position, face, _verticesScaler, transparentOnEdge);
                }
            }

            private BlockMesh GetBlockMesh()
            {
                Block block = _context.Chunk.Blocks[_context.Handle.FlatIndex];
                BlockMesh mesh = _context.BlockMeshProvider.GetBlockMesh(block.Type);
                
                return mesh;
            }
        }
    }
}
