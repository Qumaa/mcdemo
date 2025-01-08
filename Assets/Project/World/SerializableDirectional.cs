using System;
using UnityEngine;

namespace Project.World
{
    [Serializable]
    public struct SerializableDirectional<T>
    {
        [field: SerializeField] public FaceDirection Direction { get; private set; }
        [field: SerializeField] public T Face { get; private set; }

        public SerializableDirectional(T face, FaceDirection direction)
        {
            Direction = direction;
            Face = face;
        }

        public static implicit operator Directional<T>(SerializableDirectional<T> directional) =>
            new(directional.Face, directional.Direction);
    }
}
