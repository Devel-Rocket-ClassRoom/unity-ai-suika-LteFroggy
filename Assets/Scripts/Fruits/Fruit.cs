using UnityEngine;
using Suika.Merge;

namespace Suika.Fruits
{
    /// <summary>
    /// 과일 프리팹에 붙는 런타임 컴포넌트.
    /// Initialize(def) 호출로 FruitDefinition 값을 물리/그래픽 컴포넌트에 동기화한다.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(CircleCollider2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class Fruit : MonoBehaviour
    {
        [SerializeField]
        FruitDefinition definition;

        [SerializeField]
        Rigidbody2D body;

        [SerializeField]
        CircleCollider2D circle;

        [SerializeField]
        SpriteRenderer spriteRenderer;

        // ── 공개 프로퍼티 ─────────────────────────────────────────────────────
        public FruitDefinition Definition => definition;
        public int Level => definition != null ? definition.Level : 0;
        public float Radius => definition != null ? definition.Radius : 0f;

        // MergeResolver(#4)에서 중복 머지 방지에 사용
        public bool HasMerged { get; private set; }

        // ── 초기화 ────────────────────────────────────────────────────────────

        /// <summary>
        /// FruitSpawner에서 Instantiate 직후 호출한다.
        /// Collider 반지름, Rigidbody 질량, 스프라이트/색상을 한 번에 적용한다.
        /// </summary>
        public void Initialize(FruitDefinition def)
        {
            definition = def;
            spriteRenderer.sprite = def.Sprite;
            // 임시 원형 Sprite를 붙였으므로, sprite != null이라도 색상 적용
            spriteRenderer.color = def.TintWhenSpriteMissing;
            // spriteRenderer.color = def.Sprite != null ? Color.white : def.TintWhenSpriteMissing;

            // 스프라이트 실제 크기 기준으로 scale 계산 → 콜라이더와 렌더링 크기 일치
            float spriteWorldSize = def.Sprite != null ? def.Sprite.bounds.size.x : 1f;
            float scale = (def.Radius * 2f) / spriteWorldSize;
            transform.localScale = new Vector3(scale, scale, 1f);
            circle.radius = 0.5f;

            body.mass = def.Mass;
            gameObject.name = $"Fruit_L{def.Level:00}_{def.DisplayName}";
        }

        /// <summary>MergeResolver(#4)가 호출. 이후 이 과일은 머지 대상에서 제외된다.</summary>
        public void MarkMerged() => HasMerged = true;

        // ── 충돌 감지 ──────────────────────────────────────────────────────────

        void OnCollisionEnter2D(Collision2D col)
        {
            if (HasMerged) return;
            var other = col.gameObject.GetComponent<Fruit>();
            if (other == null || other.HasMerged) return;
            if (Level != other.Level) return;
            MergeResolver.Instance?.RequestMerge(this, other);
        }

        // ── 에디터 미리보기 ───────────────────────────────────────────────────
#if UNITY_EDITOR
        void OnValidate()
        {
            if (definition == null)
                return;
            if (circle == null)
                circle = GetComponent<CircleCollider2D>();
            if (body == null)
                body = GetComponent<Rigidbody2D>();

            float spriteWorldSize = definition.Sprite != null ? definition.Sprite.bounds.size.x : 1f;
            float scale = (definition.Radius * 2f) / spriteWorldSize;
            transform.localScale = new Vector3(scale, scale, 1f);
            circle.radius = 0.5f;
            body.mass = definition.Mass;
        }
#endif
    }
}
