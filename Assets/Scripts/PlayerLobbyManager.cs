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
            StartCoroutine(CountDown());
        }
    }

    private IEnumerator CountDown()
    {
        countdownText.gameObject.SetActive(true);
        countdownText.text = "3";
        LapManager.Instance.beepSound.Play();
        yield return new WaitForSeconds(1);
        countdownText.text = "2";
        LapManager.Instance.beepSound.Play();
        yield return new WaitForSeconds(1);
        countdownText.text = "1";
        LapManager.Instance.beepSound.Play();
        yield return new WaitForSeconds(1);
        countdownText.text = "GO";
        LapManager.Instance.goSound.Play();
        startYourEngines = true;
        LapManager.Instance.StartCoroutine(LapManager.Instance.DisplayLap());
        yield return new WaitForSeconds(1);
        countdownText.gameObject.SetActive(false);
    }
}