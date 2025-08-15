using UnityEngine;
using UnityEngine.InputSystem;

namespace _PROJECT.Scripts
{
    public class CameraFollow : MonoBehaviour
    {
        private const float YMin = -50.0f;
        private const float YMax = 50.0f;

        public Transform lookAt;

        public Transform Player;

        public float distance = 10.0f;
        private float currentX = 0.0f;
        private float currentY = 0.0f;
        public float sensivity = 400.0f;
        public Vector2 mousePos;

        // Update is called once per frame
        void LateUpdate()
        {
            currentX += mousePos.x * sensivity * Time.deltaTime;
            currentY += mousePos.y * sensivity * Time.deltaTime;

            currentY = Mathf.Clamp(currentY, YMin, YMax);

            Vector3 Direction = new Vector3(0, 0, distance);
            Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
            transform.position = lookAt.position + rotation * Direction;

            transform.LookAt(lookAt.position);
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            mousePos = context.ReadValue<Vector2>();
        }
    }
}
