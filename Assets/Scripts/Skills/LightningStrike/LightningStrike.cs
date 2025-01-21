using UnityEngine;

namespace LightningStrike {
    [CreateAssetMenu(fileName = "LightningStrike", menuName = "Skills/LightningStrike")]
    public class LightningStrike : Skill {
        public override void Use() {
            throw new System.NotImplementedException();
        }

        public void OnEnable() {
            skillName = "LightningStrike";
        }
    }
}
