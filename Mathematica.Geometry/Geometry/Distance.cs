using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using Mathematica;

namespace Mathematica
{
    public static partial class Geometry
    {
        //�㵽a,b��ʾ��ֱ�ߵľ���
        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="a">ֱ����һ��</param>
        /// <param name="b">ֱ������һ��</param>
        /// <returns></returns>
        public static fix PointToLineDistance(fix3 point, fix3 a, fix3 b)
        {
            fix3 m = a - point;
            fix3 n = a - b;
            if (n == fix3.zero)
                return fix.Min;

            fix d = math.length(math.cross(m, n)) / math.length(n);
            return d;
        }

        // Returns the squared distance between point  and segment ab .Chapter 5 Basic Primitive Tests
        public static fix PointToSegmentDistanceSq(fix3 point, fix3 a, fix3 b)
        {
            fix3 ab = b - a;
            fix3 ac = point - a;
            fix3 bc = point - b;
            fix e = math.dot(ac, ab);
            // Handle cases where c projects outside ab
            if (e <= 0) return math.dot(ac, ac);
            fix f = math.dot(ab, ab);
            if (e >= f) return math.dot(bc, bc);
            // Handle cases where c projects onto ab
            return math.dot(ac, ac) - e * e / f;
        }


        //�㵽a,b�߶εľ���
        public static fix PointToSegmentDistance(fix3 point, fix3 a, fix3 b)
        {
            fix3 m1 = a - point;
            fix3 m2 = b - point;
            fix3 n = a - b;
            if (n == fix3.zero)
                return fix.NaN;

            if (math.dot(m1, a) < 0)
                return math.length(m1);
            if (math.dot(m2, a) > 0)
                return math.length(m2);
            return math.length(math.cross(m1, n)) / math.length(n);
        }

        //�㵽ƽ��ľ���
        public static fix PointToPlaneDistance(fix3 point, fix3 pointPlane, fix3 normal)
        {
            fix3 vector = point - pointPlane;
            fix3 prj = math.project(vector, normal);

            return math.length(prj);
        }
    }
}
