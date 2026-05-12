using UnityEngine;

public sealed class CodexFireflyDance : MonoBehaviour
{
    [SerializeField] private float danceSpeed = 1.25f;
    [SerializeField] private float hoverHeight = 0.28f;
    [SerializeField] private float swayDistance = 0.18f;
    [SerializeField] private float bodyPulse = 0.08f;
    [SerializeField] private float wingFlapAngle = 32f;
    [SerializeField] private float glowPulse = 0.45f;

    private Transform body;
    private Transform leftWing;
    private Transform rightWing;
    private Light tinyGlow;

    private Vector3 startPosition;
    private Vector3 bodyStartScale;
    private Quaternion leftWingStartRotation;
    private Quaternion rightWingStartRotation;
    private float glowStartIntensity;

    private void Awake()
    {
        CacheParts();
        CaptureStartingPose();
    }

    private void OnEnable()
    {
        CacheParts();
        CaptureStartingPose();
    }

    private void Update()
    {
        float beat = Time.time * danceSpeed;
        float bob = Mathf.Sin(beat * 2f) * hoverHeight;
        float sway = Mathf.Sin(beat) * swayDistance;
        transform.localPosition = startPosition + new Vector3(sway, bob, 0f);
        transform.localRotation = Quaternion.Euler(0f, 25f + Mathf.Sin(beat * 0.8f) * 12f, -8f + Mathf.Sin(beat * 1.4f) * 8f);

        if (body != null)
        {
            float pulse = 1f + Mathf.Sin(beat * 4f) * bodyPulse;
            body.localScale = bodyStartScale * pulse;
        }

        float flap = Mathf.Sin(beat * 8f) * wingFlapAngle;
        if (leftWing != null)
        {
            leftWing.localRotation = leftWingStartRotation * Quaternion.Euler(0f, 0f, flap);
        }

        if (rightWing != null)
        {
            rightWing.localRotation = rightWingStartRotation * Quaternion.Euler(0f, 0f, -flap);
        }

        if (tinyGlow != null)
        {
            tinyGlow.intensity = glowStartIntensity + Mathf.Abs(Mathf.Sin(beat * 3f)) * glowPulse;
        }
    }

    private void CacheParts()
    {
        body = transform.Find("GlowBody");
        leftWing = transform.Find("LeftWing");
        rightWing = transform.Find("RightWing");

        Transform glowTransform = transform.Find("TinyGlow");
        tinyGlow = glowTransform != null ? glowTransform.GetComponent<Light>() : null;
    }

    private void CaptureStartingPose()
    {
        startPosition = transform.localPosition;
        bodyStartScale = body != null ? body.localScale : Vector3.one;
        leftWingStartRotation = leftWing != null ? leftWing.localRotation : Quaternion.identity;
        rightWingStartRotation = rightWing != null ? rightWing.localRotation : Quaternion.identity;
        glowStartIntensity = tinyGlow != null ? tinyGlow.intensity : 1f;
    }
}
