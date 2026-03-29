using System;
using UnityEngine;

namespace Platformer
{
    public class DeathZoneTrigger : MonoBehaviour
    {
        [SerializeField] private Respawner respawner;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
                respawner.RespawnPlayer();
        }
    }
}
