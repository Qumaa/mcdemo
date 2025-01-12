using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Project.World.Generation.Chunks;
using UnityEngine;
using Object = UnityEngine.Object;

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
            Object.Instantiate(_chunkPrefab, position.ToWorld(), Quaternion.identity).GetComponent<IChunkView>();
    }

    public struct InstantiatingAwaiter<T> : INotifyCompletion where T : Object
    {
        private readonly AsyncInstantiateOperation<T> _operation;
        private Action _continuation;
            
        public InstantiatingAwaiter(AsyncInstantiateOperation<T> operation) : this()
        {
            _operation = operation;
        }

        public bool IsCompleted => _operation.isDone;

        public void OnCompleted(Action continuation)
        {
            _continuation = continuation;
            _operation.completed += HandleCompletion;
        }

        private void HandleCompletion(AsyncOperation obj) =>
            _continuation();

        public T GetResult() =>
            _operation.Result[0];
    }

    public static class InstantiateAsyncAwaiterExtension
    {
        public static InstantiatingAwaiter<T> GetAwaiter<T>(this AsyncInstantiateOperation<T> operation)
            where T : Object =>
            new(operation);
    }
}
