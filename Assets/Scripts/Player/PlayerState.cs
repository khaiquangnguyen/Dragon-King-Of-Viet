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
    DragonIdle,
    // float is movement, left and right
    DragonFloatMove,
    DragonFly,
    DragonAttack,
    DragonFire,
    DragonDefense,
    DragonEmpoweredDefense,
    DragonJump,
    ManToDragon,
    DragonToMan,
    DragonCastSpell
}

public enum PlayerForm {
    Dragon,
    Man
}