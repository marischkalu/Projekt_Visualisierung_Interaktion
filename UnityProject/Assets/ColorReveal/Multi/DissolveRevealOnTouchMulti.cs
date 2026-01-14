using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class DissolveRevealOnTouchMulti : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Renderer _renderer;
    private Rigidbody _rigidbody;

    [Header("Trigger Settings")]
    [SerializeField] private string _interactorTag = "Interactor";

    [Header("Reveal Settings")]
    [SerializeField] private float _startRadius = 0.05f;
    [SerializeField] private float _targetRadius = 50f;
    [SerializeField] private float _growSpeed = 10f;

    [Header("Multi Texture (Secondary Stages)")]
    [SerializeField] private Texture2D[] _secondaryStages;
    [SerializeField] private int _maxPaints = 3;

    [Header("Shader Reference (do not rename in shader)")]
    [SerializeField] private string _secondaryTextureRef = "_SecondaryTexture";

    [Header("Anti Spam")]
    [SerializeField] private float _hitCooldown = 0.05f;

    private static readonly int _PositionID = Shader.PropertyToID("_Position");
    private static readonly int _RadiusID = Shader.PropertyToID("_Radius");

    private int _secondaryTexID;
    private MaterialPropertyBlock _mpb;

    private bool _revealing;
    private Vector3 _center;
    private float _radius;

    private int _paintIndex;
    private float _nextAllowedHitTime;

    private void Reset()
    {
        _renderer = GetComponent<Renderer>();
    }

    private void Awake()
    {
        if (_renderer == null) _renderer = GetComponent<Renderer>();

        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.isKinematic = true;

        _secondaryTexID = Shader.PropertyToID(_secondaryTextureRef);

        _mpb = new MaterialPropertyBlock();

        // Start as "not revealed" (shows Main)
        _radius = 0f;
        ApplyRevealOnly();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(_interactorTag)) return;

        if (Time.time < _nextAllowedHitTime) return;
        _nextAllowedHitTime = Time.time + _hitCooldown;

        if (_secondaryStages == null || _secondaryStages.Length == 0) return;
        if (_paintIndex >= _maxPaints) return;
        if (_paintIndex >= _secondaryStages.Length) return;

        // Swap Secondary for this hit
        SetSecondaryTexture(_secondaryStages[_paintIndex]);
        _paintIndex++;

        // Restart reveal for every hit
        _center = other.transform.position;
        _revealing = true;
        _radius = _startRadius;

        ApplyRevealOnly();
    }

    private void Update()
    {
        if (!_revealing) return;

        _radius = Mathf.MoveTowards(_radius, _targetRadius, _growSpeed * Time.deltaTime);
        ApplyRevealOnly();

        if (Mathf.Abs(_radius - _targetRadius) < 0.0001f)
        {
            _revealing = false;

            // IMPORTANT: keep fully revealed so Secondary stays visible
            _radius = _targetRadius;
            ApplyRevealOnly();
        }
    }

    private void SetSecondaryTexture(Texture2D tex)
    {
        _renderer.GetPropertyBlock(_mpb);
        _mpb.SetTexture(_secondaryTexID, tex);
        _renderer.SetPropertyBlock(_mpb);
    }

    private void ApplyRevealOnly()
    {
        _renderer.GetPropertyBlock(_mpb);
        _mpb.SetVector(_PositionID, _center);
        _mpb.SetFloat(_RadiusID, _radius);
        _renderer.SetPropertyBlock(_mpb);
    }
}
