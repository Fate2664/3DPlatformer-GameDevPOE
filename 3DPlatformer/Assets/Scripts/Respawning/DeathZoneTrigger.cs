using System;
using UnityEngine;

namespace Platformer
{
    //This script is for the death zone below the water to respawn the player if they fall in it
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
