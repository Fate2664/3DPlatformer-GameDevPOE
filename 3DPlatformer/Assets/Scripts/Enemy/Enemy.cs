using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Platformer
{
    public class Enemy : Entity
    {
        [SerializeField] private float timeBetweenAttacks = 1f;
        
        private NavMeshAgent agent;
        private Animator animator;
        private StateMachine stateMachine;
        private PlayerDetector playerDetector;
        private CountDownTimer attackTimer;
        private LinkedListBase<Transform> wanderPoints;
        
        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            animator = GetComponentInChildren<Animator>();
            playerDetector = GetComponent<PlayerDetector>();
            stateMachine = new StateMachine();
            attackTimer = new CountDownTimer(timeBetweenAttacks);
        }

        private void Start()
        {
            var wanderState = new EnemyWanderState(this, animator, agent, wanderPoints);
            var chaseState = new EnemyChaseState(this, animator, agent, playerDetector.Player);
            var attackState = new EnemyAttackState(this, animator, agent, playerDetector.Player);
            
            At(wanderState, chaseState, new FuncPredicate(() => playerDetector.CanDetectPlayer()));
            At(chaseState, wanderState, new FuncPredicate(() => !playerDetector.CanDetectPlayer()));
            At(chaseState, attackState, new FuncPredicate(() => playerDetector.CanAttackPlayer()));
            At(attackState, chaseState, new FuncPredicate(() => !playerDetector.CanAttackPlayer()));
            
            stateMachine.SetState(wanderState);
        }
        
        private void At(IState from, IState to, IPredicate condition) => stateMachine.AddTransition(from, to, condition);
        private void Any(IState to, IPredicate condition) => stateMachine.AddAnyTransition(to, condition);

        private void Update()
        {
            stateMachine.Update();
            attackTimer.Tick(Time.deltaTime);
        }

        private void FixedUpdate()
        {
            stateMachine.FixedUpdate(); 
        }

        public void SetWanderPath(Transform[] points)
        {
            wanderPoints = new LinkedListBase<Transform>();
            foreach (Transform wanderPoint in points)
            {
                wanderPoints.AddLast(new Node<Transform>(wanderPoint));
            }
        }

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
