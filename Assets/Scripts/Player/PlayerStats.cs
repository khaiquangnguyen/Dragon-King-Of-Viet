using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "PlayerMStats", menuName = "PlayerStats", order = 0)]
public class PlayerStats : ScriptableObject {
    public float empoweredNextMoveBufferDuration = 0.05f;
    public float empowerCooldown = 0.2f;
    public float empowerAllowedHoldDuration = 0.2f;
    public float healthRegenDelayDuration = 1f;

    #region Run
    [Header("Run")]
    // the frames are in fixed update so 0.02 is 1 frame
    public float manGroundAccelTime = 10;
    public float manGroundDecelTime = 10;
    public float manGroundMaxSpeed = 2f;
    public float manAirAccelTime = 10;
    public float manAirDecelTime = 10;
    public float manAirMaxSpeed = 40f;
    public float manWaterAccelTime = 10;
    public float manWaterDecelTime = 10;
    public float manWaterMaxSpeed = 100f;
    public float manGroundAccel => manGroundMaxSpeed / manGroundAccelTime;
    public float manGroundDecel => manGroundMaxSpeed / manGroundDecelTime;
    public float manAirAccel => manAirMaxSpeed / manAirAccelTime;
    public float manAirDecel => manAirMaxSpeed / manAirDecelTime;
    public float manWaterAccel => manWaterMaxSpeed / manWaterAccelTime;
    public float manWaterDecel => manWaterMaxSpeed / manWaterDecelTime;
    #endregion

    #region Dash
    [Header("Man Dash")]
    public float dashDuration = 0.2f;
    public float dashDistance = 5f;
    public float baseDashCooldown = 0.5f;
    public AnimationCurve dashDistanceCurve = AnimationCurve.Linear(0, 0, 1, 1);
    public AnimationClip dashStartupAnimation;
    public AnimationClip dashActiveAnimation;
    public AnimationClip dashRecoveryAnimation;
    #endregion

    #region Dodge Hop
    [Header("Man Dodge Hope")]
    public float dodgeHopDuration = 0.2f;
    public float dodgeHopDistance = 5f;
    public AnimationCurve dodgeHopDistanceCurve = AnimationCurve.Linear(0, 0, 1, 1);
    public float dodgeHopCooldown = 0.2f;
    public AnimationClip dodgeHopStartupAnimation;
    public AnimationClip dodgeHopActiveAnimation;
    public AnimationClip dodgeHopRecoveryAnimation;
    #endregion

    #region Jump
    [Header("Man Jump")]
    // coyote time is the time after the player has left the ground that they can still jump
    public float jumpCoyoteDuration = 0.1f;

    // buffer time is the time after the player has pressed jump that they can still jump
    // often used for when then player presses jump just before they hit the ground
    public float jumpBufferDuration = 0.1f;

    // the height the player can jump if they hold the jump button. the longer the button is held the higher they jump
    public float jumpMaxHeight = 5;
    public float jumpDuration = 0.35f;
    public AnimationCurve jumpHeightCurve = AnimationCurve.Linear(0, 0, 1, 1);

    // time the player is at the peak of their jump. we slow down the speed here but don't start falling yet
    public float jumpPeakHangThreshold = 0.95f;
    public float maxFallSpeed = 100;
    public float jumpCutHeight = 0.5f;
    public float jumpCutDuration = 0.1f;
    public AnimationCurve jumpCutHeightCurve = AnimationCurve.Linear(0, 0, 1, 1);

    // higher number means player will change from going up to going down faster
    // lower number thus, will mean the player will hang at the top of the jump cut for longer
    public float fastFallingGravityMult = 2f;
    public float coyoteTimeGravityMult = 0.5f;
    public float gravity = -90;
    #endregion

    #region Crouch
    [Header("Crouch")]
    public float crouchMaxSpeed = 10f;
    #endregion

    #region Empowered Fall
    [Header("Empowered Fall")]
    public float empoweredFallMaxSpeed = 10f;
    public float empoweredFallStartupDuration = 0.1f;
    public float empoweredFallRecoveryDuration = 0.1f;
    #endregion

