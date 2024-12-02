using System;
using Project.World.Generation.Blocks;
using Project.World.Generation.Chunks;
using Project.World.Generation.Terrain;
using UnityEngine;

namespace Project.World
{
    public class Root : MonoBehaviour
    {
        [SerializeField] private MeshFilter _prefab;
        [SerializeField] private int _chunksToGenerate = 16;
        
        private void Start()
        {
            IChunkMeshGenerator chunkMeshGenerator = new LODChunkMeshGenerator(new DummyMeshProvider());
            IBlocksIteratorProvider iteratorProvider = new BlocksIteratorProvider(new DummyBlockGenerator());

            for (var x = 0; x < _chunksToGenerate; x++)
            {
                for (var z = 0; z < _chunksToGenerate; z++)
                {
                    MeshFilter filter = Instantiate(_prefab, new Vector3(x, 0, z) * Chunk.STANDARD_SIZE, Quaternion.identity);
                    
                    var meshSetter = new ChunkMeshSetter(filter);
                    Vector3Int position = Vector3Int.FloorToInt(filter.transform.position);
                    var chunk = new Chunk(position, chunkMeshGenerator, iteratorProvider, meshSetter);
                    
                    chunk.GenerateMesh(ChunkLOD.Full);
                }
            }
        }

        [Serializable]
        private struct LODMeshFilterPair
        {
            [field: SerializeField] public MeshFilter meshFilter { get; private set; }
            [field: SerializeField] public ChunkLOD chunkLOD { get; private set; }
        }
    }
}
