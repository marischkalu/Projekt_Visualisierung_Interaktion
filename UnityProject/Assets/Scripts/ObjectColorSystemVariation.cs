using UnityEngine;
using System;

public class ObjectColorSystemVariation : MonoBehaviour
{
    private ObjectColorSystemVariationParent _objectColorSystemVariationParent;
    [SerializeField] private Renderer _renderer;

    [SerializeField] public Color CorrectColor;
    public Color AppliedColor;

    [SerializeField] private string _interactorTag = "Interactor";
    [SerializeField] private int _dynamicMarerialIndex;

    public bool IsDynamic = false;


    void Start()
    {
        AppliedColor = _renderer.materials[_dynamicMarerialIndex].color;
        _objectColorSystemVariationParent = GetComponentInParent<ObjectColorSystemVariationParent>();
    }

    private void Awake()
    {
        if (_renderer == null)
        {
            _renderer = GetComponent<Renderer>();
        }


    }

    void OnTriggerEnter(Collider other)
    {

        if (!other.CompareTag(_interactorTag)) return;


        if (other.gameObject.TryGetComponent<SplashBehaviour>(out SplashBehaviour splash))
        {
            AppliedColor = splash.SplashColor;
        }
        _renderer.materials[_dynamicMarerialIndex].color = AppliedColor;

        IsDynamic = true;
        _objectColorSystemVariationParent.OnSetDynamicEvent();

    }

    private void Update()
    {

    }


}
