using UnityEngine;

namespace Platformer
{
    public class PlayerDetector : MonoBehaviour
    {
        [SerializeField] private float detectionAngle = 60.0f; //Cone in front of enemy
        [SerializeField] private float detectionRadius = 10.0f; //Distance from enemy
        [SerializeField] private float innerDetectionRadius = 5.0f; //Small detection circle around enemy
        [SerializeField] private float detectionCooldown = 1f; //Time between detections

        public Transform Player { get; private set; }
        private CountDownTimer detectionTimer;
        
        IDetectionStrategy detectionStrategy;

        void Start()
        {
            detectionTimer = new CountDownTimer(detectionCooldown);
            Player = GameObject.FindWithTag("Player").transform;
            detectionStrategy = new ConeDetectionStrategy(detectionAngle, detectionRadius, innerDetectionRadius);
        }
        
        void Update() => detectionTimer.Tick(Time.deltaTime);

        public bool CanDetectPlayer()
        {
            return detectionTimer.IsRunning || detectionStrategy.Execute(Player, transform, detectionTimer);
        }
        
        public void SetDetectionStrategy(IDetectionStrategy strategy) => detectionStrategy = strategy;
    }
}
