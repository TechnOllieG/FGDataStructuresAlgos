using UnityEngine;

namespace FG
{
    [CreateAssetMenu(fileName = "New character data", menuName = "FG/Character")]
    public class PlayerData : ScriptableObject
    {
        [Header("Moving")]
        public float walkSpeed = 6.75f;
        public float runSpeed = 10f;

        [Header("Crouching")]
        public float crouchSpeed = 4f;
        public float crouchHeight = 1f;

        [Header("Jumping")]
        public float jumpForce = 8f;
        public float gravityMultiplier = 1f;
        public float inAirMovementMultiplier = 1f;
    }
}