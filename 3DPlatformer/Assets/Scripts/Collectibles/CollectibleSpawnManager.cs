using UnityEngine;

namespace Platformer
{
    //This script is for managing HOW collectibles will be spawned
    public class CollectibleSpawnManager : EntitySpawnManager
    {
        [SerializeField] private CollectibleData[] collectibleData;
        [SerializeField] private float spawnRate = 1f;

        private EntitySpawner<Collectible> spawner;
        
        //Countdown timer for the spawn rate of the collectibles
        private CountDownTimer spawnTimer;
        private int counter;

        protected override void Awake()
        {
            base.Awake();
            //Create the actual spawner for the collecitbles with the correct factory
            spawner = new EntitySpawner<Collectible>(new EntityFactory<Collectible>(collectibleData),
                spawnPointStrategy);

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

        public override void Spawn() => spawner.Spawn(out _);
    }
}