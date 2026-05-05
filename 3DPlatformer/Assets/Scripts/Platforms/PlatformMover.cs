using System;
using DG.Tweening;
using UnityEngine;

namespace Platformer
{
    //This is a script for moving a platform. It uses the DOTween addon to move the platform back and forth
    public class PlatformMover : MonoBehaviour
    {
        [SerializeField] private Vector3 moveTo = Vector3.zero;
        [SerializeField] private float moveTime = 1f;
        [SerializeField] private Ease ease = Ease.InOutQuad;
        
        private Vector3 startPosition;
        private Vector3 lastPosition; 
            
        private void Start()
        {
            startPosition = transform.position;
            lastPosition = startPosition;
            Move();
        }

        private void FixedUpdate()
        {
            lastPosition = transform.position;
        }

        private void Move()
        {
            transform.DOMove(startPosition + moveTo, moveTime).SetEase(ease).SetLoops(-1, LoopType.Yoyo);
        }
    }
}
