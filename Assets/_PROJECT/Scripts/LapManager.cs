using System.Collections.Generic;
using UnityEngine;

namespace _PROJECT.Scripts
{
    public class LapManager : MonoBehaviour
    {
        public List<Checkpoint> checkpoints;
        public int totalLaps;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.GetComponent<PlayerController>())
            {
                PlayerController playerController = other.gameObject.GetComponent<PlayerController>();
                if (playerController.checkpointIndex == checkpoints.Count)
                {
                    playerController.checkpointIndex = 0;
                    playerController.lapNumber++;

                    if (playerController.lapNumber > totalLaps)
                    {
                        Debug.Log("You Win!");
                    }
                }
            }
        }
    }
}
