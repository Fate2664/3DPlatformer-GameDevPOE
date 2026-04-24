using UnityEngine;
using UnityEngine.AI;

namespace Platformer
{
    public class EnemyChaseState : EnemyBaseState
    {
        private readonly NavMeshAgent agent;
        private readonly Transform player;

        public EnemyChaseState(Enemy enemy, Animator animator, NavMeshAgent agent, Transform player) : base(enemy, animator)
        {
            this.player = player;
            this.agent = agent;
        }

        public override void OnEnter()
        {
            animator.CrossFade(runHash, crossFadeDuration);
            agent.speed += 2;
        }

        public override void Update()
        {
            agent.SetDestination(player.position);
        }

        public override void OnExit()
        {
            agent.speed -= 2;
        }
    }
}
