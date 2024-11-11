using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "GameSettings", order = 0)]
public class GameSettings: ScriptableObject {
    public  LayerMask playerProjectileCollideLayerMask;
    public LayerMask enemyProjectileCollideLayerMask;
}
