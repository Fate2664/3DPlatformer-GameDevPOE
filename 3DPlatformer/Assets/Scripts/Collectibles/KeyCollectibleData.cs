using UnityEngine;

namespace Platformer
{
    [CreateAssetMenu(menuName = "Entity/Key Collectible Data")]
    public class KeyCollectibleData : CollectibleData
    {
        public override bool TryCollect(PlayerController player)
        {
            var inventory = player.GetComponent<PlayerInventory>();
            inventory.AddKey(this);
            return true;
        }
    }
}
