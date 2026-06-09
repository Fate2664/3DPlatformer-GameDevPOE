using UnityEngine;
using UnityEngine.AI;

namespace Platformer
{
    //This script is for the enemy chasing state.
    public class EnemyChaseState : EnemyBaseState
    {
        private readonly NavMeshAgent agent;
        private readonly Transform player;
        private readonly float sprintSpeed = 2.0f;

        public EnemyChaseState(EnemyBase enemyBase, Animator animator, NavMeshAgent agent, Transform player) : base(enemyBase, animator)
        {
            this.agent = agent;
            this.player = player;
        }

        public override void OnEnter()
        {
            animator.CrossFade(runHash, crossFadeDuration);
            //Increase the enemy's speed 
            agent.speed += sprintSpeed;
        }
        
        //Constantly set the enemy's destination to the player position
        public override void Update()
        {
            agent.SetDestination(player.position);
        }

        public override void OnExit()
        {
            //Reset speed
            agent.speed -= sprintSpeed;
        }
    }
}
