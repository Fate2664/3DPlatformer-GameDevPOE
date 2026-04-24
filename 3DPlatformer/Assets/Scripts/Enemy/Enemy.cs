using System;
using UnityEngine;
using UnityEngine.AI;

namespace Platformer
{
    public class Enemy : MonoBehaviour
    {
        [SerializeField] private float wanderRadius = 5f;
        [SerializeField] private float timeBetweenAttacks = 1f;
        
        private NavMeshAgent agent;
        private Animator animator;
        private StateMachine stateMachine;
        private PlayerDetector playerDetector;
        private CountDownTimer attackTimer;
        
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
            var wanderState = new EnemyWanderState(this, animator, agent, wanderRadius);
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

        public void Attack()
        {
            if (attackTimer.IsRunning) return;

            attackTimer.Start();
        }
    }
}
