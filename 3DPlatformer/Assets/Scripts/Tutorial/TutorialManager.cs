using System;
using Nova;
using Unity.VisualScripting;
using UnityEngine;

namespace Platformer
{
    public class TutorialManager : MonoBehaviour
    {
        [SerializeField] private InputReader input;
        [SerializeField] private UIBlock2D tutorialBlock;

        private void Awake()
        {
            Time.timeScale = 0;
        }

        private void Update()
        {
            if (input == null) return;
            
            if (input.NextPressed)
            {
                tutorialBlock.gameObject.SetActive(false);
                Time.timeScale = 1;
            }
        }
    }
}
