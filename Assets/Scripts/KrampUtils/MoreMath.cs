using UnityEngine;

namespace KrampUtils {

    public static class MoreMath {
        public static bool LinePlaneIntersection(out Vector3 intersection, Ray ray, Vector3 planeNormal, Vector3 planePoint) {
            float dotNumerator = Vector3.Dot(planePoint - ray.origin, planeNormal);
            float dotDenominator = Vector3.Dot(ray.direction, planeNormal);

            if (dotDenominator != 0.0f) {
                float length = dotNumerator / dotDenominator;
                var vector = ray.direction * length;
                intersection = ray.origin + vector;
                return true;
            } else {
                intersection = Vector3.zero;
                return false;
            }
        }

        public static float LinePointDistance(Ray ray, Vector3 point) {
            return Vector3.Cross(ray.direction, point - ray.origin).magnitude;
        }

        public static Vector3 LineClosestPoint(Ray ray, Vector3 point) {
            return ray.origin + Vector3.Project(point - ray.origin, ray.direction);
        }
    }

}