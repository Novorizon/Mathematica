using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using Mathematica;

namespace Mathematica
{
    public static partial class Geometry
    {
        public static bool IsConvex(Polygon polygon)
        {
            for (int i = 0; i < polygon.edges.Length; i++)
            {
                if (math.cross(polygon.edges[i], polygon.edges[(i + 1) % polygon.edges.Length]) * math.cross(polygon.edges[(i + 1) % polygon.edges.Length], polygon.edges[(i + 2) % polygon.edges.Length]) <= 0)
                    return false;
            }
            return true;
        }


        public static bool IsOverlap(Circular c1, Circular c2)
        {
            fix dis = math.distancesq(c1.center, c2.center);
            if (dis <= (c1.radius + c2.radius) * (c1.radius + c2.radius))
            {
                return true;
            }
            return false;
        }

        //点到a,b表示的直线的距离
        public static fix PointToLineDistance(fix2 point, fix2 a, fix2 b)
        {
            fix2 m = a - point;
            fix2 n = a - b;
            return PointToLineDistance(new fix3(point.x, point.y, fix._0), new fix3(m.x, m.y, fix._0), new fix3(n.x, n.y, fix._0));
        }



        /// 二次贝塞尔
        public static fix2 Bezier2(fix2 p0, fix2 p1, fix2 p2, fix t)
        {
            return (1 - t) * ((1 - t) * p0 + t * p1) + t * ((1 - t) * p1 + t * p2);
        }

        /// 三次贝塞尔
        public static fix2 Bezier3(fix2 p0, fix2 p1, fix2 p2, fix2 p3, fix t)
        {
            return (1 - t) * ((1 - t) * ((1 - t) * p0 + t * p1) + t * ((1 - t) * p1 + t * p2)) + t * ((1 - t) * ((1 - t) * p1 + t * p2) + t * ((1 - t) * p2 + t * p3));
        }
    }
}
