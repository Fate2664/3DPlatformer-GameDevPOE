using System;
using UnityEngine;

namespace Platformer
{
    public class EnemySpawnManager : EntitySpawnManager
    {
        [System.Serializable]
        private class WaypointPath
        {
            public Transform[] points;
        }
        
        [SerializeField] private EnemyData[] enemyData;
        [SerializeField] private WaypointPath[] waypointPaths;
        [SerializeField] private float spawnRate = 1f;
        
        private EntitySpawner<Enemy> spawner;
        private CountDownTimer spawnTimer;
        private int counter;

        protected override void Awake()
        {
            base.Awake();
            spawner = new EntitySpawner<Enemy>(new EntityFactory<Enemy>(enemyData), spawnPointStrategy);
            
            spawnTimer = new CountDownTimer(spawnRate);
            spawnTimer.OnTimerStop += () =>
            {
                if (counter++ >= spawnPoints.Length)
                {
                    spawnTimer.Stop();
                    return;
                }

                Spawn();
                spawnTimer.Start();
            };
        }
        
        void Start() => spawnTimer.Start();
        void Update() => spawnTimer.Tick(Time.deltaTime);

        public override void Spawn()
        {
            Enemy enemy = spawner.Spawn(out Transform spawnPoint);
            
            int spawnIndex = Array.IndexOf(spawnPoints, spawnPoint);
            Transform[] path = waypointPaths[spawnIndex].points;
            enemy.SetWanderPath(path);
        }
    }
}