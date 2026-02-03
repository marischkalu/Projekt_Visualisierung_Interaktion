using UnityEngine;
using System;

[Obsolete("Not used any more", true)]
public class StrokeMover : MonoBehaviour
{
    // Trail used to visualize the brush stroke in motion
    [SerializeField] private TrailRenderer _trail;

    // World-space target the stroke moves towards
    private Vector3 _target;

    // Movement speed
    private float _speed;

    // Lifetime before the stroke gets destroyed
    private float _life;

    // Initializes stroke movement and visuals
    public void Init(Vector3 target, float speed, float life)
    {
        _target = target;
        _speed = speed;
        _life = life;

        // Reset trail so it starts clean on spawn
        if (_trail != null)
        {
            _trail.Clear();
            _trail.emitting = true;
        }

        // Auto-destroy after lifetime expires
        Destroy(gameObject, _life);
    }

    void Update()
    {
        // Move stroke towards target point
        transform.position = Vector3.MoveTowards(
            transform.position,
            _target,
            _speed * Time.deltaTime
        );
    }
}
