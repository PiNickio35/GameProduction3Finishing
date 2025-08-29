using UnityEngine;

namespace _PROJECT.Scripts
{
    public class UtilityFunctions : MonoBehaviour
    {
        public void SetDrifting()
        {
            transform.parent.GetComponent<PlayerController>().isSliding = true;
        }
    }
}
