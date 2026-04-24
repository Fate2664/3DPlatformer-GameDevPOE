using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Platformer
{
    public class RandomSpawnPointStrategy : ISpawnPointStrategy
    {
        private List<Transform> unusedSpawnPoints;
        private Transform[] spawnPoints;

        public RandomSpawnPointStrategy(Transform[] spawnPoints)
        {
            this.spawnPoints = spawnPoints;
            unusedSpawnPoints = new List<Transform>(spawnPoints);
        }
        
        public Transform NextSpawnPoint()
        {
            if (!unusedSpawnPoints.Any())
                unusedSpawnPoints = new List<Transform>(spawnPoints);
            
            var randormIndex  = Random.Range(0, unusedSpawnPoints.Count);
            Transform result = unusedSpawnPoints[randormIndex];
            unusedSpawnPoints.RemoveAt(randormIndex);
            return result;
        }
    }
}