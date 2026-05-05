using System;
using Nova;
using UnityEngine;
using UnityEngine.Events;

namespace Platformer
{
    //This is the player stats script. It manages all the stats of the player such as lives and score
    public class PlayerStats : MonoBehaviour
    {
        [SerializeField] private int startingLives = 3;
        [SerializeField] private int startingScore = 0;
        [SerializeField] private TextBlock scoreText;
        [SerializeField] private TextBlock livesText;

        public int Lives { get; private set; }
        public int Score { get; private set; }

        //Unity Actions for when lives or score changes
        public UnityAction<int> LivesChanged;
        public UnityAction<int> ScoreChanged;

        private void Awake()
        {
            //Initiate starting lives
            RestoreLives();
        }
        
        //Subscrible to the unity actions
        private void OnEnable()
        {
            LivesChanged += UpdateLivesText;
            ScoreChanged += UpdateScoreText;
        }
        
        //Unsubscribe from the unity actions
        private void OnDisable()
        {
            LivesChanged -= UpdateLivesText;
            ScoreChanged -= UpdateScoreText;
        }
        
        //Update text on start
        private void Start()
        {
            UpdateLivesText(Lives);
            UpdateScoreText(Score);
        }
        
        //This method increments the score by an amount
        public void IncrementScore(int amount)
        {
            Score += amount;
            ScoreChanged?.Invoke(Score);
        }
        
        //This method decrements the lives of the player
        public void DecrementLives()
        {
            if (Lives > 0)
            {
                Lives--;
                LivesChanged?.Invoke(Lives);
            }
        }
        
        //This method creates the snapshot of the player's score
        public PlayerStatsSnapshot CreateSnapshot()
        {
            return new PlayerStatsSnapshot(Score);
        }
        
        //This method restores the lives of the player
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
    
    //This struct takes a snapshot of the player's score. It will be used when create a snapshot when the player respawns 
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