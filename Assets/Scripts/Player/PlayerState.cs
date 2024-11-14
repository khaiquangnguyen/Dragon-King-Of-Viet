public enum PlayerState {
    ManIdle,
    ManRun,
    ManDash,
    ManAttack,
    ManJump,
    ManDodgeHop,
    ManWallHang,
    ManCrouch,
    ManEmpoweredAttack,
    ManEmpoweredDefense,
    ManEmpoweredFall,
    ManFall,
    ManDefense,
    ManExecution,
    ManCounter,
    ManCastSpell,
    // hover is in place
    DragonHover,
    // float is movement, left and right
    DragonFloat,
    DragonFly,
    DragonAttack,
    DragonFire,
    DragonDefense,
    DragonCounter,
    ManToDragon,
    DragonWallHang,
    DragonCeilHang,
    DragonGroundHang,
    DragonToMan,
    DragonCastSpell
}

public enum PlayerForm {
    Dragon,
    Man
}
