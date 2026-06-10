using Platformer;
using UnityEngine;

public class PlayerInteractionDetector : MonoBehaviour
{
    [SerializeField] private InputReader input;
    private PlayerController player;
    private IInteractable currentTarget;
    private Collider currentIteractableObject;

    public IInteractable CurrentTarget => currentTarget;
    public Collider CurrentIteractableObject => currentIteractableObject;

    private void Awake()
    {
        player = GetComponent<PlayerController>();
        input ??= GetComponent<InputReader>();
    }

    private void Update()
    {
        if (input.InteractPressed && currentTarget != null)
        {
            currentTarget.Interact(player);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        IInteractable interactable = other.GetComponentInParent<IInteractable>();
        if (interactable != null && IsInteractionCollider(other))
        {
            currentTarget = interactable;
            currentIteractableObject = other;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        IInteractable interactable = other.GetComponentInParent<IInteractable>();
        if (interactable != null && interactable == currentTarget && IsInteractionCollider(other))
        {
            currentTarget = null;
            currentIteractableObject = null;
        }
    }

    private static bool IsInteractionCollider(Collider other)
    {
        return other.CompareTag("InteractionTrigger");
    }
}
