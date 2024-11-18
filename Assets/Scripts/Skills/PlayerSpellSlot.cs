using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerSpellSlot : MonoBehaviour {
   public SpellSlotStats stats;
   public float currentEnergy;
   public float energyCost;
   public int currentSpellCardIndex;
   public bool isHoldingCard;
   public bool isSelectingSpellCard;
   public float spellRotationCountdown;
   public float timeLastInterrupted;
   public List<Skill> equippedSpellCards = new();

   protected PlayerSpellSlot(float energyCost) {
      this.energyCost = energyCost;
   }
   public void OnEnable() {
      timeLastInterrupted = -1;
      currentEnergy = 0;
      isHoldingCard = false;
      isSelectingSpellCard = false;
      spellRotationCountdown = 0;
   }

   public void Update() {
      if (isSelectingSpellCard) {
         if (currentEnergy < energyCost) {
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
   }

   public void ChargeEnergy(float energy) {
      currentEnergy += energy;
   }

   public Skill UseSpell() {
      if (currentEnergy < energyCost) {
         InterruptSpellCardSelection();
         return null;
      }
      var currentSkill = equippedSpellCards[currentSpellCardIndex];
      isSelectingSpellCard = false;
      isHoldingCard = false;
      currentEnergy = 0;
      return currentSkill;

   }


}
