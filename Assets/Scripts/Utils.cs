using System.Linq;
using UnityEngine;

public static class Utils {
    public static void PlayAnimationMatchingDuration(Animator animator, AnimationClip animationClip, float duration) {
        var clips = animator.runtimeAnimatorController.animationClips;
        var clipLength = animationClip.length;
        animator.speed = clipLength / duration;
        animator.Play(animationClip.name);
    }
}
