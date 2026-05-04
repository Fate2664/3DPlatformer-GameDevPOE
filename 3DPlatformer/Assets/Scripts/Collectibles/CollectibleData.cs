using UnityEngine;

namespace Platformer
{
    //This scriptable object will hold all the data needed for a collectible.
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