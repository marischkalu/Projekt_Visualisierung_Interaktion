using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Unity.Cinemachine;

public class PortalArrivalEmergence : MonoBehaviour
{
    // ===================== REFS =====================
    [Header("Refs")]
    [SerializeField] private Volume _globalVolume;
    [SerializeField] private CinemachineCamera _cinemachineCamera;


    // ===================== FOV (EMERGE) =====================
    [Header("FOV (Emerge)")]
    [SerializeField] private float _fovAtStart = 50f;   // FOV when arriving in the new scene
    [SerializeField] private float _fovTo = 60f;        // Final gameplay FOV
    [SerializeField] private float _fovTime = 1.1f;


    // ===================== VIGNETTE (EMERGE) =====================
    [Header("Vignette (Emerge)")]
    [SerializeField] private float _vignetteStartIntensity = 1.0f;   // Almost black at start
    [SerializeField] private float _vignetteStartSmoothness = 1.0f;
    [SerializeField] private float _vignetteToIntensity = 0.0f;      // Fully open
    [SerializeField] private float _vignetteToSmoothness = 0.0f;
    [SerializeField] private float _vignetteTime = 1.0f;


    // ===================== TIMING =====================
    [Header("Timing")]
    [SerializeField] private float _holdBlackTime = 0.10f;     // Small hold on black screen
    [SerializeField] private float _delayBeforeOpen = 0.05f;   // Optional delay before opening


    // ===================== PLAYER CONTROL =====================
    [Header("Optional: Enable player controls after emerge")]
    [SerializeField] private MonoBehaviour[] _disableWhileEmerge; // Player controller scripts


    // ===================== INTERNAL =====================
    private Vignette _vignette;


    // Called once before the first Update
    void Awake()
    {
        // Disable player control scripts during emergence
        if (_disableWhileEmerge != null)
        {
            foreach (MonoBehaviour behaviour in _disableWhileEmerge)
            {
                if (behaviour)
                    behaviour.enabled = false;
            }
        }

        SetupVolume();
        ForceStartState();
    }

    void Start()
    {
        StartCoroutine(Emerge());
    }


    // ===================== VOLUME SETUP =====================
    void SetupVolume()
    {
        if (!_globalVolume || !_globalVolume.profile)
        {
            Debug.LogWarning("PortalArrivalEmergence: Volume or Profile not assigned.");
            return;
        }

        VolumeProfile profile = _globalVolume.profile;

        if (!profile.TryGet(out _vignette))
            _vignette = profile.Add<Vignette>(true);

        _vignette.active = true;

        _vignette.intensity.overrideState = true;
        _vignette.smoothness.overrideState = true;
        _vignette.rounded.overrideState = true;
        _vignette.rounded.value = true;
    }


    // ===================== INITIAL STATE =====================
    void ForceStartState()
    {
        // Force black vignette at scene start
        if (_vignette)
        {
            _vignette.intensity.value = _vignetteStartIntensity;
            _vignette.smoothness.value = _vignetteStartSmoothness;
        }

        // Force narrow FOV at scene start
        if (_cinemachineCamera)
        {
            var lens = _cinemachineCamera.Lens;
            lens.FieldOfView = _fovAtStart;
            _cinemachineCamera.Lens = lens;
        }
    }


    // ===================== EMERGE SEQUENCE =====================
    IEnumerator Emerge()
    {
        if (_holdBlackTime > 0f)
            yield return new WaitForSeconds(_holdBlackTime);

        if (_delayBeforeOpen > 0f)
            yield return new WaitForSeconds(_delayBeforeOpen);

        // Run vignette and FOV animation in parallel
        Coroutine vignetteRoutine = null;
        Coroutine fovRoutine = null;

        if (_vignette)
        {
            vignetteRoutine = StartCoroutine(
                Tween01(_vignetteTime, t =>
                {
                    _vignette.intensity.value =
                        Mathf.Lerp(_vignetteStartIntensity, _vignetteToIntensity, t);

                    _vignette.smoothness.value =
                        Mathf.Lerp(_vignetteStartSmoothness, _vignetteToSmoothness, t);
                })
            );
        }

        if (_cinemachineCamera)
        {
            fovRoutine = StartCoroutine(
                Tween01(_fovTime, t =>
                {
                    var lens = _cinemachineCamera.Lens;
                    lens.FieldOfView = Mathf.Lerp(_fovAtStart, _fovTo, t);
                    _cinemachineCamera.Lens = lens;
                })
            );
        }

        if (vignetteRoutine != null) yield return vignetteRoutine;
        if (fovRoutine != null) yield return fovRoutine;

        // Re-enable player controls
        if (_disableWhileEmerge != null)
        {
            foreach (MonoBehaviour behaviour in _disableWhileEmerge)
            {
                if (behaviour)
                    behaviour.enabled = true;
            }
        }

        // Remove this component after emergence is finished
        Destroy(this);
    }


    // ===================== HELPERS =====================
    IEnumerator Tween01(float time, System.Action<float> set01)
    {
        float t = 0f;
        float safeTime = Mathf.Max(0.0001f, time);

        while (t < 1f)
        {
            t += Time.deltaTime / safeTime;
            float k = Mathf.SmoothStep(0f, 1f, t);
            set01(k);
            yield return null;
        }

        set01(1f);
    }
}
