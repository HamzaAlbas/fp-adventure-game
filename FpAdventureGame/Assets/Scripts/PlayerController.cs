using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    #region VARIABLES 

    public float walkingSpeed = 7.5f;
    public float runningSpeed = 11.5f;
    public float stamina = 20f;
    private float _speed;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;
    public Camera playerCamera;
    public float lookSpeed = 2.0f;
    public float lookXLimit = 45.0f;

    public AudioClip[] dirtSounds = new AudioClip[10];
    public AudioClip[] concreteSounds = new AudioClip[10];
    private AudioClip _selected;
    public AudioSource audioSource;
    public float footStepDelay, runStepDelay;
    private float _nextFootStep;
    private int _randomMusic;

    private CharacterController _characterController;
    public Vector3 moveDirection = Vector3.zero;
    private float _rotationX;

    public Slider slider;

    [HideInInspector]
    public bool canMove = true;

    #endregion

    private void Start()
    {
        _characterController = GetComponent<CharacterController>();

        // Lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        var forward = transform.TransformDirection(Vector3.forward);
        var right = transform.TransformDirection(Vector3.right);
        
        // Press Left Shift to run
        var isRunning = Input.GetKey(KeyCode.LeftShift);

        if (isRunning && stamina >= 0)
        {
            _speed = runningSpeed;
            stamina -= Time.deltaTime;
            slider.value = stamina;
        }
        else
        {
            _speed = walkingSpeed;
        }
        
        
        var curSpeedX = canMove ? (_speed) * Input.GetAxis("Vertical") : 0;
        var curSpeedY = canMove ? (_speed) * Input.GetAxis("Horizontal") : 0;
        var movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        if (Input.GetButton("Jump") && canMove && _characterController.isGrounded)
        {
            // Jumped
            moveDirection.y = jumpSpeed;
            audioSource.Stop();
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }

        // Apply gravity
        if (!_characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        // Move 
        _characterController.Move(moveDirection * Time.deltaTime);

        // Player and Camera rotation
        if (canMove)
        {
            _rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            _rotationX = Mathf.Clamp(_rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(_rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }


        if (curSpeedX is > 0 and <= 5f || curSpeedY is > 0 and <= 5f )
        {
            // Walking
            if (!_characterController.isGrounded) return;
            _nextFootStep -= Time.deltaTime;
            ChooseWalkSound();
            if (!(_nextFootStep <= 0)) return;
            audioSource.PlayOneShot(_selected, 0.3f);
            _nextFootStep += footStepDelay;
            if (stamina <= 10f && !isRunning)
            {
                stamina += Time.deltaTime * 10;
                slider.value = stamina;
            }
        }
        else if (curSpeedX is < 0 and >= -5f || curSpeedY is < 0 and >= -5f)
        {
            // Walking back
            if (!_characterController.isGrounded) return;
            _nextFootStep -= Time.deltaTime;
            ChooseWalkSound();
            if (!(_nextFootStep <= 0)) return;
            audioSource.PlayOneShot(_selected, 0.3f);
            _nextFootStep += footStepDelay;
            if (stamina <= 10f && !isRunning)
            {
                stamina += Time.deltaTime * 10;
                slider.value = stamina;
            }
        }
        else if (curSpeedX > 5f || curSpeedY > 5f)
        {
            // Running
            if (!_characterController.isGrounded) return;
            _nextFootStep -= Time.deltaTime;
            ChooseWalkSound();
            if (!(_nextFootStep <= 0)) return;
            audioSource.PlayOneShot(_selected, 0.4f);
            _nextFootStep += runStepDelay;
        }
        else if (curSpeedX < -5f || curSpeedY < -5f)
        {
            // Running Back
            if (!_characterController.isGrounded) return;
            _nextFootStep -= Time.deltaTime;
            ChooseWalkSound();
            if (!(_nextFootStep <= 0)) return;
            audioSource.PlayOneShot(_selected, 0.4f);
            _nextFootStep += runStepDelay;
        }
        else
        {
            // Not walking
            audioSource.Stop();
            if (stamina <= 10f && !isRunning)
            {
                stamina += Time.deltaTime;
                slider.value = stamina;
            }
        }

    }
    
    private void ChooseWalkSound()
    {
        // Get the type of ground
        if (!Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out var hit, 2f)) return;
        
        // Select the footstep to play
        if (hit.transform.gameObject.CompareTag("Dirt"))
        {
            _selected = dirtSounds[Random.Range(0, dirtSounds.Length)];
        }
        else if (hit.transform.gameObject.CompareTag("Concrete"))
        {
            _selected = concreteSounds[Random.Range(0, concreteSounds.Length)];

        }
    }
}
