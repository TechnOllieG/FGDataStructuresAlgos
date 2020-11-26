using UnityEngine;
using UnityEngine.Assertions;

namespace FG
{
    [DefaultExecutionOrder(-50)]
    public class GameManager : MonoBehaviour
    {
        public static Camera PlayerCamera { get; private set; }
        public static Transform PlayerCameraTransform { get; private set; }
        public static bool LookCursor
        {
            set
            {
                if (value)
                {
                    Cursor.lockState = CursorLockMode.Locked;
                }
                else
                {
                    Cursor.lockState = CursorLockMode.None;
                }
            }
        }

        private void Awake()
        {
            PlayerCamera = Camera.main;
            Assert.IsNotNull(PlayerCamera, "Camera is null");
            PlayerCameraTransform = PlayerCamera.transform;
        }

        private void OnEnable()
        {
            LookCursor = true;
        }

        private void OnDisable()
        {
            LookCursor = false;
        }
    }
}