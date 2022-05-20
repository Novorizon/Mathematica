
using System.Collections.Generic;
using System;
using Mathematica;

namespace RTree
{
    public class RTree<T>
    {
        private const int DEFAULT_MAX_NODE_ENTRIES = 10;
        internal int maxNodeEntries;
        int minNodeEntries;

        private Dictionary<int, Node<T>> nodeMap = new Dictionary<int, Node<T>>();

        private const int ENTRY_STATUS_ASSIGNED = 0;
        private const int ENTRY_STATUS_UNASSIGNED = 1;
        private byte[] entryStatus = null;
        private byte[] initialEntryStatus = null;

        private Stack<int> parents = new Stack<int>();
        private Stack<int> parentsEntry = new Stack<int>();

        private int treeHeight = 1;
        private int rootNodeId = 0;
        private int msize = 0;

        // Enables creation of new nodes
        private int highestUsedNodeId = 0;

        // Deleted Node<T> objects are retained in the nodeMap, so that they can be reused. Store the IDs of nodes which can be reused.
        private Stack<int> deletedNodeIds = new Stack<int>();

        // List of nearest rectangles. Use a member variable to avoid recreating the object each time nearest() is called.
        List<int> nearestIds = new List<int>();

        private Dictionary<int, T> IdsToItems = new Dictionary<int, T>();
        private Dictionary<T, int> ItemsToIds = new Dictionary<T, int>();
        private volatile int idcounter = int.MinValue;

        //the recursion methods require a delegate to retrieve data
        private delegate void intproc(int x);

        public RTree()
        {
            init();
        }

        public RTree(int MaxNodeEntries, int MinNodeEntries)
        {
            minNodeEntries = MinNodeEntries;
            maxNodeEntries = MaxNodeEntries;
            init();
        }

        private void init()
        {
            if (maxNodeEntries < 2)
            {
                maxNodeEntries = DEFAULT_MAX_NODE_ENTRIES;
            }

            if (minNodeEntries < 1 || minNodeEntries > maxNodeEntries / 2)
            {
                minNodeEntries = maxNodeEntries / 2;
            }

            entryStatus = new byte[maxNodeEntries];
            initialEntryStatus = new byte[maxNodeEntries];

            for (int i = 0; i < maxNodeEntries; i++)
            {
                initialEntryStatus[i] = ENTRY_STATUS_UNASSIGNED;
            }

            Node<T> root = new Node<T>(rootNodeId, 1, maxNodeEntries);
            nodeMap.Add(rootNodeId, root);

        }

        public void Add(Rectangle r, T item)
        {
            idcounter++;
            int id = idcounter;

            IdsToItems.Add(id, item);
            ItemsToIds.Add(item, id);

            add(r, id);
        }

        private void add(Rectangle r, int id)
        {
            add(r, id, 1);

            msize++;
        }

        private void add(Rectangle r, int id, int level)
        {
            Node<T> n = chooseNode(r, level);
            Node<T> newLeaf = null;

            if (n.entryCount < maxNodeEntries)
            {
                n.addEntryNoCopy(r, id);
            }
            else
            {
                newLeaf = splitNode(n, r, id);
            }

            Node<T> newNode = adjustTree(n, newLeaf);

            if (newNode != null)
            {
                int oldRootNodeId = rootNodeId;
                Node<T> oldRoot = getNode(oldRootNodeId);

                rootNodeId = getNextNodeId();
                treeHeight++;
                Node<T> root = new Node<T>(rootNodeId, treeHeight, maxNodeEntries);
                root.addEntry(newNode.mbr, newNode.nodeId);
                root.addEntry(oldRoot.mbr, oldRoot.nodeId);
                nodeMap.Add(rootNodeId, root);
            }
        }

        public bool Delete(Rectangle r, T item)
        {
            int id = ItemsToIds[item];

            bool success = delete(r, id);
            if (success == true)
            {
                IdsToItems.Remove(id);
                ItemsToIds.Remove(item);
            }
            return success;
        }

