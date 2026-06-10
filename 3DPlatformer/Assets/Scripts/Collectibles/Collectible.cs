using UnityEngine;

namespace Platformer
{
    //Collects the configured collectible data when the player enters its trigger.
    public class Collectible : Entity
    {
        [SerializeField] private CollectibleData collectibleData;

        private void OnTriggerEnter(Collider other)
        {
            PlayerController player = other.GetComponentInParent<PlayerController>();
            if (player == null || collectibleData == null)
                return;

            if (collectibleData.TryCollect(player))
                Destroy(gameObject);
        }
    }
}
