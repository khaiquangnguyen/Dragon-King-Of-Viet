using UnityEngine;

namespace WaterReflection {
    [CreateAssetMenu(fileName = "WaterReflection", menuName = "Skills/WaterReflection")]
    public class WaterReflection : Skill {
        public override void Use() {
            throw new System.NotImplementedException();
        }

        public void OnEnable() {
            skillName = "Water Reflection";
        }
    }
}
