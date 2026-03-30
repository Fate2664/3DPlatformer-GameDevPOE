using UnityEngine;

namespace Platformer
{
    public struct RespawnPointData
    {
        public string Id;
        public Vector3 Position;
        public Quaternion Rotation;
        public PlayerStatsSnapshot Stats;

        public RespawnPointData(string id, Vector3 position, Quaternion rotation,  PlayerStatsSnapshot stats)
        {
            Id = id;
            Position = position;
            Rotation = rotation;
            Stats = stats;
        }

        public RespawnPointData WithStats(PlayerStatsSnapshot stats)
        {
            return new RespawnPointData(Id, Position, Rotation, stats);
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
