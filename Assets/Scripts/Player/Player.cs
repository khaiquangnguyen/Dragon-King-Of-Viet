using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using CharacterBehavior;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

internal enum Forms {
    Dragon,
    Man
}

public enum Environment {
    Ground,
    Air,
    Water
}

[RequireComponent(typeof(CharacterController2D))]
public class Player : GameCharacter {
    [HideInInspector]
    public CharacterController2D characterController;

    private readonly IReadOnlyList<PlayerState> airStates = new List<PlayerState> {
        PlayerState.ManJump,
        PlayerState.ManFall
    };

    #region -------------------- Health Attributes --------------------------------------
    private int maxHealth;
    private int currentHealth;
    private int healthRegenRate;
    private float delayBeforeHealthRegen;
    private float dragonHealthScale;
    #endregion

    #region --------------------- State Attributes -------------------------------------
    private Forms form = Forms.Man;
    #endregion

    #region --------------------- Movement Attributes -------------------------------------
    [HideInInspector]
    public int inputDirectionX;
    [HideInInspector]
    #endregion

    #region --------------------- Jump Attributes -------------------------------------
    private float coyoteTimeCountdown;
    private float jumpBufferCountdown;
    [HideInInspector]
    public bool isJumpCut;
    [HideInInspector]
    public bool isFastFalling;
    private int jumpCount;
    private const int MAX_JUMP_COUNT = 1;
    #endregion

    #region --------------------- Empower Attributes -------------------------------------
    private float empoweredHoldBufferCountdown;
    private float empoweredNextMoveBufferCountdown;
    [HideInInspector]
    public bool isEmpowering = false;
    public float dashCooldown;
    public float dashCooldownCountdown;
    #endregion

    #region --------------------- Dragon Movement Attributes ----------------------------
    [HideInInspector]
    public Vector3 dragonMoveDirection;
    [FormerlySerializedAs("dragonMoveTargetPosition")]
    [HideInInspector]
    public Vector3 dragonFlyTowardPosition;
    private float dragonHoverBufferCountdown;
    [HideInInspector]
    public float dragonMaxSpeed;
    [HideInInspector]
    public float dragonRotationSpeed;
    private Vector3 dragonHoverTargetPosition;
    private float dragonMovementBufferCountdown;
    [FormerlySerializedAs("dragonMoveTargetLocked")]
    public bool dragonForcedHoverInPlace;
    private bool isDragonHovering;
    #endregion

    #region -------------------- Dragon Energy Attributes -------------------------------
    public float maxDragonEnergy;
    public float currentDragonEnergy;
    public float dragonEnergyRegenRate;
    public float dragonEnergyDrainRate;
    #endregion

    #region --------------------- Dragon Transform Attributes ----------------------------
    #endregion

    #region --------------------- Man Attack Attributes -----------------------------------
    [HideInInspector]
    public float attackInputBufferCountdown;
    #endregion

    #region --------------------- Other Attributes --------------------------------------
    public LayerMask groundLayer;
    public PlayerStats playerStats;
    public SkillList skillList;
    private readonly bool bumpHead = false;
    [HideInInspector]
    public bool inputDisabled;
    private float lastEmpoweredSkillUsageTimestamp = Time.time;
    private RaycastHit2D groundHit;
    private RaycastHit2D headHit;
    [HideInInspector]
    public Environment environment = Environment.Ground;

    // other components
    [NonSerialized]
    public Rigidbody2D body;
    private Collider2D humanBodyCollider;
    [HideInInspector]
    public Animator humanAnimator;
    [HideInInspector]
    public Component humanRenderer;
    [HideInInspector]
    public Component dragonRenderer;
    [HideInInspector]
    public Animator dragonAnimator;
    [HideInInspector]
    public Component dragonBody;
    [HideInInspector]
    public Component humanBody;
    [HideInInspector]
    public PlayerStateMachine playerStateMachine;

