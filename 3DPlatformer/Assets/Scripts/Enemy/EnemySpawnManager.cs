using System;
using UnityEngine;

namespace Platformer
{
    //This script manages the spawning of all enemy entities
    public class EnemySpawnManager : EntitySpawnManager
    {
        [System.Serializable]
        private struct GraphEdge
        {
            public int from;
            public int to;
        }
        
        //Class for storing waypoints for the enemy
        [System.Serializable]
        private class WaypointPath
        {
            public Transform[] points;
            public GraphEdge[] edges;
        }
        
        [SerializeField] private EnemyData[] enemyData;     //Enemies to spawn
        [SerializeField] private WaypointPath[] waypointPaths;  //Corresponding waypoints
        [SerializeField] private float spawnRate = 1f;
        
        private EntitySpawner<EnemyBase> spawner;
        private CountDownTimer spawnTimer;
        private int counter;

        protected override void Awake()
        {
            base.Awake();
            //Create the spawner for the enemy entity with the correct factory
            spawner = new EntitySpawner<EnemyBase>(new EntityFactory<EnemyBase>(enemyData), spawnPointStrategy);
            
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
            EnemyBase enemy = spawner.Spawn(out Transform spawnPoint);
            int spawnIndex = Array.IndexOf(spawnPoints, spawnPoint);
            
            WaypointPath path =  waypointPaths[spawnIndex];

            switch (enemy)
            {
                case CommonEnemy commonEnemy:
                    commonEnemy.SetWanderPath(path.points);
                    break;
                case BossEnemy bossEnemy:
                    bossEnemy.SetWanderGraph(CreateGraph(path));
                    break;
            }
        }

        private GraphBase<Transform> CreateGraph(WaypointPath path)
        {
            var graph = new GraphBase<Transform>();

            foreach (var point in path.points)
            {
                if (point != null)
                    graph.AddNode(point);
            }

            foreach (var edge in path.edges)
            {
                bool validGraphEdge = edge.from >= 0 && edge.from < path.points.Length && edge.to >= 0 && edge.to < path.points.Length;
                if (!validGraphEdge) continue;
                
                graph.AddEdge(path.points[edge.from], path.points[edge.to]);
            }
            return graph;
        }
    }
}