using UnityEngine;

namespace Platformer
{
    //This is the base enemy state. All enemy states will inherit from this class
    public abstract class EnemyBaseState : IState
    {
        protected readonly Enemy enemy;
        protected readonly Animator animator;
        
        //Get animation hashes 
        protected static readonly int idleHash = Animator.StringToHash("Idle");
        protected static readonly int walkHash = Animator.StringToHash("Walk");
        protected static readonly int jumpHash = Animator.StringToHash("Jump");
        protected static readonly int runHash = Animator.StringToHash("Run");
        protected static readonly int attackHash = Animator.StringToHash("Attack"); //Currently don't have an enemy attack animation
        
        protected const float crossFadeDuration = 0.2f;

        protected EnemyBaseState(Enemy enemy, Animator animator)
        {
            this.enemy = enemy;
            this.animator = animator;
        }
        
        public virtual void OnEnter() { }
        public virtual void Update() { }
        public virtual void FixedUpdate() { }
        public virtual void OnExit() { }
    }
}