    [ReadOnly]
    public PlayerState currentPlayerState;
    private ManJump manJump;
    private ManIdle manIdle;
    private ManRun manRun;
    private ManCrouch manCrouch;
    private ManDash manDash;
    private ManAttack manAttack;
    private ManFall manFall;
    private ManDefense manDefense;
    private ManEmpoweredAttack manEmpoweredAttack;
    private ManEmpoweredDefense manEmpoweredDefense;
    private ManEmpoweredFall manEmpoweredFall;
    private ManDashAttack manDashAttack;
    private ManRunAttack manRunAttack;
    private ManToDragonTransform dragonFromManTransform;
    private DragonToManTransform dragonToManTransform;
    private DragonHover dragonHover;
    private DragonFly dragonFly;
    private DragonGroundHang dragonGroundHang;
    private DragonWallHang dragonWallHang;
    private DragonCeilHang dragonCeilHang;
    private DragonFloat dragonFloat;
    public CircleCollider2D manAttackCollider;
    #endregion

    private void OnEnable() {
        body = transform.GetComponent<Rigidbody2D>();
        characterController = GetComponent<CharacterController2D>();
        dragonBody = transform.Find("DragonBody");
        humanBody = transform.Find("HumanBody");
        humanRenderer = humanBody.transform.Find("Renderer");
        dragonRenderer = dragonBody.transform.Find("Renderer");
        humanAnimator = humanBody.GetComponentInChildren<Animator>();
        dragonAnimator = dragonBody.GetComponentInChildren<Animator>();
        humanBodyCollider = humanBody.transform.Find("BodyCollider").GetComponent<CapsuleCollider2D>();
        dashCooldown = playerStats.baseDashCooldown;
        dashCooldownCountdown = -1f;
        healthManager = GetComponent<HealthManager>();
        projectileLayer = LayerMask.NameToLayer("PlayerProjectile");
        attackLayer = LayerMask.NameToLayer("PlayerAttack");

        playerStateMachine = new PlayerStateMachine();
        playerStateMachine.AddState(manIdle = new ManIdle(this));
        playerStateMachine.AddState(manJump = new ManJump(this));
        playerStateMachine.AddState(manRun = new ManRun(this));
        playerStateMachine.AddState(manDash = new ManDash(this));
        playerStateMachine.AddState(manAttack = new ManAttack(this));
        playerStateMachine.AddState(manDefense = new ManDefense(this));
        playerStateMachine.AddState(manEmpoweredAttack = new ManEmpoweredAttack(this));
        playerStateMachine.AddState(manEmpoweredDefense = new ManEmpoweredDefense(this));
        playerStateMachine.AddState(manFall = new ManFall(this));
        playerStateMachine.AddState(manEmpoweredFall = new ManEmpoweredFall(this));
        playerStateMachine.AddState(manDashAttack = new ManDashAttack(this));
        playerStateMachine.AddState(manRunAttack = new ManRunAttack(this));
        playerStateMachine.AddState(manCrouch = new ManCrouch(this));
        playerStateMachine.AddState(dragonFromManTransform = new ManToDragonTransform(this));
        playerStateMachine.AddState(dragonToManTransform = new DragonToManTransform(this));
        playerStateMachine.AddState(dragonHover = new DragonHover(this));
        playerStateMachine.AddState(dragonFly = new DragonFly(this));
        playerStateMachine.AddState(dragonGroundHang = new DragonGroundHang(this));
        playerStateMachine.AddState(dragonWallHang = new DragonWallHang(this));
        playerStateMachine.AddState(dragonCeilHang = new DragonCeilHang(this));
        playerStateMachine.AddState(dragonFloat = new DragonFloat(this));
    }

    private void Start() { }

