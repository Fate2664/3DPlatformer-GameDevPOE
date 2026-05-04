using UnityEngine;

namespace Platformer
{
    //This script manages the detection of the player
    public class PlayerDetector : MonoBehaviour
    {
        [SerializeField] private float detectionAngle = 60.0f; //Cone in front of enemy
        [SerializeField] private float detectionRadius = 10.0f; //Distance from enemy
        [SerializeField] private float innerDetectionRadius = 5.0f; //Small detection circle around enemy
        [SerializeField] private float detectionCooldown = 1f; //Time between detections
        [SerializeField] private float attackRange = 2f;

        public Transform Player { get; private set; }
        private CountDownTimer detectionTimer;
        
        IDetectionStrategy detectionStrategy;

        private void Awake()
        {
            detectionTimer = new CountDownTimer(detectionCooldown);
            Player = GameObject.FindWithTag("Player").transform;
            //Create a new cone detection strategy
            detectionStrategy = new ConeDetectionStrategy(detectionAngle, detectionRadius, innerDetectionRadius);
        }
        
        void Update() => detectionTimer.Tick(Time.deltaTime);

        //Check if enemy can see player
        public bool CanDetectPlayer()
        {
            if (Player == null || detectionTimer == null || detectionStrategy == null) return false;
            //Detection timer is running and the detection strategy returns true
            return detectionTimer.IsRunning || detectionStrategy.Execute(Player, transform, detectionTimer);
        }
    
        //Check if enemy can attack player
        public bool CanAttackPlayer()
        {
            var directionToPlayer = Player.position - transform.position;
            return directionToPlayer.magnitude < attackRange;
        }
        
        public void SetDetectionStrategy(IDetectionStrategy strategy) => detectionStrategy = strategy;
        
    }
}
