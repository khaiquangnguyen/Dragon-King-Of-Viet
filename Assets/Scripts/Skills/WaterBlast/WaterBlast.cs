using UnityEngine;

namespace WaterBlast {
    [CreateAssetMenu(fileName = "WaterBlast", menuName = "Skills/WaterBlast")]
    public class WaterBlast : Skill {
        public override void Use() {
            throw new System.NotImplementedException();
        }

        public void OnEnable() {
            skillName = "Water Blast";
        }
    }
}
