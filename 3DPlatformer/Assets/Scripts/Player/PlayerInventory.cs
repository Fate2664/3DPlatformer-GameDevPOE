using System;
using System.Collections.Generic;
using DG.Tweening;
using Nova;
using UnityEngine;

namespace Platformer
{
    public class PlayerInventory : MonoBehaviour
    {
        [SerializeField] private UIBlock2D keyUI;
        
        private readonly Dictionary<KeyCollectibleData, int> keys = new();

        private void Awake()
        {
            //Hide key icon on awake
            keyUI.transform.localScale = Vector3.zero;
        }

        public void AddKey(KeyCollectibleData key)
        {
            keys.TryGetValue(key, out int amount);
            keys[key] = amount + 1;
            
            //Show key UI
            keyUI.transform.DOScale(1f, 0.2f).SetEase(Ease.InOutQuad);
        }

        public bool HasKey(KeyCollectibleData key)
        {
            return key != null && keys.TryGetValue(key, out int amount) && amount > 0;
        }

        public bool TryUseKey(KeyCollectibleData key)
        {
            if (!HasKey(key))
                return false;

            int remaining = keys[key] - 1;
            if (remaining == 0)
                keys.Remove(key);
            else
                keys[key] = remaining;
            
            //Hide key UI
            keyUI.transform.DOScale(0f, 0.2f).SetEase(Ease.InOutQuad);
            
            return true;
        }
    }
}
