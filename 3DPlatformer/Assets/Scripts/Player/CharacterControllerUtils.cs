using UnityEngine;

namespace Platformer
{
    public static class CharacterControllerUtils
    {
        public static bool TryGetStepOffset(CapsuleCollider col, Vector3 moveDelta, float stepOffset, float slopeLimit,
            LayerMask collisionMask, LayerMask stepSurfaceMask, out Vector3 stepDelta)
        {
            stepDelta = Vector3.zero;

            Vector3 flatMove = Vector3.ProjectOnPlane(moveDelta, Vector3.up);
            float moveDistance = flatMove.magnitude;
            if (moveDistance <= 0.001f) return false;
            
            Vector3 moveDirection = flatMove / moveDistance;
            GetCapsuleData(col, out Vector3 top, out Vector3 bottom, out float radius);

            const float skin = 0.02f;
            float castRadius = radius * 0.95f;

            if (!Physics.CapsuleCast(top, bottom, castRadius, moveDirection, out RaycastHit obstacleHit,
                    moveDistance + skin, collisionMask, QueryTriggerInteraction.Ignore))
                return false;


            float obstacleAngle = Vector3.Angle(obstacleHit.normal, Vector3.up);
            if (obstacleAngle <= slopeLimit)
                return false;

            float forwardDistance = Mathf.Max(moveDistance + skin, obstacleHit.distance + (radius * 0.25f));
            Vector3 candidateOffset = moveDirection * forwardDistance + Vector3.up * (stepOffset + skin);

            if (Physics.CheckCapsule(top + candidateOffset, bottom + candidateOffset, castRadius, collisionMask,
                    QueryTriggerInteraction.Ignore))
                return false;

            if (!Physics.CapsuleCast(top + candidateOffset, bottom + candidateOffset, castRadius, Vector3.down,
                    out RaycastHit landingHit, stepOffset + (skin * 2f), stepSurfaceMask,
                    QueryTriggerInteraction.Ignore))
                return false;

            float landingAngle = Vector3.Angle(landingHit.normal, Vector3.up);
            if (landingAngle > slopeLimit)
                return false;

            float verticalLift = candidateOffset.y - landingHit.distance;
            if (verticalLift <= 0)
                return false;
            
            stepDelta = Vector3.up * verticalLift;
            return true;
        }

        private static bool CheckWallHit(CapsuleCollider col, Vector3 direction, float distance, LayerMask mask,
            out RaycastHit hit)
        {
            GetCapsuleData(col, out Vector3 top, out Vector3 bottom, out float radius);

            return Physics.CapsuleCast(top, bottom, radius * 0.95f, direction.normalized, out hit, distance, mask,
                QueryTriggerInteraction.Ignore);
        }

        public static bool TryGetClimbWall(CapsuleCollider col, Vector3 moveDir, out RaycastHit hit,
            Vector3 lookForwardXZ, float climbCheckDistance, LayerMask climbableLayer, float slopeLimit,
            float maxClimbAngle)
        {
            Vector3 probeDir = moveDir.sqrMagnitude > 0.001f ? moveDir.normalized : lookForwardXZ;

            if (!CheckWallHit(col, probeDir, climbCheckDistance,
                    climbableLayer, out hit))
                return false;

            float angle = Vector3.Angle(hit.normal, Vector3.up);
            return angle > slopeLimit + 1f && angle <= maxClimbAngle;
        }

        public static bool TryMoveOffWall(RaycastHit currentWallHit, Transform transform, CapsuleCollider col,
            LayerMask groundLayer, LayerMask climbableLayer, float slopeLimit, float ledgeSnapUp,
            float ledgeSnapForward)
        {
            float ledgeProbeHeight = 1.2f;
            float ledgeProbeForward = 0.45f;
            float ledgeProbeDown = 2.0f;
            Vector3 wallNormal = currentWallHit.normal;
            if (wallNormal == Vector3.zero)
                return false;

            Vector3 center = transform.TransformPoint(col.center);

            //Probe from above the player and slightly over the ledge
            Vector3 probeOrigin = center + Vector3.up * ledgeProbeHeight - wallNormal * ledgeProbeForward;

            if (!Physics.Raycast(probeOrigin, Vector3.down, out RaycastHit topHit, ledgeProbeDown,
                    groundLayer | climbableLayer, QueryTriggerInteraction.Ignore))
                return false;

            float topAngle = Vector3.Angle(topHit.normal, Vector3.up);
            if (topAngle > slopeLimit)
                return false;

            Vector3 targetCenter =
                topHit.point + Vector3.up * ledgeSnapUp - wallNormal * ledgeSnapForward;

            transform.position = targetCenter;

            return true;
        }

        private static void GetCapsuleData(CapsuleCollider col, out Vector3 top, out Vector3 bottom, out float radius)
        {
            Transform transform = col.transform;
            Vector3 lossyScale = transform.lossyScale;

            float radiusScale = Mathf.Max(Mathf.Abs(lossyScale.x), Mathf.Abs(lossyScale.z));
            float heightScale = Mathf.Abs(lossyScale.y);

            radius = col.radius * radiusScale;
            float height = Mathf.Max(col.height * heightScale, radius * 2f);
            Vector3 center = transform.TransformPoint(col.center);
            float half = Mathf.Max(0f, (height * 0.5f) - radius);

            top = center + (transform.up * half);
            bottom = center - (transform.up * half);
        }
        
        public static bool TryGetGroundHit(out RaycastHit hit, Transform transform, CapsuleCollider col, LayerMask groundLayer, LayerMask climbableLayer, float  slopeLimit, float groundDistance)
        {
            Vector3 center = transform.TransformPoint(col.center);
            float halfSegment = Mathf.Max(0f, (col.height * 0.5f) - col.radius);
            Vector3 bottomSphereCenter = center - Vector3.up * halfSegment;

            const float skin = 0.02f;
            float castDistance = groundDistance + skin;

            if (!Physics.SphereCast(bottomSphereCenter + Vector3.up * skin, col.radius * 0.95f, Vector3.down, out hit,
                    castDistance, groundLayer | climbableLayer, QueryTriggerInteraction.Ignore))
                return false;


            float angle = Vector3.Angle(hit.normal, Vector3.up);
            return angle <= slopeLimit;
        }
    }
}