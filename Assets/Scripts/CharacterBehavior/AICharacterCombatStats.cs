using System.Collections.Generic;
using UnityEngine;

namespace CharacterBehavior {
    [CreateAssetMenu(fileName = "Character Basic Combat Stats", menuName = "AI Character Basic Combat Stats",
        order = 0)]
    public class AICharacterCombatStats : ScriptableObject {
        #region Attack
        [Header("Attack")]
        public List<AttackStats> attackStats = new();
        #endregion

        #region Defense
        [Header("Defense")]
        public float defenseStartupDuration = 0.1f;
        public float defenseActiveDuration = 0.5f;
        public float defenseRecoveryDuration = 0.5f;
        public AnimationClip defenseStartupAnimation;
        public AnimationClip defenseActiveAnimation;
        public AnimationClip defenseRecoveryAnimation;
        #endregion
    }
}