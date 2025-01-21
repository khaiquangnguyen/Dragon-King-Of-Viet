using UnityEngine;

namespace DragonSkyFall {
    [CreateAssetMenu(fileName = "DragonSkyFall", menuName = "Skills/DragonSkyFall")]
    public class DragonSkyFall : Skill {
        public override void Use() {
            throw new System.NotImplementedException();
        }

        public void OnEnable() {
            skillName = "DragonSkyFall";
        }
    }
}
