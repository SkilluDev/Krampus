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

        public static Vector3 RandomInBounds(Bounds bounds) => new Vector3(Random.Range(bounds.min.x, bounds.max.x), Random.Range(bounds.min.y, bounds.max.y), Random.Range(bounds.min.z, bounds.max.z));

        public static Vector2 WihtX(this Vector2 vec, float x) => new Vector2(x, vec.y);
        public static Vector2 WithY(this Vector2 vec, float y) => new Vector2(vec.x, y);

        public static Vector3 NoX(this Vector3 vector) => new Vector3(0, vector.y, vector.z);
        public static Vector3 NoY(this Vector3 vector) => new Vector3(vector.x, 0, vector.z);
        public static Vector3 NoZ(this Vector3 vector) => new Vector3(vector.x, vector.y, 0);
    }

}