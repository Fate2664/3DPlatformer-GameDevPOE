using UnityEngine;

namespace Platformer
{
    //This interface is a contract for all spawn point strategies to inherit from
    public interface ISpawnPointStrategy
    {
        Transform NextSpawnPoint();
    }
}