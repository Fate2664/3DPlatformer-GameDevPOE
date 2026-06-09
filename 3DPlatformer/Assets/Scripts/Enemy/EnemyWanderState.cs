using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Platformer
{
    //This is the script for the enemy wander state
    public class EnemyWanderState : EnemyBaseState
    {
        private readonly NavMeshAgent agent;
        //Custom linked list for waypoint points
        private readonly LinkedListBase<Transform> wanderPoints;
        
        private Node<Transform> currentPoint;

        public EnemyWanderState(EnemyBase enemyBase, Animator animator, NavMeshAgent agent, LinkedListBase<Transform> wanderPoints) : base(enemyBase, animator)
        {
            this.agent = agent;
            this.wanderPoints = wanderPoints;
        }

        public override void OnEnter()
        {
            animator.CrossFade(walkHash,  crossFadeDuration);
            
            //Set the enemy's destination to the first wander point in the linked list
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
