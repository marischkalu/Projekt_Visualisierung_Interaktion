using UnityEngine;
using System;
[Obsolete("Not used any more", true)]
public class PortalSoundController : MonoBehaviour
{
    // ===================== REFS =====================
    [Header("Refs")]
    [SerializeField] private PortalTransitionTrigger _portal;

    // ===================== AUDIO =====================
    [Header("Audio")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _particlesStartClip;
    [SerializeField, Range(0f, 1f)] private float _volume = 1f;


    // Called once before the first Update
    void Awake()
    {
        // Auto-assign AudioSource if not set manually
        if (!_audioSource)
            _audioSource = GetComponent<AudioSource>();

        // Auto-assign PortalTransitionTrigger if not set manually
        if (!_portal)
            _portal = GetComponent<PortalTransitionTrigger>();
    }


    // ===================== PORTAL CALLBACK =====================
    // Called when portal particles start playing
    public void OnPortalParticlesStarted()
    {
        if (!_particlesStartClip) return;

        PlayOneShotPersistent();
    }


    // ===================== AUDIO LOGIC =====================
    // Plays a one-shot sound that survives scene loading
    void PlayOneShotPersistent()
    {
        GameObject audioObject = new GameObject("PortalSFX_OneShot");
        DontDestroyOnLoad(audioObject);

        AudioSource source = audioObject.AddComponent<AudioSource>();
        source.clip = _particlesStartClip;
        source.volume = _volume;

        // 2D sound (camera-based effect)
        source.spatialBlend = 0f;

        source.Play();

        // Destroy the object after the clip finishes playing
        Destroy(audioObject, _particlesStartClip.length + 0.1f);
    }
}
