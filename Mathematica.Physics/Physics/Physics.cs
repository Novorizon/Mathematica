namespace Mathematica.Physics
{
    public static partial class Physics
    {

        // 1000次装拆箱造成的额外耗时10ms,只在测试时使用
        public static bool IsOverlap(Bounds a, Bounds b)
        {
            if (a is AABB a0)
            {
                if (b is AABB aabb)
                {
                    return Geometry.IsOverlap(a0, aabb);

                }
                else if (b is OBB obb)
                {
                    return Geometry.IsOverlap(a0, obb);

                }
                else if (b is Sphere sphere)
                {
                    return Geometry.IsOverlap(a0, sphere);

                }
                else if (b is Capsule capsule)
                {
                    return Geometry.IsOverlap(a0, capsule);

                }
            }
            else if (a is OBB a1)
            {
                if (b is AABB aabb)
                {
                    return Geometry.IsOverlap(a1, aabb);

                }
                else if (b is OBB obb)
                {
                    return Geometry.IsOverlap(a1, obb);

                }
                else if (b is Sphere sphere)
                {
                    return Geometry.IsOverlap(a1, sphere);

                }
                else if (b is Capsule capsule)
                {
                    return Geometry.IsOverlap(a1, capsule);

                }
            }
            else if (a is Sphere a2)
            {
                if (b is AABB aabb)
                {
                    return Geometry.IsOverlap(a2, aabb);

                }
                else if (b is OBB obb)
                {
                    return Geometry.IsOverlap(a2, obb);

                }
                else if (b is Sphere sphere)
                {
                    return Geometry.IsOverlap(a2, sphere);

                }
                else if (b is Capsule capsule)
                {
                    return Geometry.IsOverlap(a2, capsule);

                }
            }
            else if (a is Capsule a3)
            {
                if (b is AABB aabb)
                {
                    return Geometry.IsOverlap(a3, aabb);

                }
                else if (b is OBB obb)
                {
                    return Geometry.IsOverlap(a3, obb);

                }
                else if (b is Sphere sphere)
                {
                    return Geometry.IsOverlap(a3, sphere);

                }
                else if (b is Capsule capsule)
                {
                    return Geometry.IsOverlap(a3, capsule);

                }
            }
            return false;
        }

        //public static bool IsOverlap<T1,T2>(T1 a, T2 b) where T1  : Bounds.Bounds where T2 : Bounds.Bounds
        //{
        //    return IsOverlap(a, b);
        //    return false;
        //}
        public static bool Raycast(Ray ray, out RaycastHit hitInfo, fix maxDistance, int layerMask)
        {
            hitInfo = new RaycastHit();
            fix dis = 0;
            while (dis < maxDistance)
            {
                //hitInfo.point = geometry.IsOverlap(new AABB(), ray.origin);
                if (hitInfo.point != fix3.MinValue)
                {
                    return true;
                }
                dis += fix._0_1;
                ray.origin += dis * ray.direction;
            }
            return false;
        }

        public static bool Raycast(fix3 origin, fix3 direction, out RaycastHit hitInfo, fix maxDistance, int layerMask)
        {
            hitInfo = new RaycastHit();
            fix dis = 0;
            while (dis < maxDistance)
            {
                //hitInfo.point = geometry.IsOverlap(new Cuboid(), origin);
                if (hitInfo.point != fix3.MinValue)
                {
                    return true;
                }
                dis += fix._0_1;
                origin += dis * direction;
            }
            return false;
        }
    }
}