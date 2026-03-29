using System;
using UnityEngine;

namespace Platformer
{
    public class PlatformCollisionHandler : MonoBehaviour
    {
        private Transform platform;
        private PlayerController playerController;

        private void Awake()
        {
            playerController = GetComponent<PlayerController>();
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag("MovingPlatform"))
            {
                ContactPoint contact = other.GetContact(0);
                if (contact.normal.y < 0.5f) return;
                
                playerController.SetMovingPlatform(other.transform.GetComponent<PlatformMover>());
            }
        }

        private void OnCollisionExit(Collision other)
        {
            if (other.gameObject.CompareTag("MovingPlatform"))
            {
                playerController.SetMovingPlatform(null);
            }
        }
    }
}
