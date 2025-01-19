using System;
using Project.World.Generation.Chunks;

namespace Project.World
{
    public class World : IChunksSupervisor
    {
        private readonly ChunksGenerator _generator;
        private Chunks _chunks;

        public World(ChunkPosition center, int loadDistance, IChunkMeshGenerator meshGenerator,
            BlocksIteratorProvider blocksProvider, IChunkLODProvider lodProvider, ChunkViewFactory factory)
        {
            _generator = new(meshGenerator, blocksProvider, lodProvider, factory);
            _chunks = _generator.Generate(center, loadDistance);
        }

        public void UpdateChunks(ChunkPosition newCenter)
        {
            
        }

        public void SetLoadDistance(int distance) =>
            throw new NotImplementedException();
        
        

        private void HideChunkFaces()
        {
            
        }
    }
}
