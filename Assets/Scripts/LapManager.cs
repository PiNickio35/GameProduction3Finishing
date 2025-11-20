using System.Collections;
using System.Collections.Generic;
using Client;
using Server;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LapManager : MonoBehaviour
{
    public static LapManager Instance;
    public List<Checkpoint> checkpoints;
    public int totalLaps;
    [SerializeField] private TextMeshProUGUI lapNumberText;
    [SerializeField] public GameObject winText;
    [SerializeField] public AudioSource goSound, beepSound;
    [SerializeField] public AudioSource winSound;
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
                if (playerController.lapNumber > totalLaps)
                {
                    Debug.Log("win");
                    ServerRelay.Instance.WinServerRpc(playerController.name);
                    return;
                }
                lapNumberText.text = "Lap " + playerController.lapNumber.ToString() + "/" + totalLaps.ToString();
            }
        }
    }

    public IEnumerator WindDown()
    {
        yield return new WaitForSeconds(2.5f);
        Application.Quit();
    }
    
    public IEnumerator CountDown()
    {
        Debug.Log("Hier gat ons");
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
        lapNumberText.gameObject.SetActive(true);
        yield return new WaitForSeconds(1);
        countdownText.gameObject.SetActive(false);
    }
}