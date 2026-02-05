using UnityEngine;

public class ObjectColorSystem : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private Renderer _renderer;
    private Rigidbody _rigidbody;

    private Color _dissolveColor = Color.clear;

    [Header("Trigger Settings")]
    [SerializeField] private string _interactorTag = "Interactor";
    [SerializeField] private int _dynamicMarerialIndex;


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

    }

    void OnTriggerEnter(Collider other)
    {

        if (!other.CompareTag(_interactorTag)) return;


        if (other.gameObject.TryGetComponent<SplashBehaviour>(out SplashBehaviour splash))
        {
            _dissolveColor = splash.SplashColor;
        }
        _rigidbody.isKinematic = false;

        _renderer.materials[_dynamicMarerialIndex].color = _dissolveColor;
    }

    private void Update()
    {

    }


}
