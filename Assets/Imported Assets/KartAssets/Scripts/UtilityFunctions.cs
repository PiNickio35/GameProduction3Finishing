using UnityEngine;

namespace Imported_Assets.KartAssets.Scripts
{
    public class UtilityFunctions : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        public void SetDrifting()
        {
            transform.parent.GetComponent<PlayerScript>().isSliding = true;
        }
    }
}
