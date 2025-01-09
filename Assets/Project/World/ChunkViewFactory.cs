using Project.World.Generation.Chunks;
using UnityEngine;

namespace Project.World
{
    public class ChunkViewFactory
    {
        private readonly GameObject _chunkPrefab;
        
        public ChunkViewFactory(GameObject chunkPrefab) 
        {
            _chunkPrefab = chunkPrefab;
        }

        public IChunkView Create(ChunkPosition position) =>
            Object.Instantiate(_chunkPrefab, position.ToWorld(), Quaternion.identity)
                .GetComponent<IChunkView>();
    }
}
