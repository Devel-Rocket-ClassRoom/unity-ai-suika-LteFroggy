using System;
using Suika.Board;
using Suika.Fruits;
using Suika.Merge;
using UnityEngine;

namespace Suika.Drop
{
    public sealed class DropController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField]
        BoardController board;

        [SerializeField]
        MergeResolver mergeResolver;

        [SerializeField]
        FruitTable fruitTable;

        [SerializeField]
        Fruit fruitPrefab;

        [SerializeField]
        Camera mainCamera;

        [SerializeField]
        SpriteRenderer previewRenderer;

        [Header("Drop Settings")]
        [SerializeField]
        float dropCooldown = 0.1f;

        [SerializeField]
        float previewAlpha = 0.5f;

        [Header("Drop Weights")]
        [SerializeField]
        float[] dropWeights = { 60f, 25f, 8f, 5f, 2f };

        FruitDefinition _currentDef;
        FruitDefinition _nextDef;
        float _cooldownRemaining;
        bool _canDrop;
        bool _inputEnabled = true;
        Vector3 _mouseScreenPos;

        public FruitDefinition CurrentFruit => _currentDef;
        public FruitDefinition NextFruit => _nextDef;

        public event Action<FruitDefinition, FruitDefinition> FruitQueueChanged;

        // ── 공개 API (GameManager #6) ─────────────────────────────────────────

        /// <summary>게임오버 시 드롭 입력을 차단, 재시작 시 다시 활성화한다.</summary>
        public void SetInputEnabled(bool enabled) => _inputEnabled = enabled;

        /// <summary>재시작 시 큐와 쿨다운을 초기화한다.</summary>
        public void ResetForRestart()
        {
            _inputEnabled = true;
            _cooldownRemaining = 0f;
            _canDrop = true;
            InitializeQueue();
        }

        void Start()
        {
            InitializeQueue();
            _canDrop = true;
        }

        void Update()
        {
            _mouseScreenPos = Input.mousePosition;

            UpdateCooldown(Time.deltaTime);
            UpdatePreviewPosition();

            if (
                _inputEnabled
                && _canDrop
                && (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
            )
                ExecuteDrop();
        }

        void ExecuteDrop()
        {
            if (_currentDef == null)
                return;

            _canDrop = false;

            float clampedX = board.ClampDropX(ScreenToWorld(_mouseScreenPos).x, _currentDef.Radius);
            var spawnPos = new Vector3(clampedX, board.DropSpawnY, 0f);

            var fruit = Instantiate(fruitPrefab, spawnPos, Quaternion.identity);
            fruit.Initialize(_currentDef, mergeResolver);

            _cooldownRemaining = dropCooldown;
            AdvanceQueue();
        }

        void InitializeQueue()
        {
            _currentDef = PickWeightedRandom();
            _nextDef = PickWeightedRandom();
            ApplyPreview();
            FruitQueueChanged?.Invoke(_currentDef, _nextDef);
        }

        void AdvanceQueue()
        {
            _currentDef = _nextDef;
            _nextDef = PickWeightedRandom();
            ApplyPreview();
            FruitQueueChanged?.Invoke(_currentDef, _nextDef);
        }

        void ApplyPreview()
        {
            if (previewRenderer == null || _currentDef == null)
                return;

            previewRenderer.sprite = _currentDef.Sprite;
            var color = _currentDef.TintWhenSpriteMissing;
            color.a = previewAlpha;
            previewRenderer.color = color;

            float spriteWorldSize =
                _currentDef.Sprite != null ? _currentDef.Sprite.bounds.size.x : 1f;
            float scale = (_currentDef.Radius * 2f) / spriteWorldSize;
            previewRenderer.transform.localScale = new Vector3(scale, scale, 1f);
        }

        void UpdatePreviewPosition()
        {
            if (previewRenderer == null || _currentDef == null)
                return;

            float clampedX = board.ClampDropX(ScreenToWorld(_mouseScreenPos).x, _currentDef.Radius);
            previewRenderer.transform.position = new Vector3(clampedX, board.DropSpawnY, 0f);
        }

        void UpdateCooldown(float dt)
        {
            if (_cooldownRemaining <= 0f)
                return;

            _cooldownRemaining -= dt;
            if (_cooldownRemaining <= 0f)
            {
                _cooldownRemaining = 0f;
                _canDrop = true;
            }
        }

        FruitDefinition PickWeightedRandom()
        {
            var pool = fruitTable.DroppableDefinitions;
            int count = Mathf.Min(pool.Count, dropWeights.Length);
            if (count <= 0)
            {
                Debug.LogError(
                    "[DropController] No droppable fruit definitions are configured.",
                    this
                );
                return null;
            }

            float total = 0f;
            for (int i = 0; i < count; i++)
                total += Mathf.Max(0f, dropWeights[i]);

            if (total <= 0f)
            {
                Debug.LogError(
                    "[DropController] Drop weights must contain at least one positive value.",
                    this
                );
                return pool[0];
            }

            float roll = UnityEngine.Random.Range(0f, total);
            float cumulative = 0f;
            for (int i = 0; i < count; i++)
            {
                cumulative += Mathf.Max(0f, dropWeights[i]);
                if (roll < cumulative)
                    return pool[i];
            }

            return pool[count - 1];
        }

        Vector3 ScreenToWorld(Vector3 screenPos)
        {
            var cam = mainCamera != null ? mainCamera : Camera.main;
            screenPos.z = 0f;
            var pos = cam.ScreenToWorldPoint(screenPos);
            return new Vector3(pos.x, pos.y, 0f);
        }

#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            if (board == null)
                return;

            float radius = _currentDef != null ? _currentDef.Radius : 0.18f;
            var (min, max) = board.GetDropXRange(radius);

            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(
                new Vector3(min, board.DropSpawnY, 0f),
                new Vector3(max, board.DropSpawnY, 0f)
            );
        }
#endif
    }
}
