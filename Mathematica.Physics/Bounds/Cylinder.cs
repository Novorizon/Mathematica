

namespace Mathematica
{
    public struct Cylinder
    {
        public fix3 Center { get; private set; }
        public fix Radius { get; private set; }
        public fix Height { get; private set; }
        public quaternion Orientation { get; private set; }
        public fix3 Axis { get; private set; }

        public fix Radius2 { get; private set; }
        public fix3 Center1 { get; private set; }
        public fix3 Center2 { get; private set; }
    }
}



