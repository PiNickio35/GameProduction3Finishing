using System.Collections;
using System.Collections.Generic;
using Client;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LapManager : NetworkBehaviour
{
    public static LapManager Instance;
    public List<Checkpoint> checkpoints;
    public int totalLaps;
    [SerializeField] private TextMeshProUGUI lapNumberText;
    [SerializeField] private GameObject winText;
    [SerializeField] public AudioSource goSound, beepSound;
    [SerializeField] private AudioSource winSound;
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerController>())
        {
            PlayerController playerController = other.gameObject.GetComponent<PlayerController>();
            if (playerController.checkpointIndex == checkpoints.Count)
            {
                playerController.checkpointIndex = 0;
                playerController.lapNumber++;
                Debug.Log("Lap");
                lapNumberText.text = playerController.lapNumber.ToString();
                StartCoroutine(DisplayLap());
                if (playerController.lapNumber > totalLaps)
                {
                    Debug.Log("win");
                    WinServerRpc(playerController.name);
                }
            }
        }
    }

    private IEnumerator WindDown()
    {
        yield return new WaitForSeconds(2.5f);
        NetworkManager.SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
    }

    public IEnumerator DisplayLap()
    {
        lapNumberText.gameObject.SetActive(true);
        yield return new WaitForSeconds(2.5f);
        lapNumberText.gameObject.SetActive(false);
    }
    
    public IEnumerator CountDown()
    {
        countdownText.gameObject.SetActive(true);
        countdownText.text = "3";
        beepSound.Play();
        yield return new WaitForSeconds(1);
        countdownText.text = "2";
        beepSound.Play();
        yield return new WaitForSeconds(1);
        countdownText.text = "1";
        beepSound.Play();
        yield return new WaitForSeconds(1);
        countdownText.text = "GO";
        goSound.Play();
        startYourEngines = true;
        StartCoroutine(DisplayLap());
        yield return new WaitForSeconds(1);
        countdownText.gameObject.SetActive(false);
    }

    [ServerRpc]
    private void WinServerRpc(string playerName)
    {
        WinClientRpc(playerName);
    }

    [ClientRpc]
    private void WinClientRpc(string playerName)
    {
        winText.GetComponentInChildren<TextMeshProUGUI>().text = "You Win " + playerName + "!";
        winText.gameObject.SetActive(true);
        winSound.Play();
        startYourEngines = false;
        if (!IsHost) return;
        StartCoroutine(WindDown());
    }

    [ServerRpc]
    public void StartCountDownServerRpc()
    {
        StartCountDownClientRpc();
    }

    [ClientRpc]
    private void StartCountDownClientRpc()
    {
        StartCoroutine(CountDown());
    }
}