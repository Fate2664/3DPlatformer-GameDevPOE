using UnityEngine;

namespace Platformer
{
    //This script is a helper to the PlayerController script. The methods coduct checks to help determine the state that the player is in. 
    public static class CharacterControllerUtils
    {
        //This method creates a step delta for the player to be able to step up small edges
        public static bool TryGetStepOffset(CapsuleCollider col, Vector3 moveDelta, float stepOffset, float slopeLimit,
            LayerMask collisionMask, LayerMask stepSurfaceMask, out Vector3 stepDelta)
        {
            stepDelta = Vector3.zero;
            
            //Get the flat movement vector
            Vector3 flatMove = Vector3.ProjectOnPlane(moveDelta, Vector3.up);
            float moveDistance = flatMove.magnitude;
            if (moveDistance <= 0.001f) return false;
            
            //Normalize the move direction and get the player capsule data
            Vector3 moveDirection = flatMove / moveDistance;
            GetCapsuleData(col, out Vector3 top, out Vector3 bottom, out float radius);
            
            //Create casting conditions (skin is a small buffer to avoid precision issues)
            const float skin = 0.02f;
            float castRadius = radius * 0.95f;
            
            //Check if the forward motion is colliding with something
            if (!Physics.CapsuleCast(top, bottom, castRadius, moveDirection, out RaycastHit obstacleHit,
                    moveDistance + skin, collisionMask, QueryTriggerInteraction.Ignore))
                return false;

            //Return false if the angle of the obstacle is a slope
            float obstacleAngle = Vector3.Angle(obstacleHit.normal, Vector3.up);
            if (obstacleAngle <= slopeLimit)
                return false;
            
            //Create a step up and forward position
            float forwardDistance = Mathf.Max(moveDistance + skin, obstacleHit.distance + (radius * 0.25f));
            Vector3 candidateOffset = moveDirection * forwardDistance + Vector3.up * (stepOffset + skin);
            
            //Make sure the raised position is not inside geometry
            if (Physics.CheckCapsule(top + candidateOffset, bottom + candidateOffset, castRadius, collisionMask,
                    QueryTriggerInteraction.Ignore))
                return false;
            
            //Get the ground hit to land on
            if (!Physics.CapsuleCast(top + candidateOffset, bottom + candidateOffset, castRadius, Vector3.down,
                    out RaycastHit landingHit, stepOffset + (skin * 2f), stepSurfaceMask,
                    QueryTriggerInteraction.Ignore))
                return false;
            
            //Make sure landing surface is walkable
            float landingAngle = Vector3.Angle(landingHit.normal, Vector3.up);
            if (landingAngle > slopeLimit)
                return false;
            
            //Apply the transform 
            float verticalLift = candidateOffset.y - landingHit.distance;
            if (verticalLift <= 0)
                return false;
            
            //Output the stepDelta
            stepDelta = Vector3.up * verticalLift;
            return true;
        }

        //This method checks the player is walking into a climbable wall and returns the wall hit
        private static bool CheckWallHit(CapsuleCollider col, Vector3 direction, float distance, LayerMask mask,
            out RaycastHit hit)
        {
            GetCapsuleData(col, out Vector3 top, out Vector3 bottom, out float radius);

            return Physics.CapsuleCast(top, bottom, radius * 0.95f, direction.normalized, out hit, distance, mask,
                QueryTriggerInteraction.Ignore);
        }
        
        //This method checks to see if the wall hit is climbable according to parameters set in playercontroller
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
        
        //This method applies a transform to snap the player over the ledge of the wall after climbing
        public static bool TryMoveOffWall(RaycastHit currentWallHit, Transform transform, CapsuleCollider col,
            LayerMask groundLayer, LayerMask climbableLayer, float slopeLimit, float ledgeSnapUp,
            float ledgeSnapForward, float ledgeProbeHeight)
        {
            Vector3 wallNormal = currentWallHit.normal;
            if (wallNormal == Vector3.zero)
                return false;
            
            //Find wall ledge
            if (!TryGetWallLedge(col, transform, currentWallHit, groundLayer, climbableLayer, ledgeProbeHeight,
                    out RaycastHit topHit))
                return false;
            
            //Check if ledge angle is walkable
            // float topAngle = Vector3.Angle(topHit.normal, Vector3.up);
            // if (topAngle > slopeLimit)
            //     return false;
            
            //Create a transform delta to move player
            Vector3 targetCenter =
                topHit.point + Vector3.up * ledgeSnapUp - wallNormal * ledgeSnapForward;
            
            //Apply transform
            transform.position = targetCenter;
            return true;
        }
        
        //This method attempts to find the ledge of the wall that the player is climbing
        public static bool TryGetWallLedge(CapsuleCollider col, Transform transform, RaycastHit currentWallHit,
            LayerMask groundLayer, LayerMask climbableLayer, float ledgeProbeHeight, out RaycastHit topHit)
        {
            //Define checking values
            float ledgeProbeForward = 0.45f;
            float ledgeProbeDown = 1.0f;
            float minTopDot = 0.4f;
            
            //Get wall normal and flatten it
            Vector3 wallNormal = Vector3.ProjectOnPlane(currentWallHit.normal, Vector3.up);
            if (wallNormal.sqrMagnitude < 0.001f)
            {
                topHit = default;
                return false;
            }
            wallNormal.Normalize();
            
            //Get the top of the player and create a probe origin to check from
            GetCapsuleData(col, out Vector3 top, out _, out _);
            Vector3 probeOrigin = top + Vector3.up * ledgeProbeHeight - wallNormal * ledgeProbeForward;
            
            //Actually check by raycasting at the probe origin
            if (!Physics.Raycast(probeOrigin, Vector3.down, out RaycastHit hit, ledgeProbeDown,
                    groundLayer | climbableLayer, QueryTriggerInteraction.Ignore))
            {
                topHit = default;
                return false;
            }
            
            //Make sure we found a top surface and not part of the wall
            if (Vector3.Dot(hit.normal, Vector3.up) < minTopDot)
            {
                topHit = default;
                return false;
            }
            
            //Return the ledge hit
            topHit = hit;
            return true;
        }
    
        //This method is a helper to get the player's capsule data
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
        
        //This method is the main ground detection method. This will be used to check wether the player is grounded or not 
        public static bool TryGetGroundHit(out RaycastHit hit, Transform transform, CapsuleCollider col,
            LayerMask groundLayer, LayerMask climbableLayer, float slopeLimit, float groundDistance)
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
