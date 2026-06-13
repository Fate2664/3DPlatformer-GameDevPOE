using System;
using DG.Tweening;
using Nova;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Platformer
{
    public class MenuManager : MonoBehaviour
    {
        [SerializeField] private UIBlock2D settingsUI;
        [SerializeField] private UIBlock2D pauseMenuUI;

        public static MenuManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void LoadBeginnerLevel()
        {
            SceneManager.LoadScene("BeginnerLevel");
        }

        public void ShowSettings()
        {
            if (settingsUI == null) return;
            
            settingsUI.transform.DOKill();
            settingsUI.transform.DOScale(1f, .5f).SetEase(Ease.OutBack);
        }

        public void HideSettings()
        {
            if (settingsUI == null) return;
            
            settingsUI.transform.DOKill();
            settingsUI.transform.DOScale(0f, .3f).SetEase(Ease.OutQuad);
        }

        public void ShowPauseMenu()
        {
            
        }
    }
}
