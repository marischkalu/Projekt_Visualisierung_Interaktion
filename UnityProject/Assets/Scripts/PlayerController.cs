using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody _rigidbody;
    [SerializeField] private GameObject _camera;
    private float _hor;
    private float _ver;
    [SerializeField] private float _rotationSpeed = 10f;
    [SerializeField] private float _movementSpeed = 0.2f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        _hor = Input.GetAxisRaw("Horizontal"); // A/D or Left/Right
        _ver = Input.GetAxisRaw("Vertical");   // W/S or Up/Down

        MovePlayerInLookDirection();

    }


    void MovePlayerInLookDirection()
    {
        // lookdirection from camera direction and player position

        Vector3 cameraPositionOnPlane = new Vector3(
            _camera.transform.position.x,
            _rigidbody.transform.position.y,
            _camera.transform.position.z
        );

        Vector3 playerPositionOnPlane = _rigidbody.transform.position;
        Vector3 direction = playerPositionOnPlane - cameraPositionOnPlane;

        // movement direction from input
        Vector3 moveDir = (direction * _ver) + (Quaternion.Euler(0, 90, 0) * direction * _hor);

        if (moveDir!=Vector3.zero) // check if input
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDir.normalized);

            _rigidbody.MoveRotation(
                Quaternion.Slerp(
                    _rigidbody.rotation,
                    targetRotation,
                    _rotationSpeed * Time.deltaTime
                )
            );
        }

        // Movement forward in the rotated direction (only if any axis input not zero)

        _rigidbody.MovePosition(
            _rigidbody.transform.position +
            _rigidbody.transform.forward * Mathf.Max(Mathf.Abs(_ver), Mathf.Abs(_hor)) * _movementSpeed
        );
    }

}
