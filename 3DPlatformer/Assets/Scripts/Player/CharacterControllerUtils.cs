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

            float forwardDistance = Mathf.Max(obstacleHit.distance + skin, radius * 0.5f);
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
            stepDelta = moveDirection * forwardDistance + Vector3.up * verticalLift;
            return true;
        }

        public static bool CheckWallHit(CapsuleCollider col, Vector3 direction, float distance, LayerMask mask,
            out RaycastHit hit)
        {
            GetCapsuleData(col, out Vector3 top, out Vector3 bottom, out float radius);

            return Physics.CapsuleCast(top, bottom, radius * 0.95f, direction.normalized, out hit, distance, mask,
                QueryTriggerInteraction.Ignore);
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
    }
}