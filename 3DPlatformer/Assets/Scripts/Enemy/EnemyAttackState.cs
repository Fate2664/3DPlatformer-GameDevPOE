using UnityEngine;
using UnityEngine.AI;

namespace Platformer
{
    //This is the enemy attack state script
    public class EnemyAttackState : EnemyBaseState
    {
        private readonly NavMeshAgent agent;
        private readonly Transform player;

        public EnemyAttackState(Enemy enemy, Animator animator, NavMeshAgent agent, Transform player) : base(enemy, animator)
        {
            this.agent = agent;
            this.player = player;
        }

        public override void OnEnter()
        {
            animator.CrossFade(attackHash, crossFadeDuration);
        }
        
        //Go for the player and call the attack method
        public override void Update()
        {
            agent.SetDestination(player.position);
            enemy.Attack();
        }
    }
}
