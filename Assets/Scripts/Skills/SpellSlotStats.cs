using UnityEngine;
using UnityEngine.Serialization;

public class SpellSlotStats:ScriptableObject {
    public float firstSpellEnergyCost;
    public float secondSpellEnergyCost;
    public float thirdSpellEnergyCost;
    public float spellCardRotationPeriod;
    public float spellCardHoldDuration;
    public float coolDownWhenInterrupted;

}
