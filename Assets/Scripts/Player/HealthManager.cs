using CharacterBehavior;
using UnityEngine;

public class HealthManager : MonoBehaviour {
    [HideInInspector]
    public GameCharacter gameCharacter;
    public int currentHealth;
    public int maxHealth;
    public int healthRegen;
    public GameObject healthBar;
    public bool canRegen;
    public bool canHeal;
    public bool canTakeDamage;
    public float regenRate;
    public float timeSinceLastRegen;
    public bool allowOverHeal;
    public float noDamageUntilRegenDuration = 3;
    public float damageLastTakenAt;

    public HealthManager(int maxHealth, int currentHealth, int healthRegen) {
        gameCharacter = GetComponent<GameCharacter>();
        this.currentHealth = currentHealth;
        this.maxHealth = maxHealth;
        this.healthRegen = healthRegen;
        canRegen = true;
        canHeal = true;
        canTakeDamage = true;
        timeSinceLastRegen = 0;
    }

    public void TakeDamage(int damage) {
        if (canTakeDamage) {
            currentHealth -= damage;
            damageLastTakenAt = Time.time;
        }
    }

    public void Heal(int healAmount) {
        if (canHeal) {
            currentHealth += healAmount;
        }
    }

    public void RegenHealth() {
        if (canRegen && currentHealth < maxHealth && timeSinceLastRegen >= 1 &&
            Time.time - damageLastTakenAt >= noDamageUntilRegenDuration) {
            currentHealth += healthRegen;
            timeSinceLastRegen = 0;
        }
    }

    public void IncreaseMaxHealth(int amount) {
        maxHealth += amount;
        currentHealth += amount;
    }

    public void UpdateHealthBar() {
        // healthBar.transform.localScale = new Vector3((float)currentHealth / maxHealth, healthBar.transform.localScale.y, healthBar.transform.localScale.z);
    }

    public void Update() {
        timeSinceLastRegen += Time.deltaTime;
        RegenHealth();
        UpdateHealthBar();
        if (!allowOverHeal) {
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        }
    }
}