using UnityEngine;

[RequireComponent (typeof(Rigidbody))]
public class ObjectColorSystem : MonoBehaviour
{
    [SerializeField] private Renderer _renderer;
    private Rigidbody _rigidbody;

    [SerializeField] public Color CorrectColor;
    public Color AppliedColor;

    [SerializeField] private string _interactorTag = "Interactor";
    [SerializeField] private int _dynamicMarerialIndex;


    void Start()
    {
        AppliedColor = _renderer.materials[_dynamicMarerialIndex].color;

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

    }

    void OnTriggerEnter(Collider other)
    {

        if (!other.CompareTag(_interactorTag)) return;


        if (other.gameObject.TryGetComponent<SplashBehaviour>(out SplashBehaviour splash))
        {
            AppliedColor = splash.SplashColor;
        }
        _renderer.materials[_dynamicMarerialIndex].color = AppliedColor;
        _rigidbody.isKinematic = false;

    }

    private void Update()
    {

    }


}
