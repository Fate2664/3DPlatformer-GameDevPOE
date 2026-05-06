using UnityEngine;

namespace Platformer
{
    //This is the predicate interface. It is used to evaluate the condition that needs to be met in order for a state change
    public interface IPredicate
    {
        bool Evaluate();
    }
}
