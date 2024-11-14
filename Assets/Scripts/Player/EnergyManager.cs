using UnityEngine;
using UnityEngine.Serialization;

namespace DefaultNamespace {
    public class EnergyManager : MonoBehaviour {
        public int currentEnergy;
        public int maxEnergy;
        public int damagedEnergy;
        public int energyRegen;
        public bool canRegen;
        public bool canLoseEnergy;
        public float noDamageUntilRegenDuration = 3;
        public float noDamageUntilBrokenEnergyHealDuration = 5;
        public float timeSinceLastRegen;
        public float damageLastTakenAt;
        public int maxAvailableEnergy => maxEnergy - damagedEnergy;
        [FormerlySerializedAs("totalRegenAmountSinceLastBrokenEnergyHeal")]
        public int totalRegenAmountSinceLastDamagedBrokenEnergyHeal = 0;
        public int regenAmountRequiredToHealBrokenEnergy = 100;

        public EnergyManager(int maxEnergy, int currentEnergy, int energyRegen) {
            this.currentEnergy = currentEnergy;
            this.maxEnergy = maxEnergy;
            this.energyRegen = energyRegen;
            this.damagedEnergy = 0;
            canRegen = true;
            timeSinceLastRegen = 0;
        }

        public void LoseEnergy(int energy) {
            if (canLoseEnergy) {
                currentEnergy -= energy;
                totalRegenAmountSinceLastDamagedBrokenEnergyHeal = 0;
                damageLastTakenAt = Time.time;
            }
        }

        public void IncreaseMaxEnergy(int amount) {
            maxEnergy += amount;
            currentEnergy += amount;
        }

        public void RestoreEnergy(int restoreAmount) {
            totalRegenAmountSinceLastDamagedBrokenEnergyHeal += restoreAmount;
            currentEnergy += restoreAmount;
        }

        public void RegenEnergy() {
            if (canRegen && currentEnergy < maxAvailableEnergy && timeSinceLastRegen >= 1 &&
                Time.time - damageLastTakenAt >= noDamageUntilRegenDuration) {
                currentEnergy += energyRegen;
                totalRegenAmountSinceLastDamagedBrokenEnergyHeal += energyRegen;
                timeSinceLastRegen = 0;
            }
        }

        public void BreakEnergy(int breakAmount) {
            damagedEnergy += breakAmount;
            damagedEnergy = Mathf.Clamp(damagedEnergy, 0, maxEnergy);
        }

        public void HealBrokenEnergy() {
            if (Time.time - damageLastTakenAt >= noDamageUntilBrokenEnergyHealDuration &&
                totalRegenAmountSinceLastDamagedBrokenEnergyHeal > regenAmountRequiredToHealBrokenEnergy) {
                damagedEnergy -= 1;
                totalRegenAmountSinceLastDamagedBrokenEnergyHeal = 0;
            }
        }

        public void UpdateEnergyBar() { }

        public void Update() {
            timeSinceLastRegen += Time.deltaTime;
            RegenEnergy();
            UpdateEnergyBar();
            HealBrokenEnergy();
            currentEnergy = Mathf.Clamp(currentEnergy, 0, maxAvailableEnergy);
            damagedEnergy = Mathf.Clamp(damagedEnergy, 0, maxEnergy);
        }
    }
}