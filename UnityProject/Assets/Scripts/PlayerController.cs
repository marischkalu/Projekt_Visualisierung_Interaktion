using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody _rigidbody;
    [SerializeField] private GameObject _camera;
    private float _horizontalMouse;
    private float _verticalMouse;
    private float _horizontalInput;
    private float _verticalInput;
    private float _pitch = 0f;
    [SerializeField] private float _rotationSpeedVertical = 500f;
    [SerializeField] private float _rotationSpeedHorizontal = 750f;
    [SerializeField] private float _movementSpeed = 0.2f;
    [SerializeField] private float _throwForce = 200f;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        _horizontalMouse = Input.GetAxis("Mouse X"); // Left/Right
        _verticalMouse = Input.GetAxis("Mouse Y");   // Up/Down
        _horizontalInput = Input.GetAxis("Horizontal"); // A/D
        _verticalInput = Input.GetAxis("Vertical");   // W/S

        RotatePlayer();
        MovePlayer();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            ThrowOrb();
        }

    }


    void RotatePlayer()
    {

        Quaternion deltaRotation = Quaternion.Euler(
            0f,
            _horizontalMouse * _rotationSpeedHorizontal * Time.deltaTime,
            0f
        );
        _rigidbody.MoveRotation(_rigidbody.rotation * deltaRotation);

        float inputPitch = _verticalMouse * _rotationSpeedVertical * Time.deltaTime * -1f;
        _pitch += inputPitch;

        _pitch = Mathf.Clamp(_pitch, -80f, 80f);
        _camera.transform.localRotation = Quaternion.Euler(
            _pitch,
            0f,
            0f
        );

    }

    void MovePlayer()
    {
        _rigidbody.MovePosition(
            _rigidbody.transform.position +
            _rigidbody.transform.forward * _verticalInput * _movementSpeed +
            _rigidbody.transform.right * _horizontalInput * _movementSpeed
        );
    }

    void ThrowOrb()
    {
        GameObject orb = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        orb.transform.position = _camera.transform.position + _camera.transform.forward.normalized * 2;
        orb.transform.localScale = new Vector3(
            0.5f,
            0.5f,
            0.5f
        );

        // Add a standard Rigidbody
        Rigidbody rb = orb.AddComponent<Rigidbody>();

        rb.AddForce(_camera.transform.forward * _throwForce);
    }

}
