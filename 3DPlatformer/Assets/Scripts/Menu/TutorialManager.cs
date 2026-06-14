using System;
using DG.Tweening;
using Nova;
using Unity.VisualScripting;
using UnityEngine;

namespace Platformer
{
    public class TutorialManager : MonoBehaviour
    {
        [SerializeField] private InputReader input;
        [SerializeField] private UIBlock2D tutorialBlock;

        private void Start()
        {
            if (tutorialBlock == null) return;

            Time.timeScale = 0;
        }

        private void Update()
        {
            if (input == null || tutorialBlock == null) return;

            if (input.NextPressed)
            {
                tutorialBlock.transform.DOScale(0f, 0.25f).SetEase(Ease.OutQuad).OnComplete(() =>
                {
                    tutorialBlock.gameObject.SetActive(false);
                });
                Time.timeScale = 1;
            }
        }
    }
}