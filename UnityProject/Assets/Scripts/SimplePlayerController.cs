using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class SimplePlayerController : MonoBehaviour
{
    // how fast the character can move
    public float MaxMovementSpeed;

    // how fast the character can turn
    public float RotationSpeed;

    // How far in m the distance check work
    public float GroundCheckDistance = 0.1f;

    // Influence the gravity 
    public float GravityMultiplier = 1f;

    // Is character audible (moving fast)
    public bool IsAudible { get; private set; }

    // Component to move character
    private CharacterController _characterController;

    // Cache the camera transform
    private Transform _cameraTransform;    

    // How fast the charakter moves to the ground (gravity speed)
    private float _ySpeed;

    // Start is called before the first frame update
    void Start()
    {
        _ySpeed = 0;
        _cameraTransform = Camera.main.transform;

        _characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        // Stores inputs
        float verticalInput = Input.GetAxis("Vertical");
        float horizontalInput = Input.GetAxis("Horizontal");

        // Create Vector from inputs
        Vector3 movementDirection = new Vector3(horizontalInput, 0, verticalInput);
        float inputMagnitude = Mathf.Clamp01(movementDirection.magnitude);

        // Should walk? (left or right shift held)
        bool shouldWalk = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        // Set speed to half of input when charakter should walk
        // otherwise use horizontal input
        float speed = shouldWalk ? inputMagnitude * 0.333f : inputMagnitude;        

        // Make movement direction depend on camera rotation
        movementDirection = Quaternion.AngleAxis(_cameraTransform.rotation.eulerAngles.y, Vector3.up)
            * movementDirection;
        
        // Rotate the character to movement direction
        if (movementDirection != Vector3.zero)
        {
            Quaternion targetCharacterRotation = Quaternion.LookRotation(movementDirection, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetCharacterRotation, RotationSpeed * Time.deltaTime);
        }

        // Calculate gravity
        _ySpeed = IsGrounded() ? _ySpeed = 0 : _ySpeed += Physics.gravity.y * GravityMultiplier * Time.deltaTime;
        movementDirection.y = _ySpeed;

        // Move the character        
        _characterController.Move(movementDirection * speed * MaxMovementSpeed * Time.deltaTime);

        // Character is audible, when moving fast
        IsAudible = speed >= 0.5f;
    }

    /// <summary>
    /// Check if the chracter is on ground (Perform a raycast)
    /// </summary>
    /// <returns>True if the raycasts hit smthg. in distance</returns>
    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, _characterController.height * 0.5f + GroundCheckDistance);
    }

    /// <summary>
    /// Visualize the raycast for testing purposes
    /// </summary>
    private void OnDrawGizmos()
    {
        if(_characterController!=null)
        {
            Debug.DrawRay(transform.position, Vector3.down * (_characterController.height * 0.5f + GroundCheckDistance), IsGrounded() ? Color.cyan : Color.red);
        }        
    }
}
