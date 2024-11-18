using System;
using UnityEngine;

[Serializable]
public struct AttackStats {
    public float startupDuration;
    public float activeDuration;
    public float recoveryDuration;
    public float attackCancelableAfter;
    public int damage;
    public bool cancelable;
    public AnimationClip startupAnimation;
    public AnimationClip activeAnimation;
    public AnimationClip recoveryAnimation;
}
