using System;

namespace Mathematica
{
    public static partial class Geometry
    {

        /// Determine the signed angle between two vectors, with normal 'n' as the rotation axis.
        /// ��������֮��ļнǣ��з���. ˳ʱ��Ϊ�� [0,180] ��ʱ��Ϊ��(0,180)
        public static fix AngleSigned(fix3 from, fix3 to, fix3 n)
        {
            return math.atan2(math.dot(n, math.cross(from, to)), math.dot(from, to)) * math.Rad2Deg;
        }

        /// Determine the signed angle between two vectors
        /// ��������֮��ļнǣ��޷��� [0,180]
        public static fix kEpsilon = fix._0_00001;
        public static fix kEpsilonNormalSqrt = 1e-15F;
        public static fix Angle(fix3 from, fix3 to)
        {
            fix denominator = math.sqrt(math.lengthsq(from) * math.lengthsq(to));
            if (denominator < kEpsilonNormalSqrt)
                return 0F;

            float dot = math.clamp(math.dot(from, to) / denominator, -fix._1, fix._1);
            //fix dot = math.dot(v1, v2);
            //if (dot == 0)
            //    return 0;
            return math.acos(dot) * math.Rad2Deg;
        }


        //��point��OA��OB��ɵļн�����
        public static bool IsPointInAngle(fix3 point, fix3 O, fix3 A, fix3 B)
        {
            fix3 p = point - O;
            fix3 a = A - O;
            fix3 b = B - O;
            fix dot1 = math.dot(a, p);
            fix dot2 = math.dot(b, p);

            return dot1 * dot2 <= 0;
        }

        //����ͬһ����������ɵļн�����
        public static bool IsPointInBetweenTheRays(fix3 point, Ray ray1, Ray ray2)
        {
            fix3 p = point - ray1.origin;
            fix dot1 = math.dot(ray1.direction, p);
            fix dot2 = math.dot(ray2.direction, p);

            return dot1 * dot2 <= 0;
        }
    }

}

