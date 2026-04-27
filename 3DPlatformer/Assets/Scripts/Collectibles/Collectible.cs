using UnityEngine;

namespace Platformer
{
    public class Collectible : Entity
    {
        [SerializeField] private CollectibleData collectibleData;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player") || !other.TryGetComponent(out PlayerStats playerStats))
                return;
            
            collectibleData.IncrementScore(playerStats);
            Destroy(gameObject);
        }
    }
}