using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLobbyManager : MonoBehaviour
{
    public static PlayerLobbyManager Instance;
    private int _playerNumber;
    public int currentPlayerNumber;
    [SerializeField] private PlayerInputManager playerInputManager;
    [SerializeField] private GameObject joinPanel;
    [SerializeField] private TextMeshProUGUI countdownText;
    public bool startYourEngines;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); 
        }
    }

    public void SetPlayerNumber(int number)
    {
        _playerNumber = number;
        joinPanel.SetActive(false);
        Debug.Log(_playerNumber);
    }

    public void OnJoinLobby()
    {
        currentPlayerNumber++;
        Debug.Log(currentPlayerNumber);
        if (currentPlayerNumber == _playerNumber)
        {
            Debug.Log("Start");
            playerInputManager.DisableJoining();
            LapManager.Instance.StartCoroutine(LapManager.Instance.CountDown());
        }
    }
}