        private bool delete(Rectangle r, int id)
        {
            parents.Clear();
            parents.Push(rootNodeId);

            parentsEntry.Clear();
            parentsEntry.Push(-1);
            Node<T> n = null;
            int foundIndex = -1;  // index of entry to be deleted in leaf

            while (foundIndex == -1 && parents.Count > 0)
            {
                n = getNode(parents.Peek());
                int startIndex = parentsEntry.Peek() + 1;

                if (!n.isLeaf())
                {
                    bool contains = false;
                    for (int i = startIndex; i < n.entryCount; i++)
                    {
                        if (n.entries[i].contains(r))
                        {
                            parents.Push(n.ids[i]);
                            parentsEntry.Pop();
                            parentsEntry.Push(i); // this becomes the start index when the child has been searched
                            parentsEntry.Push(-1);
                            contains = true;
                            break; // ie go to next iteration of while()
                        }
                    }
                    if (contains)
                    {
                        continue;
                    }
                }
                else
                {
                    foundIndex = n.findEntry(r, id);
                }

                parents.Pop();
                parentsEntry.Pop();
            } // while not found

            if (foundIndex != -1)
            {
                n.deleteEntry(foundIndex, minNodeEntries);
                condenseTree(n);
                msize--;
            }

            Node<T> root = getNode(rootNodeId);
            while (root.entryCount == 1 && treeHeight > 1)
            {
                root.entryCount = 0;
                rootNodeId = root.ids[0];
                treeHeight--;
                root = getNode(rootNodeId);
            }

            return (foundIndex != -1);
        }

        public List<T> Nearest(fix3 p, fix furthestDistance)
        {
            List<T> retval = new List<T>();
            nearest(p, delegate (int id)
            {
                retval.Add(IdsToItems[id]);
            }, furthestDistance);
            return retval;
        }


        private void nearest(fix3 p, intproc v, fix furthestDistance)
        {
            Node<T> rootNode = getNode(rootNodeId);

            nearest(p, rootNode, furthestDistance);

            foreach (int id in nearestIds)
                v(id);
            nearestIds.Clear();
        }

        public List<T> Intersects(Rectangle r)
        {
            List<T> retval = new List<T>();
            intersects(r, delegate (int id)
            {
                retval.Add(IdsToItems[id]);
            });
            return retval;
        }


        private void intersects(Rectangle r, intproc v)
        {
            Node<T> rootNode = getNode(rootNodeId);
            intersects(r, v, rootNode);
        }

        public List<T> Contains(Rectangle r)
        {
            List<T> retval = new List<T>();
            contains(r, delegate (int id)
            {
                retval.Add(IdsToItems[id]);
            });

            return retval;
        }

        private void contains(Rectangle r, intproc v)
        {
            parents.Clear();
            parents.Push(rootNodeId);

            parentsEntry.Clear();
            parentsEntry.Push(-1);


            while (parents.Count > 0)
            {
                Node<T> n = getNode(parents.Peek());
                int startIndex = parentsEntry.Peek() + 1;

                if (!n.isLeaf())
                {
                    bool intersects = false;
                    for (int i = startIndex; i < n.entryCount; i++)
                    {
                        if (r.intersects(n.entries[i]))
                        {
                            parents.Push(n.ids[i]);
                            parentsEntry.Pop();
                            parentsEntry.Push(i); // this becomes the start index when the child has been searched
                            parentsEntry.Push(-1);
                            intersects = true;
                            break; // ie go to next iteration of while()
                        }
                    }
                    if (intersects)
                    {
                        continue;
                    }
                }
                else
                {
                    for (int i = 0; i < n.entryCount; i++)
                    {
                        if (r.contains(n.entries[i]))
                        {
                            v(n.ids[i]);
                        }
                    }
                }
                parents.Pop();
                parentsEntry.Pop();
            }
        }

        public Rectangle getBounds()
        {
            Rectangle bounds = null;

            Node<T> n = getNode(getRootNodeId());
            if (n != null && n.getMBR() != null)
            {
                bounds = n.getMBR().copy();
            }
            return bounds;
        }

        private int getNextNodeId()
        {
            int nextNodeId = 0;
            if (deletedNodeIds.Count > 0)
            {
                nextNodeId = deletedNodeIds.Pop();
            }
            else
            {
                nextNodeId = 1 + highestUsedNodeId++;
            }
            return nextNodeId;
        }


        private Node<T> getNode(int index)
        {
            return (Node<T>)nodeMap[index];
        }

        private int getHighestUsedNodeId()
        {
            return highestUsedNodeId;
        }

