using UnityEngine;

namespace CharacterBehavior {
    [CreateAssetMenu(fileName = "Character Controller Stats", menuName = "Character Controller Stats", order = 0)]
    public class CharacterControllerStats : ScriptableObject {
        public float groundCheckDistance = 0.1f;
        public float stickToGroundDistance = 0.4f;
        public float slopeCheckDistance = 0.1f;
        public float maxVyStickToGroundCorrectionVelocity = 20f;
        public float maxSlopeAngle = 70;
        public LayerMask groundLayer;
    }
}