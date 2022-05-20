using Mathematica.Physics;
using System;
using System.Collections.Generic;
using System.Linq;
namespace Mathematica
{
    public static class BVHTree
    {
        public static Node Root;
        public static Node[] nodes = new Node[1024];
        public static int count;
        static public void Init(int n)
        {
            count = n;
            for (int i = 0; i < count; i++)
            {
                nodes[i] = new Node();
            };
        }
        static public void BuildTree(AABB[] list, int n)
        {
            for (int i = 0; i < n; i++)
            {
                nodes[i].aabb = list[i];
            };

            List<Node> temp_list = nodes.ToList();

            //随机一个轴。 x轴:0 y轴:1 z轴:2
            int axis = (int)(3 * UnityEngine.Random.Range(0, 3) % 3);
            temp_list.Sort((a, b) => Compare(a, b, axis));
            nodes = temp_list.ToArray();
            //Array.Sort(list, new Comparison<Node>((a, b) => a.aabb.Min[axis].CompareTo(b.aabb.Min[axis])));
            Root = new Node(nodes, n);
        }
        static public long Hit(fix3 a, RaycastHit hit)
        {
            Node.count = 0;
            Root.Hit(a, out long id);

            return id;
        }
        static int Compare(Node a, Node b, int i)
        {
            return a.aabb.Min[i] - b.aabb.Min[i] < 0 ? -1 : 1;
        }
    }

    public class Node
    {
        public static int count = 0;
        static Node[] leftNodes = new Node[1024], rightNodes = new Node[1024];
        public bool isLeaf;
        public Node left;
        public Node right;
        public AABB aabb;
        public Node()
        {
        }


        Node[] SplitArray(Node[] Source, int StartIndex, int EndIndex)
        {
            Node[] result = new Node[EndIndex - StartIndex + 1];
            for (var i = 0; i <= EndIndex - StartIndex; i++)
                result[i] = Source[i + StartIndex];
            return result;
        }
        //1000个物体 改用静态变量 从1.6ms减少到1.5ms
        void SplitArray1(Node[] Source, int StartIndex, int EndIndex)
        {
            for (var i = 0; i <= EndIndex - StartIndex; i++)
                leftNodes[i] = Source[i + StartIndex];
        }
        void SplitArray2(Node[] Source, int StartIndex, int EndIndex)
        {
            for (var i = 0; i <= EndIndex - StartIndex; i++)
                rightNodes[i] = Source[i + StartIndex];
        }
        public Node(Node[] list, int n)
        {
            //QSort(ref list, 0, n - 1);
            //检测当前子节点数量，如果大于2则继续分割。
            switch (n)
            {
                case 1:
                    left = right = list[0];
                    left.isLeaf = true;
                    right.isLeaf = true;
                    break;
                case 2:
                    left = list[0];
                    right = list[1];
                    left.isLeaf = true;
                    right.isLeaf = true;
                    break;
                default: //拆分 
                    SplitArray1(list, 0, n / 2 - 1);
                    SplitArray2(list, n / 2, n - 1);
                    //left = new Node(SplitArray(list, 0, n / 2 - 1), n / 2);
                    //right = new Node(SplitArray(list, n / 2, n - 1), n - n / 2);
                    left = new Node(leftNodes, n / 2);
                    right = new Node(rightNodes, n - n / 2);
                    break;
            }
            aabb = new AABB(left.aabb, right.aabb);

        }

        public bool Hit(fix3 a, out long id)
        {
            count++;
            id = 0;
            //检测包围和碰撞，返回碰撞的子树的信息
            if (Geometry.IsOverlap( aabb, a))
            {
                if (isLeaf)
                {
                    return true;
                }
                bool isHitLeft = false;
                bool isHitRight = false;
                if (left != null)
                {
                    isHitLeft = left.Hit(a, out long leftHit);
                    if (isHitLeft)
                    {
                        id = leftHit;
                        return true;
                    }
                }
                if (right != null)
                {
                    isHitRight = right.Hit(a, out long rightHit);
                    if (isHitRight)
                    {
                        id = rightHit;
                        return true;
                    }
                }

                //if (isHitLeft && isHitRight)
                //{
                //    hit = leftHit.t < rightHit.t ? leftHit : rightHit;
                //    return true;
                //}
                return false;
            }
            return false;
        }
        public static void QSort(ref Node[] array, int low, int high)
        {
            if (low >= high)
                return;
            //完成一次单元排序
            int index = Sort(array, low, high);
            //递归调用，对左边部分的数组进行单元排序
            QSort(ref array, low, index - 1);
            //递归调用，对右边部分的数组进行单元排序
            QSort(ref array, index + 1, high);
        }

        public static int Sort(Node[] array, int low, int high)
        {
            int axis = UnityEngine.Random.Range(0, 3);
            Node tem = array[low];
            int key = array[low].aabb.Min[axis];//基准数
            while (low < high)
            {
                //从high往前找小于或等于key的值
                while (low < high && array[high].aabb.Min[axis] > key)
                    high--;
                //比key小开等的放左边
                array[low] = array[high];
                //从low往后找大于key的值
                while (low < high && array[low].aabb.Min[axis] <= key)
                    low++;
                //比key大的放右边
                array[high] = array[low];
            }
            //结束循环时，此时low等于high，左边都小于或等于key，右边都大于key。将key放在游标当前位置。 
            //array[low]= key;
            array[low] = tem;
            return high;
        }
    }


    //    public bool Hit(AABB a, out Node node)
    //    {
    //        node = this;

    //        if (Geometry.IsOverlap(a, aabb))
    //        {

    //            bool isHitLeft = left.Hit(a, out Node leftHit);
    //            bool isHitRight = right.Hit(a, out Node rightHit);

    //            //if (isHitLeft && isHitRight)
    //            //{
    //            //    hit = leftHit.t < rightHit.t ? leftHit : rightHit;
    //            //    return true;
    //            //}
    //            if (isHitLeft)
    //            {
    //                node = leftHit;
    //                return true;
    //            }
    //            if (isHitRight)
    //            {
    //                node = rightHit;
    //                return true;
    //            }
    //            return false;
    //        }
    //        return false;
    //    }
    //}
}
