using UnityEngine;
using System;
[Obsolete("Not used any more", true)]
public class BrushVFXShooter : MonoBehaviour
{
    [SerializeField] private Camera _camera;        // Player camera
    [SerializeField] private Transform _brushTip;   // Stroke spawn point
    [SerializeField] private float _range = 25f;    // Raycast distance

    [Header("Stroke")]
    [SerializeField] private StrokeMover _strokePrefab; // Flying stroke prefab
    [SerializeField] private float _strokeSpeed = 35f;  // Stroke speed
    [SerializeField] private float _strokeLife = 0.25f; // Stroke lifetime

    [Header("Impact")]
    [SerializeField] private ParticleSystem _impactPrefab; // Hit particles

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            ShootVFX();
    }

    void ShootVFX()
    {
        Ray ray = new Ray(_camera.transform.position, _camera.transform.forward);

        Vector3 hitPoint = _camera.transform.position + _camera.transform.forward * _range;
        Vector3 hitNormal = -_camera.transform.forward;

        if (Physics.Raycast(ray, out RaycastHit hit, _range))
        {
            hitPoint = hit.point;
            hitNormal = hit.normal;

            if (_impactPrefab != null)
            {
                var ps = Instantiate(_impactPrefab, hitPoint, Quaternion.LookRotation(hitNormal));
                var main = ps.main;
                Destroy(ps.gameObject, main.duration + main.startLifetime.constantMax + 0.1f);
            }
        }

        if (_strokePrefab != null && _brushTip != null)
        {
            var stroke = Instantiate(_strokePrefab, _brushTip.position, Quaternion.identity);
            stroke.transform.forward = (hitPoint - _brushTip.position).normalized;
            stroke.Init(hitPoint, _strokeSpeed, _strokeLife);
        }
    }
}
