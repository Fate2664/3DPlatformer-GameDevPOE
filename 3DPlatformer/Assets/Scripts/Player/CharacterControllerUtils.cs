using UnityEditor.Search;
using UnityEngine;
using UnityEngine.LightTransport;

namespace Platformer
{
    public static class CharacterControllerUtils
    {
        public static Vector3 GetNormalWithSphereCast(CharacterController characterController,
            LayerMask layerMask = default)
        {
            Vector3 normal = Vector3.up;
            Vector3 center = characterController.transform.position + characterController.center;
            float distance = characterController.height / 2f + characterController.stepOffset + 0.1f;

            RaycastHit hit;
            if (Physics.SphereCast(center, characterController.radius, Vector3.down, out hit, distance, layerMask))
            {
                normal = hit.normal;
            }

            return normal;
        }

        public static bool TryGetWallHit(CharacterController cc, Vector3 direction, float distance, LayerMask mask,
            out RaycastHit hit)
        {
            Vector3 center = cc.transform.TransformPoint(cc.center);
            float half = Mathf.Max(0f, (cc.height * 0.5f) - cc.radius);
            Vector3 top = center + Vector3.up * half;
            Vector3 bottom = center - Vector3.up * half;

            return Physics.CapsuleCast(top, bottom, cc.radius * 0.95f, direction.normalized, out hit, distance, mask,
                QueryTriggerInteraction.Ignore);
        }
    }
}