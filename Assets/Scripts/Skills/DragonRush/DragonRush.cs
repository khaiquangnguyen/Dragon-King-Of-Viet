using UnityEngine;

namespace DragonRush {
    [CreateAssetMenu(fileName = "DragonRush", menuName = "Skills/DragonRush")]
    public class DragonRush : Skill {
        public override void Use() {
            throw new System.NotImplementedException();
        }

        public void OnEnable() {
            skillName = "DragonRush";
        }
    }
}
