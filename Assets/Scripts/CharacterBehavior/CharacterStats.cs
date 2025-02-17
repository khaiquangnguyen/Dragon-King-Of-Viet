using UnityEngine;

namespace CharacterBehavior {
    [CreateAssetMenu(fileName = "Character Stats", menuName = "Character Stats", order = 0)]
    public class CharacterStats : ScriptableObject {
        public float maxHealth;
        public float currentHealth;
        public float damageMult;
        public float abilityHaste;
        public int facingDirection;
        public float groundAccel;
        public float groundDecel;
        public float groundMaxSpeed;
        public float airAccel;
        public float airDecel;
        public float airMaxSpeed;
        public float waterAccel;
        public float waterDecel;
        public float waterMaxSpeed;
        public float gravity;
        public float maxFallSpeed;
        public float jumpHeight;
        public int maxJumpsCount;
        public AnimationClip idleAnimation;
        public bool canFly = false;
        public bool canSwim = false;
        public bool canJump = false;
        public bool canWalk = false;
    }
}