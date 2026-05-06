using UnityEngine;

namespace Platformer
{
    public class CatSpawnManager : EntitySpawnManager
    {
        [SerializeField] private CatData[] catData;
        [SerializeField] private float spawnRate = 1f;

        private EntitySpawner<Cat> spawner;
        
        //Countdown timer for the spawn rate of the collectibles
        private CountDownTimer spawnTimer;
        private int counter;

        protected override void Awake()
        {
            base.Awake();
            //Create the actual spawner for the collecitbles with the correct factory
            spawner = new EntitySpawner<Cat>(new EntityFactory<Cat>(catData),
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