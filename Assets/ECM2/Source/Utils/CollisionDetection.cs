using System;
using UnityEngine;

namespace ECM2 {
    /// <summary>
    /// General purpose collision detection functions.
    /// Lets you filter results implementing the IColliderFilter interface.
    /// </summary>
    public static class CollisionDetection {
        #region CONSTANTS
        private const int kMaxHits = 8;
        #endregion

        #region FIELDS
        private static readonly RaycastHit[] HitsBuffer = new RaycastHit[kMaxHits];
        #endregion

        #region METHODS
        public static int Raycast(Vector3 origin, Vector3 direction, float distance, int layerMask,
            QueryTriggerInteraction queryTriggerInteraction, out RaycastHit closestHit, RaycastHit[] hits,
            IColliderFilter colliderFilter) {
            closestHit = default;

            var rawHitCount = Physics.RaycastNonAlloc(origin, direction, HitsBuffer, distance, layerMask,
                queryTriggerInteraction);

            if (rawHitCount == 0)
                return 0;

            var filteredHitCount = 0;
            var closestDistance = Mathf.Infinity;

            Array.Clear(hits, 0, hits.Length);

            for (var i = 0; i < rawHitCount; i++) {
                if (HitsBuffer[i].distance <= 0.0f ||
                    (colliderFilter != null && colliderFilter.Filter(HitsBuffer[i].collider)))
                    continue;

                if (HitsBuffer[i].distance < closestDistance) {
                    closestHit = HitsBuffer[i];
                    closestDistance = closestHit.distance;
                }

                hits[filteredHitCount++] = HitsBuffer[i];
            }

            return filteredHitCount;
        }

        public static int SphereCast(Vector3 origin, float radius, Vector3 direction, float distance, int layerMask,
            QueryTriggerInteraction queryTriggerInteraction, out RaycastHit closestHit, RaycastHit[] hits,
            IColliderFilter colliderFilter, float backStepDistance) {
            closestHit = default;

            var optOrigin = origin - direction * backStepDistance;
            var optDistance = distance + backStepDistance;

            var rawHitCount = Physics.SphereCastNonAlloc(optOrigin, radius, direction, HitsBuffer, optDistance,
                layerMask, queryTriggerInteraction);

            if (rawHitCount == 0)
                return 0;

            var filteredHitCount = 0;
            var closestDistance = Mathf.Infinity;

            Array.Clear(hits, 0, hits.Length);

            for (var i = 0; i < rawHitCount; i++) {
                if (HitsBuffer[i].distance <= 0.0f ||
                    (colliderFilter != null && colliderFilter.Filter(HitsBuffer[i].collider)))
                    continue;

                HitsBuffer[i].distance -= backStepDistance;

                if (HitsBuffer[i].distance < closestDistance) {
                    closestHit = HitsBuffer[i];
                    closestDistance = closestHit.distance;
                }

                hits[filteredHitCount++] = HitsBuffer[i];
            }

            return filteredHitCount;
        }

        public static int CapsuleCast(Vector3 point1, Vector3 point2, float radius, Vector3 direction, float distance,
            int layerMask, QueryTriggerInteraction queryTriggerInteraction, out RaycastHit closestHit,
            RaycastHit[] hits, IColliderFilter colliderFilter, float backStepDistance) {
            closestHit = default;

            var optPoint1 = point1 - direction * backStepDistance;
            var optPoint2 = point2 - direction * backStepDistance;

            var optDistance = distance + backStepDistance;

            var rawHitCount = Physics.CapsuleCastNonAlloc(optPoint1, optPoint2, radius, direction, HitsBuffer,
                optDistance, layerMask, queryTriggerInteraction);

            if (rawHitCount == 0)
                return 0;

            var filteredHitCount = 0;
            var closestDistance = Mathf.Infinity;

            Array.Clear(hits, 0, hits.Length);

            for (var i = 0; i < rawHitCount; i++) {
                if (HitsBuffer[i].distance <= 0.0f ||
                    (colliderFilter != null && colliderFilter.Filter(HitsBuffer[i].collider)))
                    continue;

                HitsBuffer[i].distance -= backStepDistance;

                if (HitsBuffer[i].distance < closestDistance) {
                    closestHit = HitsBuffer[i];
                    closestDistance = closestHit.distance;
                }

                hits[filteredHitCount++] = HitsBuffer[i];
            }

            return filteredHitCount;
        }

        public static int OverlapCapsule(Vector3 point1, Vector3 point2, float radius, int layerMask,
            QueryTriggerInteraction queryTriggerInteraction, Collider[] results, IColliderFilter colliderFilter) {
            var rawOverlapCount =
                Physics.OverlapCapsuleNonAlloc(point1, point2, radius, results, layerMask, queryTriggerInteraction);

            if (rawOverlapCount == 0)
                return 0;

            var filteredOverlapCount = rawOverlapCount;

            for (var i = 0; i < rawOverlapCount; i++) {
                var overlappedCollider = results[i];

                if (colliderFilter != null && !colliderFilter.Filter(overlappedCollider))
                    continue;

                if (i < --filteredOverlapCount)
                    results[i] = results[filteredOverlapCount];
            }

            return filteredOverlapCount;
        }
        #endregion
    }
}