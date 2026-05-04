using UnityEngine;

namespace Platformer
{
    //This script is to check when the player walks over a collectible -> in this case a coin
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