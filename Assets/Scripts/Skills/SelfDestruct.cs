using UnityEngine;

public class SelfDestruct : MonoBehaviour {
  public float timeToExpire = 1.0f;

  private void Start() {
    Invoke(nameof(DestroySelf), timeToExpire);
  }

  private void DestroySelf() {
    // Destroy the game object this script is attached to
    Destroy(this.gameObject);
  }
}
