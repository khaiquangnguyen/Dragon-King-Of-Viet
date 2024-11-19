using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class PlayerSpellSlot : MonoBehaviour {
   public SpellSlotStats stats;
   public float currentEnergy;
   public float baseEnergyCost;
   public int currentSpellCardIndex;
   public bool isHoldingCard;
   public float holdingCardCountdown;
   public bool isSelectingSpellCard;
   public float spellRotationCountdown;
   public float timeLastInterrupted;
   public List<Skill> equippedSpellCards = new();

   public void OnEnable() {
      timeLastInterrupted = -1;
      currentEnergy = 0;
      isHoldingCard = false;
      isSelectingSpellCard = false;
      spellRotationCountdown = 0;
   }

   public void Update() {
      holdingCardCountdown = Mathf.Clamp(holdingCardCountdown - Time.deltaTime, 0, stats.spellCardHoldDuration);
      if (holdingCardCountdown <= 0 && isHoldingCard) {
         isHoldingCard = false;
         currentEnergy = stats.refundEnergyWhenSkillNotUsed;
      }
      if (isSelectingSpellCard) {
         if (currentEnergy < baseEnergyCost) {
         }
         spellRotationCountdown -= Time.deltaTime;
         if (spellRotationCountdown <= 0) {
            var newSpellCardIndex = Math.Clamp(currentSpellCardIndex + 1, 0, equippedSpellCards.Count - 1);
            RotateToSpellCard(newSpellCardIndex);
         }
      }

   }

   public void InterruptSpellCardSelection() {
      timeLastInterrupted = Time.time;
      isSelectingSpellCard = false;
      currentEnergy = stats.refundEnergyWhenInterrupted;
      isHoldingCard = false;
   }


   public void RotateToSpellCard(int index) {
      if (isHoldingCard) return;
      currentSpellCardIndex = index;
      spellRotationCountdown = stats.spellCardRotationPeriod;
   }

   public void TriggerSpellsRotation() {
      if (Time.time - timeLastInterrupted < stats.coolDownWhenInterrupted) return;
      isHoldingCard = false;
      isSelectingSpellCard = true;
      RotateToSpellCard(currentSpellCardIndex);
   }

   public void HoldCurrentSpell() {
      isHoldingCard = true;
      holdingCardCountdown = stats.spellCardHoldDuration;
   }

   public void ChargeEnergy(float energy) {
      currentEnergy += energy;
   }

   public Skill UseSpell() {
      if (currentEnergy < baseEnergyCost) {
         InterruptSpellCardSelection();
         return null;
      }
      var currentSkill = equippedSpellCards[currentSpellCardIndex];
      isSelectingSpellCard = false;
      isHoldingCard = false;
      currentEnergy = 0;
      return currentSkill;
   }


   public void DisplayEnergy() {

   }

   void OnGUI()
   {
      GUI.Label(new Rect(10, 10, 200, 40), currentEnergy.ToString(CultureInfo.CurrentCulture));
   }


}
