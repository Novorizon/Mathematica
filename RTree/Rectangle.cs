
using Mathematica;
using System;

namespace RTree
{
    public class Rectangle
    {
        public Rectangle()
        {
        }

        public fix[] min;
        public fix[] max;
        public fix[] BevelRadius;

        public void Update(fix3 center)
        {
            min[0] = center.x - BevelRadius[0];
            min[1] = center.y - BevelRadius[1];
            min[2] = center.z - BevelRadius[2];
            max[0] = center.x + BevelRadius[0];
            max[1] = center.y + BevelRadius[1];
            max[2] = center.z + BevelRadius[2];
        }
        public Rectangle(fix x1, fix y1, fix z1, fix x2, fix y2, fix z2)
        {
            min = new fix[3];
            max = new fix[3];
            set(x1, y1, x2, y2, z1, z2);
        }
        public Rectangle(fix[] min, fix[] max)
        {
            this.min = new fix[3];
            this.max = new fix[3];

            set(min, max);
        }

        public static implicit operator Rectangle(AABB aabb) { return new Rectangle(aabb.Min.x, aabb.Min.y, aabb.Min.z, aabb.Max.x, aabb.Max.y, aabb.Max.z); }
        internal void set(fix x1, fix y1, fix x2, fix y2, fix z1, fix z2)
        {
            min[0] = x1;
            min[1] = y1;
            min[2] = z1;
            max[0] = x2;
            max[1] = y2;
            max[2] = z2;
        }

        internal void set(fix[] min, fix[] max)
        {
            Array.Copy(min, 0, this.min, 0, 3);
            Array.Copy(max, 0, this.max, 0, 3);
        }

        internal Rectangle copy()
        {
            return new Rectangle(min, max);
        }

        internal bool edgeOverlaps(Rectangle r)
        {
            for (int i = 0; i < 3; i++)
            {
                if (min[i] == r.min[i] || max[i] == r.max[i])
                {
                    return true;
                }
            }
            return false;
        }

        internal bool intersects(Rectangle r)
        {
            for (int i = 0; i < 3; i++)
            {
                if (max[i] < r.min[i] || min[i] > r.max[i])
                {
                    return false;
                }
            }
            return true;
        }

        internal bool contains(Rectangle r)
        {
            for (int i = 0; i < 3; i++)
            {
                if (max[i] < r.max[i] || min[i] > r.min[i])
                {
                    return false;
                }
            }
            return true;
        }

        internal bool containedBy(Rectangle r)
        {
            for (int i = 0; i < 3; i++)
            {
                if (max[i] > r.max[i] || min[i] < r.min[i])
                {
                    return false;
                }
            }
            return true;
        }

        internal fix distance(fix3 p)
        {
            fix distanceSquared = 0;
            for (int i = 0; i < 3; i++)
            {
                fix greatestMin = math.max(min[i], p[i]);
                fix leastMax = math.min(max[i], p[i]);
                if (greatestMin > leastMax)
                {
                    distanceSquared += ((greatestMin - leastMax) * (greatestMin - leastMax));
                }
            }
            return (fix)Math.Sqrt(distanceSquared);
        }

        internal fix distance(Rectangle r)
        {
            fix distanceSquared = 0;
            for (int i = 0; i < 3; i++)
            {
                fix greatestMin = math.max(min[i], r.min[i]);
                fix leastMax = math.min(max[i], r.max[i]);
                if (greatestMin > leastMax)
                {
                    distanceSquared += ((greatestMin - leastMax) * (greatestMin - leastMax));
                }
            }
            return math.sqrt(distanceSquared);
        }

        internal fix distanceSquared(int dimension, fix point)
        {
            fix distanceSquared = 0;
            fix tempDistance = point - max[dimension];
            for (int i = 0; i < 2; i++)
            {
                if (tempDistance > 0)
                {
                    distanceSquared = (tempDistance * tempDistance);
                    break;
                }
                tempDistance = min[dimension] - point;
            }
            return distanceSquared;
        }

        internal fix furthestDistance(Rectangle r)
        {
            fix distanceSquared = 0;

            for (int i = 0; i < 3; i++)
            {
                distanceSquared += math.max(r.min[i], r.max[i]);
            }

            return math.sqrt(distanceSquared);
        }

        internal fix enlargement(Rectangle r)
        {
            fix enlargedArea = (math.max(max[0], r.max[0]) - math.min(min[0], r.min[0])) * (math.max(max[1], r.max[1]) - math.min(min[1], r.min[1]));

            return enlargedArea - area();
        }

        internal fix area()
        {
            return (max[0] - min[0]) * (max[1] - min[1]);
        }

        internal void add(Rectangle r)
        {
            for (int i = 0; i < 3; i++)
            {
                if (r.min[i] < min[i])
                {
                    min[i] = r.min[i];
                }
                if (r.max[i] > max[i])
                {
                    max[i] = r.max[i];
                }
            }
        }


        internal bool CompareArrays(fix[] a1, fix[] a2)
        {
            if ((a1 == null) || (a2 == null))
                return false;
            if (a1.Length != a2.Length)
                return false;

            for (int i = 0; i < a1.Length; i++)
                if (a1[i] != a2[i])
                    return false;
            return true;
        }

        public override bool Equals(object obj)
        {
            bool equals = false;
            if (obj.GetType() == typeof(Rectangle))
            {
                Rectangle r = (Rectangle)obj;
                if (CompareArrays(r.min, min) && CompareArrays(r.max, max))
                {
                    equals = true;
                }
            }
            return equals;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}