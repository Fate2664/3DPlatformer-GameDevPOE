using System.Collections.Generic;
using UnityEngine;

namespace Platformer
{
    public class PlayerInventory : MonoBehaviour
    {
        private readonly Dictionary<KeyCollectibleData, int> keys = new();

        public void AddKey(KeyCollectibleData key)
        {
            keys.TryGetValue(key, out int amount);
            keys[key] = amount + 1;
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

            return true;
        }
    }
}
