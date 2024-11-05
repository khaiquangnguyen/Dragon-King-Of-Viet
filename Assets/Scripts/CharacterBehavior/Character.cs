using UnityEngine;

namespace CharacterBehavior {
    public abstract class Character : MonoBehaviour {
        public string uniqueId { get; }
        public float damageMult { get; }
        public float abilityHaste { get; }
        public abstract void OnDamageDealt(float damageDealt, Character character);
        public abstract float TakeDamage(float damage, Character damageDealer);
    }
}
