using UnityEngine;

namespace Platformer
{
    public class BossEnemy : EnemyBase
    {
        private GraphBase<Transform> wanderGraph;
        
        public void Start()
        {
            wanderState = new BossWanderState(this, animator, agent, wanderGraph);
            chaseState = new EnemyChaseState(this, animator, agent, playerDetector.Player);
            attackState = new EnemyAttackState(this, animator, agent, playerDetector.Player);
            
            //Define state transitions and conditions
            At(wanderState, chaseState, new FuncPredicate(() => playerDetector.CanDetectPlayer()));
            At(chaseState, wanderState, new FuncPredicate(() => !playerDetector.CanDetectPlayer()));
            At(chaseState, attackState, new FuncPredicate(() => playerDetector.CanAttackPlayer()));
            At(attackState, chaseState, new FuncPredicate(() => !playerDetector.CanAttackPlayer()));
            
            stateMachine.SetState(wanderState);
        }
        
        public void SetWanderGraph(GraphBase<Transform> graph) => wanderGraph = graph;
        
        
    }
}