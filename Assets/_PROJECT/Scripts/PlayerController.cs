using UnityEngine;
using UnityEngine.InputSystem;

namespace _PROJECT.Scripts
{
    public class PlayerController : MonoBehaviour, CharacterInput.IPlayerMapActions
    {
        private Rigidbody _rigidbody;
        private CharacterInput _characterInput;
        private Vector3 _moveInput;
        private const float Speed = 5.00f;

        private void Awake()
        {
            if (_rigidbody == null)
            {
                _rigidbody = GetComponent<Rigidbody>();
            }
            _characterInput = new CharacterInput();
            _characterInput.Enable();
        }
        
        private void FixedUpdate()
        {
            _rigidbody.transform.Translate(_moveInput * (Speed * Time.deltaTime));
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            Debug.Log(context.ReadValue<Vector2>());
            _moveInput.x = context.ReadValue<Vector2>().x;
            _moveInput.z = context.ReadValue<Vector2>().y;
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            _rigidbody.AddForce(0, 5, 0, ForceMode.Impulse);
        }
    }
}
