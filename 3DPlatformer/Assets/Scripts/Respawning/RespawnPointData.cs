using UnityEngine;

namespace Platformer
{
    public struct RespawnPointData
    {
        public string Id;
        public Vector3 Position;
        public Quaternion Rotation;

        public RespawnPointData(string id, Vector3 position, Quaternion rotation)
        {
            Id = id;
            Position = position;
            Rotation = rotation;
        }
    }
}

namespace Platformer
{
    public interface IRespawnable
    {
        void RespawnAt(RespawnPointData checkpoint);
    }
}
