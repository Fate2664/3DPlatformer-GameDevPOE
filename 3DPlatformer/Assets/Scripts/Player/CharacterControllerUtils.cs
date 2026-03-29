using UnityEditor.Search;
using UnityEngine;
using UnityEngine.LightTransport;

namespace Platformer
{
    public static class CharacterControllerUtils
    {
        public static bool TryGetWallHit(CapsuleCollider col, Vector3 direction, float distance, LayerMask mask,
            out RaycastHit hit)
        {
            Vector3 center = col.transform.TransformPoint(col.center);
            float half = Mathf.Max(0f, (col.height * 0.5f) - col.radius);
            Vector3 top = center + Vector3.up * half;
            Vector3 bottom = center - Vector3.up * half;

            return Physics.CapsuleCast(top, bottom, col.radius * 0.95f, direction.normalized, out hit, distance, mask,
                QueryTriggerInteraction.Ignore);
        }
    }
}