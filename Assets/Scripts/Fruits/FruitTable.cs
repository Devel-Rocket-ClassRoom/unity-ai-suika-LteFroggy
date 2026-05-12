using System.Collections.Generic;
using UnityEngine;

namespace Suika.Fruits
{
    /// <summary>
    /// 11단계 FruitDefinition을 모아두는 룩업 테이블.
    /// MergeResolver(#4), FruitSpawner(#5)에서 이 에셋 하나만 참조한다.
    /// </summary>
    [CreateAssetMenu(menuName = "Suika/Fruit Table", fileName = "FruitTable")]
    public sealed class FruitTable : ScriptableObject
    {
        public const int MaxLevel = 11;

        [SerializeField] FruitDefinition[] definitions;

        List<FruitDefinition> _droppableCache;

        // ── 조회 API ──────────────────────────────────────────────────────────

        /// <summary>레벨(1~11)로 정의를 반환한다.</summary>
        public FruitDefinition GetByLevel(int level)
        {
            foreach (var def in definitions)
                if (def != null && def.Level == level) return def;
            return null;
        }

        /// <summary>
        /// 머지 결과로 생성될 다음 단계 정의를 반환한다.
        /// 수박(레벨 11)이면 null — 양쪽 제거 + 보너스 케이스(MergeResolver가 처리).
        /// </summary>
        public FruitDefinition GetNextLevel(FruitDefinition current)
        {
            if (current == null || current.Level >= MaxLevel) return null;
            return GetByLevel(current.Level + 1);
        }

        /// <summary>
        /// 랜덤 드롭 풀. canDropFromQueue == true인 항목만 포함.
        /// FruitSpawner에서 가중치 뽑기에 사용한다.
        /// </summary>
        public IReadOnlyList<FruitDefinition> DroppableDefinitions
        {
            get
            {
                if (_droppableCache == null) RebuildDropCache();
                return _droppableCache;
            }
        }

        // ── 내부 ─────────────────────────────────────────────────────────────

        void RebuildDropCache()
        {
            _droppableCache = new List<FruitDefinition>();
            if (definitions == null) return;
            foreach (var def in definitions)
                if (def != null && def.CanDropFromQueue)
                    _droppableCache.Add(def);
        }

        void OnValidate()
        {
            _droppableCache = null; // 인스펙터 변경 시 캐시 무효화

#if UNITY_EDITOR
            if (definitions != null && definitions.Length != MaxLevel)
                Debug.LogWarning($"[FruitTable] definitions 배열 길이가 {MaxLevel}이어야 합니다. (현재 {definitions.Length})", this);
#endif
        }

#if UNITY_EDITOR
        [ContextMenu("드롭 가능 과일 목록 출력")]
        void LogDroppable()
        {
            RebuildDropCache();
            var sb = new System.Text.StringBuilder("[FruitTable] 드롭 가능 과일:\n");
            foreach (var d in _droppableCache)
                sb.AppendLine($"  Lv{d.Level} {d.DisplayName}");
            Debug.Log(sb.ToString(), this);
        }
#endif
    }
}
