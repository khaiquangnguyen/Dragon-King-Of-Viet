using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

[RequireComponent(typeof(SpriteRenderer))]
public class SpellSlotUI: MonoBehaviour {
    public List<Image> spellSlotImages;
    public bool showingPlayerSpellSelection;
    public GameObject playerSpellRotationCanvas;
    private List<GameObject> spellCardImages = new List<GameObject>();
    public float SelectionCircleRadius = 2;
    public float SelectionCircleCardSize = 1;
    public float rotationAnimationPeriod = 0.1f;
    private float timeOfLastRotation = Time.time;
    private float rotationAnimationCountdown = 0.1f;
    public AnimationCurve rotationAnimationCurve = AnimationCurve.Linear(0, 0, 1, 1);
    private float lastSpellCardIndex = 0;



    public void Update() {
        rotationAnimationCountdown = Mathf.Max(0, rotationAnimationCountdown - Time.deltaTime);
        var playerSpellManagement =  GameObject.FindWithTag("Player").GetComponent<PlayerSpellManager>();
        // update the sprite of the spell slot
        for (var i = 0; i < spellSlotImages.Count; i++) {
            var spellEquippedAtSlot = playerSpellManagement.GetCurrentSpellOfSlot(i);
            if (spellEquippedAtSlot != null) spellSlotImages[i].sprite = spellEquippedAtSlot.icon;
        }

        if (playerSpellManagement.isPreparingSpell) {
            if (!showingPlayerSpellSelection) {
                CreateSpellSelectionCircle();
            }
            UpdateShowingPlayerSPellSelectionCircle();
        }
        if (!playerSpellManagement.isPreparingSpell) {
            showingPlayerSpellSelection = false;
            spellCardImages.ForEach(Destroy);
            spellCardImages.Clear();
        }
    }

    public void CreateSpellSelectionCircle() {
        showingPlayerSpellSelection = true;
        var playerSpellSlot =  GameObject.FindWithTag("Player").GetComponent<PlayerSpellManager>();
        playerSpellRotationCanvas.GetComponent<Canvas>().renderMode = RenderMode.WorldSpace;
        spellCardImages = new List<GameObject>();
        var currentSpellsOfSlot = playerSpellSlot.GetAllSpellsOfSlot();
        timeOfLastRotation = Time.time;
        lastSpellCardIndex = playerSpellSlot.GetCurrentSpellCardIndex();
        for (var i = 0; i < currentSpellsOfSlot.Count; i++) {
            var spellCard = new GameObject("SpellCard");
            spellCard.transform.SetParent(playerSpellRotationCanvas.transform);
            var image = spellCard.AddComponent<Image>();
            image.sprite = currentSpellsOfSlot[i].icon;
            image.rectTransform.sizeDelta = new Vector2(SelectionCircleCardSize, SelectionCircleCardSize);
            image.rectTransform.localPosition = new Vector3(0, 0, 0);
            spellCardImages.Add(spellCard);
        }

        // create game object to contain spell slot
    }
    public void UpdateShowingPlayerSPellSelectionCircle() {
        var canvas = playerSpellRotationCanvas.GetComponent<Canvas>();
        var playerSpellManagement =  GameObject.FindWithTag("Player").GetComponent<PlayerSpellManager>();
        var currentSpellCardIndex = playerSpellManagement.GetCurrentSpellCardIndex();
        if (currentSpellCardIndex != lastSpellCardIndex) {
            timeOfLastRotation = Time.time;
            lastSpellCardIndex = currentSpellCardIndex;
            rotationAnimationCountdown = rotationAnimationPeriod;

        }
        // set position of canvas to center of player
        canvas.transform.position = GameObject.FindWithTag("Player").transform.position;
        var playerSpellSlot =  GameObject.FindWithTag("Player").GetComponent<PlayerSpellManager>();
        var currentSpellsOfSlot = playerSpellSlot.GetAllSpellsOfSlot();

        for (var i = 0; i < currentSpellsOfSlot.Count; i++) {
            // rotate the spell card around the player at a distance of 2
            var baseAngle = (360 / spellCardImages.Count) * i;
            var spellCardArc = 360 / spellCardImages.Count;
            var rotationAnimationCurveAtTime = rotationAnimationCurve.Evaluate(1 - rotationAnimationCountdown / rotationAnimationPeriod);
            var rotation = (currentSpellCardIndex - rotationAnimationCurveAtTime) * spellCardArc;
            var angle = Mathf.Deg2Rad * (baseAngle - rotation);
            var x = SelectionCircleRadius * Mathf.Cos(angle);
            var y = SelectionCircleRadius * Mathf.Sin(angle);
            if (playerSpellManagement.isPreparingSpell) {
                if (playerSpellManagement.isHoldingCard) {
                    spellCardImages[i].GetComponent<Image>().color = new Color(1, 1, 1, 0f);
                } else {
                    spellCardImages[i].GetComponent<Image>().color = new Color(1, 1, 1, 0.3f);
                }
            }
            // set opacity of the selected image to 1
            spellCardImages[currentSpellCardIndex].GetComponent<Image>().color = new Color(1, 1, 1, 1);
            spellCardImages[i].GetComponent<RectTransform>().localPosition = new Vector3(x, y, 0);
            // rotate the circle according to the current card selection

        }
        // create images for each spell card

    }
}
