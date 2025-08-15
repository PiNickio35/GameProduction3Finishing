using UnityEngine;
using UnityEngine.InputSystem;

namespace _PROJECT.Scripts
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        private CharacterController _controller;
        private Vector3 _playerVelocity;
        private bool _groundedPlayer;
        
        [SerializeField] private float playerSpeed = 5.0f;
        [SerializeField] private float jumpHeight = 1.5f;
        [SerializeField] private float gravityValue = -9.81f;
        
        private Vector2 _moveInput = Vector2.zero;
        private bool _jumped = false;
        
        [SerializeField] private Transform cam;

        private void Awake()
        {
            _controller = gameObject.GetComponent<CharacterController>();
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            _moveInput = context.ReadValue<Vector2>();
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            _jumped = context.action.triggered;
        }

        private void Update()
        {
            _groundedPlayer = _controller.isGrounded;
            if (_groundedPlayer && _playerVelocity.y < 0)
            {
                _playerVelocity.y = 0f;
            }

            float Horizontal = _moveInput.x * playerSpeed * Time.deltaTime;
            float Vertical = _moveInput.y * playerSpeed * Time.deltaTime;

            Vector3 Movement = cam.transform.right * Horizontal + cam.transform.forward * Vertical;
            Movement.y = 0f;

            _controller.Move(Movement);

            if (Movement.magnitude != 0f)
            {
                transform.Rotate(Vector3.up * cam.GetComponent<CameraFollow>().mousePos.x * cam.GetComponent<CameraFollow>().sensivity * Time.deltaTime);


                Quaternion CamRotation = cam.rotation;
                CamRotation.x = 0f;
                CamRotation.z = 0f;

                transform.rotation = Quaternion.Lerp(transform.rotation, CamRotation, 0.1f);
            }

            // Jump
            if (_jumped && _groundedPlayer)
            {
                _playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
            }

            // Apply gravity
            _playerVelocity.y += gravityValue * Time.deltaTime;
            
            _controller.Move(_playerVelocity * Time.deltaTime);
        }
    }
}
