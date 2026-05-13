using UnityEngine;
using Suika.Game;

namespace Suika.Board
{
    /// <summary>
    /// GameOverDetector.IsAnyFruitAboveLine 상태에 따라 위험 라인 SpriteRenderer 색상을 전환한다.
    /// 위험 상태에서는 warningColor로 PingPong 펄스, 평상시에는 normalColor 유지.
    /// GDD 6.3 "위험 라인 초과 중 라인을 붉게 강조" 충족.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class DangerLineView : MonoBehaviour
    {
        [Header("References")]
        [SerializeField]
        GameOverDetector detector;

        [Header("Colors")]
        [SerializeField]
        Color normalColor = new Color(0.9f, 0.2f, 0.2f, 0.45f);

        [SerializeField]
        Color warningColor = new Color(1f, 0f, 0f, 0.9f);

        [Header("Pulse")]
        [SerializeField]
        float pulseSpeed = 3f;

        SpriteRenderer _renderer;

        void Awake()
        {
            _renderer = GetComponent<SpriteRenderer>();
            _renderer.color = normalColor;
        }

        void Update()
        {
            if (detector == null) return;

            if (detector.IsAnyFruitAboveLine)
            {
                float t = Mathf.PingPong(Time.time * pulseSpeed, 1f);
                _renderer.color = Color.Lerp(normalColor, warningColor, t);
            }
            else
            {
                _renderer.color = normalColor;
            }
        }
    }
}
