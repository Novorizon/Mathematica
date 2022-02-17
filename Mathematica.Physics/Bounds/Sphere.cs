using System;

namespace Mathematica
{
    [Serializable]
    public struct Sphere : Bounds
    {
        public fix3 Center { get; private set; }
        public fix Radius { get; private set; }
        public fix Radius2 { get; private set; }
        public AABB Bounds { get; private set; }
        fix3 R;

        public Sphere(fix3 center, fix radius)
        {
            Center = center;
            Radius = radius;
            Radius2 = radius * radius;
            R = new fix3(Radius, Radius, Radius);
            Bounds = new AABB(Center - R, Center + R);
        }

        public void Update(fix3 center)
        {
            Center = center;
            Bounds = new AABB(Center - R, Center + R);
        }
    }


}

