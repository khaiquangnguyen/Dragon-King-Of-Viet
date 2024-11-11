using UnityEngine;

namespace CharacterBehavior {
    [RequireComponent(typeof(HealthManager))]
    public abstract class GameCharacter : MonoBehaviour, IDamageTaker, IDamageDealer {
        public string uniqueId { get; }
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
    }
}
