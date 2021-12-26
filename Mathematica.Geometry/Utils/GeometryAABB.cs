using Mathematica;
using System;

namespace Mathematica
{
    public static partial class Geometry
    {
        public static bool IsOverlap1(AABB aabb, fix3 point)
        {
            fix3 test0 = aabb.Points[0];
            for (int i = 0; i < 4; i++)
            {
                fix3 test1 = (point - aabb.Points[i - 1]);
                fix3 n = AABB.Normals[i];

                if (math.dot(test0, n) * math.dot(test1, n) > 0)
                    return false;
            }
            return true;
        }
        public static bool IsOverlap(AABB aabb, fix3 point)
        {
            if (aabb.Min.x > point.x || aabb.Min.y > point.y || aabb.Min.z > point.z || aabb.Max.x < point.x || aabb.Max.y < point.y || aabb.Max.z < point.z)
                return false;
            return true;
        }


        /// SeparatingAxisTest  AABB AABB
        public static bool IsOverlap(AABB a, AABB b) { return SeparatingAxisTest(a, b); }

        [Obsolete("不完善 暂时不用")]
        public static bool IsOverlap1(AABB a, AABB b)
        {
            if (a.Min.x > b.Max.x || a.Min.y > b.Max.y || a.Min.z > b.Max.z || a.Max.x < b.Min.x || a.Max.y < b.Min.y || a.Max.z < b.Min.z)
                return false;
            return true;
        }

        [Obsolete("重新Update造成多两倍的GC耗时")]
        public static bool IsOverlap1(AABB aabb, OBB obb)
        {
            fix3 p = obb.Center - aabb.Center;
            obb.Update(p, quaternion.identity);

            for (int i = 0; i < OBB.VERTEX; i++)
            {
                if (IsOverlap(aabb, obb.Points[i]))
                    return true;
            }
            return false;
        }

        public static bool IsOverlap(AABB aabb, OBB obb) { return SeparatingAxisTest(aabb, obb); }
        public static bool IsOverlap(AABB aabb, Sphere sphere) { return SeparatingAxisTest(aabb, sphere); }
        public static bool IsOverlap(AABB aabb, Capsule capsule) { return SeparatingAxisTest(aabb, capsule); }


        public static fix3 CollidePoint(AABB aabb, Sphere sphere)
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

        public static bool IsOverlap(AABB aabb, Ray ray)
        {
            fix min;
            fix max;
            for (var i = 0; i < 3; i++)
            {
                min = fix.Min;
                max = fix.Max;
                fix t0 = math.min((aabb.Min[i] - ray.origin[i]) / ray.direction[i],
                    (aabb.Max[i] - ray.origin[i]) / ray.direction[i]);
                fix t1 = math.max((aabb.Min[i] - ray.origin[i]) / ray.direction[i],
                    (aabb.Max[i] - ray.origin[i]) / ray.direction[i]);
                min = math.max(t0, min);
                max = math.min(t1, max);
                if (max <= min)
                    return false;
            }
            return true;
        }

        public static AABB MergeAABB(AABB a, AABB b)
        {
            fix3 min = new fix3(math.min(a.Min.x, b.Min.x), math.min(a.Min.y, b.Min.y), math.min(a.Min.z, b.Min.z));
            fix3 max = new fix3(math.max(a.Max.x, b.Max.x), math.max(a.Max.y, b.Max.y), math.max(a.Max.z, b.Max.z));
            return new AABB(min, max);
        }
    }

}
