using UnityEngine;
using Suika.Board;
using Suika.Fruits;

namespace Suika.Game
{
    /// <summary>
    /// 씬에 존재하는 과일 중 위험 라인 위에 있는 것의 체류 시간을 추적한다.
    /// 체류 시간이 dangerLineDuration(기본 1.0초) 이상이면 GameManager.TriggerGameOver()를 호출한다.
    /// IsAnyFruitAboveLine 프로퍼티로 UI/VFX가 위험 상태를 표시할 수 있다.
    /// </summary>
    public sealed class GameOverDetector : MonoBehaviour
    {
        [Header("References")]
        [SerializeField]
        BoardController board;

        [SerializeField]
        GameManager gameManager;

        [Header("Settings")]
        [SerializeField]
        float dangerLineDuration = 1.0f;

        float _timer;

        /// <summary>
        /// 현재 위험 라인 위에 과일이 하나 이상 있으면 true.
        /// UI에서 위험 라인 강조 표시에 사용한다.
        /// </summary>
        public bool IsAnyFruitAboveLine { get; private set; }

        /// <summary>위험 라인 체류 누적 시간 (0 ~ dangerLineDuration)</summary>
        public float DangerTimer => _timer;

        void Update()
        {
            if (gameManager.State != GameState.Playing)
                return;

            IsAnyFruitAboveLine = CheckAnyFruitAboveLine();

            if (IsAnyFruitAboveLine)
            {
                _timer += Time.deltaTime;
                if (_timer >= dangerLineDuration)
                    gameManager.TriggerGameOver();
            }
            else
            {
                _timer = 0f;
            }
        }

        bool CheckAnyFruitAboveLine()
        {
            var fruits = FindObjectsByType<Fruit>(FindObjectsSortMode.None);
            foreach (var fruit in fruits)
            {
                if (board.IsAboveDangerLine(fruit.transform.position.y))
                    return true;
            }
            return false;
        }
    }
}
