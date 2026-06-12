﻿using UnityEngine;

namespace Platformer
{
    public class CatSpawnManager : EntitySpawnManager
    {
        [SerializeField] private CatData[] catData;

        private IEntityFactory<Cat> catFactory;
        private EntitySpawner<Cat> spawner;

        protected override void Awake()
        {
            base.Awake();
            catFactory = new EntityFactory<Cat>(catData);
            spawner = new EntitySpawner<Cat>(catFactory, spawnPointStrategy);
        }

        public override void Spawn() => spawner.Spawn(out _);

        public Cat SpawnAt(Transform spawnPoint)
        {
            return catFactory.Create(spawnPoint);
        }
    }
}
