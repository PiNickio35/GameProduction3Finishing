using System.Collections;
using TMPro;
using UnityEngine;

namespace _PROJECT.Scripts
{
    public class PlayerLobbyManager : MonoBehaviour
    {
        public static PlayerLobbyManager Instance;
        private int _playerNumber;
        private int _currentPlayerNumber;
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
            _currentPlayerNumber++;
            Debug.Log(_currentPlayerNumber);
            if (_currentPlayerNumber == _playerNumber)
            {
                Debug.Log("Start");
                StartCoroutine(CountDown());
            }
        }

        private IEnumerator CountDown()
        {
            countdownText.gameObject.SetActive(true);
            countdownText.text = "3";
            yield return new WaitForSeconds(1);
            countdownText.text = "2";
            yield return new WaitForSeconds(1);
            countdownText.text = "1";
            yield return new WaitForSeconds(1);
            countdownText.text = "GO";
            startYourEngines = true;
            LapManager.Instance.StartCoroutine(LapManager.Instance.DisplayLap());
            yield return new WaitForSeconds(1);
            countdownText.gameObject.SetActive(false);
        }
    }
}
