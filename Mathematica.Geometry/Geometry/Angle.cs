using System;

namespace Mathematica
{
    public static partial class Geometry
    {

        /// Determine the signed angle between two vectors, with normal 'n' as the rotation axis.
        /// 两个向量之间的夹角，有符号.
        public static fix AngleSigned(fix3 v1, fix3 v2, fix3 n)
        {
            return math.atan2(math.dot(n, math.cross(v1, v2)), math.dot(v1, v2)) * math.Rad2Deg;
        }

        /// Determine the signed angle between two vectors
        /// 两个向量之间的夹角，无符号
        public static fix Angle(fix3 v1, fix3 v2)
        {
            fix dot = math.dot(v1, v2);
            if (dot == 0)
                return 0;

            dot = math.clamp(math.dot(v1, v2) / dot, -fix._1, fix._1);
            return math.acos(dot) * math.Rad2Deg;
        }


        //点point在OA、OB组成的夹角内外
        public static bool IsPointInAngle(fix3 point, fix3 O, fix3 A, fix3 B)
        {
            fix3 p = point - O;
            fix3 a = A - O;
            fix3 b = B - O;
            fix dot1 = math.dot(a, p);
            fix dot2 = math.dot(b, p);

            return dot1 * dot2 <= 0;
        }

        //点在同一起点的射线组成的夹角内外
        public static bool IsPointInBetweenTheRays(fix3 point, Ray ray1, Ray ray2)
        {
            fix3 p = point - ray1.origin;
            fix dot1 = math.dot(ray1.direction, p);
            fix dot2 = math.dot(ray2.direction, p);

            return dot1 * dot2 <= 0;
        }
    }

}

