using UnityEngine;

namespace Platformer
{
    [CreateAssetMenu(menuName = "Entity/Collectible Data")]
    public class CollectibleData : EntityData
    {
        public int score;
        
        public void IncrementScore(PlayerStats playerStats)
        {
            playerStats.IncrementScore(score);
        }
    }
}