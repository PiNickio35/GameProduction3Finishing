using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _PROJECT.Scripts
{
    public class LapManager : MonoBehaviour
    {
        public static LapManager Instance;
        public List<Checkpoint> checkpoints;
        public int totalLaps;
        [SerializeField] private TextMeshProUGUI lapNumberText;
        [SerializeField] private GameObject winText;
        [SerializeField] public AudioSource goSound, beepSound;
        [SerializeField] private AudioSource winSound;
        [SerializeField] private List<RectTransform> lapNumberTransform;
        [SerializeField] private List<Color> lapColor;

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
                    lapNumberText.rectTransform.position = lapNumberTransform[int.Parse(playerController.name.Last().ToString()) - 1].position;
                    lapNumberText.color = lapColor[int.Parse(playerController.name.Last().ToString()) - 1];
                    lapNumberText.text = playerController.lapNumber.ToString();
                    StartCoroutine(DisplayLap());
                    if (playerController.lapNumber > totalLaps)
                    {
                        Debug.Log("win");
                        winText.GetComponentInChildren<TextMeshProUGUI>().text = "You Win " + playerController.name + " !";
                        winText.gameObject.SetActive(true);
                        winSound.Play();
                        PlayerLobbyManager.Instance.startYourEngines = false;
                        StartCoroutine(WindDown());
                    }
                }
            }
        }

        private IEnumerator WindDown()
        {
            yield return new WaitForSeconds(2.5f);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public IEnumerator DisplayLap()
        {
            lapNumberText.gameObject.SetActive(true);
            yield return new WaitForSeconds(2.5f);
            lapNumberText.gameObject.SetActive(false);
        }
    }
}