        public int getRootNodeId()
        {
            return rootNodeId;
        }

        private Node<T> splitNode(Node<T> n, Rectangle newRect, int newId)
        {
            fix initialArea = 0;

            System.Array.Copy(initialEntryStatus, 0, entryStatus, 0, maxNodeEntries);

            Node<T> newNode = null;
            newNode = new Node<T>(getNextNodeId(), n.level, maxNodeEntries);
            nodeMap.Add(newNode.nodeId, newNode);

            pickSeeds(n, newRect, newId, newNode);

            while (n.entryCount + newNode.entryCount < maxNodeEntries + 1)
            {
                if (maxNodeEntries + 1 - newNode.entryCount == minNodeEntries)
                {
                    for (int i = 0; i < maxNodeEntries; i++)
                    {
                        if (entryStatus[i] == ENTRY_STATUS_UNASSIGNED)
                        {
                            entryStatus[i] = ENTRY_STATUS_ASSIGNED;
                            n.mbr.add(n.entries[i]);
                            n.entryCount++;
                        }
                    }
                    break;
                }
                if (maxNodeEntries + 1 - n.entryCount == minNodeEntries)
                {
                    for (int i = 0; i < maxNodeEntries; i++)
                    {
                        if (entryStatus[i] == ENTRY_STATUS_UNASSIGNED)
                        {
                            entryStatus[i] = ENTRY_STATUS_ASSIGNED;
                            newNode.addEntryNoCopy(n.entries[i], n.ids[i]);
                            n.entries[i] = null;
                        }
                    }
                    break;
                }

                pickNext(n, newNode);
            }

            n.reorganize(this);
            return newNode;
        }

        private void pickSeeds(Node<T> n, Rectangle newRect, int newId, Node<T> newNode)
        {
            fix maxNormalizedSeparation = 0;
            int highestLowIndex = 0;
            int lowestHighIndex = 0;

            n.mbr.add(newRect);

            for (int d = 0; d < 3; d++)
            {
                fix tempHighestLow = newRect.min[d];
                int tempHighestLowIndex = -1;

                fix tempLowestHigh = newRect.max[d];
                int tempLowestHighIndex = -1;

                for (int i = 0; i < n.entryCount; i++)
                {
                    fix tempLow = n.entries[i].min[d];
                    if (tempLow >= tempHighestLow)
                    {
                        tempHighestLow = tempLow;
                        tempHighestLowIndex = i;
                    }
                    else
                    {  // ensure that the same index cannot be both lowestHigh and highestLow
                        fix tempHigh = n.entries[i].max[d];
                        if (tempHigh <= tempLowestHigh)
                        {
                            tempLowestHigh = tempHigh;
                            tempLowestHighIndex = i;
                        }
                    }

                    // PS2 [Adjust for shape of the rectangle cluster] Normalize the separations
                    // by dividing by the widths of the entire set along the corresponding
                    // dimension
                    fix normalizedSeparation = (tempHighestLow - tempLowestHigh) / (n.mbr.max[d] - n.mbr.min[d]);

                    // PS3 [Select the most extreme pair] Choose the pair with the greatest
                    // normalized separation along any dimension.
                    if (normalizedSeparation > maxNormalizedSeparation)
                    {
                        maxNormalizedSeparation = normalizedSeparation;
                        highestLowIndex = tempHighestLowIndex;
                        lowestHighIndex = tempLowestHighIndex;
                    }
                }
            }

            // highestLowIndex is the seed for the new node.
            if (highestLowIndex == -1)
            {
                newNode.addEntry(newRect, newId);
            }
            else
            {
                newNode.addEntryNoCopy(n.entries[highestLowIndex], n.ids[highestLowIndex]);
                n.entries[highestLowIndex] = null;

                // move the new rectangle into the space vacated by the seed for the new node
                n.entries[highestLowIndex] = newRect;
                n.ids[highestLowIndex] = newId;
            }

            // lowestHighIndex is the seed for the original node. 
            if (lowestHighIndex == -1)
            {
                lowestHighIndex = highestLowIndex;
            }

            //if (lowestHighIndex >= entryStatus.Length)
            //{
            //    int a = 0;
            //}
            entryStatus[lowestHighIndex] = ENTRY_STATUS_ASSIGNED;
            n.entryCount = 1;
            n.mbr.set(n.entries[lowestHighIndex].min, n.entries[lowestHighIndex].max);
        }




