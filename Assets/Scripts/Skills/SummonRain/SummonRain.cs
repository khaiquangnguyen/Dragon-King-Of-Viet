using UnityEngine;

namespace SummonRain {
    [CreateAssetMenu(fileName = "SummonRain", menuName = "Skills/SummonRain")]
    public class SummonRain : Skill {
        public override void Use() {
            throw new System.NotImplementedException();
        }

        public void OnEnable() {
            skillName = "Summon Rain";
        }
    }
}
