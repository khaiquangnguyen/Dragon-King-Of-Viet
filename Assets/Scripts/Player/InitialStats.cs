using UnityEngine;

[CreateAssetMenu(fileName = "InitialStats", menuName = "InitialStats", order = 0)]
public class InitialStats : ScriptableObject {
    #region Health
    [Header("Health")]
    public int initialHealth = 100;
    public int initialHealthRegenRate = 1;
    public float initialDelayBeforeHealthRegen = 1f;
    #endregion

    #region dragon energy stats
    [Header("Dragon Energy")]
    public int initialDragonEnergy = 100;
    public float initialDragonEnergyRegenRate = 10;
    public float initialDragonEnergyDrainRate = 10;
    public float initialDragonHealthScale = 2;
    public float initialDragonHealthRegenRate = 0.1f;
    #endregion
}