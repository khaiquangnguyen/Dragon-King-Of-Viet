using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "Spell Slot Stats", menuName = "Spell Slot Stats", order = 0)]
public class SpellSlotStats:ScriptableObject {
    public float firstSpellEnergyCost;
    public float secondSpellEnergyCost;
    public float thirdSpellEnergyCost;
    public float spellCardRotationPeriod;
    public float spellCardHoldDuration;
    public float refundEnergyWhenInterrupted;
    public float refundEnergyWhenSkillNotUsed;
    public float coolDownWhenInterrupted;

}
