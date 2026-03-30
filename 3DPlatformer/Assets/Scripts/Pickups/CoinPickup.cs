using UnityEngine;

namespace Platformer
{
    public class CoinPickup : PickupEffect
    {
        [SerializeField] private int scoreAmount = 1;

        public override void ApplyEffect(PlayerStats playerStats)
        {
            playerStats.IncrementScore(scoreAmount);
        }
    }
}
