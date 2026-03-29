using System;
using DG.Tweening;
using UnityEngine;

namespace Platformer
{
    public class PlatformMover : MonoBehaviour
    {
        [SerializeField] private Vector3 moveTo = Vector3.zero;
        [SerializeField] private float moveTime = 1f;
        [SerializeField] private Ease ease = Ease.InOutQuad;
        
        public Vector3 Velocity {  get; private set; }
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
            Velocity = (transform.position - lastPosition) / Time.fixedDeltaTime;
            lastPosition = transform.position;
        }

        private void Move()
        {
            transform.DOMove(startPosition + moveTo, moveTime).SetEase(ease).SetLoops(-1, LoopType.Yoyo);
        }
    }
}
