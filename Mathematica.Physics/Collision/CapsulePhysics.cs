using Mathematica;

namespace Mathematica
{
    public static partial class Physics
    {
        //��������
        internal static bool IsOverlap(Capsule capsule, fix3 point)
        {
            //�ж������ĵ�1�ľ���
            if (math.distancesq(point, capsule.Center1) <= capsule.Radius2)
                return true;
            //�ж������ĵ�2�ľ���
            if (math.distancesq(point, capsule.Center2) <= capsule.Radius2)
                return true;

            //���point������(center1 - center2���ϵ�ͶӰ��λ��center1��center2֮�䣬����point����ľ���С�ڵ���Radius
            var p = math.dot(capsule.Center1 - capsule.Center2, point);
            fix2 extremePoints = ExtremeProjectPoint(capsule.Center1 - capsule.Center2, new fix3[] { capsule.Center1, capsule.Center2 });
            if (extremePoints.x <= p && extremePoints.y >= p)
            {
                if (Geometry.PointToLineDistance(point, capsule.Center1, capsule.Center2) <= capsule.Radius * capsule.Radius)
                    return true;
            }
            return false;
        }

        internal static bool IsOverlap(Capsule a, Capsule b)
        {
            //�˵����
            fix dis = (a.Radius + b.Radius) * (a.Radius + b.Radius);
            if (math.distancesq(a.Center1, b.Center1) <= dis)
                return true;
            if (math.distancesq(a.Center1, b.Center2) <= dis)
                return true;
            if (math.distancesq(a.Center2, b.Center1) <= dis)
                return true;
            if (math.distancesq(a.Center2, b.Center2) <= dis)
                return true;

            dis = a.Radius + b.Radius;
            if (math.dot(a.Center1 - b.Center2, b.Center1 - b.Center2) > 0 && math.dot(a.Center1 - b.Center1, b.Center1 - b.Center2) < 0)
            {
                if (Geometry.PointToLineDistance(a.Center1, b.Center1, b.Center2) <= dis)
                    return true;
            }
            if (math.dot(a.Center2 - b.Center2, b.Center1 - b.Center2) > 0 && math.dot(a.Center2 - b.Center1, b.Center1 - b.Center2) < 0)
            {
                if (Geometry.PointToLineDistance(a.Center2, b.Center1, b.Center2) <= dis)
                    return true;
            }
            if (math.dot(b.Center1 - a.Center2, a.Center1 - a.Center2) > 0 && math.dot(b.Center1 - a.Center1, a.Center1 - a.Center2) < 0)
            {
                if (Geometry.PointToLineDistance(b.Center1, a.Center1, a.Center2) <= dis)
                    return true;
            }
            if (math.dot(b.Center2 - a.Center2, a.Center1 - a.Center2) > 0 && math.dot(b.Center2 - a.Center1, a.Center1 - a.Center2) < 0)
            {
                if (Geometry.PointToLineDistance(b.Center2, a.Center1, a.Center2) <= dis)
                    return true;
            }

            return false;
        }
        internal static bool IsOverlapParallel(Capsule a, Capsule b)
        {
            //�˵����
            fix dis = (a.Radius + b.Radius) * (a.Radius + b.Radius);
            if (math.distancesq(a.Center1, b.Center1) <= dis)
                return true;
            if (math.distancesq(a.Center1, b.Center2) <= dis)
                return true;
            if (math.distancesq(a.Center2, b.Center1) <= dis)
                return true;
            if (math.distancesq(a.Center2, b.Center2) <= dis)
                return true;

            dis = a.Radius + b.Radius;
            if (math.dot(a.Center1 - b.Center2, b.Center1 - b.Center2) > 0 && math.dot(a.Center1 - b.Center1, b.Center1 - b.Center2) < 0)
            {
                if (Geometry.PointToLineDistance(a.Center1, b.Center1, b.Center2) <= dis)
                    return true;
            }
            if (math.dot(a.Center2 - b.Center2, b.Center1 - b.Center2) > 0 && math.dot(a.Center2 - b.Center1, b.Center1 - b.Center2) < 0)
            {
                if (Geometry.PointToLineDistance(a.Center2, b.Center1, b.Center2) <= dis)
                    return true;
            }
            if (math.dot(b.Center1 - a.Center2, a.Center1 - a.Center2) > 0 && math.dot(b.Center1 - a.Center1, a.Center1 - a.Center2) < 0)
            {
                if (Geometry.PointToLineDistance(b.Center1, a.Center1, a.Center2) <= dis)
                    return true;
            }
            if (math.dot(b.Center2 - a.Center2, a.Center1 - a.Center2) > 0 && math.dot(b.Center2 - a.Center1, a.Center1 - a.Center2) < 0)
            {
                if (Geometry.PointToLineDistance(b.Center2, a.Center1, a.Center2) <= dis)
                    return true;
            }

            return false;
        }
    }
}
