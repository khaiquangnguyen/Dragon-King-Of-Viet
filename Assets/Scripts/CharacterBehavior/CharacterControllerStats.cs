using UnityEngine;
using UnityEngine.Serialization;

namespace CharacterBehavior {
    [CreateAssetMenu(fileName = "Character Controller Stats", menuName = "Character Controller Stats", order = 0)]
    public class CharacterControllerStats : ScriptableObject {
        public float groundCheckDistance = 0.1f;
        public float stickToGroundDistance = 0.4f;
        public float verticalSlopeCheckDistance = 0.1f;
        public float horizontalSlopeCheckDistance = 0.5f;
        public float maxVyStickToGroundCorrectionVelocity = 20f;
        public float maxSlopeAngle = 70;
        public float wallCheckDistance = 0.1f;
        public float wallClimbOverCheckDistance = 0.1f;
        public LayerMask groundLayer;
        public LayerMask wallLayer;
        public LayerMask waterLayer;
    }
}