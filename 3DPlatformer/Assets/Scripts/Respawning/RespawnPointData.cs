using UnityEngine;

namespace Platformer
{
    //This sctruct defines the variables for a respawn point
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
    //This interface is a contract for all respawnables to inherit from
    public interface IRespawnable
    {
        void RespawnAt(RespawnPointData checkpoint);
    }
}
