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

            // Read input
            Vector3 move = new Vector3(_moveInput.x, 0, _moveInput.y);
            _controller.Move(move * (playerSpeed * Time.deltaTime));

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
