using System;
using UnityEngine;

namespace Platformer
{
    public class GroundCheck : MonoBehaviour
    {
        [SerializeField] private float groundDistance = 2.0f;
        [SerializeField] LayerMask groundLayers;
        
        public bool IsGrounded { get; private set;}

        private void Update()
        {
            //IsGrounded = Physics
        }
    }
}
