using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerSpellSlotLevelThree : PlayerSpellSlot {

   protected PlayerSpellSlotLevelThree() {
      baseEnergyCost = stats.firstSpellEnergyCost;
   }
}
