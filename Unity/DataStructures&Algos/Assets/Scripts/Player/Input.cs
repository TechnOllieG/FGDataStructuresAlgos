using UnityEngine;

namespace FG
{
    public class Input : MonoBehaviour
    {
        private PlayerMovement _movement;
        private MousePitch _mousePitch;

        private void Awake()
        {
            _movement = GetComponent<PlayerMovement>();
            _mousePitch = GameManager.PlayerCamera.GetComponent<MousePitch>();
        }

        private void Update()
        {
            _mousePitch.input = UnityEngine.Input.GetAxis("Mouse Y") * GameplaySettings.mouseSensitivity.y;

            _movement.turnInput = UnityEngine.Input.GetAxis("Mouse X") * GameplaySettings.mouseSensitivity.x;
            _movement.forwardInput = UnityEngine.Input.GetAxis("Vertical");
            _movement.sidewaysInput = UnityEngine.Input.GetAxis("Horizontal");
            _movement.runInput = UnityEngine.Input.GetKey(KeyCode.LeftControl);
            _movement.jumpInput = UnityEngine.Input.GetKey(KeyCode.Space);
            _movement.crouchInput = UnityEngine.Input.GetKey(KeyCode.LeftShift);
        }
    }
}