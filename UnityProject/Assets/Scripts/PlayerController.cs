using UnityEngine;
using System;
using UnityEngine.UIElements;
using UnityEngine.Rendering;

[RequireComponent(typeof(CharacterController))]
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
    [SerializeField] private Material _orbMaterial;

    [SerializeField] private float _orbThrowForce = 400f;
    [SerializeField] private float _colorThrowForce = 800f;

    private int _orbCount = 0;

    //private Color _colorOne = Color.clear;
    //private Color _colorTwo = Color.clear;
    private Color _colorOne = Color.red;
    private Color _colorTwo = Color.blue;


    private Vector3 _moveDirection = Vector3.zero;
    private CharacterController _characterController;
    private float _rotationX = 0;
    private float _walkSpeed = 6f;
    private float _runSpeed = 12f;
    private float _jumpPower = 7f;
    private float _gravity = 10f;
    private float _lookSpeed = 2f;
    private float _lookXLimit = 45f;
    private float _defaultHeight = 2f;
    private float _crouchHeight = 1f;
    private float _crouchSpeed = 3f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;

        _rigidbody = GetComponent<Rigidbody>();
        _characterController = GetComponent<CharacterController>();
        Collectible.CollectibleCollisionEvent += OnCollectibleCollision;
        Minigame.MinigameStartEvent += OnMinigameStart;
        Minigame.MinigameEndEvent += OnMinigameEnd;
        Minigame.GainedColorEvent += OnGainedColor;

        PauseGame.PauseMenuOpenedEvent += OnPauseMenuOpened;
        PauseGame.PauseMenuClosedEvent += OnPauseMenuClosed;
    }

    // Update is called once per frame
    void Update()
    {

        switch (CurrentPlayerState)
        {
            case PlayerState.Active:

                //RotatePlayer();

                MoveAndJump();

                if (Input.GetAxis("Mouse ScrollWheel") != 0f)
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



    void MoveAndJump()
    {
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        bool isRunning = _characterController.isGrounded ? Input.GetKey(KeyCode.LeftShift) : false;
        float curSpeedX = (isRunning ? _runSpeed : _walkSpeed) * Input.GetAxis("Vertical");
        float curSpeedY = (isRunning ? _runSpeed : _walkSpeed) * Input.GetAxis("Horizontal");
        float movementDirectionY = _moveDirection.y;
        _moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        if (Input.GetButton("Jump") && _characterController.isGrounded)
        {
            _moveDirection.y = _jumpPower;
        }
        else
        {
            _moveDirection.y = movementDirectionY;
        }

        if (!_characterController.isGrounded)
        {
            _moveDirection.y -= _gravity * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.LeftControl))
        {
            _characterController.height = _crouchHeight;
            _walkSpeed = _crouchSpeed;
            _runSpeed = _crouchSpeed;

        }
        else
        {
            _characterController.height = _defaultHeight;
            _walkSpeed = 6f;
            _runSpeed = 12f;
        }

        _characterController.Move(_moveDirection * Time.deltaTime);

        _rotationX += -Input.GetAxis("Mouse Y") * _lookSpeed;
        _rotationX = Mathf.Clamp(_rotationX, -_lookXLimit, _lookXLimit);
        _camera.transform.localRotation = Quaternion.Euler(_rotationX, 0, 0);
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * _lookSpeed, 0);
    }


    void UseBrush(Color brushColor)
    {
        GameObject splash = GameObject.CreatePrimitive(PrimitiveType.Sphere);
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

        splashBehaviour.SplashColor = brushColor;

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
        if (_orbMaterial != null) orb.GetComponent<Renderer>().material = _orbMaterial;
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
        LockPlayer();
    }

    void OnMinigameEnd()
    {
        UnlockPlayer();
    }

    void OnPauseMenuOpened()
    {
        LockPlayer();
    }

    void OnPauseMenuClosed()
    {
        UnlockPlayer();
    }

    void LockPlayer()
    {
        CurrentPlayerState = PlayerState.Passive;
        UnityEngine.Cursor.lockState = CursorLockMode.Confined;
        UnityEngine.Cursor.visible = true;
    }
    
    void UnlockPlayer()
    {
        CurrentPlayerState = PlayerState.Active;
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;
    }
    
    void OnGainedColor(Color[] colors)
    {
        _colorOne = colors[0];
        _colorTwo = colors[1];
        UpdateHudColorSplashesEvent?.Invoke(colors);
        Debug.Log("You gained color.");
    }

    void OnDestroy()
    {
        Collectible.CollectibleCollisionEvent -= OnCollectibleCollision;
        Minigame.MinigameStartEvent -= OnMinigameStart;
        Minigame.MinigameEndEvent -= OnMinigameEnd;
        Minigame.GainedColorEvent -= OnGainedColor;

        PauseGame.PauseMenuOpenedEvent -= OnPauseMenuOpened;
        PauseGame.PauseMenuClosedEvent -= OnPauseMenuClosed;
    }
}