using Platformer;
using UnityEngine;

public class LockInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private KeyCollectibleData requiredKey;
    [SerializeField] private PlatformMover platformToMove;

    private bool isUnlocked;

    public void Interact(PlayerController interactor)
    {
        if (isUnlocked || interactor == null)
            return;

        if (requiredKey == null)
        {
            //Does not have key
            return;
        }

        var inventory = interactor.GetComponent<PlayerInventory>();

        if (inventory.TryUseKey(requiredKey))
        {
            isUnlocked = true;
            platformToMove.Activate();
        }
    }
}
