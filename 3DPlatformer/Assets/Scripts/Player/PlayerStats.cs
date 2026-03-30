using System;
using Nova;
using UnityEngine;
using UnityEngine.Events;

namespace Platformer
{
    public class PlayerStats : MonoBehaviour
    {
        [SerializeField] private int startingLives = 3;
        [SerializeField] private int startingScore = 0;
        [SerializeField] private TextBlock scoreText;
        [SerializeField] private TextBlock livesText;

        public int Lives { get; private set; }
        public int Score { get; private set; }

        public UnityAction<int> LivesChanged;
        public UnityAction<int> ScoreChanged;

        private void Awake()
        {
            RestoreLives();
        }

        private void OnEnable()
        {
            LivesChanged += UpdateLivesText;
            ScoreChanged += UpdateScoreText;
        }

        private void OnDisable()
        {
            LivesChanged -= UpdateLivesText;
            ScoreChanged -= UpdateScoreText;
        }

        private void Start()
        {
            UpdateLivesText(Lives);
            UpdateScoreText(Score);
        }

        public void IncrementScore(int amount)
        {
            Score += amount;
            ScoreChanged?.Invoke(Score);
        }

        public void DecrementLives()
        {
            if (Lives > 0)
            {
                Lives--;
                LivesChanged?.Invoke(Lives);
            }
        }

        public PlayerStatsSnapshot CreateSnapshot()
        {
            return new PlayerStatsSnapshot(Score);
        }

        public void RestoreLives()
        {
            Lives = startingLives;
            LivesChanged?.Invoke(Lives);
        }

        private void UpdateScoreText(int score)
        {
            scoreText.Text = score.ToString();
        }

        private void UpdateLivesText(int lives)
        {
            livesText.Text = lives.ToString();
        }
    }

    [Serializable]
    public struct PlayerStatsSnapshot
    {
        public int Score { get; }

        public PlayerStatsSnapshot(int score)
        {
            Score = score;
        }
    }
}