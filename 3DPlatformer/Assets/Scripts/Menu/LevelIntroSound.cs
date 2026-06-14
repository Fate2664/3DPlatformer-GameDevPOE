using System;
using UnityEngine;

namespace Platformer
{
    public class LevelIntroSound : MonoBehaviour
    {
        private void Start()
        {
            AudioManager.Instance.Play("StartLevel");
        }
    }
}