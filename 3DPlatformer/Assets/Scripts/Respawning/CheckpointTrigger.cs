using System;
using UnityEngine;

namespace Platformer
{
    public class CheckpointTrigger : MonoBehaviour
    {
        [SerializeField] private Respawner respawner;
        [SerializeField] private string checkpointId;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player"))
                return;
            
            respawner.RegisterCheckpoint(checkpointId, transform);
        }
    }
}
