using System;
using System.Collections.Generic;
using System.Linq;
using CharacterBehavior;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.Serialization;
using EditorAttributes;

public enum Environment {
    Ground,
    Air,
    Water
}

[RequireComponent(typeof(EnergyManager))]
public class Player : GameCharacter {
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
    private PlayerForm form = PlayerForm.Man;
    #endregion

    #region --------------------- Movement Attributes -------------------------------------
    [HideInInspector]
    public int inputDirectionX;
    [HideInInspector]
    public int inputDirectionY;
    #endregion

    #region --------------------- Jump Attributes -------------------------------------
    [ReadOnly]
    public float coyoteTimeCountdown;
    private float jumpBufferCountdown;
    [HideInInspector]
    public bool isJumpCut;
    [HideInInspector]
    public bool isFastFalling;
    private const int MAX_JUMP_COUNT = 1;
    #endregion

    #region --------------------- Empower Attributes -------------------------------------
    private float collectEnergyToTransformDuration = 0;
    private float empoweredNextMoveBufferCountdown;
    [ReadOnly]
    public bool isEmpowering = false;
    public float dashCooldown;
    public float dashCooldownCountdown;
    #endregion

    #region --------------------- Dragon Movement Attributes ----------------------------
    [HideInInspector]
    public Vector3 dragonMoveDirection;
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
    public SkillList manSkills;
    public SkillList dragonSkills;
    private readonly bool bumpHead = false;
    [HideInInspector]
    public bool inputDisabled;
    private float lastEmpoweredSkillUsageTimestamp = Time.time;
    private RaycastHit2D groundHit;
    private RaycastHit2D headHit;
    [ReadOnly]
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
    public GameObject dragonBody;
    [HideInInspector]
    public GameObject humanBody;
    [HideInInspector]
    public PlayerStateMachine stateMachine;

    [ReadOnly]
    public PlayerState currentPlayerState;
    private ManJump manJump;
    private ManIdle manIdle;
    private ManRun manRun;
    private ManCrouch manCrouch;
    private ManDash manDash;
    private ManAttack manAttack;
    private ManFall manFall;
    private ManCastSpell manCastSpell;
    private ManDefense manDefense;
    private ManEmpoweredAttack manEmpoweredAttack;
    private ManEmpoweredDefense manEmpoweredDefense;
    private ManEmpoweredFall manEmpoweredFall;
    private ManEmpoweredJump manEmpoweredJump;
    private ManWallHang manWallHang;
    private ManExecution manExecution;
    private ManDodge manShortDash;
    private ManToDragonTransform dragonFromManTransform;
    private DragonToManTransform dragonToManTransform;
    private DragonFloatMove dragonFloatMove;
    private DragonFly dragonFly;
    private DragonIdle dragonIdle;
    private DragonDefense dragonDefense;
    private DragonAttack dragonAttack;
    private DragonCastSpell dragonCastSpell;
    public CircleCollider2D manAttackCollider;
    public CircleCollider2D dragonAttackCollider;
    public bool canWallHang = true;
    [HideInInspector]
    public PlayerSpellSlotLevelOne playerSpellSlotLevelOne;
    [HideInInspector]
    public PlayerSpellSlotLevelTwo playerSpellSlotLevelTwo;
    [HideInInspector]
    public PlayerSpellSlotLevelThree playerSpellSlotLevelThree;
    private EnergyManager energyManager;
    #endregion

