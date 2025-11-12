using Unity.Netcode;
using UnityEngine;

namespace Client
{
    public class CameraFollow : NetworkBehaviour
    {
        public Vector3 offset;
        public Transform target;
        
        private PlayerController _player;
        [SerializeField] private Camera playerCamera;
        
        public Vector3 origCamPos;
        public Vector3 boostCamPos;

        public override void OnNetworkSpawn()
        {
            if (!IsOwner) return;
            _player = target.GetComponent<PlayerController>();
            playerCamera.gameObject.SetActive(true);
        }

        private void LateUpdate()
        {
            if (!IsOwner) return;
            transform.position = target.position + offset;
            transform.rotation = Quaternion.Slerp(transform.rotation, target.rotation, 3 * Time.deltaTime);

            transform.GetChild(0).localPosition = Vector3.Lerp(transform.GetChild(0).localPosition, _player.boostTime > 0 ? boostCamPos : origCamPos, 3 * Time.deltaTime);
        }
    }
}