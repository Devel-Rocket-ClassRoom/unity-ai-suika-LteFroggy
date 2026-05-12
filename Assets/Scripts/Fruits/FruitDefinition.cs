using UnityEngine;

namespace Suika.Fruits
{
    /// <summary>
    /// 과일 한 단계의 정적 데이터를 담는 ScriptableObject.
    /// 스프라이트나 밸런스가 바뀌어도 이 에셋만 교체하면 된다.
    /// </summary>
    [CreateAssetMenu(menuName = "Suika/Fruit Definition", fileName = "FruitDefinition")]
    public sealed class FruitDefinition : ScriptableObject
    {
        [Header("기본 정보")]
        [SerializeField] int   level;
        [SerializeField] string displayName;

        [Header("물리")]
        [SerializeField] float radius;
        [SerializeField] float mass;

        [Header("점수 / 드롭")]
        [SerializeField] int   score;
        [SerializeField] bool  canDropFromQueue;

        [Header("그래픽")]
        [SerializeField] Sprite sprite;
        [SerializeField] Color  tintWhenSpriteMissing = Color.white;

        // ── 공개 프로퍼티 ─────────────────────────────────────────────────────
        public int    Level                  => level;
        public string DisplayName            => displayName;
        public float  Radius                 => radius;
        public float  Mass                   => mass;
        public int    Score                  => score;
        public bool   CanDropFromQueue       => canDropFromQueue;
        public Sprite Sprite                 => sprite;
        public Color  TintWhenSpriteMissing  => tintWhenSpriteMissing;
    }
}
