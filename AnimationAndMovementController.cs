using UnityEngine;
using UnityEngine.InputSystem;

public class AnimationAndMovementController : MonoBehaviour
{
    private PlayerInput _playerInput;
    private CharacterController _characterController;
    private Animator _animator;

    private Vector2 _currentMovementInput;
    private Vector3 _currenMovement;
    private Vector3 _currenRunMovement;

    private float _rotationFactorPerFrame = 15.0f;
    private bool _isMovementPressed;
    private bool _isRunPressed;
    private int _isWalkingHash;
    private int _isRunningHash;

    private void Awake()
    {
        // set referance variables
        _playerInput = new PlayerInput();
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();

        _isWalkingHash = Animator.StringToHash("isWalking");
        _isRunningHash = Animator.StringToHash("isRunning");

        // set the player input callbacks
        _playerInput.CharacterControls.Move.started += OnMovementInput; // reaches the Move action in action map and listen when the player starts using this action.
        _playerInput.CharacterControls.Move.canceled+= OnMovementInput;
        _playerInput.CharacterControls.Move.performed += OnMovementInput;
        _playerInput.CharacterControls.Run.started += OnRun;
        _playerInput.CharacterControls.Run.canceled += OnRun;
     
    }
    void OnEnable()
    {
        _playerInput.CharacterControls.Enable();
    }
    private void OnDisable()
    {
        _playerInput.CharacterControls.Disable();
    }

    void Update()
    {
        HandleRotation();
        HandleAnimation();
        HandleGravity();

        if (_isRunPressed)
        {
            _characterController.Move(_currenRunMovement * Time.deltaTime);
        }
        else
        {
            _characterController.Move(_currenMovement * Time.deltaTime);
        }
    }
    private void OnRun(InputAction.CallbackContext context)
    {
        // run action is set to button function, so we can read value.
        _isRunPressed = context.ReadValueAsButton();
    }
    private void OnMovementInput(InputAction.CallbackContext context) 
    {
        _currentMovementInput = context.ReadValue<Vector2>(); // store the inputs.

        _currenMovement.x = _currentMovementInput.x;
        _currenMovement.z = _currentMovementInput.y;
        _currenRunMovement.x = _currentMovementInput.x * 3.0f;
        _currenRunMovement.z = _currentMovementInput.y * 3.0f;

        _isMovementPressed = _currenMovement.x != 0 || _currenMovement.z != 0;

    }

    private void HandleGravity()
    {
        if(_characterController.isGrounded)
        {
            float _groundGravity = -0.05f;
            _currenMovement.y = _groundGravity;
            _currenRunMovement.y = _groundGravity;
        }
        else
        {
            float _gravity = -9.8f;
            _currenMovement.y += _gravity;
            _currenRunMovement.y += _gravity;
        }

    }
    private void HandleAnimation()
    {
        // get parameter values from animator. 
        bool isWalking = _animator.GetBool(_isWalkingHash);
        bool isRunning = _animator.GetBool(_isRunningHash);

        // start walking if movement pressed is true and not already walking.
        if (_isMovementPressed && !isWalking)
        {
            _animator.SetBool("isWalking", true);
        }
        // stop walking if movement pressed is false and not already walking.
        else if (!_isMovementPressed && isWalking)
        {
            _animator.SetBool("isWalking", false);
        }
        // run if movement and run pressed are true and not currently running.
        if((_isMovementPressed && _isRunPressed) && !isRunning)
        {
            _animator.SetBool(_isRunningHash, true);
        }

        // stop running if movement and run pressed are false and currently running.
        else if ((!_isMovementPressed || !_isRunPressed) && isRunning)
        {
            _animator.SetBool(_isRunningHash, false);
        }
    }

    private void HandleRotation()
    {
        Vector3 _positionToLookAt;

        // the change in position our character should point to
        _positionToLookAt.x = _currenMovement.x;
        _positionToLookAt.y = 0.0f;
        _positionToLookAt.z=_currenMovement.z;
        // the current rotation of our character. 
        Quaternion _currentRotation = transform.rotation;

        if(_isMovementPressed)
        {
            // creates a new rotation based on where the player is currently pressing.
            Quaternion _targetRotation = Quaternion.LookRotation(_positionToLookAt);
            transform.rotation= Quaternion.Slerp(_currentRotation, _targetRotation, _rotationFactorPerFrame * Time.deltaTime);
        }
    }
 
   
}
