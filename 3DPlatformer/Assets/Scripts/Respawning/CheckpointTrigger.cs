using System;
using UnityEngine;

namespace Platformer
{
    //This script registers a checkpoint when the player enters the trigger box
    public class CheckpointTrigger : MonoBehaviour
    {
        [SerializeField] private Respawner respawner;
        [SerializeField] private string checkpointId;
        [SerializeField] private Transform spawnPoint;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player"))
                return;
            
            respawner.RegisterCheckpoint(checkpointId, spawnPoint);
        }
    }
}
