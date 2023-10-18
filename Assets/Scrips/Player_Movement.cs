using UnityEngine;

public class Player_Movement : MonoBehaviour
{
    public float rotationSpeedX;
    public float rotationSpeedY;
    private float _cameraRotationX = 0f;
    private float _cameraRotationY = 0f;
    private AudioSource _audioSource;

    public float playerSpeed = 8f;
    public float jumpForce = 5f;
    public Transform playerCameraTransform;

    private Rigidbody _rb;
    private bool _isJumping;

    public float fallMultiplier = 3f;
    public float lowJumpMultiplier = 2f;


    public bool isMovementEnabled = true;


    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _audioSource = GetComponent<AudioSource>();
        playerCameraTransform.rotation = transform.rotation;
        _rb.freezeRotation = true;
        _isJumping = false;
    }

    private void Update()
    {
        if (isMovementEnabled)
        {
            if (Input.GetButtonDown("Jump") && IsGrounded())
            {
                _isJumping = true;
            }
        }
    }

    private void FixedUpdate()
    {
        if (isMovementEnabled)
        {
            // camera rotation
            float mouseX = Input.GetAxis("Mouse X") * rotationSpeedX * Time.fixedDeltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * rotationSpeedY * Time.fixedDeltaTime;

            _cameraRotationX -= mouseY;
            _cameraRotationY += mouseX;

            _cameraRotationX = Mathf.Clamp(_cameraRotationX, -90f, 90f);
            playerCameraTransform.rotation = Quaternion.Euler(_cameraRotationX, _cameraRotationY, 0f);

            //new Vector
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            bool isMoving = horizontal != 0f || vertical != 0f;

            Vector3 forwardDir = Vector3.ProjectOnPlane(playerCameraTransform.forward, Vector3.up).normalized;
            Vector3 rightDir = Vector3.Cross(Vector3.up, forwardDir).normalized;
            Vector3 movementDirection = vertical * forwardDir + horizontal * rightDir;
            
            if (movementDirection.magnitude > 1f)
                movementDirection.Normalize();
            

            if (isMoving)
            {
                if (!_audioSource.isPlaying)
                {
                    _audioSource.Play();
                }
            }
            else
            {
                _audioSource.Stop();
            }

            Vector3 newPosition = _rb.position + playerSpeed * Time.fixedDeltaTime * movementDirection;
            _rb.MovePosition(newPosition);

            // is player move ?
            if (movementDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(new Vector3(movementDirection.x, 0f, movementDirection.z));
                _rb.MoveRotation(Quaternion.Lerp(_rb.rotation, targetRotation, Time.fixedDeltaTime * rotationSpeedY));
            }

            if (_isJumping && Mathf.Abs(_rb.velocity.y) < 0.01f)
            {
                _rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
                _isJumping = false;
            }

            // jump setting
            if (_rb.velocity.y < 0)
                _rb.velocity += (fallMultiplier - 1) * Physics.gravity.y * Time.fixedDeltaTime * Vector3.up;
            else if (_rb.velocity.y > 0 && !Input.GetButton("Jump"))
                _rb.velocity += (lowJumpMultiplier - 1) * Physics.gravity.y * Time.fixedDeltaTime * Vector3.up;
        }
    }

    private bool IsGrounded()
    {
        float groundCheckDistance = 0.2f;
        return Physics.Raycast(transform.position, Vector3.down, groundCheckDistance);
    }

    public void ToggleSound(bool isSoundEnabled)
    {
        _audioSource.mute = !isSoundEnabled;
    }
}

