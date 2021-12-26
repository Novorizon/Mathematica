using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using Mathematica;

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

        /// 三角形是否相邻
        public static Tuple<bool, fix3, fix3> CoincideTriangle(Triangle TriangleA, Triangle TriangleB)
        {
            bool isCoincide = false;
            fix3 left = new fix3();
            fix3 right = new fix3();

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (Equals(TriangleA.points[i], TriangleB.points[(j + 1) % 3]) && Equals(TriangleA.points[(i + 1) % 3], TriangleB.points[j]))
                    {
                        left = TriangleA.points[i];
                        right = TriangleB.points[j];
                        isCoincide = true;
                        break;
                    }
                }
                if (isCoincide)
                    break;
            }

            if (isCoincide)
            {
                fix3 l = left - TriangleA.centroid;
                fix3 r = right - TriangleA.centroid;
                fix angle = AngleSigned(l, r, fix3.up);
                if (angle < 0)
                {
                    fix3 p = left;
                    left = right;
                    right = p;
                }
            }
            return new Tuple<bool, fix3, fix3>(isCoincide, left, right);
        }

        /// 二次贝塞尔
        public static fix3 Bezier2(fix3 p0, fix3 p1, fix3 p2, fix t)
        {
            return (1 - t) * ((1 - t) * p0 + t * p1) + t * ((1 - t) * p1 + t * p2);
        }

        /// 三次贝塞尔
        public static fix3 Bezier3(fix3 p0, fix3 p1, fix3 p2, fix3 p3, fix t)
        {
            return (1 - t) * ((1 - t) * ((1 - t) * p0 + t * p1) + t * ((1 - t) * p1 + t * p2)) + t * ((1 - t) * ((1 - t) * p1 + t * p2) + t * ((1 - t) * p2 + t * p3));
        }
    }
}
