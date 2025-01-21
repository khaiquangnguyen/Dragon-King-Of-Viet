using UnityEditor.Rendering;
using UnityEngine;

namespace CharacterBehavior {
    [RequireComponent(typeof(HealthManager))]
    [RequireComponent(typeof(CharacterController2D))]
    public abstract class GameCharacter : MonoBehaviour, IDamageTaker, IDamageDealer {
        public string uniqueId { get; }
        public float healingMult;
        public float damageMult;
        public float abilityHaste { get; }
        public int facingDirection;
        public int attackLayer;
        public int projectileLayer;
        public int characterLayer;
        [HideInInspector]
        public HealthManager healthManager;
        public abstract void OnDealDamage(int damageDealt, IDamageTaker gameCharacter);
        public abstract DamageResult OnTakeDamage(int damage, IDamageDealer damageDealer);
        [HideInInspector]
        public CharacterController2D characterController;
        public Environment environment;

        public void UpdateEnvironment() {
            if (characterController.CheckIsOnGround()) {
                environment = Environment.Ground;
            }
            else if (characterController.CheckIsInWater()) {
                environment = Environment.Water;
            }
            else {
                environment = Environment.Air;
            }
        }
    }
}
