using UnityEngine;

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
            circle.radius = def.Radius;
            body.mass = def.Mass;
            spriteRenderer.sprite = def.Sprite;
            spriteRenderer.color = def.Sprite != null ? Color.white : def.TintWhenSpriteMissing;
            gameObject.name = $"Fruit_L{def.Level:00}_{def.DisplayName}";
        }

        /// <summary>MergeResolver(#4)가 호출. 이후 이 과일은 머지 대상에서 제외된다.</summary>
        public void MarkMerged() => HasMerged = true;

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

            circle.radius = definition.Radius;
            body.mass = definition.Mass;
        }
#endif
    }
}
