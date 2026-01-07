using UnityEngine;

public class Wobble : MonoBehaviour
{
    private Renderer _renderer;

    private Vector3 _lastPosition;
    private Vector3 _lastRotation;
    private Vector3 _velocity;
    private Vector3 _angularVelocity;

    [Header("Wobble From Motion")]
    [SerializeField] private float _maxWobble = 0.03f;
    [SerializeField] private float _wobbleSpeed = 1f;
    [SerializeField] private float _recovery = 1f;

    [Header("Constant Shake (Always On)")]
    [SerializeField] private bool _constantShake = true;
    [SerializeField] private float _constantStrength = 0.6f;
    [SerializeField] private float _constantFrequency = 6f;
    [SerializeField] private float _constantXMultiplier = 1.0f;
    [SerializeField] private float _constantZMultiplier = 0.9f;

    private float _wobbleAmountX;
    private float _wobbleAmountZ;
    private float _wobbleAmountToAddX;
    private float _wobbleAmountToAddZ;
    private float _pulse;
    private float _time = 0.5f;

    // Start is called once before the first execution of Update
    void Start()
    {
        _renderer = GetComponent<Renderer>();
        _lastPosition = transform.position;
        _lastRotation = transform.rotation.eulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        _time += Time.deltaTime;

        // Smooth recovery
        _wobbleAmountToAddX = Mathf.Lerp(_wobbleAmountToAddX, 0f, Time.deltaTime * _recovery);
        _wobbleAmountToAddZ = Mathf.Lerp(_wobbleAmountToAddZ, 0f, Time.deltaTime * _recovery);

        // Constant shake (always active)
        if (_constantShake)
        {
            float shakeX =
                Mathf.Sin(_time * _constantFrequency) *
                _maxWobble *
                _constantStrength *
                _constantXMultiplier;

            float shakeZ =
                Mathf.Cos(_time * _constantFrequency) *
                _maxWobble *
                _constantStrength *
                _constantZMultiplier;

            _wobbleAmountToAddX += shakeX;
            _wobbleAmountToAddZ += shakeZ;
        }

        // Sine wobble
        _pulse = 2f * Mathf.PI * _wobbleSpeed;
        _wobbleAmountX = _wobbleAmountToAddX * Mathf.Sin(_pulse * _time);
        _wobbleAmountZ = _wobbleAmountToAddZ * Mathf.Sin(_pulse * _time);

        // Send values to shader
        _renderer.material.SetFloat("_WobbleX", _wobbleAmountX);
        _renderer.material.SetFloat("_WobbleZ", _wobbleAmountZ);

        // Motion-based wobble  
        _velocity = (_lastPosition - transform.position) / Mathf.Max(Time.deltaTime, 0.0001f);
        _angularVelocity = transform.rotation.eulerAngles - _lastRotation;

        _wobbleAmountToAddX += Mathf.Clamp(
            (_velocity.x + (_angularVelocity.z * 0.2f)) * _maxWobble,
            -_maxWobble,
            _maxWobble
        );

        _wobbleAmountToAddZ += Mathf.Clamp(
            (_velocity.z + (_angularVelocity.x * 0.2f)) * _maxWobble,
            -_maxWobble,
            _maxWobble
        );

        _lastPosition = transform.position;
        _lastRotation = transform.rotation.eulerAngles;
    }
}
