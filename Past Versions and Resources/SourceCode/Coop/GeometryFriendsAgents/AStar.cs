namespace GeometryFriendsAgents
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    internal class AStar : Board
    {
        public static List<node[]> Nodes;
        public static List<int[]> endNodes;
        public int up_cnt;
        public int a_cnt;

        public void Access()
        {
            base.CreateBoard(0x500, 760);
            this.CreateNodes(Board.xCellsNum, Board.yCellsNum);
        }

        public unsafe List<node> AStarFinder(node startNode, int endNode_x, int endNode_y)
        {
            node source = new node();
            node item = Nodes[startNode.x][startNode.y];
            node end = new node();
            end = Nodes[endNode_x][endNode_y];
            List<node> list = new List<node>();
            node node4 = new node();
            List<node> list2 = new List<node>();
            node node5 = new node();
            bool allowDiagonal = false;
            bool dontCrossCorners = true;
            item.g = 0;
            item.f = 0;
            this.up_cnt = 0;
            List<node> openList = new List<node>();
            item.opened = true;
            openList.Add(item);
            this.UpdateNodes(item);
            while (true)
            {
                if (openList.Count != 0)
                {
                    source = this.findCheapestPath(openList, end);
                    if (source.parent_x == 0x3e7)
                    {
                        Console.WriteLine("No Path!!!");
                        node5.x = 0x7cf;
                        node5.y = 0x7cf;
                        node5.f = 0x7cf;
                        list2.Add(node5);
                    }
                    else
                    {
                        source.closed = true;
                        this.UpdateNodes(source);
                        if ((source.x != end.x) || (source.y != end.y))
                        {
                            list = this.getNeighbors(source, allowDiagonal, dontCrossCorners);
                            for (int i = 0; i < list.Count; i++)
                            {
                                node4 = list[i];
                                Board.sub_count++;
                                if (!node4.closed)
                                {
                                    int num3;
                                    int x = node4.x;
                                    int y = node4.y;
                                    if ((x == 0x17) && (y == 0x11))
                                    {
                                        x = x;
                                    }
                                    if (y == source.y)
                                    {
                                        num3 = source.g + 10;
                                    }
                                    else if ((x == source.x) && (y < source.y))
                                    {
                                        num3 = source.g + 15;
                                    }
                                    else if ((x != source.x) || (y <= source.y))
                                    {
                                        num3 = source.g + 0x19;
                                    }
                                    else
                                    {
                                        num3 = source.g + 8;
                                    }
                                    if (!node4.opened || (num3 < node4.g))
                                    {
                                        node4.g = num3;
                                        node4.h = (Math.Abs((int) (x - end.x)) + Math.Abs((int) (y - end.y))) * 10;
                                        node* nodePtr1 = &node4;
                                        nodePtr1->f = node4.g + node4.h;
                                        node4.parent_x = source.x;
                                        node4.parent_y = source.y;
                                        if (!node4.opened)
                                        {
                                            node4.opened = true;
                                            openList.Add(node4);
                                            this.UpdateNodes(node4);
                                        }
                                        else
                                        {
                                            this.UpdateNodes(node4);
                                            for (int j = 0; j < openList.Count; j++)
                                            {
                                                if ((openList[j].x == node4.x) && (openList[j].y == node4.y))
                                                {
                                                    openList[j] = node4;
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            continue;
                        }
                        list2 = this.FinalPath(source, openList);
                        node5 = source;
                    }
                }
                return list2;
            }
        }

        public void CreateNodes(int x, int y)
        {
            Nodes = new List<node[]>();
            int num = 0;
            while (num < x)
            {
                Nodes.Add(new node[y]);
                int index = 0;
                while (true)
                {
                    if (index >= y)
                    {
                        num++;
                        break;
                    }
                    Nodes[num][index].x = num;
                    Nodes[num][index].y = index;
                    index++;
                }
            }
        }

        private List<node> FinalPath(node currentNode, List<node> openList)
        {
            List<node> list = new List<node> {
                currentNode
            };
            node item = openList[openList.Count - 1];
            int num = openList.Count - 1;
            while (true)
            {
                if (num >= 0)
                {
                    if ((currentNode.x != openList[num].x) || (currentNode.y != openList[num].y))
                    {
                        openList.Remove(openList[num]);
                        num--;
                        continue;
                    }
                    item = openList[num];
                }
                if (openList.Count == 1)
                {
                    list.Add(item);
                }
                else
                {
                    item = Nodes[item.parent_x][item.parent_y];
                    foreach (node local1 in openList)
                    {
                        list.Add(item);
                        item = Nodes[item.parent_x][item.parent_y];
                        if (item.parent_x == 0)
                        {
                            break;
                        }
                    }
                    list.Add(item);
                }
                return list;
            }
        }

        private node findCheapestPath(List<node> openList, node end)
        {
            node node = new node();
            int num = 0x7d0;
            bool flag = false;
            for (int i = 0; i < openList.Count; i++)
            {
                if (!Nodes[openList[i].x][openList[i].y].closed)
                {
                    flag = true;
                    int f = openList[i].f;
                    if (f < num)
                    {
                        node = openList[i];
                        num = f;
                    }
                }
            }
            if (!flag)
            {
                node.parent_x = 0x3e7;
            }
            return node;
        }

        private List<node> getNeighbors(node currentNode, bool allowDiagonal, bool dontCrossCorners)
        {
            int x = currentNode.x;
            int y = currentNode.y;
            List<node> list = new List<node>();
            bool flag = false;
            bool flag2 = false;
            bool flag3 = false;
            bool flag4 = false;
            bool flag5 = false;
            bool flag6 = false;
            bool flag7 = false;
            bool flag8 = false;
            if (this.Isexist(x, y - 1))
            {
                list.Add(Nodes[x][y - 1]);
                flag = true;
            }
            if (this.Isexist(x + 1, y))
            {
                list.Add(Nodes[x + 1][y]);
                flag2 = true;
            }
            if (this.Isexist(x, y + 1))
            {
                list.Add(Nodes[x][y + 1]);
                flag3 = true;
            }
            if (this.Isexist(x - 1, y))
            {
                list.Add(Nodes[x - 1][y]);
                flag4 = true;
            }
            if (allowDiagonal)
            {
                if (dontCrossCorners)
                {
                    flag5 = flag4 && flag;
                    flag6 = flag && flag2;
                    flag7 = flag2 && flag3;
                    flag8 = flag3 && flag4;
                }
                else
                {
                    flag5 = flag4 || flag;
                    flag6 = flag || flag2;
                    flag7 = flag2 || flag3;
                    flag8 = flag3 || flag4;
                }
                if (flag5 && this.Isexist(x - 1, y - 1))
                {
                    list.Add(Nodes[x - 1][y - 1]);
                }
                if (flag6 && this.Isexist(x + 1, y - 1))
                {
                    list.Add(Nodes[x + 1][y - 1]);
                }
                if (flag7 && this.Isexist(x + 1, y + 1))
                {
                    list.Add(Nodes[x + 1][y + 1]);
                }
                if (flag8 && this.Isexist(x - 1, y + 1))
                {
                    list.Add(Nodes[x - 1][y + 1]);
                }
            }
            return list;
        }

        private bool Isexist(int x, int y)
        {
            bool flag = true;
            if (((x >= Board.xCellsNum) || ((x < 0) || (y >= Board.yCellsNum))) || (y < 0))
            {
                flag = false;
            }
            else if (Nodes[x][y].obstacle)
            {
                flag = false;
            }
            else if (Nodes[x][y].semi_block)
            {
                flag = false;
            }
            return flag;
        }

        private void nodeListCheck(List<node> List)
        {
            using (StreamWriter writer = new StreamWriter(@"C:\test\nodeListCheck.txt", true))
            {
                for (int i = 0; i < List.Count; i++)
                {
                    object[] arg = new object[] { i, List[i].x, List[i].y, List[i].opened, List[i].g, List[i].h };
                    writer.WriteLine("Listcheck{0} :{1} / {2} / {3} / {4} / {5} ", arg);
                }
            }
            Console.WriteLine(" i번째 / x / y / opened / g / h");
        }

        public void printNodes()
        {
            string[] strArray = new string[Board.xCellsNum * 2];
            int num = 1;
            if (this.a_cnt == 1)
            {
                int index = 0;
                while (true)
                {
                    if (index >= Board.yCellsNum)
                    {
                        using (StreamWriter writer = new StreamWriter(@"C:\test\printNodes.txt", true))
                        {
                            foreach (string str2 in strArray)
                            {
                                writer.WriteLine(str2);
                            }
                        }
                        break;
                    }
                    int num3 = 0;
                    while (true)
                    {
                        if (num3 >= Nodes.Count)
                        {
                            index++;
                            break;
                        }
                        string str = !Nodes[num3][index].obstacle ? "X" : "○";
                        object[] objArray = new object[] { "  ", Nodes[num3][index].x, ",", Nodes[num3][index].y, "(", str, ") |" };
                        strArray[((num3 + 1) * 2) - 2] = strArray[((num3 + 1) * 2) - 2] + Convert.ToString(string.Concat(objArray));
                        strArray[((num3 + 1) * 2) - 1] = strArray[((num3 + 1) * 2) - 1] + "----------";
                        num++;
                        num3++;
                    }
                }
            }
        }

        private void printRealPath(List<node> RealPath)
        {
            string[] strArray = new string[RealPath.Count];
            for (int i = 0; i < RealPath.Count; i++)
            {
                strArray[i] = Convert.ToString(string.Concat(new object[] { " (", RealPath[i].x, ",", RealPath[i].y, ") " }));
            }
            using (StreamWriter writer = new StreamWriter(@"C:\test\printRealPath.txt", true))
            {
                string[] strArray2 = strArray;
                int index = 0;
                while (true)
                {
                    if (index >= strArray2.Length)
                    {
                        writer.WriteLine("~~~~~~~~~~~~~~~~");
                        break;
                    }
                    string str = strArray2[index];
                    if (strArray[strArray.Length - 1] == str)
                    {
                        writer.WriteLine("{0} ;", str);
                    }
                    else
                    {
                        writer.Write("{0}<-", str);
                    }
                    index++;
                }
            }
        }

        private void UpdateNodes(node source)
        {
            Nodes[source.x][source.y] = (node[]) source;
        }
    }
}

