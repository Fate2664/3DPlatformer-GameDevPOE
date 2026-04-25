using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Platformer
{
    public class EnemyWanderState : EnemyBaseState
    {
        private readonly NavMeshAgent agent;
        private readonly LinkedListBase<Transform> wanderPoints;
        
        private Node<Transform> currentPoint;

        public EnemyWanderState(Enemy enemy, Animator animator, NavMeshAgent agent, LinkedListBase<Transform> wanderPoints) : base(enemy, animator)
        {
            this.agent = agent;
            this.wanderPoints = wanderPoints;
        }

        public override void OnEnter()
        {
            animator.CrossFade(walkHash,  crossFadeDuration);
            
            currentPoint ??= wanderPoints.First;
            agent.SetDestination(currentPoint.Data.position);
        }

        public override void Update()
        {
            if (HasReachedDestination())
            {
                //If next point is null -> go back to the first point
                currentPoint = currentPoint.Next ?? wanderPoints.First;
                
                agent.SetDestination(currentPoint.Data.position);
            }
        }

        private bool HasReachedDestination()
        {
            return !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance && (!agent.hasPath || agent.velocity.sqrMagnitude == 0f);
        }
    }
}
