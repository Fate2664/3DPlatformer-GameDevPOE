using UnityEngine;

namespace Platformer
{
    [CreateAssetMenu(menuName = "Entity/Score Collectible Data")]
    public class ScoreCollectibleData : CollectibleData
    {
        public int score;
        public override bool TryCollect(PlayerController player)
        {
            var playerStats = player.GetComponent<PlayerStats>();
            playerStats.IncrementScore(score);
            //Play SFX
            AudioManager.Instance.Play("CoinCollect");
            return true;
        }
    }
}