    #region Empowered Defense
    [Header("Empowered Defense")]
    public float empoweredDefenseBaseEnergyDrainRate = 1f;
    public AnimationClip empoweredDefenseStartupAnimation;
    public AnimationClip empoweredDefenseActiveCounterAnimation;
    public AnimationClip empoweredDefenseActiveNoCounterAnimation;
    public AnimationClip empoweredDefenseRecoveryAnimation;
    #endregion

    #region Empowered Attack
    [Header("Empowered Attack")]
    public List<AttackStats> manEmpoweredAttackStats = new();

    #endregion

    #region Man Power Jump
    [Header("Man Power Jump")]
    public float powerJumpMaxHeight = 8;
    public float powerJumpPeakHangDuration = 0.15f;
    #endregion

    #region Attack
    [Header("Attack")]
    public List<AttackStats> manAttackStats = new();
    public float attackInputBufferDuration = 0.1f;
    public float attackInputBufferPostAttackDuration = 0.2f;
    public float blackFlashFrameDuration = 0.05f;
    public GameObject blackFlashPrefab;
    #endregion

    #region Execution
    [Header("Execution")]
    public float executionDuration = 0.5f;
    public AnimationClip executionAnimation;
    #endregion

    #region Defense
    [Header("Defense")]
    public float defenseStartupDuration = 0.1f;
    public float defenseActiveCounterDuration = 0.1f;
    public float defenseRecoveryDuration = 0.1f;
    public AnimationClip defenseStartupAnimation;
    public AnimationClip defenseActiveCounterAnimation;
    public AnimationClip defenseActiveNoCounterAnimation;
    public AnimationClip defenseRecoveryAnimation;
    #endregion

     #region Slow Dragon Transform
    [Header("Slow Dragon Transform")]
    public float manToDragonTransformDuration = 2f;
    public float dragonToManTransformDuration = 1f;
    public float ManToDragonTransformIframeDuration = 0.5f;
    public float dragonToManTransformIframeDuration = 0.5f;
    #endregion

    #region Dragon Move Stats
    public float dragonRotationSpeed = 3f;
    public float dragonToManRotationSpeed = 1f;
    public float dragonMaxSpeed = 6f;
    public float dragonAccelTime = 0.2f;
    public float dragonDecelTime = 0.6f;
    public float dragonAccel => dragonMaxSpeed / dragonAccelTime;
    public float dragonDecel => dragonMaxSpeed / dragonDecelTime;
    #endregion

    #region Dragon Attack Stats
    [Header("Dragon Attack Stats")]
    public List<AttackStats> dragonAttackStats = new();
    #endregion

    #region Dragon Defense Stats
    [Header("Dragon Defense Stats")]
    public AnimationClip dragonDefenseStartupAnimation;
    public AnimationClip dragonDefenseActiveCounterAnimation;
    public AnimationClip dragonDefenseActiveNoCounterAnimation;
    public AnimationClip dragonDefenseRecoveryAnimation;
    #endregion

    #region Dragon Empowered Defense Stats
    [Header("Dragon Empowered Defense Stats")]
    public AnimationClip dragonEmpoweredDefenseStartupAnimation;
    public AnimationClip dragonEmpoweredDefenseActiveCounterAnimation;
    public AnimationClip dragonEmpoweredDefenseActiveNoCounterAnimation;
    public AnimationClip dragonEmpoweredDefenseRecoveryAnimation;
    #endregion

    #region Dragon Empowered Attack Stats
    [Header("Dragon Empowered Attack Stats")]
    public List<AttackStats> dragonEmpoweredAttackStats = new();
    #endregion

    #region Dragon Dash Stats
    [Header("Dragon Dash Stats")]
    #endregion



    #region dragon energy stats
    [Header("Dragon Energy Stats")]
    public float dragonEnergyRegenDelayDuration = 1f;
    #endregion
}