    private void OnEnable() {
        // we only use the collider to sphere cast, so no need to actually enable it
        canWallHang = true;
        manAttackCollider.enabled = false;
        energyManager = GetComponent<EnergyManager>();
        body = transform.GetComponent<Rigidbody2D>();
        characterController = GetComponent<CharacterController2D>();
        dragonBody = transform.Find("DragonBody").gameObject;
        humanBody = transform.Find("HumanBody").gameObject;
        humanRenderer = humanBody.transform.Find("Renderer");
        dragonRenderer = dragonBody.transform.Find("Renderer");
        humanAnimator = humanBody.GetComponentInChildren<Animator>();
        dragonAnimator = dragonBody.GetComponentInChildren<Animator>();
        playerSpellSlotLevelOne = GetComponent<PlayerSpellSlotLevelOne>();
        playerSpellSlotLevelTwo = GetComponent<PlayerSpellSlotLevelTwo>();
        playerSpellSlotLevelThree = GetComponent<PlayerSpellSlotLevelThree>();
        humanBodyCollider = humanBody.transform.Find("BodyCollider").GetComponent<CapsuleCollider2D>();
        dashCooldown = playerStats.baseDashCooldown;
        dashCooldownCountdown = -1f;
        healthManager = GetComponent<HealthManager>();
        projectileLayer = LayerMask.NameToLayer("PlayerProjectile");
        attackLayer = LayerMask.NameToLayer("PlayerAttack");

        stateMachine = new PlayerStateMachine();
        stateMachine.AddState(manIdle = new ManIdle(this));
        stateMachine.AddState(manJump = new ManJump(this));
        stateMachine.AddState(manRun = new ManRun(this));
        stateMachine.AddState(manDash = new ManDash(this));
        stateMachine.AddState(manAttack = new ManAttack(this));
        stateMachine.AddState(manDefense = new ManDefense(this));
        stateMachine.AddState(manCastSpell = new ManCastSpell(this));
        stateMachine.AddState(manEmpoweredAttack = new ManEmpoweredAttack(this));
        stateMachine.AddState(manEmpoweredDefense = new ManEmpoweredDefense(this));
        stateMachine.AddState(manEmpoweredJump = new ManEmpoweredJump(this));
        stateMachine.AddState(manFall = new ManFall(this));
        stateMachine.AddState(manEmpoweredFall = new ManEmpoweredFall(this));
        stateMachine.AddState(manCrouch = new ManCrouch(this));
        stateMachine.AddState(manShortDash = new ManDodge(this));
        stateMachine.AddState(manExecution = new ManExecution(this));
        stateMachine.AddState(manWallHang = new ManWallHang(this));
        stateMachine.AddState(dragonFromManTransform = new ManToDragonTransform(this));
        stateMachine.AddState(dragonToManTransform = new DragonToManTransform(this));
        stateMachine.AddState(dragonIdle = new DragonIdle(this));
        stateMachine.AddState(dragonAttack = new DragonAttack(this));
        stateMachine.AddState(dragonFly = new DragonFly(this));
        stateMachine.AddState(dragonFloatMove = new DragonFloatMove(this));
        stateMachine.AddState(dragonDefense = new DragonDefense(this));
        stateMachine.AddState(dragonCastSpell = new DragonCastSpell(this));
        stateMachine.ChangeState(PlayerState.ManIdle);
    }

    private void Start() { }

    public void SetStateAfterMovement() {
        // out of dash duration, set to false
        if (environment == Environment.Ground) {
            if (inputDirectionX == 0)
                stateMachine.ChangeState(PlayerState.ManIdle);
            else
                stateMachine.ChangeState(PlayerState.ManRun);
        }
        else if (environment == Environment.Air) {
            stateMachine.ChangeState(PlayerState.ManFall);
        }
    }

    private void Update() {
        currentPlayerState = stateMachine.playerState;
        UpdateTimer();
        UpdateEnvironment();
        UpdateInputAndDirection();
        stateMachine.Update();
        if (inputDisabled) return;
        transform.localScale = new Vector3(facingDirection, transform.localScale.y, transform.localScale.z);
        CheckEmpowerAndTransformInputs();
        if (environment == Environment.Ground || stateMachine.playerState is PlayerState.ManWallHang) {
            coyoteTimeCountdown = playerStats.jumpCoyoteDuration;
        }
    }

    // Change to idle should be add to fixed update movement
    // since if you put it in update like other CheckChange functions,
    // it will trigger almost immediately
    public bool CheckChangeToManIdle() {
        if (form != PlayerForm.Man) return false;
        var notMoving = Mathf.Approximately(characterController.velocity.magnitude, 0);
        if (notMoving && environment == Environment.Ground) {
            stateMachine.ChangeState(PlayerState.ManIdle);
            return true;
        }

        return false;
    }

    public bool CheckChangeToManFall() {
        var isOnGround = characterController.CheckIsOnGround();
        if (!isOnGround) {
            stateMachine.ChangeState(PlayerState.ManFall);
            return true;
        }

        return false;
    }

    public bool CheckChangeToManRunState() {
        var formValidated = stateMachine.currentStateBehavior.form == PlayerForm.Man;
        var inputValidated = inputDirectionX != 0 && !isEmpowering;
        var environmentValidated = environment == Environment.Ground;
        var canRun = formValidated && inputValidated && environmentValidated;
        if (canRun) {
            stateMachine.ChangeState(PlayerState.ManRun);
        }

        return canRun;
    }