    private void Update() {
        currentPlayerState = playerStateMachine.currentPlayerState;
        UpdateTimer();
        UpdateInputAndDirection();
        if (inputDisabled) return;
        CheckManJumpInputs();
        if (playerStateMachine.currentPlayerState == PlayerState.ManIdle) {
            CheckChangeToManRunState();
            CheckChangeToManDashState();
            CheckChangeToManAttackState();
            CheckChangeToManDefenseState();
        }

        if (playerStateMachine.currentPlayerState == PlayerState.ManRun) {
            CheckChangeToManRunState();
            CheckChangeToManDashState();
            CheckChangeToManAttackState();
            CheckChangeToManDefenseState();
        }

        if (playerStateMachine.currentPlayerState == PlayerState.ManAttack) {
            CheckChangeToManAttackState();
            if (manAttack.GetAttackAnimationCancellable()) {
                CheckChangeToManDashState();
                CheckChangeToManDefenseState();
            }
        }

        if (playerStateMachine.currentPlayerState == PlayerState.ManDefense) {
            CheckChangeToManDashState();
            CheckChangeToManAttackState();
            CheckChangeToManDefenseState();
            CheckChangeToIdleState();
        }

        if (playerStateMachine.currentPlayerState == PlayerState.ManDash) CheckChangeToIdleState();

        // if (stateMachine.currentPlayerState == PlayerState.ManFall) {
        //     CheckChangeToManDashState();
        //     CheckChangeToManRunState();
        //     CheckChangeToIdleState();
        // }

        if (playerStateMachine.currentPlayerState == PlayerState.ManJump) CheckChangeToManDashState();
        // if (inputDirectionX != 0) {
        //     if (stateMachine.currentCharacterState == CharacterState.ManIdle)
        //         stateMachine.ChangeState(CharacterState.ManRun);
        //     if (isEmpowering)
        //         if (dashCooldownCountdown <= 0) {
        //             stateMachine.ChangeState(CharacterState.ManDash);
        //         }
        //
        //     if (stateMachine.currentCharacterState == CharacterState.DragonHover)
        //         stateMachine.ChangeState(CharacterState.DragonFloat);
        // }

        if (playerStateMachine.currentStateBehavior.form == PlayerForm.Dragon && Input.GetButtonDown("Empower"))
            playerStateMachine.ChangeState(PlayerState.DragonFly);

        if (playerStateMachine.currentPlayerState == PlayerState.DragonFly && Input.GetButtonUp("Empower"))
            playerStateMachine.ChangeState(PlayerState.DragonHover);

        playerStateMachine.Update();
        transform.localScale = new Vector3(facingDirection, transform.localScale.y, transform.localScale.z);
        CheckEmpowerAndTransformInputs();
        CheckSkillTrigger();
        // CheckChangeToManAttackState();
    }

    private void CheckChangeToManRunState() {
        var formValidated = playerStateMachine.currentStateBehavior.form == PlayerForm.Man;
        var inputValidated = inputDirectionX != 0 && !isEmpowering;
        var environmentValidated = environment == Environment.Ground;
        var canRun = formValidated && inputValidated && environmentValidated;
        if (canRun) playerStateMachine.ChangeState(PlayerState.ManRun);
    }

    private void CheckChangeToIdleState() {
        var formValidated = playerStateMachine.currentStateBehavior.form == PlayerForm.Man;
        var environmentValidated = environment == Environment.Ground;
        var speedValidated = Mathf.Approximately(body.linearVelocity.x, 0);
        if (environmentValidated && formValidated && speedValidated)
            playerStateMachine.ChangeState(PlayerState.ManIdle);
    }

    private void CheckChangeToManDashState() {
        var formValidated = playerStateMachine.currentStateBehavior.form == PlayerForm.Man;
        var inputValidated = Input.GetButtonDown("Horizontal") && isEmpowering;
        var cooldownValidated = dashCooldownCountdown <= 0;
        var canDash = formValidated && inputValidated && cooldownValidated;
        if (canDash) playerStateMachine.ChangeState(PlayerState.ManDash);
    }

    private void CheckChangeToManAttackState() {
        var inputValidated = Input.GetButtonDown("Attack");
        if (inputValidated) {
            attackInputBufferCountdown = playerStats.attackInputBufferDuration;
            playerStateMachine.ChangeState(PlayerState.ManAttack);
        }
    }

    private void CheckChangeToManDefenseState() {
        var inputValidated = Input.GetButtonDown("Defense");
        if (inputValidated) {
            attackInputBufferCountdown = playerStats.attackInputBufferDuration;
            playerStateMachine.ChangeState(PlayerState.ManDefense);
        }
    }

