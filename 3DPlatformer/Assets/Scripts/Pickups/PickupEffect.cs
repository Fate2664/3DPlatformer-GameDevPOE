using UnityEngine;

namespace Platformer
{
    public abstract class PickupEffect : MonoBehaviour
    {
        public abstract void ApplyEffect(PlayerStats playerStats);
    }
}
