using System;
using UnityEngine;

namespace Platformer
{
    //This script manages the player when they are on the moving platform
    public class PlatformCollisionHandler : MonoBehaviour
    {
        private Transform platform;
        
        //When the player lands on the platform, make the player GameObject a child of that platform so that they move with the platform
        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag("MovingPlatform"))
            {
                //If contact normal is pointing up, then when have landed from above and collided with the top of the platform 
                ContactPoint contact = other.GetContact(0);
                if (contact.normal.y < 0.5f) return;
                
                platform = other.transform;
                transform.SetParent(platform);
            }
        }

        private void OnCollisionExit(Collision other)
        {
            if (other.gameObject.CompareTag("MovingPlatform"))
            {
                transform.SetParent(null);
                platform = null;
            }
        }
    }
}