    private void UpdateInputAndDirection() {
        inputDirectionX = (int)Input.GetAxisRaw("Horizontal");
        facingDirection = inputDirectionX switch {
            < 0 => -1,
            > 0 => 1,
            _ => facingDirection
        };
    }

    public void CommonManInputChecks() { }

    public float GetGravityMult() {
        var gravityMult = 1f;
        if (isFastFalling) gravityMult = playerStats.fastFallingGravityMult;
        if (coyoteTimeCountdown > 0) gravityMult = playerStats.coyoteTimeGravityMult;
        return gravityMult;
    }

    public void UpdateVelocityY(float moveY) {
        body.linearVelocity = new Vector2(body.linearVelocity.x, moveY);
    }

    public void UpdateVelocityX(float moveX) {
        body.linearVelocity = new Vector2(moveX, body.linearVelocity.y);
    }

    public void MoveX(float acceleration, float deceleration, float maxEnvSpeedX) {
        // can only run on ground
        // human x movement is dependent on the environment
        var maxVx = maxEnvSpeedX * inputDirectionX;
        var maxSpeedX = Math.Abs(maxVx);
        // if direction of moving is opposite of facing direction, then change to facing direction
        float vX;
        if (body.linearVelocity.x * facingDirection < 0f) {
            vX = Mathf.Abs(body.linearVelocity.x) * facingDirection;
        }
        else {
            // greater than max speed and moving in the same direction, then decelerate
            var accelerationFactor = Mathf.Abs(body.linearVelocity.x) > maxSpeedX
                ? deceleration
                : acceleration;
            vX = Mathf.MoveTowards(body.linearVelocity.x, maxSpeedX * facingDirection,
                accelerationFactor * Time.fixedDeltaTime);
        }

        UpdateVelocityX(vX);
    }

    public void UpdateVelocity(float vX, float vY) {
        body.linearVelocity = new Vector2(vX, vY);
    }

    public void CheckIdleState() {
        if (body.linearVelocity.x == 0 && environment == Environment.Ground)
            playerStateMachine.ChangeState(PlayerState.ManIdle);
    }

    private void FixedUpdate() {
        CheckGround();
        playerStateMachine.FixedUpdate();
    }

    public void ResetEmpowermentAfterTrigger() {
        empoweredNextMoveBufferCountdown = -1f;
        lastEmpoweredSkillUsageTimestamp = Time.time;
    }

    public void CheckGround() {
        if (environment == Environment.Air)
            playerStateMachine.ChangeState(PlayerState.ManFall);
    }

    private void CheckEmpowerAndTransformInputs() {
        // if (Input.GetButtonDown("Empower")) {
        //     if (empoweredHoldBufferCountdown > 0) {
        //         if (stateMachine.currentStateBehavior.form == PlayerForm.Man)
        //             stateMachine.ChangeState(CharacterState.ManToDragon);
        //         else if (stateMachine.currentStateBehavior.form == PlayerForm.Dragon)
        //             stateMachine.ChangeState(CharacterState.DragonToMan);
        //     }
        //     else {
        //         empoweredHoldBufferCountdown = playerStats.empowerAllowedHoldDuration;
        //     }
        // }

        // still holding the empower button
        isEmpowering = Input.GetButton("Empower") &&
                       Time.time - lastEmpoweredSkillUsageTimestamp > playerStats.empowerCooldown;
    }

    private void CheckManJumpInputs() {
        if (Input.GetButtonDown("Jump")) {
            isJumpCut = false;
            jumpBufferCountdown = playerStats.jumpBufferDuration;
        }

        if (Input.GetButtonUp("Jump"))
            // if release jump button before even jump, or during jumping, trigger jump cut
            if (jumpBufferCountdown > 0 ||
                (playerStateMachine.currentPlayerState == PlayerState.ManJump && manJump.CanJumpCut())) {
                isJumpCut = true;
                isFastFalling = true;
            }

        var isJumpCountValid = jumpCount < MAX_JUMP_COUNT;
        var isJumpGracePeriod = coyoteTimeCountdown > 0;
        var isJumpBufferPeriod = jumpBufferCountdown > 0;
        var isGrounded = environment == Environment.Ground;
        var canJump = isJumpCountValid && isJumpBufferPeriod && (isGrounded || isJumpGracePeriod);
        if (canJump) {
            jumpBufferCountdown = -1;
            playerStateMachine.ChangeState(PlayerState.ManJump);
        }
    }

