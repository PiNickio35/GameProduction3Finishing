using UnityEngine;

public class UtilityFunctions : MonoBehaviour
{
    public void SetDrifting()
    {
        transform.parent.GetComponent<PlayerController>().isSliding = true;
    }
}