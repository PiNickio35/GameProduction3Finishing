using UnityEngine;
using UnityEngine.InputSystem;

namespace _PROJECT.Scripts
{
    public class PlayerData : MonoBehaviour
    {
        public string playerName;
        [SerializeField] private float playerHealth;
        [SerializeField] private float playerScore;
        
        private PlayerInput _playerInput;

        private void Awake()
        {
            _playerInput = GetComponent<PlayerInput>();
        }
    }
}
