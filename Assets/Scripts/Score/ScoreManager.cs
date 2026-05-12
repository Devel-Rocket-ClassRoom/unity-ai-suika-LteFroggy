using System;
using Suika.Merge;
using UnityEngine;

namespace Suika.Score
{
    public sealed class ScoreManager : MonoBehaviour
    {
        [SerializeField]
        string highScoreKey = "Suika.HighScore";

        [SerializeField]
        bool resetScoreOnStart = true;

        public int CurrentScore { get; private set; }
        public int HighScore { get; private set; }

        public event Action<int> ScoreChanged;
        public event Action<int> HighScoreChanged;

        void Awake()
        {
            HighScore = PlayerPrefs.GetInt(highScoreKey, 0);

            if (resetScoreOnStart)
                CurrentScore = 0;
        }

        void OnEnable()
        {
            MergeResolver.MergeScored += AddScore;
        }

        void OnDisable()
        {
            MergeResolver.MergeScored -= AddScore;
        }

        void Start()
        {
            ScoreChanged?.Invoke(CurrentScore);
            HighScoreChanged?.Invoke(HighScore);
        }

        public void AddScore(int amount)
        {
            if (amount <= 0)
                return;

            CurrentScore += amount;
            ScoreChanged?.Invoke(CurrentScore);

            if (CurrentScore <= HighScore)
                return;

            HighScore = CurrentScore;
            PlayerPrefs.SetInt(highScoreKey, HighScore);
            PlayerPrefs.Save();
            HighScoreChanged?.Invoke(HighScore);
        }

        public void ResetRunScore()
        {
            CurrentScore = 0;
            ScoreChanged?.Invoke(CurrentScore);
        }
    }
}
