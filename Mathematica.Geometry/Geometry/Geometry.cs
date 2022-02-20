using System;

namespace Mathematica
{
    public enum GeometryType
    {
        Polygon=1,
        Triangle = 2,
        Rectangle = 3,
        Hexagon=4,
        Circular=5
    }

    public static partial class Geometry
    {
        //�㵽�߶εľ���
        public static fix PointToSegmentDistance(fix3 point, Segment a)
        {
            return PointToSegmentDistance(point, a.start, a.end);
        }

        //�㵽ֱ�ߵľ���
        public static fix PointToLineDistance(fix3 point, Line a)
        {
            return PointToSegmentDistance(point, a.point, a.point+a.direction);
        }

        //�㵽�߶εľ���
        public static fix PointToPlaneDistance(fix3 point, Plane a)
        {
            return PointToPlaneDistance(point, a.point, a.normal);
        }


        /// ���α�����
        public static fix3 Bezier2(fix3 p0, fix3 p1, fix3 p2, fix t)
        {
            return (1 - t) * ((1 - t) * p0 + t * p1) + t * ((1 - t) * p1 + t * p2);
        }

        /// ���α�����
        public static fix3 Bezier3(fix3 p0, fix3 p1, fix3 p2, fix3 p3, fix t)
        {
            return (1 - t) * ((1 - t) * ((1 - t) * p0 + t * p1) + t * ((1 - t) * p1 + t * p2)) + t * ((1 - t) * ((1 - t) * p1 + t * p2) + t * ((1 - t) * p2 + t * p3));
        }
    }

}

