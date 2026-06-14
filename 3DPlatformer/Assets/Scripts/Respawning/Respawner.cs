using System;
using UnityEngine;

namespace Platformer
{
    //This script manages the respawning and registering of checkpoints for the player
    public class Respawner : MonoBehaviour
    {
        [SerializeField] private Transform startRespawnPoint;
        
        private PlayerController playerController; 
        private PlayerStats playerStats;
        private CheckpointStack<RespawnPointData>  checkpointHistory = new ();
        private IRespawnable respawnable;
        private bool respawnQueued;
        
        private void Awake()
        {
            playerController = GetComponent<PlayerController>();
            playerStats = playerController.GetComponent<PlayerStats>();
            
            respawnable = playerController as IRespawnable;
            //Register the begining start point
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

        //Push the checkpoint stack to add the new checkpoint
        public void RegisterCheckpoint(string checkpointId, Transform checkpointTransform)
        {
            checkpointHistory.Push(new RespawnPointData(checkpointId, checkpointTransform.position, checkpointTransform.rotation, playerStats.CreateSnapshot()));
        }

        public void RespawnPlayer()
        {
            respawnQueued = true;
        }

        private void HandleRespawn()
        {
            bool hasLivesRemaining = playerStats.Lives > 0;
            
            //Respawn back to the start if no lives remaining
            if (!hasLivesRemaining)
            {
                ResetRunToStart();
                return;
            }
            
            //Respawn to last checkpoint
            playerStats.DecrementLives();
            respawnable.RespawnAt(checkpointHistory.Peek());
            
            //Play SFX
            AudioManager.Instance.Play("Death");
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
