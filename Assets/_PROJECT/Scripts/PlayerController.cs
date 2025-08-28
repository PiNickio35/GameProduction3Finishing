using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _PROJECT.Scripts
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        private Rigidbody _rb;
        
        private float _currentSpeed;
        public float maxSpeed;
        public float boostSpeed;
        private float _realSpeed;

        private float _steerDirection;
        private float driftTime;

        private bool driftLeft;
        private bool driftRight;
        private float outwardsDriftForce = 50000;
        
        public bool touchingGround;

        [HideInInspector]
        public float boostTime;
        
        private Vector2 _moveInput = Vector2.zero;
        private bool _drifting;

        public int lapNumber;
        public int checkpointIndex;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
        }

        private void Start()
        {
            lapNumber = 1;
            checkpointIndex = 0;
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            _moveInput = context.ReadValue<Vector2>();
        }

        public void OnDrift(InputAction.CallbackContext context)
        {
            _drifting = context.action.triggered;
        }

        private void FixedUpdate()
        {
            Move();
            // TireSteer();
            Steer();
            GroundNormalRotation();
            Drift();
            Boosts();
        }

        private void Move()
        {
            _realSpeed = transform.InverseTransformDirection(_rb.linearVelocity).z;
            _currentSpeed = Mathf.Lerp(_currentSpeed, maxSpeed * _moveInput.y, Time.fixedDeltaTime);
            Vector3 vel = transform.forward * _currentSpeed;
            vel.y = _rb.linearVelocity.y;
            _rb.linearVelocity = vel;
        }

        private void Steer()
        {
            _steerDirection = _moveInput.x;

            if (driftLeft && !driftRight)
            {
                _steerDirection = _moveInput.x < 0 ? -1.5f : -0.5f;
                transform.GetChild(0).localRotation = Quaternion.Lerp(transform.GetChild(0).localRotation, Quaternion.Euler(0, -20f, 0), 8f * Time.fixedDeltaTime);
                // isSliding if hop is added
                if (touchingGround) _rb.AddForce(transform.right * (outwardsDriftForce * Time.fixedDeltaTime), ForceMode.Acceleration);
            }
            else if (driftRight && !driftLeft)
            {
                _steerDirection = _moveInput.x < 0 ? 1.5f : 0.5f;
                transform.GetChild(0).localRotation = Quaternion.Lerp(transform.GetChild(0).localRotation, Quaternion.Euler(0, 20f, 0), 8f * Time.fixedDeltaTime);
                // isSliding if hop is added
                if (touchingGround) _rb.AddForce(transform.right * (-outwardsDriftForce * Time.fixedDeltaTime), ForceMode.Acceleration);
            }
            else
            {
                transform.GetChild(0).localRotation = Quaternion.Lerp(transform.GetChild(0).localRotation, Quaternion.Euler(0, 0, 0), 8f * Time.fixedDeltaTime);
            }
            
            var steerAmount = _realSpeed > 20 ? _realSpeed / 1.5f * _steerDirection * 2 : _realSpeed * _steerDirection * 2;
            
            var steerDirectionVector = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y + steerAmount, transform.eulerAngles.z);

            transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, steerDirectionVector, Time.fixedDeltaTime * 3);
        }

        private void GroundNormalRotation()
        {
            if (Physics.Raycast(transform.position, -transform.up, out var hit, 1.75f))
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.FromToRotation(transform.up * 2, hit.normal) * transform.rotation, 7.5f * Time.fixedDeltaTime);
                touchingGround = true;
            }
            else
            {
                touchingGround = false;
            }
        }

        private void Drift()
        {
            if (_drifting && touchingGround)
            {
                // transform.GetChild(0).GetComponent<Animator>().SetTrigger("Hop");
                if (_steerDirection > 0)
                {
                    driftRight = true;
                    driftLeft = false;
                }
                else if (_steerDirection < 0)
                {
                    driftRight = false;
                    driftLeft = true;
                }
            }

            if (_drifting && touchingGround && _currentSpeed > 20 && _moveInput.x != 0)
            {
                driftTime += Time.fixedDeltaTime;

               // Particle system and different boost colours 
            }

            if (!_drifting || _realSpeed < 20)
            {
                driftLeft = false;
                driftRight = false;
                // isSliding = false;
                
                if (driftTime >= 1.5 && driftTime < 4)
                {
                    boostTime = 0.75f;
                }

                if (driftTime >= 4 && driftTime < 7)
                {
                    boostTime = 1.5f;
                }

                if (driftTime >= 7)
                {
                    boostTime = 2.5f;
                }
                
                driftTime = 0;
                // Stop particles
            }
        }

        private void Boosts()
        {
            boostTime -= Time.fixedDeltaTime;
            if (boostTime > 0)
            {
                // Particles
                maxSpeed = boostSpeed;
                _currentSpeed = Mathf.Lerp(_currentSpeed, maxSpeed, Time.fixedDeltaTime);
            }
            else
            {
                // Particles
                maxSpeed = boostSpeed - 20;
            }
        }
    }
}
