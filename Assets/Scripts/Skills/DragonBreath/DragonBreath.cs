using UnityEngine;

namespace DragonBreath {
    [CreateAssetMenu(fileName = "DragonBreath", menuName = "Skills/DragonBreath")]
    public class DragonBreath : Skill {
        public override void Use() {
            throw new System.NotImplementedException();
        }

        public void OnEnable() {
            skillName = "DragonBreath";
        }
    }
}
