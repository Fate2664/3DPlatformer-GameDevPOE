using UnityEngine;

namespace Platformer
{
    //This interface is a contract for all detection strategies to inherit from
    public interface IDetectionStrategy
    {
        bool Execute(Transform player, Transform detector, CountDownTimer timer);
    }
}
