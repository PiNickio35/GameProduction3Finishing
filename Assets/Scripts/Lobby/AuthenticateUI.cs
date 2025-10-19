using UnityEngine;
using UnityEngine.UI;

namespace Lobby
{
    public class AuthenticateUI : MonoBehaviour
    {
        [SerializeField] private Button authenticateButton;

        private void Awake()
        {
            authenticateButton.onClick.AddListener(() =>
            {
                LobbyManager.Instance.Authenticate(EditPlayerName.Instance.GetPlayerName());
                gameObject.SetActive(false);
            });
        }
    }
}
