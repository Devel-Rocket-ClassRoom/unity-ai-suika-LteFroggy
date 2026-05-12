using UnityEngine;

namespace Suika.Board
{
    /// <summary>
    /// 보드 경계, 위험 라인, 드롭 가능 범위 정보를 다른 시스템에 제공한다.
    /// 인스펙터에 노출된 Transform/수치를 기준으로 동작하므로
    /// 씬에서 벽 오브젝트를 이동하면 자동으로 반영된다.
    /// </summary>
    public sealed class BoardController : MonoBehaviour
    {
        [Header("Walls / Floor")]
        [SerializeField]
        Transform leftWall;

        [SerializeField]
        Transform rightWall;

        [SerializeField]
        Transform floor;

        [Header("Lines")]
        [SerializeField]
        float dangerLineY = 3.45f;

        [SerializeField]
        float dropSpawnY = 4.15f;

        // ── 경계 프로퍼티 ─────────────────────────────────────────────────────
        public float LeftX => leftWall.position.x;
        public float RightX => rightWall.position.x;
        public float FloorY => floor.position.y;
        public float DangerLineY => dangerLineY;
        public float DropSpawnY => dropSpawnY;
        public float InnerWidth => RightX - LeftX;

        // ── 드롭 X 범위 API ───────────────────────────────────────────────────

        /// <summary>
        /// 주어진 과일 반지름을 고려해 드롭 가능한 x 범위를 반환한다.
        /// DropController, FruitSpawner 등에서 사용한다.
        /// </summary>
        public (float min, float max) GetDropXRange(float fruitRadius)
        {
            return (LeftX + fruitRadius, RightX - fruitRadius);
        }

        /// <summary>
        /// 임의의 x 좌표를 드롭 가능 범위 안으로 clamp 해서 반환한다.
        /// </summary>
        public float ClampDropX(float x, float fruitRadius)
        {
            var (min, max) = GetDropXRange(fruitRadius);
            return Mathf.Clamp(x, min, max);
        }

        // ── 위험 라인 판단 ────────────────────────────────────────────────────

        /// <summary>
        /// 해당 y 좌표가 위험 라인 위에 있는지 검사한다.
        /// 체류 시간 추적은 GameOverDetector(이슈 #6)에서 처리한다.
        /// </summary>
        public bool IsAboveDangerLine(float y) => y > dangerLineY;

        // ── 머지 위치 보정 ────────────────────────────────────────────────────

        /// <summary>
        /// 머지 생성 위치를 보드 내부로 clamp 한다.
        /// MergeResolver(이슈 #4)에서 사용한다.
        /// </summary>
        public Vector2 ClampMergePosition(Vector2 pos, float fruitRadius)
        {
            float x = Mathf.Clamp(pos.x, LeftX + fruitRadius, RightX - fruitRadius);
            float y = Mathf.Max(pos.y, FloorY + fruitRadius);
            return new Vector2(x, y);
        }

#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            // 보드 외곽 (흰색)
            Gizmos.color = Color.white;
            Gizmos.DrawWireCube(
                new Vector3(0f, (dangerLineY + FloorY) * 0.5f, 0f),
                new Vector3(InnerWidth, dangerLineY - FloorY, 0f)
            );

            // 위험 라인 (빨간색)
            Gizmos.color = Color.red;
            Gizmos.DrawLine(
                new Vector3(LeftX, dangerLineY, 0f),
                new Vector3(RightX, dangerLineY, 0f)
            );

            // 드롭 스폰 라인 (노란색)
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(
                new Vector3(LeftX, dropSpawnY, 0f),
                new Vector3(RightX, dropSpawnY, 0f)
            );
        }
#endif
    }
}