    public bool CheckChangeToDragonFloat() {
        var formValidated = stateMachine.currentStateBehavior.form == PlayerForm.Dragon;
        var inputValidated = inputDirectionX != 0 && !isEmpowering;
        var canFloat = formValidated && inputValidated;
        if (canFloat) {
            stateMachine.ChangeState(PlayerState.DragonFloatMove);
            return true;
        }

        return false;
    }

    public bool CheckChangeToDragonFly() {
        var input = Input.GetButtonDown("Jump");
        if (input) {
            stateMachine.ChangeState(PlayerState.DragonFly);
            return true;
        }

        return false;
    }

    public bool CheckChangeToManShortDashState() {
        var formValidated = stateMachine.currentStateBehavior.form == PlayerForm.Man;
        var dodgeBack = Input.GetButtonDown("DodgeBack");
        var dodgeForward = Input.GetButtonDown("DodgeForward");
        var cooldownValidated = dashCooldownCountdown <= 0;
        var inputValidated = dodgeBack || dodgeForward;
        if (!isEmpowering) {
            // can only hop on ground and water
            var environmentValidated = environment is Environment.Ground or Environment.Water;
            var canHop = formValidated && inputValidated && cooldownValidated && environmentValidated;
            if (canHop) {
                var direction = dodgeBack ? -1 : 1;
                manShortDash.SetDirection(direction);
                stateMachine.ChangeState(PlayerState.ManShortDash);
                return true;
            }
        }
        else {
            // can dash in all environment
            var canDash = formValidated && inputValidated && cooldownValidated;
            if (canDash) {
                var direction = dodgeBack ? -1 : 1;
                manDash.SetDirection(direction);
                stateMachine.ChangeState(PlayerState.ManDash);
                return true;
            }
        }

        return false;
    }

    public bool CheckChangeToManAttackState() {
        var inputValidated = Input.GetButtonDown("Attack");
        if (!inputValidated) return false;
        if (form != PlayerForm.Man) return false;
        attackInputBufferCountdown = playerStats.attackInputBufferDuration;
        stateMachine.ChangeState(isEmpowering ? PlayerState.ManEmpoweredAttack : PlayerState.ManAttack);
        return true;
    }

    public bool CheckChangeToDragonAttackState() {
        var inputValidated = Input.GetButtonDown("Attack");
        if (!inputValidated) return false;
        if (form != PlayerForm.Dragon) return false;
        attackInputBufferCountdown = playerStats.attackInputBufferDuration;
        stateMachine.ChangeState(PlayerState.DragonAttack);
        return true;
    }

    public bool CheckChangeToManDefenseState() {
        var inputValidated = Input.GetButtonDown("Defense");
        if (form != PlayerForm.Man) return false;
        if (inputValidated) {
            attackInputBufferCountdown = playerStats.attackInputBufferDuration;
            stateMachine.ChangeState(isEmpowering ? PlayerState.ManEmpoweredDefense : PlayerState.ManDefense);
            return true;
        }

        return false;
    }

    public bool CheckChangeToDragonDefenseState() {
        var inputValidated = Input.GetButtonDown("Defense");
        if (form != PlayerForm.Dragon) return false;
        if (inputValidated) {
            attackInputBufferCountdown = playerStats.attackInputBufferDuration;
            stateMachine.ChangeState(PlayerState.DragonDefense);
            return true;
        }

        return false;
    }

