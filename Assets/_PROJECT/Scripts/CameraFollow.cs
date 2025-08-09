using UnityEngine;

namespace _PROJECT.Scripts
{
    public class CameraFollow : MonoBehaviour
    {
        [Header("Follow Parameters")]
        
        [Tooltip("GameObject you want the camera to follow.")]
        public Transform target;
        
        [SerializeField, Range(0.01f, 1f), Tooltip("How fast the camera follows the object")]
        private float smoothSpeed = 0.125f;
        
        [SerializeField, Tooltip("Camera offset from the transform target")]
        private Vector3 offset = new Vector3(0f, 2.25f, -1.5f);
        
        // Necessary for Smooth Damp function
        private Vector3 _velocity = Vector3.zero;

        private void LateUpdate()
        {
            // Get the position the camera should move to
            Vector3 desiredPosition = target.position + offset;
            // Move the camera using a SmoothDamp function
            transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref _velocity, smoothSpeed);
        }

        public void CenterOnTarget()
        {
            transform.position = target.position + offset;
        }
    }
}
