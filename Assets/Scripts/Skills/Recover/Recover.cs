using UnityEngine;

[CreateAssetMenu(fileName = "RecoverSkill", menuName = "Skills/Recover", order = 0)]
public class Recover: Skill {
    public GameObject recoverSelfBuffPrefab;
    public int healingAmount;
    public float castDuration;

    public void OnEnable() {
        skillName = "Recover";
    }

    public override void Use() {
        var spawnLocation = Caster.transform.position;
        var recoverSelfBuffAnimation = Instantiate(recoverSelfBuffPrefab, spawnLocation, Quaternion.identity);
        Caster.healthManager.Heal(healingAmount);
    }

}
