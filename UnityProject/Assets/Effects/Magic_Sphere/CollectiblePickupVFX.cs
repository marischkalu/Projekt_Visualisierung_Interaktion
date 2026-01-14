using UnityEngine;

public class CollectiblePickupVFX : MonoBehaviour
{
    [Header("VFX")]
    [SerializeField] private ParticleSystem _pickupVfxPrefab;

    [Header("Trigger Settings")]
    [SerializeField] private string _playerTag = "Player";

    private bool _picked;

    private void OnTriggerEnter(Collider collider)
    {
        // Prevent multiple trigger calls
        if (_picked) return;

        // React only to the player
        if (!collider.CompareTag(_playerTag)) return;

        // Mark collectible as picked
        // Actual VFX spawn is deferred to OnDestroy()
        _picked = true;
    }

    private void OnDestroy()
    {
        // Spawn VFX only if this object was picked up
        if (!_picked) return;

        // Safety check
        if (_pickupVfxPrefab == null) return;

        // Instantiate pickup VFX at collectible position
        // Use prefab rotation to preserve authored orientation
        ParticleSystem vfx = Instantiate(
            _pickupVfxPrefab,
            transform.position,
            _pickupVfxPrefab.transform.rotation
        );

        // Enforce one-shot behavior
        var main = vfx.main;
        main.loop = false;
        main.playOnAwake = false;

        vfx.Play();

        // Destroy VFX after it finishes playing
        float lifetime = main.duration + main.startLifetime.constantMax;
        Destroy(vfx.gameObject, lifetime);
    }
}