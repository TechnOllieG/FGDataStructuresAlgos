using System;
using UnityEngine;

namespace FG
{
    public class MousePitch : MonoBehaviour
    {
        [NonSerialized] public float input;
        private Vector2 _pitchLimit = new Vector2(-90f, 90f);

        private Transform _cameraTransform;
        private Quaternion _cameraRotation;

        private void Awake()
        {
            _cameraTransform = GameManager.PlayerCameraTransform;
            _cameraRotation = _cameraTransform.localRotation;
        }

        private void LateUpdate()
        {
            _cameraRotation *= Quaternion.Euler(-input, 0f, 0f);
            _cameraRotation = ClampRotationAroundX(_cameraRotation);
            _cameraTransform.localRotation = _cameraRotation;
        }

        private Quaternion ClampRotationAroundX(Quaternion rotation)
        {
            rotation.x /= rotation.w;
            rotation.y /= rotation.w;
            rotation.z /= rotation.w;
            rotation.w = 1;

            float angleX = 2f * Mathf.Rad2Deg * Mathf.Atan(rotation.x);
            angleX = Mathf.Clamp(angleX, _pitchLimit.x, _pitchLimit.y);
            rotation.x = Mathf.Tan(Mathf.Deg2Rad * angleX * 0.5f);

            return rotation;
        }
    }
}