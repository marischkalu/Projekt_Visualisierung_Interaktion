using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
public class DissolveRevealOnTouch : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private Renderer _renderer;
    private Rigidbody _rigidbody;

    [Header("Trigger Settings")]
    [SerializeField] private string _interactorTag = "Interactor";

    [Header("Reveal Settings")]
    [SerializeField] private float _startRadius = 0.05f;
    [SerializeField] private float _targetRadius = 50f;   // Must be larger than the object bounds
    [SerializeField] private float _growSpeed = 15f;      // Speed at which the reveal expands
    [SerializeField] private bool _lockAfterComplete = true;

    private static readonly int _PositionID = Shader.PropertyToID("_Position");
    private static readonly int _RadiusID = Shader.PropertyToID("_Radius");

    private MaterialPropertyBlock _materialPropertyBlock;
    private bool _started;
    private Vector3 _center;
    private float _radius;

    private void Reset()
    {
        _renderer = GetComponent<Renderer>();
    }

    void Start()
    {
        
        
    }

    private void Awake()
    {
        if (_renderer == null)
        {
            _renderer = GetComponent<Renderer>();
        }

        if (_rigidbody == null)
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        _rigidbody.isKinematic = true;

        _materialPropertyBlock = new MaterialPropertyBlock();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_started) return;
        if (!other.CompareTag(_interactorTag)) return;

        _started = true;
        _rigidbody.isKinematic = false;

        // Lock the reveal center at the point of interaction
        // (usually the interactor position)
        _center = other.transform.position;

        // Start reveal from a small radius
        _radius = _startRadius;
        ApplyProperties();
    }

    private void Update()
    {
        if (!_started) return;

        // Gradually expand the dissolve radius
        _radius = Mathf.MoveTowards(
            _radius,
            _targetRadius,
            _growSpeed * Time.deltaTime
        );

        ApplyProperties();

        // Stop updating once the object is fully revealed
        if (_lockAfterComplete && Mathf.Abs(_radius - _targetRadius) < 0.0001f)
        {
            enabled = false;
        }
    }

    private void ApplyProperties()
    {
        _renderer.GetPropertyBlock(_materialPropertyBlock);
        _materialPropertyBlock.SetVector(_PositionID, _center);
        _materialPropertyBlock.SetFloat(_RadiusID, _radius);
        _renderer.SetPropertyBlock(_materialPropertyBlock);
    }
}
