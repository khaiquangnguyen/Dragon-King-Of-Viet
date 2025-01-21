using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerSpellManager : MonoBehaviour {
    public SpellSlotStats stats;
    public float[] currentEnergies = new float[4];
    public float[] baseEnergyCosts = new float[4];
    public int[] currentSpellCardIndices = new int[4];
    public int selectedSpellSlot;
    public bool isHoldingCard;
    public float holdingCardCountdown;
    public bool isRotatingSpellCard;
    public bool isPreparingSpell;
    public float spellRotationCountdown;
    public float timeLastInterrupted;
    private Player player;
    public List<Skill> levelOneManEquippedSpellCards = new List<Skill>();
    public List<Skill> levelTwoManEquippedSpellCards = new List<Skill>();
    public List<Skill> levelThreeManEquippedSpellCards = new List<Skill>();
    public List<Skill> levelOneDragonEquippedSpellCards = new List<Skill>();
    public List<Skill> levelTwoDragonEquippedSpellCards = new List<Skill>();
    public List<Skill> levelThreeDragonEquippedSpellCards = new List<Skill>();
    public List<Skill> levelFourDragonEquippedSpellCards = new List<Skill>();

    private float timeStartRotation;

    public void OnEnable() {
        // set a default amount of energy for testing
        currentEnergies[0] = 100;
        currentEnergies[1] = 100;
        currentEnergies[2] = 100;
        currentEnergies[3] = 100;
        player = GetComponent<Player>();
        timeLastInterrupted = -1;
        selectedSpellSlot = 0;
        isHoldingCard = false;
        isRotatingSpellCard = false;
        spellRotationCountdown = 0;
        baseEnergyCosts[0] = stats.firstSpellEnergyCost;
        baseEnergyCosts[1] = stats.secondSpellEnergyCost;
        baseEnergyCosts[2] = stats.thirdSpellEnergyCost;
        baseEnergyCosts[3] = stats.fourthSpellEnergyCost;
    }

    public int GetCurrentSpellCardIndex() {
        return currentSpellCardIndices[selectedSpellSlot];
    }

    public List<Skill> GetAllSpellsOfSlot(int? slot = null) {
        var slotToCheck = slot ?? selectedSpellSlot;
        var form = player.form;
        var humanFormSpells = slotToCheck switch {
            0 => levelOneManEquippedSpellCards,
            1 => levelTwoManEquippedSpellCards,
            2 => levelThreeManEquippedSpellCards,
            3 => new List<Skill>(),
            _ => new List<Skill>(),
        };
        var dragonFormSpells = slotToCheck switch {
            0 => levelOneDragonEquippedSpellCards,
            1 => levelTwoDragonEquippedSpellCards,
            2 => levelThreeDragonEquippedSpellCards,
            3 => levelFourDragonEquippedSpellCards,
            _ => new List<Skill>(),
        };

        return form == PlayerForm.Man ? humanFormSpells : dragonFormSpells;
    }

    public void Update() {
        // get list of spell cards depending on the spell slot
        var equippedSpellCards = GetAllSpellsOfSlot();

        // if is holding card, we run a timer
        if (isHoldingCard) {
            holdingCardCountdown = Mathf.Clamp(holdingCardCountdown - Time.deltaTime, 0, stats.spellCardHoldDuration);
            if (holdingCardCountdown <= 0) {
                isHoldingCard = false;
                isPreparingSpell = false;
                isRotatingSpellCard = false;
                currentEnergies[selectedSpellSlot] = stats.refundEnergyWhenSkillNotUsed;
            }
        }

        // if is selecting spell card, we rotate the spell card
        if (isRotatingSpellCard) {
            if (Time.time - timeStartRotation > stats.spellCardRotationTotalDuration) {
                isRotatingSpellCard = false;
                isHoldingCard = false;
                currentEnergies[selectedSpellSlot] = stats.refundEnergyWhenSkillNotUsed;
            }

            // not enough energy, do nothing
            if (currentEnergies[selectedSpellSlot] < baseEnergyCosts[selectedSpellSlot]) {
                return;
            }

            spellRotationCountdown -= Time.deltaTime;
            if (spellRotationCountdown <= 0) {
                var newSpellCardIndex = (currentSpellCardIndices[selectedSpellSlot] + 1) % equippedSpellCards.Count;
                RotateToSpellCard(newSpellCardIndex);
            }
        }
    }

    public void InterruptSpellCardSelection() {
        timeLastInterrupted = Time.time;
        isRotatingSpellCard = false;
        currentEnergies[selectedSpellSlot] = stats.refundEnergyWhenInterrupted;
        isHoldingCard = false;
    }

    public void RotateToSpellCard(int index) {
        if (isHoldingCard) return;
        currentSpellCardIndices[selectedSpellSlot] = index;
        spellRotationCountdown = stats.spellCardRotationPeriod;
    }

    public void TriggerSpellsRotationOrHold(int? slot) {
        if (slot is null) return;
        if (isHoldingCard) return;
        if (isRotatingSpellCard) {
            if (slot != selectedSpellSlot) return;
            HoldCurrentSpell();
            return;
        }

        selectedSpellSlot = (int)slot;
        var spellCards = GetAllSpellsOfSlot();
        // not enough energy, do nothing
        if (currentEnergies[selectedSpellSlot] < baseEnergyCosts[selectedSpellSlot]) {
            return;
        }
        if (spellCards.Count <= 0) return;
        if (spellCards.Count == 1) {
            if (player.allowQuickCastSpell) {
                print("cast");
                isRotatingSpellCard = false;
                isHoldingCard = true;
                isPreparingSpell = true;
                player.CastSpell();
            }
            else {
                currentSpellCardIndices[selectedSpellSlot] = 0;
                HoldCurrentSpell();
            }
            return;
        }

        if (Time.time - timeLastInterrupted < stats.coolDownWhenInterrupted) return;
        timeStartRotation = Time.time;
        isHoldingCard = false;
        isRotatingSpellCard = true;
        isPreparingSpell = true;
        spellRotationCountdown = stats.spellCardRotationPeriod;
    }

    public void HoldCurrentSpell() {
        isRotatingSpellCard = false;
        isHoldingCard = true;
        holdingCardCountdown = stats.spellCardHoldDuration;
    }

    public void ChargeEnergy(float energy) {
        // don't charge energy when selecting spell card
        if (isPreparingSpell) return;
        // charge all spell slots
        for (var i = 0; i < 4; i++) {
            currentEnergies[i] = Mathf.Clamp(currentEnergies[i] + energy, 0, baseEnergyCosts[i]);
        }
    }

    public Skill StartSpellCast() {
        // not enough energy, do nothing
        if (currentEnergies[selectedSpellSlot] < baseEnergyCosts[selectedSpellSlot]) {
            return null;
        }

        var currentSkill = GetCurrentSpell();
        isPreparingSpell = false;
        isHoldingCard = false;
        isRotatingSpellCard = false;
        currentEnergies[selectedSpellSlot] -= baseEnergyCosts[selectedSpellSlot];
        return currentSkill;
    }

    public Skill GetCurrentSpell() {
        var equippedSpellCards = GetAllSpellsOfSlot();
        var currentSpellCardIndex = currentSpellCardIndices[selectedSpellSlot];
        return equippedSpellCards[currentSpellCardIndex];
    }

    public bool CanCastSpell() {
        return isPreparingSpell;
    }

    [CanBeNull]
    public Skill GetCurrentSpellOfSlot(int slot) {
        var equippedSpells = GetAllSpellsOfSlot(slot);
        var currentSpellCardIndex = currentSpellCardIndices[slot];
        return currentSpellCardIndex < equippedSpells.Count ? equippedSpells[currentSpellCardIndex] : null;
    }

    public void DisplayEnergy() { }
}
