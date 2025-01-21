using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(SpriteRenderer))]
public class SpellSlotUI: MonoBehaviour {
    public List<Image> spellSlotImages;

    public void FixedUpdate() {
        var player = GameObject.FindWithTag("Player");
        var playerSpellSlot = player.GetComponent<PlayerSpellSlotManagement>();
        // update the sprite of the spell slot
        for (var i = 0; i < spellSlotImages.Count; i++) {
            var spellEquippedAtSlot = playerSpellSlot.GetCurrentSpellOfSlot(i);
            if (spellEquippedAtSlot != null) spellSlotImages[i].sprite = spellEquippedAtSlot.icon;
        }
    }
}
