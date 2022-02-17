//using Mathematica;

//using UnityEngine;


//namespace Mathematica
//{
//    public static partial class Geometry
//    {
//        /// SeparatingAxisTest
//        public static bool SeparatingAxisTest(Polygon g0, Polygon g1)
//        {
//            for (int i = 0; i < g0.normals.Length; i++)
//            {
//                fix2 extremePoints0 = new fix2();
//                fix2 extremePoints1 = new fix2();
//                for (int j = 0; j < g0.points.Length; j++)
//                {
//                    extremePoints0 = ExtremeProjectPoint(g0.normals[i], g0.points);
//                }
//                for (int j = 0; j < g1.points.Length; j++)
//                {
//                    extremePoints1 = ExtremeProjectPoint(g1.normals[i], g1.points);
//                }
//                if (!IsOverlap(extremePoints0, extremePoints1))
//                    return false;
//            }

//            return true;
//        }

//        public static bool SeparatingAxisTest(Polygon polygon, Circular circular)
//        {
//            fix2 extremePoints0 = new fix2();
//            fix2 extremePoints1 = new fix2();
//            for (int i = 0; i < polygon.normals.Length; i++)
//            {
//                for (int j = 0; j < polygon.points.Length; j++)
//                {
//                    extremePoints0 = ExtremeProjectPoint(polygon.normals[i], polygon.points);
//                }
//                extremePoints1 = ExtremeProjectPoint(polygon.normals[i], circular);

//                if (!IsOverlap(extremePoints0, extremePoints1))
//                    return false;
//            }

//            fix2 axis = MinDistanceAxis(polygon, circular);
//            for (int i = 0; i < polygon.points.Length; i++)
//            {
//                extremePoints0 = ExtremeProjectPoint(axis, polygon.points);
//            }
//            extremePoints1 = ExtremeProjectPoint(axis, circular);
//            if (!IsOverlap(extremePoints0, extremePoints1))
//                return false;


//            return true;
//        }

//        public static fix2 ExtremeProjectPoint(fix2 axis, fix2[] points)
//        {
//            fix min = fix.Max;
//            fix max = fix.Min;
//            for (int i = 0; i < points.Length; i++)
//            {
//                var p = math.dot(axis, points[i]);
//                min = math.min(p, min);
//                max = math.max(p, max);
//            }
//            return new fix2(min, max);
//        }

//        public static fix2 ExtremeProjectPoint(fix2 axis, Circular circular)
//        {
//            fix2 a = circular.center + circular.center * math.normalize(axis);
//            fix2 b = circular.center - circular.center * math.normalize(axis);
//            fix p1 = math.dot(axis, a);
//            fix p2 = math.dot(axis, b);
//            fix min = math.min(p1, p2);
//            fix max = math.max(p1, p2);

//            return new fix2(min, max);
//        }


//        public static fix2 MinDistanceAxis(Polygon polygon, Circular circular)
//        {
//            fix distance = fix.Max;
//            fix d = fix.Max;
//            fix id = -1;
//            for (int i = 0; i < polygon.points.Length; i++)
//            {
//                d = math.distance(circular.center, polygon.points[i]);
//                if (distance > d)
//                {
//                    distance = d;
//                    id = i;
//                }
//            }
//            fix2 axis = circular.center - polygon.points[id];
//            for (int i = 0; i < polygon.edges.Length; i++)
//            {
//                fix2 m = polygon.points[i] - circular.center;
//                fix2 n = polygon.edges[i];
//                fix3 M = new fix3(m.x, m.y, fix._0);
//                fix3 N = new fix3(n.x, n.y, fix._0);
//                d = math.length(math.cross(M, N)) / math.length(N);
//                if (distance > d)
//                {
//                    distance = d;
//                    axis = polygon.edges[i];
//                }
//            }


//            return axis;
//        }


//    }
//}
