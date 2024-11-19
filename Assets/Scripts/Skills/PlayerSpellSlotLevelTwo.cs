using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerSpellSlotLevelTwo : PlayerSpellSlot {
    protected PlayerSpellSlotLevelTwo() {
        baseEnergyCost = stats.firstSpellEnergyCost;
    }
}