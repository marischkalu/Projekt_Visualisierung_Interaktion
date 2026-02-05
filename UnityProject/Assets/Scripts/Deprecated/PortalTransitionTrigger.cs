using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Unity.Cinemachine;
using UnityEngine.SceneManagement;
using System;
[Obsolete("Not used any more", true)]
public class PortalTransitionTrigger : MonoBehaviour
{
    // ===================== REFS =====================
    [Header("Refs")]
    [SerializeField] private Renderer _rippleRenderer;
    [SerializeField] private ParticleSystem _portalParticles;
    [SerializeField] private Volume _globalVolume;
    [SerializeField] private CinemachineCamera _cinemachineCamera;


    // ===================== TRIGGER =====================
    [Header("Trigger")]
    [SerializeField] private string _playerTag = "Player";
    [SerializeField] private bool _playOnce = true;


    // ===================== SCENE TRANSITION =====================
    [Header("Scene Transition")]
    [SerializeField] private string _nextSceneName = "Level1";
    [SerializeField] private float _loadDelayAtPeak = 0.05f;

    [Header("Peak Hold")]
    [SerializeField] private float _holdAfterVignette = 0.10f;


    // ===================== RIPPLE =====================
    [Header("Ripple Shader (Shader Graph reference names)")]
    [SerializeField] private string _rippleCountProp = "RippleCount";
    [SerializeField] private string _rippleSpeedProp = "RippleSpeed";

    [SerializeField] private float _rippleCountIdle = 0f;
    [SerializeField] private float _rippleSpeedIdle = 0f;
    [SerializeField] private float _rippleCountNear = 12f;
    [SerializeField] private float _rippleSpeedNear = 2f;
    [SerializeField] private float _rippleFadeTime = 0.4f;

    [Tooltip("If true: after the first enter the ripple stays in the 'Near' state.")]
    [SerializeField] private bool _freezeRippleAfterEnter = true;


    // ===================== CINEMATIC TIMING =====================
    [Header("Cinematic Timing")]
    [SerializeField] private float _waitAfterRippleEnter = 3f;
    [SerializeField] private float _waitAfterFov = 2f;


    // ===================== CINEMACHINE FOV =====================
    [Header("Cinemachine FOV (Gentle)")]
    [SerializeField] private float _fovStart = 60f;
    [SerializeField] private float _fovGentleTo = 50f;
    [SerializeField] private float _fovGentleTime = 1.2f;


    // ===================== PARTICLES =====================
    [Header("Particles")]
    [SerializeField] private float _particlesDelayBeforePlay = 0.0f;
    [SerializeField] private bool _stopAndClearBeforePlay = true;


    // ===================== VIGNETTE =====================
    [Header("Vignette Strong (URP)")]
    [SerializeField] private float _vignetteIntensityTo = 1.0f;
    [SerializeField] private float _vignetteSmoothnessTo = 0.95f;
    [SerializeField] private float _vignetteTime = 0.9f;

    [Tooltip("URP vignette uses 'Rounded' instead of roundness.")]
    [SerializeField] private bool _vignetteRounded = true;


    // ===================== INTERNAL =====================
    private Vignette _vignette;

    private MaterialPropertyBlock _mpb;
    private int _rippleCountID;
    private int _rippleSpeedID;
    private bool _ripplePropsValid;

    private float _currentRippleCount;
    private float _currentRippleSpeed;

    private bool _playerInside;
    private bool _isRunning;
    private bool _hasPlayed;


