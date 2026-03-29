using System;
using UnityEngine;

namespace Platformer
{
    public class Respawner : MonoBehaviour
    {
        [SerializeField] private PlayerController playerController;
        [SerializeField] private Transform startRespawnPoint;
        
        private CheckpointStack<RespawnPointData>  checkpointHistory = new ();
        private IRespawnable respawnable;
        private bool respawnQueued;
            
        private void Awake()
        {
            respawnable = playerController as IRespawnable;
            if (startRespawnPoint != null)
            {
                RegisterCheckpoint("Start", startRespawnPoint);
            }
        }

        private void LateUpdate()
        {
            if (!respawnQueued)
                return;
            
            respawnQueued = false;
            if (checkpointHistory.Count > 0)
                respawnable.RespawnAt(checkpointHistory.Peek());
        }

        public void RegisterCheckpoint(string checkpointId, Transform checkpointTransform)
        {
            checkpointHistory.Push(new RespawnPointData(checkpointId, checkpointTransform.position, checkpointTransform.rotation));
        }

        public void RespawnPlayer()
        {
            respawnQueued = true;
        }

        public void RevertToPreviousCheckpoint()
        {
            if (checkpointHistory.Count > 1)
                checkpointHistory.Pop();

            respawnQueued = true;
        }
    }
}
