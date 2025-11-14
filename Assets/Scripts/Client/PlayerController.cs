using System.Collections;
using Lobby;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Client
{
    public class PlayerController : NetworkBehaviour
    {
        private Rigidbody _rb;
        
        private float _currentSpeed;
        public float maxSpeed;
        public float boostSpeed;
        private float _realSpeed;

        public Transform frontLeftTyre;
        public Transform frontRightTyre;

        private float _steerDirection;
        private float _driftTime;

        private bool _driftLeft;
        private bool _driftRight;
        private float _outwardsDriftForce = 50000;

        public bool isSliding;
        
        public bool touchingGround;
        
        [Header("Particle Drift Sparks")]
        public Transform leftDrift;
        public Transform rightDrift;
        public Color drift1;
        public Color drift2;
        public Color drift3;

        [HideInInspector]
        public float boostTime;

        public Transform boostFire;
        
        private Vector2 _moveInput = Vector2.zero;
        private bool _drifting;

        public int lapNumber;
        public int checkpointIndex;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            if (!IsOwner) return;
            _rb = GetComponent<Rigidbody>();
            lapNumber = 1;
            checkpointIndex = 0;
            name = EditPlayerName.Instance.GetPlayerName();
            Debug.Log(name);
        }

        protected override void OnNetworkPostSpawn()
        {
            base.OnNetworkPostSpawn();
            if (IsHost) StartCoroutine(CountDown());
        }

        private IEnumerator CountDown()
        {
            yield return new WaitForEndOfFrame();
            LapManager.Instance.StartCountDownServerRpc();
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            if (!IsOwner) return;
            _moveInput = context.ReadValue<Vector2>();
        }

        public void OnDrift(InputAction.CallbackContext context)
        {
            if (!IsOwner) return;
            _drifting = context.action.triggered;
        }

        public void OnQuit(InputAction.CallbackContext context)
        {
            if (!IsOwner) return;
            Application.Quit();
        }

        private void FixedUpdate()
        {
            if (!IsOwner) return;
            if (LapManager.Instance.startYourEngines)
            {
                Move();
                TyreSteer();
                Steer();
                GroundNormalRotation();
                Drift();
                Boosts();
            }
        }

        private void Move()
        {
            _realSpeed = transform.InverseTransformDirection(_rb.linearVelocity).z;
            _currentSpeed = Mathf.Lerp(_currentSpeed, maxSpeed * _moveInput.y, Time.fixedDeltaTime);
            Vector3 vel = transform.forward * _currentSpeed;
            vel.y = _rb.linearVelocity.y;
            _rb.linearVelocity = vel;
        }

        private void TyreSteer()
        {
            if (_moveInput.x < 0)
            {
                frontLeftTyre.localEulerAngles = new Vector3(frontLeftTyre.localEulerAngles.x, Mathf.Lerp(frontLeftTyre.localEulerAngles.y, 155, 5 * Time.fixedDeltaTime), frontLeftTyre.localEulerAngles.z);
                frontRightTyre.localEulerAngles = new Vector3(frontRightTyre.localEulerAngles.x, Mathf.Lerp(frontRightTyre.localEulerAngles.y, 155, 5 * Time.fixedDeltaTime), frontRightTyre.localEulerAngles.z);
            }
            else if (_moveInput.x > 0)
            {
                frontLeftTyre.localEulerAngles = new Vector3(frontLeftTyre.localEulerAngles.x, Mathf.Lerp(frontLeftTyre.localEulerAngles.y, 205, 5 * Time.fixedDeltaTime), frontLeftTyre.localEulerAngles.z);
                frontRightTyre.localEulerAngles = new Vector3(frontRightTyre.localEulerAngles.x, Mathf.Lerp(frontRightTyre.localEulerAngles.y, 205, 5 * Time.fixedDeltaTime), frontRightTyre.localEulerAngles.z);
            }
            else
            {
                frontLeftTyre.localEulerAngles = new Vector3(frontLeftTyre.localEulerAngles.x, Mathf.Lerp(frontLeftTyre.localEulerAngles.y, 180, 5 * Time.fixedDeltaTime), frontLeftTyre.localEulerAngles.z);
                frontRightTyre.localEulerAngles = new Vector3(frontRightTyre.localEulerAngles.x, Mathf.Lerp(frontRightTyre.localEulerAngles.y, 180, 5 * Time.fixedDeltaTime), frontRightTyre.localEulerAngles.z);
            }

            // if (_currentSpeed > 20)
            // {
            //     frontLeftTyre.Rotate(-90 * Time.fixedDeltaTime * _currentSpeed * 0.5f, 0, 0);
            //     frontRightTyre.Rotate(-90 * Time.fixedDeltaTime * _currentSpeed * 0.5f, 0, 0);
            //     backLeftTyre.Rotate(-90 * Time.fixedDeltaTime * _currentSpeed * 0.5f, 0, 0);
            //     backRightTyre.Rotate(-90 * Time.fixedDeltaTime * _currentSpeed * 0.5f, 0, 0);
            // }
            // else
            // {
            //     frontLeftTyre.Rotate(-90 * Time.fixedDeltaTime * _realSpeed * 0.5f, 0, 0);
            //     frontRightTyre.Rotate(-90 * Time.fixedDeltaTime * _realSpeed * 0.5f, 0, 0);
            //     backLeftTyre.Rotate(-90 * Time.fixedDeltaTime * _realSpeed * 0.5f, 0, 0);
            //     backRightTyre.Rotate(-90 * Time.fixedDeltaTime * _realSpeed * 0.5f, 0, 0);
            // }
        }

        private void Steer()
        {
            if (_driftLeft && !_driftRight)
            {
                _steerDirection = _moveInput.x < 0 ? -1.2f : -0.5f;
                transform.GetChild(0).localRotation = Quaternion.Lerp(transform.GetChild(0).localRotation, Quaternion.Euler(0, -20f, 0), 8f * Time.fixedDeltaTime);
                // isSliding if hop is added
                if (isSliding && touchingGround) _rb.AddForce(transform.right * (_outwardsDriftForce * Time.fixedDeltaTime), ForceMode.Acceleration);
            }
            else if (_driftRight && !_driftLeft)
            {
                _steerDirection = _moveInput.x > 0 ? 1.2f : 0.5f;
                transform.GetChild(0).localRotation = Quaternion.Lerp(transform.GetChild(0).localRotation, Quaternion.Euler(0, 20f, 0), 8f * Time.fixedDeltaTime);
                // isSliding if hop is added
                if (isSliding && touchingGround) _rb.AddForce(transform.right * (-_outwardsDriftForce * Time.fixedDeltaTime), ForceMode.Acceleration);
            }
            else
            {
                _steerDirection = _moveInput.x;
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
                transform.GetChild(0).GetComponent<Animator>().SetTrigger("Hop");
                if (_steerDirection > 0)
                {
                    _driftRight = true;
                    _driftLeft = false;
                }
                else if (_steerDirection < 0)
                {
                    _driftRight = false;
                    _driftLeft = true;
                }
            }

            if (_drifting && touchingGround && _currentSpeed > 20 && _moveInput.x != 0)
            {
                _driftTime += Time.fixedDeltaTime;

                if (_driftTime >= 1.5 && _driftTime < 4)
                {
                    for (int i = 0; i < leftDrift.childCount; i++)
                    {
                        ParticleSystem DriftPS = rightDrift.transform.GetChild(i).GetComponent<ParticleSystem>();
                        ParticleSystem.MainModule PSMain = DriftPS.main;
                        
                        ParticleSystem DriftPS2 = leftDrift.transform.GetChild(i).GetComponent<ParticleSystem>();
                        ParticleSystem.MainModule PSMain2 = DriftPS2.main;

                        PSMain.startColor = drift1;
                        PSMain2.startColor = drift1;

                        if (!DriftPS.isPlaying && !DriftPS2.isPlaying)
                        {
                            DriftPS.Play();
                            DriftPS2.Play();
                        }
                    }
                }

                if (_driftTime >= 4 && _driftTime < 7)
                {
                    for (int i = 0; i < leftDrift.childCount; i++)
                    {
                        ParticleSystem DriftPS = rightDrift.transform.GetChild(i).GetComponent<ParticleSystem>();
                        ParticleSystem.MainModule PSMain = DriftPS.main;

                        ParticleSystem DriftPS2 = leftDrift.transform.GetChild(i).GetComponent<ParticleSystem>();
                        ParticleSystem.MainModule PSMain2 = DriftPS2.main;

                        PSMain.startColor = drift2;
                        PSMain2.startColor = drift2;
                    }
                }

                if (_driftTime >= 7)
                {
                    for (int i = 0; i < leftDrift.childCount; i++)
                    {
                        ParticleSystem DriftPS = rightDrift.transform.GetChild(i).GetComponent<ParticleSystem>();
                        ParticleSystem.MainModule PSMain = DriftPS.main;

                        ParticleSystem DriftPS2 = leftDrift.transform.GetChild(i).GetComponent<ParticleSystem>();
                        ParticleSystem.MainModule PSMain2 = DriftPS2.main;

                        PSMain.startColor = drift3;
                        PSMain2.startColor = drift3;
                    }
                } 
            }

            if (!_drifting || _realSpeed < 20)
            {
                transform.GetChild(0).GetComponent<Animator>().SetTrigger("DoneHopping");
                _driftLeft = false;
                _driftRight = false;
                isSliding = false;
                
                if (_driftTime >= 1.5 && _driftTime < 4)
                {
                    boostTime = 0.75f;
                }

                if (_driftTime >= 4 && _driftTime < 7)
                {
                    boostTime = 1.5f;
                }

                if (_driftTime >= 7)
                {
                    boostTime = 2.5f;
                }
                
                _driftTime = 0;
                for (int i = 0; i <= 2; i++)
                {
                    ParticleSystem DriftPS = rightDrift.transform.GetChild(i).GetComponent<ParticleSystem>();
                    
                    ParticleSystem DriftPS2 = leftDrift.transform.GetChild(i).GetComponent<ParticleSystem>();
                    
                    DriftPS.Stop();
                    DriftPS2.Stop();
                }
            }
        }

        private void Boosts()
        {
            boostTime -= Time.fixedDeltaTime;
            if (boostTime > 0)
            {
                for (int i = 0; i < boostFire.childCount; i++)
                {
                    if (!boostFire.GetComponent<ParticleSystem>().isPlaying)
                    {
                        boostFire.GetComponent<ParticleSystem>().Play();
                    }
                }
                maxSpeed = boostSpeed;
                _currentSpeed = Mathf.Lerp(_currentSpeed, maxSpeed, Time.fixedDeltaTime);
            }
            else
            {
                for (int i = 0; i < boostFire.childCount; i++)
                {
                    boostFire.GetComponent<ParticleSystem>().Stop();
                }
                maxSpeed = boostSpeed - 20;
            }
        }
    }
}