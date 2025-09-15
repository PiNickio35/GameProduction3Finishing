using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace _PROJECT.Scripts
{
    public class LapManager : MonoBehaviour
    {
        public List<Checkpoint> checkpoints;
        public int totalLaps;
        [SerializeField] private TextMeshProUGUI lapNumberText;
        [SerializeField] private GameObject winText;

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
                    }
                }
            }
        }
        
        private IEnumerator DisplayLap()
        {
            lapNumberText.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            lapNumberText.gameObject.SetActive(false);
        }
    }
}
