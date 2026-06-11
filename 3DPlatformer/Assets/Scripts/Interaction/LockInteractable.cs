using System;
using Nova;
using Platformer;
using UnityEngine;

public class LockInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private PlayerInteractionDetector playerInteractionDetector;
    [SerializeField] private KeyCollectibleData requiredKey;
    [SerializeField] private PlatformMover platformToMove;
    [SerializeField] private PlayerInventory playerInventory;
    [SerializeField] private Sprite lockSprite;
    [SerializeField] private Sprite keySprite;
    [SerializeField] private Material lockIconMaterial;
    [SerializeField] private Material keyIconMaterial;

    private IndicatorManager indicatorManager;
    private bool isUnlocked;

    private void Awake()
    {
        indicatorManager = GetComponentInChildren<IndicatorManager>();
    }

    private void FixedUpdate()
    {
        
        if (indicatorManager == null || playerInteractionDetector == null) return;

        if (ReferenceEquals(playerInteractionDetector.CurrentTarget, this))
        {
            if (playerInventory.HasKey(requiredKey)) //Has key - show key icon with green material
            {
                indicatorManager.icon = keySprite;
                indicatorManager.iconMaterial = keyIconMaterial;
            }
            else //Does not have key - show lock icon with red material
            {
                indicatorManager.icon = lockSprite;
                indicatorManager.iconMaterial = lockIconMaterial;
            }
            indicatorManager.ShowIndictor();
        }
        else
        {
            indicatorManager.HideIndictor();
        }
    }

    public void Interact(PlayerController interactor)
    {
        if (isUnlocked || interactor == null)
            return;


        if (playerInventory.TryUseKey(requiredKey))
        {
            isUnlocked = true;
            platformToMove.Activate();
            Destroy(gameObject);
        }
    }
}