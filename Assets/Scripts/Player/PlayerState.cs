public enum PlayerState {
    ManIdle,
    ManRun,
    ManDash,
    ManAttack,
    ManJump,
    ManShortDash,
    ManWallHang,
    ManCrouch,
    ManEmpoweredAttack,
    ManEmpoweredDefense,
    ManEmpoweredJump,
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
    DragonFloatJump,
    ManToDragon,
    DragonToMan,
    DragonCastSpell
}

public enum PlayerForm {
    Dragon,
    Man
}
