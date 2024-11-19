using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerSpellSlotLevelOne : PlayerSpellSlot {

   protected PlayerSpellSlotLevelOne() {
      baseEnergyCost = stats.firstSpellEnergyCost;
   }
}