    private void UpdateInputAndDirection() {
        inputDirectionX = (int)Input.GetAxisRaw("Horizontal");
        inputDirectionY = (int)Input.GetAxisRaw("Vertical");
        facingDirection = inputDirectionX switch {
            < 0 => -1,
            > 0 => 1,
            _ => facingDirection
        };
    }

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
        characterController.Move(vX, vY);
    }

    private void FixedUpdate() {
        CheckGround();
        stateMachine.FixedUpdate();
    }

    public void ResetEmpowermentAfterTrigger() {
        empoweredNextMoveBufferCountdown = -1f;
        lastEmpoweredSkillUsageTimestamp = Time.time;
    }

    public void CheckGround() {
        // if (environment == Environment.Air)
        //     stateMachine.ChangeState(PlayerState.ManFall);
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

    public bool CheckChangeToManJumpOrEmpoweredJumpState() {
        if (form != PlayerForm.Man) return false;
        if (Input.GetButtonDown("Jump")) {
            var canReceiveJumpInput = true;
            if (stateMachine.playerState is PlayerState.ManJump or PlayerState.ManEmpoweredJump) {
                if (!isEmpowering) {
                    canReceiveJumpInput = false;
                }
            }

            if (canReceiveJumpInput) {
                isJumpCut = false;
                jumpBufferCountdown = playerStats.jumpBufferDuration;
            }
        }

        if (Input.GetButtonUp("Jump")) {
            // if release jump button before even jump, or during jumping, trigger jump cut
            var canRegularJumpCut = stateMachine.playerState == PlayerState.ManJump && manJump.CanJumpCut();
            var canEmpoweredJumpCut = stateMachine.playerState == PlayerState.ManEmpoweredJump &&
                                      manEmpoweredJump.CanEmpoweredJumpCut();
            if (jumpBufferCountdown > 0 || canRegularJumpCut || canEmpoweredJumpCut) {
                isJumpCut = true;
                isFastFalling = true;
            }
        }

        var isJumpGracePeriod = coyoteTimeCountdown > 0;
        var isJumpBufferPeriod = jumpBufferCountdown > 0;
        var isWallHang = stateMachine.playerState == PlayerState.ManWallHang;
        var isGrounded = environment == Environment.Ground;
        // check for empowered jump
        if (isEmpowering) {
            var canJump = isJumpBufferPeriod;
            if (canJump) {
                jumpBufferCountdown = -1;
                stateMachine.ChangeState(PlayerState.ManEmpoweredJump);
                return true;
            }
        }
        else {
            var canJump = isJumpBufferPeriod && (isGrounded || isJumpGracePeriod || isWallHang);
            if (canJump) {
                jumpBufferCountdown = -1;
                stateMachine.ChangeState(PlayerState.ManJump);
                return true;
            }
        }

        return false;
    }

    private void UpdateTimer() {
        UpdateCountdownTimer(ref attackInputBufferCountdown);
        UpdateCountdownTimer(ref dashCooldownCountdown);
        UpdateCountdownTimer(ref jumpBufferCountdown);
        UpdateCountdownTimer(ref coyoteTimeCountdown);
        UpdateCountdownTimer(ref empoweredNextMoveBufferCountdown);
        return;
    }

    private void UpdateCountdownTimer(ref float timer) {
        timer = Mathf.Max(timer - Time.deltaTime, -1f);
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

    #region ------------------- Skills Usage ----------------------------------
    public bool CheckChangeToDragonOrManCastSpell() {
        BaseSpellCast spellCastForm = form == PlayerForm.Dragon ? dragonCastSpell : manCastSpell;
        var skillList = form == PlayerForm.Dragon ? dragonSkills : manSkills;
        var spellCastState = form == PlayerForm.Dragon ? PlayerState.DragonCastSpell : PlayerState.ManCastSpell;
        if (Input.GetButtonDown("Skill1")) {
            var firstSkill = skillList.GetEnabledSkills().First();
            spellCastForm.SetSkillData(
                firstSkill,
                firstSkill.skillStartupDuration,
                firstSkill.skillActiveDuration,
                firstSkill.skillRecoveryDuration,
                firstSkill.skillStartupAnimation,
                firstSkill.skillActiveAnimation,
                firstSkill.skillRecoveryAnimation);
            stateMachine.ChangeState(spellCastState);
            firstSkill.Init(this);
            return true;
        }

        return false;
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

    public bool CheckChangeToWallHang() {
        if (!canWallHang) return false;
        if (characterController.CheckCanWallHang(facingDirection)) {
            stateMachine.ChangeState(PlayerState.ManWallHang);
            return true;
        }

        return false;
    }

    public void ChangeToMan() {
        form = PlayerForm.Man;
        stateMachine.ChangeState(PlayerState.ManIdle);
        humanBody.SetActive(true);
        dragonBody.SetActive(false);
    }

    public void ChangeToDragon() {
        form = PlayerForm.Dragon;
        stateMachine.ChangeState(PlayerState.DragonIdle);
        humanBody.SetActive(false);
        dragonBody.SetActive(true);
    }

    public bool CheckTransformIntoDragonAndBack() {
        var isEmpoweringHold = Input.GetButton("Empower");
        var isFireSkillHold = Input.GetButton("FireSkill");
        if (isEmpoweringHold && isFireSkillHold) {
            collectEnergyToTransformDuration += Time.deltaTime;
            if (collectEnergyToTransformDuration >= playerStats.empowerHoldDurationBeforeTransform) {
                if (form == PlayerForm.Man) {
                    ChangeToDragon();
                    collectEnergyToTransformDuration = 0;
                    return true;
                }
                else {
                    ChangeToMan();
                    collectEnergyToTransformDuration = 0;
                    return true;
                }
            }
        }
        else {
            collectEnergyToTransformDuration = 0;
        }

        return false;
    }
}
