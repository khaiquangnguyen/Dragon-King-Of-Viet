public enum CharacterState {
    ManIdle,
    ManRun,
    ManDash,
    ManAttack,
    ManJump,
    ManWallHang,
    ManJumpAttack,
    ManDashAttack,
    ManCrouch,
    ManEmpoweredAttack,
    ManEmpoweredDefense,
    ManRunAttack,
    ManEmpoweredFall,
    ManFall,
    ManDefense,
    ManCounter,
    ManCastAbility,
    // hover is in place
    DragonHover,
    // float is movement, left and right
    DragonFloat,
    DragonFly,
    DragonClaw,
    DragonFire,
    DragonDefense,
    DragonCounter,
    ManToDragon,
    DragonWallHang,
    DragonCeilHang,
    DragonGroundHang,
    DragonToMan,
    DragonCastAbility
}

public enum SkillState {
    Ready,
    Startup,
    Active,
    Recovery
}


public enum DefenseState {
    Ready,
    Startup,
    ActivePreCounter,
    ActiveDuringCounter,
    ActivePostCounter,
    Recovery
}

public enum PlayerForm {
    Dragon,
    Man,
}
