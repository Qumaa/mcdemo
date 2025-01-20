using System;
using Project.World.Generation.Chunks;

namespace Project.World
{
    public class World : IChunksSupervisor
    {
        private readonly IChunksGenerator _generator;
        private Chunks _chunks;

        public World(Chunks chunks) {
            _chunks = chunks;
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
