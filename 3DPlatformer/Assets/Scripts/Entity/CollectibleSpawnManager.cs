using UnityEngine;

namespace Platformer
{
    public class CollectibleSpawnManager : EntitySpawneManager
    {
        [SerializeField] private CollectibleData[] collectibleData;
        [SerializeField] private float spawnRate = 1f;

        private EntitySpawner<Collectible> spawner;

        private CountDownTimer spawnTimer;
        private int counter;

        protected override void Awake()
        {
            base.Awake();
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

        public override void Spawn() => spawner.Spawn();
    }
}