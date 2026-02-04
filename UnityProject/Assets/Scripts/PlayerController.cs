using UnityEngine;
using System;
using UnityEngine.UIElements;
using UnityEngine.Rendering;

public class PlayerController : MonoBehaviour
{
    //public AudioSource walkAudio; // Sounddesign - Walking Sound

    public enum PlayerState { Active, Passive }
    public static PlayerState CurrentPlayerState { get; private set; }

    public enum Ammo { Orb, Color }
    public static Ammo CurrentAmmo { get; private set; }

    public static event Action<int> UpdateOrbCountEvent;
    public static event Action SwitchAmmoEvent;
    public static event Action<Color[]> UpdateHudColorSplashesEvent;

    private Rigidbody _rigidbody;
    [SerializeField] private GameObject _camera;
    [SerializeField] private LayerMask _floorLayer;
    private float _horizontalMouse;
    private float _verticalMouse;
    private float _horizontalInput;
    private float _verticalInput;
    private float _pitch = 0f;
    [SerializeField] private float _rotationSpeedVertical = 500f;
    [SerializeField] private float _rotationSpeedHorizontal = 500f;
    [SerializeField] private float _movementSpeed = 0.2f;
    [SerializeField] private float _airMovementSpeed = 0.04f;
    [SerializeField] private float _jumpForce = 300f;
    [SerializeField] private float _orbThrowForce = 200f;
    [SerializeField] private float _colorThrowForce = 800f;

    private bool _isGrounded = false;

    private int _orbCount = 0;

    private Color _colorOne = Color.clear;
    private Color _colorTwo = Color.clear;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        Collectible.CollectibleCollisionEvent += OnCollectibleCollision;
        Minigame.MinigameStartEvent += OnMinigameStart;
        Minigame.MinigameEndEvent += OnMinigameEnd;
        Minigame.GainedColorEvent += OnGainedColor;
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
                
                MoveAndJump();

                if(Input.GetAxis("Mouse ScrollWheel") != 0f)
                {
                    SwitchAmmo();
                }

                switch (CurrentAmmo)
                {
                    case Ammo.Orb:
                        if (Input.GetMouseButtonDown(0))
                        {
                            ThrowOrb();
                        }
                        break;

                    case Ammo.Color:
                        if (Input.GetMouseButtonDown(0) && !_colorOne.Equals(Color.clear))
                        {
                            UseBrush(_colorOne);
                        }
                        else if (Input.GetMouseButtonDown(1) && !_colorTwo.Equals(Color.clear))
                        {
                            UseBrush(_colorTwo);
                        }
                        break;
                }

                break;

            case PlayerState.Passive:   
                break;
        }


        // Sounddesign - Walking Sound
        //if (Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0)
        //{
        //    if (!walkAudio.isPlaying)
        //        walkAudio.Play();
        //}
        //else
        //{
        //    walkAudio.Stop();
        //}
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

    void MoveAndJump()
    {
        Vector3 oldPosition = _rigidbody.transform.position;

        // Move Slower while in the air
        Vector3 newPostitionForward = _rigidbody.transform.forward * _verticalInput * (_isGrounded ? _movementSpeed : _airMovementSpeed);
        Vector3 newPostitionRight = _rigidbody.transform.right * _horizontalInput * (_isGrounded? _movementSpeed : _airMovementSpeed);
       
        Vector3 newPosition = oldPosition + newPostitionForward + newPostitionRight;
        
        _rigidbody.MovePosition(
            newPosition
        );

        if (Input.GetKeyDown(KeyCode.Space) && _isGrounded)
        {
            _rigidbody.AddForce(transform.up * _jumpForce);
            _rigidbody.AddForce(newPostitionForward * _jumpForce * 4f); // Apply Jump Force in Move Direction
            _rigidbody.AddForce(newPostitionRight * _jumpForce * 2f);
        }
    }


    void UseBrush(Color brushColor)
    {
        GameObject splash = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        splash.tag = "Interactor";
        splash.GetComponent<Renderer>().material.color = brushColor;
        splash.GetComponent<Collider>().isTrigger = true;
        splash.transform.position = _camera.transform.position + _camera.transform.forward.normalized * 2;
        splash.transform.localScale = new Vector3(
            0.3f,
            0.3f,
            0.3f
        );

        // Add a standard Rigidbody
        Rigidbody rb = splash.AddComponent<Rigidbody>();
        SplashBehaviour splashBehaviour = splash.AddComponent<SplashBehaviour>();

        rb.AddForce(_camera.transform.forward * _colorThrowForce);
    }

    void SwitchAmmo()
    {
        // iterates thru Ammo states
        CurrentAmmo = (Ammo)(((int)CurrentAmmo + 1) % Enum.GetValues(typeof(Ammo)).Length);

        SwitchAmmoEvent?.Invoke();
    }

    void ThrowOrb()
    {
        if (_orbCount < 1) return;

        GameObject orb = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        orb.GetComponent<Renderer>().material.color = Color.green;
        orb.transform.position = _camera.transform.position + _camera.transform.forward.normalized * 2;
        orb.transform.localScale = new Vector3(
            0.5f,
            0.5f,
            0.5f
        );

        // Add a standard Rigidbody
        Rigidbody rb = orb.AddComponent<Rigidbody>();

        rb.AddForce(_camera.transform.forward * _orbThrowForce);

        Destroy(orb, 4);

        _orbCount--;
        UpdateOrbCountEvent?.Invoke(_orbCount);
        Debug.Log($"Orb Count: {_orbCount}");
    }

    void OnCollectibleCollision()
    {
        _orbCount++;
        UpdateOrbCountEvent?.Invoke(_orbCount);
        Debug.Log($"Orb Count: {_orbCount}");
    }

    void OnMinigameStart()
    {
        CurrentPlayerState = PlayerState.Passive;
    }

    void OnMinigameEnd()
    {
        CurrentPlayerState = PlayerState.Active;
    }
    
    void OnGainedColor(Color[] colors)
    {
        _colorOne = colors[0];
        _colorTwo = colors[1];
        UpdateHudColorSplashesEvent?.Invoke(colors);
        Debug.Log("You gained color.");
    }

    void OnCollisionEnter(Collision collision)
    {
        int collisionLayerMask = 1 << collision.gameObject.layer;
        if ((collisionLayerMask & _floorLayer) != 0)
        {
            _isGrounded = true;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        int collisionLayerMask = 1 << collision.gameObject.layer;
        if ((collisionLayerMask & _floorLayer) != 0)
        {
            _isGrounded = false;
        }
    }
    void OnDestroy()
    {
        Collectible.CollectibleCollisionEvent -= OnCollectibleCollision;
        Minigame.MinigameStartEvent -= OnMinigameStart;
        Minigame.MinigameEndEvent -= OnMinigameEnd;
        Minigame.GainedColorEvent -= OnGainedColor;
    }

}