        /// <summary>
        /// Pick the next entry to be assigned to a group during a Node&lt;T&gt; split.
        /// [Determine cost of putting each entry in each group] For each 
        /// entry not yet in a group, calculate the area increase required
        /// in the covering rectangles of each group  
        /// </summary>
        /// <param name="n"></param>
        /// <param name="newNode"></param>
        /// <returns></returns>
        private int pickNext(Node<T> n, Node<T> newNode)
        {
            fix maxDifference = fix.Min;
            int next = 0;
            int nextGroup = 0;

            maxDifference = fix.Min;

            for (int i = 0; i < maxNodeEntries; i++)
            {
                if (entryStatus[i] == ENTRY_STATUS_UNASSIGNED)
                {

                    fix nIncrease = n.mbr.enlargement(n.entries[i]);
                    fix newNodeIncrease = newNode.mbr.enlargement(n.entries[i]);
                    fix difference = Math.Abs(nIncrease - newNodeIncrease);

                    if (difference > maxDifference)
                    {
                        next = i;

                        if (nIncrease < newNodeIncrease)
                        {
                            nextGroup = 0;
                        }
                        else if (newNodeIncrease < nIncrease)
                        {
                            nextGroup = 1;
                        }
                        else if (n.mbr.area() < newNode.mbr.area())
                        {
                            nextGroup = 0;
                        }
                        else if (newNode.mbr.area() < n.mbr.area())
                        {
                            nextGroup = 1;
                        }
                        else if (newNode.entryCount < maxNodeEntries / 2)
                        {
                            nextGroup = 0;
                        }
                        else
                        {
                            nextGroup = 1;
                        }
                        maxDifference = difference;
                    }
                }
            }

            entryStatus[next] = ENTRY_STATUS_ASSIGNED;

            if (nextGroup == 0)
            {
                n.mbr.add(n.entries[next]);
                n.entryCount++;
            }
            else
            {
                // move to new node.
                newNode.addEntryNoCopy(n.entries[next], n.ids[next]);
                n.entries[next] = null;
            }

            return next;
        }


        /// <summary>
        /// Recursively searches the tree for the nearest entry. Other queries
        /// call execute() on an IntProcedure when a matching entry is found; 
        /// however nearest() must store the entry Ids as it searches the tree,
        /// in case a nearer entry is found.
        /// Uses the member variable nearestIds to store the nearest
        /// entry IDs.
        /// </summary>
        /// <remarks>TODO rewrite this to be non-recursive?</remarks>
        /// <param name="p"></param>
        /// <param name="n"></param>
        /// <param name="nearestDistance"></param>
        /// <returns></returns>
        private fix nearest(fix3 p, Node<T> n, fix nearestDistance)
        {
            for (int i = 0; i < n.entryCount; i++)
            {
                fix tempDistance = n.entries[i].distance(p);
                if (n.isLeaf())
                { // for leaves, the distance is an actual nearest distance 
                    if (tempDistance < nearestDistance)
                    {
                        nearestDistance = tempDistance;
                        nearestIds.Clear();
                    }
                    if (tempDistance <= nearestDistance)
                    {
                        nearestIds.Add(n.ids[i]);
                    }
                }
                else
                { // for index nodes, only go into them if they potentially could have
                    // a rectangle nearer than actualNearest
                    if (tempDistance <= nearestDistance)
                    {
                        // search the child node
                        nearestDistance = nearest(p, getNode(n.ids[i]), nearestDistance);
                    }
                }
            }
            return nearestDistance;
        }


        /// <summary>
        /// Recursively searches the tree for all intersecting entries.
        /// Immediately calls execute() on the passed IntProcedure when 
        /// a matching entry is found.
        /// [x] TODO rewrite this to be non-recursive? Make sure it
        /// doesn't slow it down.
        /// </summary>
        /// <param name="r"></param>
        /// <param name="v"></param>
        /// <param name="n"></param>
        private void intersects(Rectangle r, intproc v, Node<T> n)
        {
            for (int i = 0; i < n.entryCount; i++)
            {
                if (r.intersects(n.entries[i]))
                {
                    if (n.isLeaf())
                    {
                        v(n.ids[i]);
                    }
                    else
                    {
                        Node<T> childNode = getNode(n.ids[i]);
                        intersects(r, v, childNode);
                    }
                }
            }
        }


