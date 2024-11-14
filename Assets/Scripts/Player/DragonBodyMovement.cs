using System;
using UnityEngine;

public class DragonBodyMovement : MonoBehaviour {
    public int segmentCount = 50;
    private LineRenderer lineRenderer;
    private Vector3[] segments;
    public float smoothSpeed = 0.05f;

    private Vector3[] segmentVelocities;

    private void Awake() {
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void OnEnable() {
        for (var i = 1; i < segmentCount; i++) segments[i] = transform.parent.transform.position;
    }

    // Start is called before the first frame update
    private void Start() {
        lineRenderer.positionCount = segmentCount;
        segments = new Vector3[segmentCount];
        segmentVelocities = new Vector3[segmentCount];
        for (var i = 1; i < segmentCount; i++) segments[i] = transform.parent.transform.position;
    }

    // Update is called once per frame
    private void Update() {
        var headTransform = transform.parent.transform;
        segments[0] = headTransform.position;
        if (segments.Length > 1)
            for (var i = 1; i < segmentCount; i++)
                segments[i] = Vector3.SmoothDamp(segments[i], segments[i - 1],
                    ref segmentVelocities[i], smoothSpeed);
        lineRenderer.SetPositions(segments);
    }

    public void ResetPositions() {
        segments = new Vector3[segmentCount];
        Array.Fill(segments, transform.parent.transform.position);
        lineRenderer.SetPositions(segments);
    }
}