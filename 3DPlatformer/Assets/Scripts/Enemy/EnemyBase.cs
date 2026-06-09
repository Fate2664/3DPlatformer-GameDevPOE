using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

namespace Platformer
{
    //This is the main enemy script. It manages the different states and executes player detection 
    public abstract class EnemyBase : Entity
    {
        [SerializeField] private float timeBetweenAttacks = 1f;
        [SerializeField] private float moveSpeed = 1f;
        
        protected StateMachine stateMachine;
        protected NavMeshAgent agent;
        protected Animator animator;
        protected PlayerDetector playerDetector;

        protected IState wanderState;
        protected IState chaseState;
        protected IState attackState;
        
        private CountDownTimer attackTimer;
        
        protected virtual void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            animator = GetComponentInChildren<Animator>();
            playerDetector = GetComponent<PlayerDetector>();
            //Create a new state machine for the enemy
            stateMachine = new StateMachine();
            attackTimer = new CountDownTimer(timeBetweenAttacks);
            agent.speed = moveSpeed;
        }
        
        //Define the two transition methods between states
        protected void At(IState from, IState to, IPredicate condition) => stateMachine.AddTransition(from, to, condition);
        protected void Any(IState to, IPredicate condition) => stateMachine.AddAnyTransition(to, condition);

        private void Update()
        {
            stateMachine.Update();
            attackTimer.Tick(Time.deltaTime);
        }

        private void FixedUpdate()
        {
            stateMachine.FixedUpdate(); 
        }
        
        //Attack logic
        public void Attack()
        {
            if (attackTimer.IsRunning) return;
            if (playerDetector.CanAttackPlayer())
            {
                //Respawn player
                Respawner respawner = playerDetector.Player.GetComponent<Respawner>();
                respawner.RespawnPlayer();
            }
            attackTimer.Start();
        }
    }
}
