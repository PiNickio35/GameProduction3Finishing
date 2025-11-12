using Client;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public int index;
    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerController>())
        {
            PlayerController playerController = other.gameObject.GetComponent<PlayerController>();
            if (playerController.checkpointIndex == index - 1)
            {
                playerController.checkpointIndex = index;
                _audioSource.Play();
            }
        }
    }
}