    // Called once before the first Update
    void Awake()
    {
        _mpb = new MaterialPropertyBlock();
        ValidateAndResolveRippleProperties();

        _currentRippleCount = _rippleCountIdle;
        _currentRippleSpeed = _rippleSpeedIdle;
        ApplyRipple();

        if (_portalParticles)
        {
            var main = _portalParticles.main;
            main.loop = false;
            _portalParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        SetupVolumeOverrides();
        ResetPostFX();

        if (_cinemachineCamera)
        {
            var lens = _cinemachineCamera.Lens;
            lens.FieldOfView = _fovStart;
            _cinemachineCamera.Lens = lens;
        }
    }


    // ===================== RIPPLE PROPERTY RESOLVE =====================
    void ValidateAndResolveRippleProperties()
    {
        if (!_rippleRenderer)
        {
            Debug.LogError("PortalTransitionTrigger: Ripple Renderer not assigned.");
            _ripplePropsValid = false;
            return;
        }

        Material mat = _rippleRenderer.sharedMaterial;
        if (!mat)
        {
            Debug.LogError("PortalTransitionTrigger: Ripple material missing.");
            _ripplePropsValid = false;
            return;
        }

        _rippleCountID = ResolvePropertyId(mat, _rippleCountProp, "_RippleCount", "RippleCount");
        _rippleSpeedID = ResolvePropertyId(mat, _rippleSpeedProp, "_RippleSpeed", "RippleSpeed");

        _ripplePropsValid = mat.HasProperty(_rippleCountID) && mat.HasProperty(_rippleSpeedID);
    }

    int ResolvePropertyId(Material mat, params string[] candidates)
    {
        foreach (string name in candidates)
        {
            if (string.IsNullOrWhiteSpace(name)) continue;
            int id = Shader.PropertyToID(name);
            if (mat.HasProperty(id)) return id;
        }
        return Shader.PropertyToID(candidates[0]);
    }


    // ===================== UPDATE =====================
    void Update()
    {
        bool holdNear = _freezeRippleAfterEnter && _hasPlayed;

        float targetCount = (_playerInside || holdNear) ? _rippleCountNear : _rippleCountIdle;
        float targetSpeed = (_playerInside || holdNear) ? _rippleSpeedNear : _rippleSpeedIdle;

        float step = Time.deltaTime / Mathf.Max(0.0001f, _rippleFadeTime);

        _currentRippleCount = Mathf.Lerp(_currentRippleCount, targetCount, step);
        _currentRippleSpeed = Mathf.Lerp(_currentRippleSpeed, targetSpeed, step);

        ApplyRipple();
    }

    void ApplyRipple()
    {
        if (!_rippleRenderer || !_ripplePropsValid) return;

        _rippleRenderer.GetPropertyBlock(_mpb);
        _mpb.SetFloat(_rippleCountID, _currentRippleCount);
        _mpb.SetFloat(_rippleSpeedID, _currentRippleSpeed);
        _rippleRenderer.SetPropertyBlock(_mpb);
    }


    // ===================== POST FX =====================
    void SetupVolumeOverrides()
    {
        if (!_globalVolume || !_globalVolume.profile) return;

        if (!_globalVolume.profile.TryGet(out _vignette))
            _vignette = _globalVolume.profile.Add<Vignette>(true);

        _vignette.active = true;
        _vignette.intensity.overrideState = true;
        _vignette.smoothness.overrideState = true;
        _vignette.rounded.overrideState = true;
        _vignette.rounded.value = _vignetteRounded;
    }

    void ResetPostFX()
    {
        if (_vignette)
            _vignette.intensity.value = 0f;
    }


    // ===================== TRIGGER =====================
    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(_playerTag)) return;

        _playerInside = true;

        if (_isRunning) return;
        if (_playOnce && _hasPlayed) return;

        StartCoroutine(Sequence());
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(_playerTag)) return;
        _playerInside = false;
    }


    // ===================== CINEMATIC SEQUENCE =====================
    IEnumerator Sequence()
    {
        _isRunning = true;
        _hasPlayed = true;

        if (_portalParticles && _stopAndClearBeforePlay)
            _portalParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        if (_waitAfterRippleEnter > 0f)
            yield return new WaitForSeconds(_waitAfterRippleEnter);

        if (_cinemachineCamera)
        {
            float fromFov = _cinemachineCamera.Lens.FieldOfView;
            yield return TweenFOV(fromFov, _fovGentleTo, _fovGentleTime);
        }

        if (_waitAfterFov > 0f)
            yield return new WaitForSeconds(_waitAfterFov);

        if (_particlesDelayBeforePlay > 0f)
            yield return new WaitForSeconds(_particlesDelayBeforePlay);

        if (_portalParticles)
            _portalParticles.Play(true);

        PortalSoundController sound = GetComponent<PortalSoundController>();
        if (sound)
            sound.OnPortalParticlesStarted();

        if (_vignette)
        {
            float fromI = _vignette.intensity.value;
            float fromS = _vignette.smoothness.value;

            yield return Tween(0f, 1f, _vignetteTime, t =>
            {
                _vignette.intensity.value = Mathf.Lerp(fromI, _vignetteIntensityTo, t);
                _vignette.smoothness.value = Mathf.Lerp(fromS, _vignetteSmoothnessTo, t);
            });
        }

        if (_holdAfterVignette > 0f)
            yield return new WaitForSeconds(_holdAfterVignette);

        if (_loadDelayAtPeak > 0f)
            yield return new WaitForSeconds(_loadDelayAtPeak);

        if (string.IsNullOrWhiteSpace(_nextSceneName))
            yield break;

        AsyncOperation op = SceneManager.LoadSceneAsync(_nextSceneName);
        while (!op.isDone)
            yield return null;

        _isRunning = false;
    }


    // ===================== HELPERS =====================
    IEnumerator TweenFOV(float from, float to, float time)
    {
        yield return Tween(from, to, time, v =>
        {
            var lens = _cinemachineCamera.Lens;
            lens.FieldOfView = v;
            _cinemachineCamera.Lens = lens;
        });
    }

    IEnumerator Tween(float from, float to, float time, System.Action<float> set)
    {
        float t = 0f;
        float safeTime = Mathf.Max(0.0001f, time);

        while (t < 1f)
        {
            t += Time.deltaTime / safeTime;
            float k = Mathf.SmoothStep(0f, 1f, t);
            set(Mathf.Lerp(from, to, k));
            yield return null;
        }

        set(to);
    }
}
