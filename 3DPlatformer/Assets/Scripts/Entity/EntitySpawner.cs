using UnityEngine;

namespace Platformer
{
    public class EntitySpawner<T> where T : Entity
    {
        private IEntityFactory<T> entityFactory;
        private ISpawnPointStrategy spawnPointStrategy;

        public EntitySpawner(IEntityFactory<T> entityFactory, ISpawnPointStrategy spawnPointStrategy)
        {
            this.entityFactory = entityFactory;
            this.spawnPointStrategy = spawnPointStrategy;
        }

        public T Spawn(out Transform spawnPoint)    //Get a reference to the next spawn point
        {
            spawnPoint = spawnPointStrategy.NextSpawnPoint();
            return entityFactory.Create(spawnPoint);
        }
    }
}