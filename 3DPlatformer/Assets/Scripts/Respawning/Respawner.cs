using System;
using UnityEngine;

namespace Platformer
{
    public class Respawner : MonoBehaviour
    {
        [SerializeField] private PlayerController playerController;
        [SerializeField] private PlayerStats playerStats;
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
            HandleRespawn();
        }

        public void RegisterCheckpoint(string checkpointId, Transform checkpointTransform)
        {
            checkpointHistory.Push(new RespawnPointData(checkpointId, checkpointTransform.position, checkpointTransform.rotation, playerStats.CreateSnapshot()));
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

        private void HandleRespawn()
        {
            bool hasLivesRemaining = playerStats.Lives > 0;

            if (!hasLivesRemaining)
            {
                ResetRunToStart();
                return;
            }
            
            
            playerStats.DecrementLives();
            respawnable.RespawnAt(checkpointHistory.Peek());
        }

        private void ResetRunToStart()
        {
            checkpointHistory.Clear();
            
            RegisterCheckpoint("Start", startRespawnPoint);
            playerStats.RestoreLives();
            respawnable.RespawnAt(checkpointHistory.Peek());
        }
    }
}
