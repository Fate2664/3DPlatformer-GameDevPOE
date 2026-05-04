using System;
using UnityEngine;

namespace Platformer
{
    //This script manages the spawning of all enemy entities
    public class EnemySpawnManager : EntitySpawnManager
    {
        //Class for storing waypoints for the enemy
        [System.Serializable]
        private class WaypointPath
        {
            public Transform[] points;
        }
        
        [SerializeField] private EnemyData[] enemyData;     //Enemies to spawn
        [SerializeField] private WaypointPath[] waypointPaths;  //Corresponding waypoints
        [SerializeField] private float spawnRate = 1f;
        
        private EntitySpawner<Enemy> spawner;
        private CountDownTimer spawnTimer;
        private int counter;

        protected override void Awake()
        {
            base.Awake();
            //Create the spawner for the enemy entity with the correct factory
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

        //Spawn the enemies and set their waypoints -> the index of the enemy in array corrosponds to the index of the waypoints array (parallel array)
        public override void Spawn()
        {
            Enemy enemy = spawner.Spawn(out Transform spawnPoint);
            
            int spawnIndex = Array.IndexOf(spawnPoints, spawnPoint);
            Transform[] path = waypointPaths[spawnIndex].points;
            enemy.SetWanderPath(path);
        }
    }
}