        private Rectangle oldRectangle = new Rectangle(0, 0, 0, 0, 0, 0);
        private void condenseTree(Node<T> l)
        {
            Node<T> n = l;
            Node<T> parent = null;
            int parentEntry = 0;

            Stack<int> eliminatedNodeIds = new Stack<int>();

            while (n.level != treeHeight)
            {
                parent = getNode(parents.Pop());
                parentEntry = parentsEntry.Pop();

                if (n.entryCount < minNodeEntries)
                {
                    parent.deleteEntry(parentEntry, minNodeEntries);
                    eliminatedNodeIds.Push(n.nodeId);
                }
                else
                {

                    if (!n.mbr.Equals(parent.entries[parentEntry]))
                    {
                        oldRectangle.set(parent.entries[parentEntry].min, parent.entries[parentEntry].max);
                        parent.entries[parentEntry].set(n.mbr.min, n.mbr.max);
                        parent.recalculateMBR(oldRectangle);
                    }
                }

                n = parent;
            }

            while (eliminatedNodeIds.Count > 0)
            {
                Node<T> e = getNode(eliminatedNodeIds.Pop());
                for (int j = 0; j < e.entryCount; j++)
                {
                    add(e.entries[j], e.ids[j], e.level);
                    e.entries[j] = null;
                }
                e.entryCount = 0;
                deletedNodeIds.Push(e.nodeId);
            }
        }

        private Node<T> chooseNode(Rectangle r, int level)
        {
            Node<T> n = getNode(rootNodeId);
            parents.Clear();
            parentsEntry.Clear();

            while (true)
            {
                if (n.level == level)
                {
                    return n;
                }

                fix leastEnlargement = n.getEntry(0).enlargement(r);
                int index = 0; // index of rectangle in subtree
                for (int i = 1; i < n.entryCount; i++)
                {
                    Rectangle tempRectangle = n.getEntry(i);
                    fix tempEnlargement = tempRectangle.enlargement(r);
                    if ((tempEnlargement < leastEnlargement) ||
                        ((tempEnlargement == leastEnlargement) &&
                         (tempRectangle.area() < n.getEntry(index).area())))
                    {
                        index = i;
                        leastEnlargement = tempEnlargement;
                    }
                }

                parents.Push(n.nodeId);
                parentsEntry.Push(index);

                n = getNode(n.ids[index]);
            }
        }

        private Node<T> adjustTree(Node<T> n, Node<T> nn)
        {
            while (n.level != treeHeight)
            {
                Node<T> parent = getNode(parents.Pop());
                int entry = parentsEntry.Pop();

                if (!parent.entries[entry].Equals(n.mbr))
                {
                    parent.entries[entry].set(n.mbr.min, n.mbr.max);
                    parent.mbr.set(parent.entries[0].min, parent.entries[0].max);
                    for (int i = 1; i < parent.entryCount; i++)
                    {
                        parent.mbr.add(parent.entries[i]);
                    }
                }

                Node<T> newNode = null;
                if (nn != null)
                {
                    if (parent.entryCount < maxNodeEntries)
                    {
                        parent.addEntry(nn.mbr, nn.nodeId);
                    }
                    else
                    {
                        newNode = splitNode(parent, nn.mbr, nn.nodeId);
                    }
                }

                n = parent;
                nn = newNode;

                parent = null;
                newNode = null;
            }

            return nn;
        }

        private void checkConsistency(int nodeId, int expectedLevel, Rectangle expectedMBR)
        {
            Node<T> n = getNode(nodeId);

            Rectangle calculatedMBR = calculateMBR(n);

            for (int i = 0; i < n.entryCount; i++)
            {

                if (n.level > 1)
                { // if not a leaf
                    checkConsistency(n.ids[i], n.level - 1, n.entries[i]);
                }
            }
        }

        private Rectangle calculateMBR(Node<T> n)
        {
            Rectangle mbr = new Rectangle(n.entries[0].min, n.entries[0].max);

            for (int i = 1; i < n.entryCount; i++)
            {
                mbr.add(n.entries[i]);
            }
            return mbr;
        }

        public int Count
        {
            get
            {
                return this.msize;
            }
        }

    }
}