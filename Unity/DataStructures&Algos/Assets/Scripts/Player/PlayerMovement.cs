using System;
using UnityEngine;

namespace FG
{
    [RequireComponent(typeof(Rigidbody),typeof(CapsuleCollider))]
    public class PlayerMovement : MonoBehaviour
    {
        [NonSerialized] public float forwardInput;
        [NonSerialized] public float sidewaysInput;
        [NonSerialized] public float turnInput;
        [NonSerialized] public bool runInput;
        [NonSerialized] public bool jumpInput;
        [NonSerialized] public bool crouchInput;

        public PlayerData playerData;

        private Vector2 _originalCapsuleSize;

        private Vector3 _moveDirection;
        private float _currentSpeed;
        private float _adjustVerticalVelocity;
        private float _inputAmount;

        private Transform _transform;
        private Rigidbody _rigidbody;
        private CapsuleCollider _capsuleCollider;
        
        public bool IsCrouching { get; private set; }

        private void Awake()
        {
            _transform = transform;
            _rigidbody = GetComponent<Rigidbody>();
            _capsuleCollider = GetComponent<CapsuleCollider>();
            _originalCapsuleSize = new Vector2(_capsuleCollider.radius, _capsuleCollider.height);
        }

        private void LateUpdate()
        {
            // rotate
            _rigidbody.MoveRotation(_rigidbody.rotation * Quaternion.Euler(Vector3.up * turnInput));
            
            // move
            _moveDirection = (sidewaysInput * _transform.right + forwardInput * _transform.forward).normalized;
            _inputAmount = Mathf.Clamp01(Mathf.Abs(forwardInput) + Mathf.Abs(sidewaysInput));

            _adjustVerticalVelocity = _rigidbody.velocity.y;

            if (crouchInput)
            {
                EnterCrouch();
            }
            else
            {
                ExitCrouch();
            }
            
            if (CheckGrounded())
            {
                if (jumpInput)
                {
                    _adjustVerticalVelocity = playerData.jumpForce;
                }

                if (IsCrouching)
                {
                    _currentSpeed = playerData.crouchSpeed;
                }
                else
                {
                    _currentSpeed = runInput ? playerData.runSpeed : playerData.walkSpeed;
                }
            }
            else
            {
                _currentSpeed *= playerData.inAirMovementMultiplier;
            }

            SetVelocity();
        }

        private void EnterCrouch()
        {
            IsCrouching = true;
            
            _capsuleCollider.height = playerData.crouchHeight;
        }

        private void ExitCrouch()
        {
            IsCrouching = false;
            
            _capsuleCollider.height = _originalCapsuleSize.y;
        }

        private bool CheckGrounded()
        {
            // Todo make better
            Debug.DrawRay(_transform.position + _capsuleCollider.center, Vector3.down * _capsuleCollider.height / 2f, Color.red);
            return Physics.Raycast(_transform.position + _capsuleCollider.center, Vector3.down, _capsuleCollider.height / 2f + 0.1f);
        }

        private void SetVelocity()
        {
            Vector3 velocity = (_moveDirection * (_currentSpeed * _inputAmount));
            velocity.y = _adjustVerticalVelocity * playerData.gravityMultiplier;
            _rigidbody.velocity = velocity;
        }
    }
}
