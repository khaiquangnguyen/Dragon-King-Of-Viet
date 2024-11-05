using System;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;
using Vector3 = System.Numerics.Vector3;

public abstract class ProjectileSpecs : ScriptableObject {
    public float speed;
    public float lifetime;
    public float distance;
    public abstract void OnFixedUpdate(ProjectileHandler handler, float t, float fixedDeltaTime);
}