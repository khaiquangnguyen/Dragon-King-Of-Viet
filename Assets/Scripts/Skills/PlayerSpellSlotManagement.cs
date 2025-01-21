using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerSpellSlotManagement : MonoBehaviour {
    public SpellSlotStats stats;
    public float[] currentEnergies = new float[4];
    public float[] baseEnergyCosts = new float[4];
    public int[] currentSpellCardIndices = new int[4];
    public List<List<Skill>> allEquippedSpellCards = new List<List<Skill>>();
    public int selectedSpellSlot;
    public bool isHoldingCard;
    public float holdingCardCountdown;
    public bool isSelectingSpellCard;
    public float spellRotationCountdown;
    public float timeLastInterrupted;
    private float timeStartRotation;

    public void OnEnable() {
        timeLastInterrupted = -1;
        selectedSpellSlot = 0;
        isHoldingCard = false;
        isSelectingSpellCard = false;
        spellRotationCountdown = 0;
        baseEnergyCosts[0] = stats.firstSpellEnergyCost;
        baseEnergyCosts[1] = stats.secondSpellEnergyCost;
        baseEnergyCosts[2] = stats.thirdSpellEnergyCost;
        baseEnergyCosts[3] = stats.fourthSpellEnergyCost;

    }
    public void Update() {
        // get list of spell cards depending on the spell slot
        var equippedSpellCardsOfSlot = allEquippedSpellCards[selectedSpellSlot];

        // if is holding card, we run a timer
        if (isHoldingCard) {
            holdingCardCountdown = Mathf.Clamp(holdingCardCountdown - Time.deltaTime, 0, stats.spellCardHoldDuration);
            if (holdingCardCountdown <= 0) {
                isHoldingCard = false;
                currentEnergies[selectedSpellSlot] = stats.refundEnergyWhenSkillNotUsed;
            }
        }
        // if is selecting spell card, we rotate the spell card
        if (isSelectingSpellCard) {
            if (Time.time - timeStartRotation > stats.spellCardRotationTotalDuration) {
                isSelectingSpellCard = false;
                isHoldingCard = false;
                currentEnergies[selectedSpellSlot] = stats.refundEnergyWhenSkillNotUsed;
            }
            // not enough energy, do nothing
            if (currentEnergies[selectedSpellSlot] < baseEnergyCosts[selectedSpellSlot]) {
                return;
            }
            spellRotationCountdown -= Time.deltaTime;
            if (spellRotationCountdown <= 0) {
                var newSpellCardIndex = Math.Clamp(currentSpellCardIndices[selectedSpellSlot] + 1, 0, equippedSpellCardsOfSlot.Count - 1);
                RotateToSpellCard(newSpellCardIndex);
            }
        }
    }

    public void InterruptSpellCardSelection() {
        timeLastInterrupted = Time.time;
        isSelectingSpellCard = false;
        currentEnergies[selectedSpellSlot] = stats.refundEnergyWhenInterrupted;
        isHoldingCard = false;
    }

    public void RotateToSpellCard(int index) {
        if (isHoldingCard) return;
        currentSpellCardIndices[selectedSpellSlot] = index;
        spellRotationCountdown = stats.spellCardRotationPeriod;
    }

    public void TriggerSpellsRotation() {
        if (Time.time - timeLastInterrupted < stats.coolDownWhenInterrupted) return;
        timeStartRotation = Time.time;
        isHoldingCard = false;
        isSelectingSpellCard = true;
        RotateToSpellCard(currentSpellCardIndices[selectedSpellSlot]);
    }

    public void HoldCurrentSpell() {
        isHoldingCard = true;
        holdingCardCountdown = stats.spellCardHoldDuration;
    }

    public void ChargeEnergy(float energy) {
        // don't charge energy when selecting spell card
        if (isSelectingSpellCard) return;
        // charge all spell slots
        for (var i = 0; i < 4; i++) {
            currentEnergies[i] = Mathf.Clamp(currentEnergies[i] + energy, 0, baseEnergyCosts[i]);
        }
    }

    public Skill UseSpell() {
        // not enough energy, do nothing
        if ( currentEnergies[selectedSpellSlot] < baseEnergyCosts[selectedSpellSlot]) {
            return null;
        }
        var currentSkill = GetCurrentSpell();
        isSelectingSpellCard = false;
        isHoldingCard = false;
        currentEnergies[selectedSpellSlot] -= baseEnergyCosts[selectedSpellSlot];
        return currentSkill;
    }

    public Skill GetCurrentSpell() {
        var equippedSpellCards  = allEquippedSpellCards[selectedSpellSlot];
        var currentSpellCardIndex = currentSpellCardIndices[selectedSpellSlot];
        return equippedSpellCards[currentSpellCardIndex];
    }

    public Skill GetCurrentSpell(int slot) {
        var equippedSpellCards = allEquippedSpellCards[slot];
        var currentSpellCardIndex = currentSpellCardIndices[slot];
        return equippedSpellCards[currentSpellCardIndex];
    }

    public void DisplayEnergy() { }
}
