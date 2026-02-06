using UnityEngine;

[RequireComponent (typeof(Rigidbody))]
public class ObjectColorSystemVariationParent : MonoBehaviour
{

    [SerializeField] private ObjectColorSystemVariation[] _objectColorSystemVariations;
    private Rigidbody _rigidbody;


    void Start()
    {


    }

    private void Awake()
    {

        if (_rigidbody == null)
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        _rigidbody.isKinematic = true;

    }

    private void Update()
    {

    }

    public void OnSetDynamicEvent()
    {
        if (CheckObjectColorSystemVariations()) _rigidbody.isKinematic = false;
    }

    bool CheckObjectColorSystemVariations()
    {
        for (int i = 0; i < _objectColorSystemVariations.Length; i++)
        {
            if (!_objectColorSystemVariations[i].IsDynamic) return false;
        }
        return true;
    }


}
