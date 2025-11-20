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
    private float xRotation;
    [SerializeField] private float _rotationSpeedVertical = 500f;
    [SerializeField] private float _rotationSpeedHorizontal = 750f;
    [SerializeField] private float _movementSpeed = 0.2f;
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

        MovePlayerInLookDirection();

    }


    void MovePlayerInLookDirection()
    {

        Quaternion deltaRotation = Quaternion.Euler(0f, _horizontalMouse * _rotationSpeedHorizontal * Time.deltaTime, 0f);
        _rigidbody.MoveRotation(_rigidbody.rotation * deltaRotation);

        if (_camera.transform.rotation.x > -0.7f && _verticalMouse <= 0)
        {
            _camera.transform.Rotate(_verticalMouse * _rotationSpeedVertical * Time.deltaTime * -1f, 0f, 0f);
        }
        if (_camera.transform.rotation.x < 0.7f && _verticalMouse > 0)
        {
            _camera.transform.Rotate(_verticalMouse * _rotationSpeedVertical * Time.deltaTime * -1f, 0f, 0f);
        }


        _rigidbody.MovePosition(
            _rigidbody.transform.position +
            _rigidbody.transform.forward * _verticalInput * _movementSpeed +
            _rigidbody.transform.right * _horizontalInput * _movementSpeed
        );
    }

}
