using UnityEngine;

namespace ChargeOfAThousandFishes {
    [CreateAssetMenu(fileName = "ChargeOfAThousandFishes", menuName = "Skills/ChargeOfAThousandFishes")]
    public class ChargeOfAThousandFishes : Skill {
        public override void Use() {
            throw new System.NotImplementedException();
        }

        public void OnEnable() {
            skillName = "Charge Of A Thousand Fishes";
        }
    }
}
