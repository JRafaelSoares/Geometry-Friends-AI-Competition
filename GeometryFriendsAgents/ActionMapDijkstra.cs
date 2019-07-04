namespace GeometryFriendsAgents
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using GeometryFriends;

    internal class ActionMapDijkstra
    {
        private ActionMap AM;
        private int unique = 0;
        private List<TreeNode<ADNode>> origin_Nodes = new List<TreeNode<ADNode>>();
        private TreeNode<ADNode> root;

        public ActionMapDijkstra(ActionMap in_AM)
        {
            this.AM = in_AM;
            foreach (ActionPoint point in this.AM.getAPs())
            {
                int num3;
                int num4;
                this.unique = (num3 = this.unique) + 1;
                this.origin_Nodes.Add(new TreeNode<ADNode>(point.start_point, num3));
                this.unique = (num4 = this.unique) + 1;
                this.origin_Nodes.Add(new TreeNode<ADNode>(point.end_point, num4));
            }
            int num = 0;
            while (num < this.origin_Nodes.Count)
            {
                int count = 0;
                while (true)
                {
                    if (count >= this.origin_Nodes.Count)
                    {
                        num++;
                        break;
                    }
                    if (((num != count) && (this.origin_Nodes[num].content.x == this.origin_Nodes[count].content.x)) && (this.origin_Nodes[num].content.y == this.origin_Nodes[count].content.y))
                    {
                        this.origin_Nodes.Remove(this.origin_Nodes[count]);
                        count = this.origin_Nodes.Count;
                        num--;
                    }
                    count++;
                }
            }
            this.unique = 0;
            foreach (TreeNode<ADNode> node in this.origin_Nodes)
            {
                int num5;
                this.unique = (num5 = this.unique) + 1;
                node.setIndex(num5);
            }
        }

        public float getActionDistance(TreeNode<ADNode> tn) => 
            tn.getweightuntiltop();

        public List<ActionNode> getActionPlan(TreeNode<ADNode> tn)
        {
            List<ActionNode> list = new List<ActionNode>();
            tn = this.root.search(tn.getIndex());
            while (tn != null)
            {
                ActionNode item = new ActionNode {
                    posX = tn.content.x,
                    posY = tn.content.y,
                    cmd = ActEnum.stay,
                    platform = tn.content.platform
                };
                list.Insert(0, item);
                tn = tn.parent;
            }
            return list;
        }

        public TreeNode<ADNode> getTN(CVector2 vec)
        {
            using (List<TreeNode<ADNode>>.Enumerator enumerator = this.origin_Nodes.GetEnumerator())
            {
                while (true)
                {
                    if (!enumerator.MoveNext())
                    {
                        break;
                    }
                    TreeNode<ADNode> current = enumerator.Current;
                    if ((current.content.x == vec.x) && (current.content.y == vec.y))
                    {
                        return current;
                    }
                }
            }
            return null;
        }

        public TreeNode<ADNode> getTNnear(CVector2 vec, mapAnal map)
        {
            TreeNode<ADNode> node = this.origin_Nodes[0];
            float num = (vec - node.content).length();
            for (int i = 1; i < this.origin_Nodes.Count; i++)
            {
                float num3 = (vec - this.origin_Nodes[i].content).length();
                if (map != null)
                {
                    Edge edge = new Edge(vec, this.origin_Nodes[i].content);
                    if (map.collision(edge))
                    {
                        num3 = 9E+08f;
                    }
                }
                if (num3 < num)
                {
                    num = num3;
                    node = this.origin_Nodes[i];
                }
            }
            return node;
        }

        public void print()
        {
            foreach (TreeNode<ADNode> local1 in this.origin_Nodes)
            {
            }
        }

        public int process(TreeNode<ADNode> in_root)
        {
            bool[] flagArray = new bool[this.origin_Nodes.Count];
            for (int i = 0; i < this.origin_Nodes.Count; i++)
            {
                flagArray[i] = false;
            }
            this.root = in_root;
            if (this.root == null)
            {
                return -1;
            }
            Queue<TreeNode<ADNode>> queue = new Queue<TreeNode<ADNode>>();
            queue.Enqueue(this.root);
            flagArray[this.root.getIndex()] = true;
            int num2 = Parameter.LOOP_BREAK;
            while ((queue.Count > 0) && (--num2 > 0))
            {
                TreeNode<ADNode> node = queue.Dequeue();
                List<ActionPoint> list = this.AM.getAPdirection(node.content);
                int index = 0;
                while (true)
                {
                    if (index >= list.Count)
                    {
                        foreach (ActionPoint point in list)
                        {
                            TreeNode<ADNode> child = new TreeNode<ADNode>(this.getTN(point.end_point));
                            TreeNode<ADNode> node3 = this.root.search(child.getIndex());
                            if (node3 != null)
                            {
                                if (!node3.checkRoot(this.root))
                                {
                                    continue;
                                }
                                if (!child.checkRoot(this.root))
                                {
                                    continue;
                                }
                                child.weight = point.getweight();
                                if (node3.getweightuntiltop() <= node.getweightuntiltop())
                                {
                                    continue;
                                }
                                node3.parent.RemoveChild(node3);
                                node.AddChild(child);
                                continue;
                            }
                            child.weight = point.getweight();
                            if ((point.start_point.y - point.end_point.y) > Parameter.CUT_LINE_Y)
                            {
                                child.weight = 9E+07f;
                            }
                            if (((point.start_point.y - point.end_point.y) > 0f) && (point.distance > Parameter.CUT_LINE_DISTANCE))
                            {
                                child.weight = 9E+07f;
                            }
                            if (child.weight < 0f)
                            {
                                child.weight = 9E+08f;
                            }
                            node.AddChild(child);
                            queue.Enqueue(child);
                        }
                        break;
                    }
                    if (Math.Abs((float) (list[index].start_point.y - list[index].end_point.y)) > Parameter.CUT_LINE_Y)
                    {
                        list.RemoveAt(index);
                        index--;
                    }
                    index++;
                }
            }
            this.root.print();
            return 0;
        }

        public Queue<ActionNode> smoothing(List<ActionNode> list_an)
        {
            ActionNode node;
            Queue<ActionNode> source = new Queue<ActionNode>();

            for (int i = 0; i < list_an.Count; i++)
            {
                node = list_an[i];
                Log.LogInformation("X: " + node.posX + " Y: " + node.posY, true);

                if ((i > 0) && (node.platform != list_an[i - 1].platform))
                {
                    node.cmd = ActEnum.jump;
                    if (node.posY > list_an[i - 1].posY)
                    {
                        node.cmd = ActEnum.stay;
                    }
                }
            }
            for (int j = 0; j < list_an.Count; j++)
            {
                node = list_an[j];
                source.Enqueue(node);
            }
            if (source.Count == 1)
            {
                node = new ActionNode {
                    posX = source.Last<ActionNode>().posX,
                    posY = source.Last<ActionNode>().posY,
                    cmd = ActEnum.jump
                };
                source.Enqueue(node);
            }
            return source;
        }
    }
}

