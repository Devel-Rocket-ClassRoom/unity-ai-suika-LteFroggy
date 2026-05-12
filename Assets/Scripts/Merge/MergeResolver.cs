using System;
using System.Collections.Generic;
using UnityEngine;
using Suika.Board;
using Suika.Fruits;

namespace Suika.Merge
{
    public sealed class MergeResolver : MonoBehaviour
    {
        const int WatermelonBonus = 120;

        [SerializeField] Fruit fruitPrefab;
        [SerializeField] FruitTable fruitTable;
        [SerializeField] BoardController board;

        readonly List<(Fruit a, Fruit b)> _pendingMerges = new();
        readonly HashSet<int> _pendingIds = new();

        bool _mergeEnabled = true;

        // ScoreManager(#5)에서 구독
        public static event Action<int> MergeScored;

        /// <summary>게임오버 시 머지 처리를 차단, 재시작 시 다시 활성화한다.</summary>
        public void SetMergeEnabled(bool enabled) => _mergeEnabled = enabled;

        public void RequestMerge(Fruit a, Fruit b)
        {
            if (!_mergeEnabled)
                return;

            int idA = a.GetInstanceID();
            int idB = b.GetInstanceID();
            if (_pendingIds.Contains(idA) || _pendingIds.Contains(idB))
                return;

            _pendingIds.Add(idA);
            _pendingIds.Add(idB);
            _pendingMerges.Add((a, b));
        }

        void LateUpdate()
        {
            for (int i = 0; i < _pendingMerges.Count; i++)
            {
                var (a, b) = _pendingMerges[i];
                if (a == null || b == null || a.HasMerged || b.HasMerged)
                    continue;

                ProcessMerge(a, b);
            }

            _pendingMerges.Clear();
            _pendingIds.Clear();
        }

        void ProcessMerge(Fruit a, Fruit b)
        {
            a.MarkMerged();
            b.MarkMerged();

            Vector2 mergePos = ((Vector2)a.transform.position + (Vector2)b.transform.position) * 0.5f;

            FruitDefinition nextDef = fruitTable.GetNextLevel(a.Definition);
            if (nextDef != null)
            {
                Vector2 spawnPos = board.ClampMergePosition(mergePos, nextDef.Radius);
                var spawned = Instantiate(fruitPrefab, spawnPos, Quaternion.identity);
                spawned.Initialize(nextDef, this);
                MergeScored?.Invoke(nextDef.Score);
            }
            else
            {
                // 수박 2개 머지: 새 과일 없이 보너스 점수
                MergeScored?.Invoke(WatermelonBonus);
            }

            Destroy(a.gameObject);
            Destroy(b.gameObject);
        }
    }
}
