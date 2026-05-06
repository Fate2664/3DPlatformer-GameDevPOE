using UnityEngine;

namespace Platformer
{
    //This is the base state interface that all states should inherit from and can implement the methods
    public interface IState
    {
        void OnEnter();
        void Update();
        void FixedUpdate();
        void OnExit();
    }
}
