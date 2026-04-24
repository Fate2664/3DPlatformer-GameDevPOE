using UnityEngine;

namespace Platformer
{
    public class ConeDetectionStrategy : IDetectionStrategy
    {
        private readonly float detectionAngle;
        private readonly float detectionRadius;
        private readonly float innerDetectionRadius;

        public ConeDetectionStrategy(float detectionAngle, float detectionRadius, float innerDetectionRadius)
        {
            this.detectionAngle = detectionAngle;
            this.detectionRadius = detectionRadius;
            this.innerDetectionRadius = innerDetectionRadius;
        }

        public bool Execute(Transform player, Transform detector, CountDownTimer timer)
        {
            if (timer.IsRunning) return false;
            
            var directionToPlayer = player.position - detector.position;
            var angleToPlayer = Vector3.Angle(directionToPlayer, detector.forward);
            
            //If player is not within the detection angle + outer radius or is within the inner radius return false
            if ((!(angleToPlayer < detectionAngle / 2f) || !(directionToPlayer.magnitude < detectionRadius)) &&
                !(directionToPlayer.magnitude < innerDetectionRadius)) return false;
            
            timer.Start();
            return true;
        }
    }
}