    private void UpdateTimer() {
        UpdateCountdownTimer(ref attackInputBufferCountdown);
        UpdateCountdownTimer(ref dashCooldownCountdown);
        UpdateCountdownTimer(ref jumpBufferCountdown);
        UpdateCountdownTimer(ref coyoteTimeCountdown);
        UpdateCountdownTimer(ref empoweredNextMoveBufferCountdown);
        UpdateCountdownTimer(ref empoweredHoldBufferCountdown);
        return;
    }

    private void UpdateCountdownTimer(ref float timer) {
        timer = Mathf.Max(timer - Time.deltaTime, -1f);
    }

    public void DragonMove(Vector3 targetPosition) {
        dragonMoveDirection = targetPosition - transform.position;
        var angle = Mathf.Atan2(dragonMoveDirection.y, dragonMoveDirection.x) * Mathf.Rad2Deg;
        // rotate along the axis by angle
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        // transform.up = Vector3.Lerp(transform.up,
        //     Time.fixedDeltaTime * dragonRotationSpeed);
        // set rotation to toward direction + 90
        var movementAcceleration = Mathf.Abs(body.linearVelocity.magnitude) < dragonMaxSpeed
            ? playerStats.dragonAccel
            : playerStats.dragonDecel;
        var dragonSpeed = playerStats.dragonMaxSpeed;
        body.linearVelocity = dragonMoveDirection.normalized * dragonSpeed;
    }

    #region ------------------- Utils Methods ----------------------------------
    private void ChangeAlphaOfSpriteRenderer(SpriteRenderer spriteRenderer, float alpha) {
        var color = spriteRenderer.color;
        color.a = alpha;
        spriteRenderer.color = color;
    }

    public void ChangeAlphaOfHumanAnimator(float alpha) {
        var childRenderers = humanBody.gameObject.GetComponentsInChildren<SpriteRenderer>();
        foreach (var spriteRenderer in childRenderers) ChangeAlphaOfSpriteRenderer(spriteRenderer, alpha);
    }

    public void ChangeAlphaOfDragonAnimator(float alpha) {
        var childRenderers = dragonBody.gameObject.GetComponentsInChildren<SpriteRenderer>();
        foreach (var spriteRenderer in childRenderers) ChangeAlphaOfSpriteRenderer(spriteRenderer, alpha);
    }
    #endregion

    public void CheckDragonForcedHover() {
        var isDownButtonHold = Input.GetAxisRaw("Vertical") < 0;
        if (isDownButtonHold) {
            if (!dragonForcedHoverInPlace) {
                dragonForcedHoverInPlace = true;
                dragonFlyTowardPosition = body.position;
                playerStateMachine.ChangeState(PlayerState.DragonHover);
            }
        }
        else {
            dragonForcedHoverInPlace = false;
        }
    }

    #region ------------------- Skills Usage ----------------------------------
    private void CheckSkillTrigger() {
        if (Input.GetButtonDown("Skill1")) {
            var firstSkill = skillList.GetEnabledSkills().First();
            firstSkill.Init(this);
            firstSkill.Use();
        }
    }
    #endregion

    public override void OnDealDamage(int damageDealt, IDamageTaker gameCharacter) {
        gameCharacter.OnTakeDamage(damageDealt, this);
    }

    public override DamageResult OnTakeDamage(int damage, IDamageDealer damageDealer) {
        throw new NotImplementedException();
    }

    public void OnSkillOrAttackHit(int baseDamage, GameCharacter damagedCharacter) {
        // calculate real skill damage
        var damage = (int)(baseDamage * damageMult);
        OnDealDamage(damage, damagedCharacter);
    }

    public float abilityHaste { get; }
}
