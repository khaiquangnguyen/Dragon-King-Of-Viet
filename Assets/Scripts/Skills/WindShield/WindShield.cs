using UnityEngine;

namespace WindShield {
    [CreateAssetMenu(fileName = "WindShield", menuName = "Skills/WindShield")]
    public class WindShield : Skill {
        public override void Use() {
            throw new System.NotImplementedException();
        }

        public void OnEnable() {
            skillName = "Wind Shield";
        }
    }
}
