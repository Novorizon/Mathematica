using Mathematica;

namespace Mathematica
{
    public static partial class Geometry
    {
        public static bool IsOverlap(Sphere sphere, fix3 point) { return math.distancesq(point, sphere.Center) <= sphere.Radius2; }

        public static bool IsOverlap(Sphere a, Sphere b) { return math.distancesq(a.Center, b.Center) < (a.Radius + b.Radius) * (a.Radius + b.Radius); }
        public static bool IsOverlap(Sphere sphere, AABB aabb) { return SeparatingAxisTest(aabb, sphere); }
        public static bool IsOverlap(Sphere sphere, OBB obb) { return SeparatingAxisTest(obb, sphere); }
        public static bool IsOverlap(Sphere sphere, Capsule capsule) { return SeparatingAxisTest(capsule, sphere); }

        public static bool IsOverlapFast(Sphere sphere, AABB aabb)
        {
            fix3 p = sphere.Center - aabb.Center;

            fix3 v = math.max(p, -p);
            fix3 u = math.max(v - aabb.BevelRadius, fix3.zero);
            return math.length(u) < sphere.Radius;
        }


        public static fix3 CollidePoint(Sphere sphere, AABB aabb)
        {
            fix3 p = sphere.Center - aabb.Center;

            fix3 v = math.max(p, -p);
            fix3 u = math.max(v - aabb.BevelRadius, fix3.zero);
            if (math.length(u) <= sphere.Radius)
            {
                return sphere.Center - sphere.Radius * math.normalize(u);
            }
            return fix3.MinValue;
        }


        public static bool IsOverlapFast(Sphere sphere, OBB obb)
        {
            fix3 p = sphere.Center - obb.Center;
            p = obb.Orientation * p;

            fix3 v = math.max(p, -p);
            fix3 u = math.max(v - obb.BevelRadius, fix3.zero);
            return math.length(u) < sphere.Radius;
        }


        public static fix3 CollidePoint(Sphere sphere, OBB obb)
        {
            fix3 p = sphere.Center - obb.Center;
            p = obb.Orientation * p;
            p = obb.Orientation * p;

            fix3 v = math.max(p, -p);
            fix3 u = math.max(v - obb.BevelRadius, fix3.zero);
            if (math.length(u) <= sphere.Radius)
            {
                return sphere.Center - sphere.Radius * math.normalize(u);
            }
            return fix3.MinValue;
        }
    }
}
