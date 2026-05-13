using Suika.Drop;
using Suika.Fruits;
using Suika.Game;
using Suika.Score;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Suika.UI
{
    public sealed class PlayHUD : MonoBehaviour
    {
        [Header("Game References")]
        [SerializeField] GameManager gameManager;
        [SerializeField] ScoreManager scoreManager;
        [SerializeField] DropController dropController;

        [Header("Score")]
        [SerializeField] TMP_Text scoreText;
        [SerializeField] TMP_Text highScoreText;

        [Header("Next Fruit")]
        [SerializeField] Image nextFruitImage;

        [Header("Buttons")]
        [SerializeField] Button restartButton;

        void OnEnable()
        {
            scoreManager.ScoreChanged += OnScoreChanged;
            scoreManager.HighScoreChanged += OnHighScoreChanged;
            dropController.FruitQueueChanged += OnFruitQueueChanged;
            gameManager.StateChanged += OnStateChanged;
            restartButton.onClick.AddListener(OnRestartClicked);
        }

        void OnDisable()
        {
            scoreManager.ScoreChanged -= OnScoreChanged;
            scoreManager.HighScoreChanged -= OnHighScoreChanged;
            dropController.FruitQueueChanged -= OnFruitQueueChanged;
            gameManager.StateChanged -= OnStateChanged;
            restartButton.onClick.RemoveListener(OnRestartClicked);
        }

        void OnScoreChanged(int score) => scoreText.text = score.ToString("N0");

        void OnHighScoreChanged(int highScore) => highScoreText.text = highScore.ToString("N0");

        void OnFruitQueueChanged(FruitDefinition current, FruitDefinition next)
        {
            if (nextFruitImage == null || next == null)
                return;
            nextFruitImage.sprite = next.Sprite;
            nextFruitImage.color = next.Sprite != null ? Color.white : next.TintWhenSpriteMissing;
        }

        void OnStateChanged(GameState state)
        {
            gameObject.SetActive(state == GameState.Playing);
        }

        void OnRestartClicked() => gameManager.Restart();
    }
}
