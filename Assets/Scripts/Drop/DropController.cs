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
        float dropCooldown = 0.5f;

        [SerializeField]
        float previewAlpha = 0.5f;

        [Header("Drop Weights — FruitSpawner(#5) 전 임시")]
        [SerializeField]
        float[] dropWeights = { 60f, 25f, 8f, 5f, 2f };

        FruitDefinition _currentDef;
        float _cooldownRemaining;
        bool _canDrop;
        Vector3 _mouseScreenPos;

        void Start()
        {
            PrepareNextFruit();
            _canDrop = true;
        }

        void Update()
        {
            _mouseScreenPos = Input.mousePosition;

            UpdateCooldown(Time.deltaTime);
            UpdatePreviewPosition();

            if (_canDrop && (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space)))
                ExecuteDrop();
        }

        void ExecuteDrop()
        {
            _canDrop = false;

            float clampedX = board.ClampDropX(ScreenToWorld(_mouseScreenPos).x, _currentDef.Radius);
            var spawnPos = new Vector3(clampedX, board.DropSpawnY, 0f);

            var fruit = Instantiate(fruitPrefab, spawnPos, Quaternion.identity);
            fruit.Initialize(_currentDef, mergeResolver);

            _cooldownRemaining = dropCooldown;
            PrepareNextFruit();
        }

        void PrepareNextFruit()
        {
            _currentDef = PickWeightedRandom();
            if (previewRenderer == null)
                return;

            previewRenderer.sprite = _currentDef.Sprite;
            // 임시 원형 Sprite를 붙였으므로, sprite != null이라도 색상 적용
            var color = _currentDef.TintWhenSpriteMissing;
            // var color = _currentDef.Sprite != null ? Color.white : _currentDef.TintWhenSpriteMissing;
            color.a = previewAlpha;
            previewRenderer.color = color;

            float spriteWorldSize = _currentDef.Sprite != null ? _currentDef.Sprite.bounds.size.x : 1f;
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

            float total = 0f;
            for (int i = 0; i < count; i++)
                total += dropWeights[i];

            float roll = Random.value * total;
            float cumulative = 0f;
            for (int i = 0; i < count; i++)
            {
                cumulative += dropWeights[i];
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
