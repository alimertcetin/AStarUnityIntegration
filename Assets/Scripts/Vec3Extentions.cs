using UnityEngine;

namespace AStarPathFinding.UnityIntegration
{
    public static class Vec3Extentions
    {
        public static Vector3 ToVector3(this Vec3 vec3)
        {
            return new Vector3(vec3.x, vec3.y, vec3.z);
        }

        public static Vec3 ToVec3(this Vector3 vector3)
        {
            return new Vec3(vector3.x, vector3.y, vector3.z);
        }
    }

}