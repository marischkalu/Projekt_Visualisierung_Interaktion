using UnityEngine;
using System;
using UnityEngine.UIElements;
using UnityEngine.Rendering;

public class PlayerController : MonoBehaviour
{
    public enum PlayerState { Active, Passive }
    public static PlayerState CurrentPlayerState { get; private set; }

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


    private int _orbCount = 0;
    private bool _hasBrush = false;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        Collectible.CollectibleCollisionEvent += OnCollectibleCollision;
        Minigame.MinigameStartEvent += OnMinigameStart;
        Minigame.MinigameEndEvent += OnMinigameEnd;
        Minigame.GainedBrushEvent += OnGainedBrush;
    }

    // Update is called once per frame
    void Update()
    {
        _horizontalMouse = Input.GetAxis("Mouse X"); // Left/Right
        _verticalMouse = Input.GetAxis("Mouse Y");   // Up/Down
        _horizontalInput = Input.GetAxis("Horizontal"); // A/D
        _verticalInput = Input.GetAxis("Vertical");   // W/S

        switch (CurrentPlayerState)
        {
            case PlayerState.Active:

                RotatePlayer();
                MovePlayer();

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    ThrowOrb();
                }

                if (Input.GetMouseButtonDown(0) && _hasBrush)
                {
                    UseBrush();
                }
                break;

            case PlayerState.Passive:

                
                break;
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
        Vector3 oldPosition = _rigidbody.transform.position;
        Vector3 newPostitionForward = _rigidbody.transform.forward * _verticalInput * _movementSpeed;
        Vector3 newPostitionRight = _rigidbody.transform.right * _horizontalInput * _movementSpeed;
        Vector3 newPosition = oldPosition + newPostitionForward + newPostitionRight;
            
        _rigidbody.MovePosition(
            newPosition
        );
    }

    void UseBrush()
    {
        Debug.Log("You are using your brush!");
    }

    void ThrowOrb()
    {
        if (_orbCount < 1) return;

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

        _orbCount--;
        Debug.Log($"Orb Count: {_orbCount}");
    }

    void OnCollectibleCollision()
    {
        _orbCount++;
        Debug.Log($"Orb Count: {_orbCount}");
    }

    void OnMinigameStart()
    {
        CurrentPlayerState = PlayerState.Passive;
    }

    void OnMinigameEnd()//bool gainedReward)
    {
        //_hasBrush = gainedReward;
        //if (_hasBrush) Debug.Log("You gained a paint brush.");
        //else Debug.Log("Submitted color combination wrong, try again.");
        CurrentPlayerState = PlayerState.Active;
    }
    
    void OnGainedBrush()
    {
        _hasBrush = true;
        Debug.Log("You gained a paint brush.");
    }

    void OnDestroy()
    {
        Collectible.CollectibleCollisionEvent -= OnCollectibleCollision;
        Minigame.MinigameStartEvent -= OnMinigameStart;
        Minigame.MinigameEndEvent -= OnMinigameEnd;
        Minigame.GainedBrushEvent -= OnGainedBrush;
    }

}
