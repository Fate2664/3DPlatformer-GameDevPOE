using UnityEngine;

namespace Platformer
{
    public class CommonEnemy : EnemyBase
    {
        private LinkedListBase<Transform> wanderPoints;

        public void Start()
        {
            wanderState = new EnemyWanderState(this, animator, agent, wanderPoints);
            chaseState = new EnemyChaseState(this, animator, agent, playerDetector.Player);
            attackState = new EnemyAttackState(this, animator, agent, playerDetector.Player);
            
            //Define state transitions and conditions
            At(wanderState, chaseState, new FuncPredicate(() => playerDetector.CanDetectPlayer()));
            At(chaseState, wanderState, new FuncPredicate(() => !playerDetector.CanDetectPlayer()));
            At(chaseState, attackState, new FuncPredicate(() => playerDetector.CanAttackPlayer()));
            At(attackState, chaseState, new FuncPredicate(() => !playerDetector.CanAttackPlayer()));
            
            stateMachine.SetState(wanderState);
        }
        
        
        //Add waypoint positions to custom linked list
        public void SetWanderPath(Transform[] points)
        {
            wanderPoints = new LinkedListBase<Transform>();
            foreach (Transform wanderPoint in points)
            {
                wanderPoints.AddLast(new Node<Transform>(wanderPoint));
            }
        }
    }
}