using System;
using UnityEngine;

namespace Platformer
{
    public class LevelIntroSound : MonoBehaviour
    {
        private void Start()
        {
            SoundEffectsManager.Instance.Play("StartLevel");
        }
    }
}