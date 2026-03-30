using System;
using UnityEngine;

namespace Platformer
{
    public class Pickup : MonoBehaviour
    {
        private PickupEffect[] effects;

        private void Awake()
        {
            effects = GetComponents<PickupEffect>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player"))
                return;
            if (!other.TryGetComponent(out PlayerStats playerStats))
                    return;
            
            foreach (var effect in effects)
                effect.ApplyEffect(playerStats);
            
            Destroy(gameObject);
        }
    }
}
