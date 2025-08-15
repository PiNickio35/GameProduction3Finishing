using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _PROJECT.Scripts
{
    public class TextPlayerData : MonoBehaviour
    {
        private PlayerInput connectedPlayerInput;

        public void ConnectPlayerToThisText(PlayerInput playerInput)
        {
            connectedPlayerInput = playerInput;
            if (connectedPlayerInput.gameObject.IsDestroyed())
            {
                DestroyThisText();
            }
        }

        public void DestroyThisText()
        {
            Destroy(gameObject);
        }
    }
}
