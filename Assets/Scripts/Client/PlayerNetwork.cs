using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Client
{
    public class PlayerNetwork : NetworkBehaviour
    {
        [Header ("Movement")]
        private CharacterInput _characterInput;
        private Vector2 _moveInput = Vector2.zero;
        private float _realSpeed;
        private float _currentSpeed;
        public float maxSpeed = 40;
        private Rigidbody _rb;
        
        [SerializeField] private GameObject spawnedItemPrefab;

        public override void OnNetworkSpawn()
        {
            _rb = GetComponentInChildren<Rigidbody>();
            _characterInput = new CharacterInput();
            _characterInput.Enable();
        
            _characterInput.PlayerMap.Move.performed += OnMove;
            _characterInput.PlayerMap.Drift.performed += OnDrift;
            _characterInput.PlayerMap.Quit.performed += OnQuit;
        }

        private void FixedUpdate()
        {
            if (!IsOwner) return;
            _realSpeed = transform.InverseTransformDirection(_rb.linearVelocity).z;
            _currentSpeed = Mathf.Lerp(_currentSpeed, maxSpeed * _moveInput.y, Time.fixedDeltaTime);
            Vector3 vel = transform.forward * _currentSpeed;
            vel.y = _rb.linearVelocity.y;
            _rb.linearVelocity = vel;
        }

        private void OnQuit(InputAction.CallbackContext obj)
        {
            throw new NotImplementedException();
        }

        private void OnDrift(InputAction.CallbackContext obj)
        {
            GameObject tempHolder = Instantiate(spawnedItemPrefab);
            tempHolder.GetComponent<NetworkObject>().Spawn(true);
        }

        private void OnMove(InputAction.CallbackContext obj)
        {
            _moveInput = obj.ReadValue<Vector2>();
        }

        private void OnDisable()
        {
            _characterInput.PlayerMap.Move.performed -= OnMove;
            _characterInput.PlayerMap.Drift.performed -= OnDrift;
            _characterInput.PlayerMap.Quit.performed -= OnQuit;
        }

        [ServerRpc]
        private void TestServerRpc()
        {
            Debug.Log("TestServerRpc" + OwnerClientId);
        }

        [ClientRpc]
        private void TestClientRpc()
        {
            Debug.Log("TestClientRpc" + OwnerClientId);
        }
    }
}
