using System;
using UnityEngine;
using Suika.Drop;
using Suika.Merge;
using Suika.Score;
using Suika.Fruits;

namespace Suika.Game
{
    /// <summary>
    /// 게임 상태(Playing / GameOver)를 관리한다.
    /// 게임오버 트리거, 재시작 초기화, R키 입력을 처리한다.
    /// </summary>
    public sealed class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("References")]
        [SerializeField]
        DropController dropController;

        [SerializeField]
        MergeResolver mergeResolver;

        [SerializeField]
        ScoreManager scoreManager;

        // ── 상태 ─────────────────────────────────────────────────────────────

        public GameState State { get; private set; } = GameState.Playing;

        /// <summary>게임오버 패널에서 사용하는 최종 점수</summary>
        public int FinalScore => scoreManager != null ? scoreManager.CurrentScore : 0;

        /// <summary>게임오버 패널에서 사용하는 최고 점수</summary>
        public int FinalHighScore => scoreManager != null ? scoreManager.HighScore : 0;

        // ── 이벤트 ───────────────────────────────────────────────────────────

        public event Action<GameState> StateChanged;
        public event Action GameOverTriggered;

        // ── 생명주기 ─────────────────────────────────────────────────────────

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
                Restart();
        }

        // ── 공개 API ─────────────────────────────────────────────────────────

        /// <summary>
        /// 위험 라인 체류 1.0초 초과 시 GameOverDetector가 호출한다.
        /// 드롭 입력과 머지 처리를 차단하고 GameOverTriggered 이벤트를 발행한다.
        /// </summary>
        public void TriggerGameOver()
        {
            if (State != GameState.Playing)
                return;

            State = GameState.GameOver;
            dropController.SetInputEnabled(false);
            mergeResolver.SetMergeEnabled(false);

            StateChanged?.Invoke(State);
            GameOverTriggered?.Invoke();
        }

        /// <summary>
        /// 모든 과일을 제거하고 점수, 드롭 큐, 게임 상태를 초기화한다.
        /// R키 또는 UI 버튼에서 호출한다.
        /// </summary>
        public void Restart()
        {
            var fruits = FindObjectsByType<Fruit>(FindObjectsSortMode.None);
            foreach (var fruit in fruits)
                Destroy(fruit.gameObject);

            scoreManager.ResetRunScore();
            mergeResolver.SetMergeEnabled(true);
            dropController.ResetForRestart();

            State = GameState.Playing;
            StateChanged?.Invoke(State);
        }
    }
}
