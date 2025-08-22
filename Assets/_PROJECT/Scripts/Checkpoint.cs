using UnityEngine;

namespace _PROJECT.Scripts
{
    public class Checkpoint : MonoBehaviour
    {
        public int index;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.GetComponent<PlayerController>())
            {
                PlayerController playerController = other.gameObject.GetComponent<PlayerController>();
                if (playerController.checkpointIndex == index - 1)
                {
                    playerController.checkpointIndex = index;
                }
            }
        }
    }
}
