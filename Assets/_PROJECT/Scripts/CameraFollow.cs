using UnityEngine;

namespace _PROJECT.Scripts
{
    public class CameraFollow : MonoBehaviour
    {
        public Vector3 offset;
        public Transform target;
        
        private PlayerController player;
        
        public Vector3 origCamPos;
        public Vector3 boostCamPos;

        void Awake()
        {
            player = target.GetComponent<PlayerController>();
        }
        
        void LateUpdate()
        {
            transform.position = target.position + offset;
            transform.rotation = Quaternion.Slerp(transform.rotation, target.rotation, 3 * Time.deltaTime);

            transform.GetChild(0).localPosition = Vector3.Lerp(transform.GetChild(0).localPosition, player.boostTime > 0 ? boostCamPos : origCamPos, 3 * Time.deltaTime);
        }
    }
}
