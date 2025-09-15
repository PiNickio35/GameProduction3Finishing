using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace _PROJECT.Scripts
{
    public class LapManager : MonoBehaviour
    {
        public static LapManager Instance;
        public List<Checkpoint> checkpoints;
        public int totalLaps;
        [SerializeField] private TextMeshProUGUI lapNumberText;
        [SerializeField] private GameObject winText;

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
                    // lapNumberText.transform.position = playerController.gameObject.transform.position;
                    lapNumberText.text = playerController.lapNumber.ToString();
                    StartCoroutine(DisplayLap());
                    if (playerController.lapNumber > totalLaps)
                    {
                        Debug.Log("win");
                        winText.gameObject.SetActive(true);
                        PlayerLobbyManager.Instance.startYourEngines = false;
                    }
                }
            }
        }
        
        public IEnumerator DisplayLap()
        {
            lapNumberText.gameObject.SetActive(true);
            yield return new WaitForSeconds(2.5f);
            lapNumberText.gameObject.SetActive(false);
        }
    }
}
