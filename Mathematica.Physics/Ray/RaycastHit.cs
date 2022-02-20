
using Mathematica;
using System;

namespace Mathematica
{
    public struct RaycastHit
    {
        public long id { get;  set; }
        public fix3 point { get; set; }
        public Bounds bounds { get; set; }
        //public fix distance { get; set; }
    }
}