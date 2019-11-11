namespace GeometryFriendsAgents
{
    using Microsoft.Xna.Framework;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal class mapAnal
    {
        private List<Vector4> Box_set = new List<Vector4>();
        private List<Edge> Edge_set = new List<Edge>();
        private List<ADNode> InterastingPoints = new List<ADNode>();
        private List<ADNode> AssistantPoints = new List<ADNode>();

        public int addAssistPoint(CVector2 in_vec)
        {
            this.InterastingPoints.Add(new ADNode(in_vec.x, in_vec.y, -1));
            this.AssistantPoints.Add(this.InterastingPoints.Last<ADNode>());
            float num = 50000f;
            Edge edge = new Edge(in_vec, new CVector2(in_vec.x, in_vec.y + num));
            int num2 = -1;
            foreach (Edge edge2 in this.Edge_set)
            {
                CVector2 vector = edge.col(edge2);
                if ((vector != null) && (vector.y < edge.pt_2.y))
                {
                    edge.pt_2.y = vector.y;
                    num2 = edge2.platform_2;
                }
            }
            if (edge.length() < 500f)
            {
                edge.pt_2.y += 50f;
                this.InterastingPoints.Add(new ADNode(edge.pt_2.x, edge.pt_2.y, num2));
                this.AssistantPoints.Add(this.InterastingPoints.Last<ADNode>());
            }
            return 0;
        }

        public int addBox(Vector4 in_rect)
        {
            this.Box_set.Add(in_rect);
            return 0;
        }

        public int Analysis()
        {
            int num = -1;
            foreach (Vector4 vector in this.Box_set)
            {
                num++;
                float x = vector.X;
                float y = vector.Y;
                float num4 = vector.Z / 2f;
                float num5 = vector.W / 2f;
                this.Edge_set.Add(new Edge(x - num4, y - num5, x + num4, y - num5, num, num));
                this.Edge_set.Add(new Edge(x - num4, y + num5, x + num4, y + num5, num, num));
                this.Edge_set.Add(new Edge(x - num4, y - num5, x - num4, y + num5, num, num));
                this.Edge_set.Add(new Edge(x + num4, y - num5, x + num4, y + num5, num, num));
                this.InterastingPoints.Add(new ADNode((x - num4) + Parameter.EDGE_X_BUFFER, (y - num5) - Parameter.EDGE_Y_BUFFER, num));
                if (this.InterastingPoints.Last<ADNode>().x < Parameter.SCREEN_X_MIN)
                {
                    this.InterastingPoints.Last<ADNode>().x = Parameter.SCREEN_X_MIN;
                }
                if (this.InterastingPoints.Last<ADNode>().x > Parameter.SCREEN_X_MAX)
                {
                    this.InterastingPoints.Last<ADNode>().x = Parameter.SCREEN_X_MAX;
                }
                this.InterastingPoints.Add(new ADNode((x + num4) - Parameter.EDGE_X_BUFFER, (y - num5) - Parameter.EDGE_Y_BUFFER, num));
                if (this.InterastingPoints.Last<ADNode>().x < Parameter.SCREEN_X_MIN)
                {
                    this.InterastingPoints.Last<ADNode>().x = Parameter.SCREEN_X_MIN;
                }
                if (this.InterastingPoints.Last<ADNode>().x > Parameter.SCREEN_X_MAX)
                {
                    this.InterastingPoints.Last<ADNode>().x = Parameter.SCREEN_X_MAX;
                }
            }
            return 0;
        }

        public bool collision(Edge in_edge)
        {
            using (List<Edge>.Enumerator enumerator = this.Edge_set.GetEnumerator())
            {
                while (true)
                {
                    if (!enumerator.MoveNext())
                    {
                        break;
                    }
                    Edge current = enumerator.Current;
                    if (current.col(in_edge) != null)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public ActionMap Generating()
        {
            ActionMap map = new ActionMap();
            int num = 0;
            while (num < this.InterastingPoints.Count)
            {
                ADNode node = this.InterastingPoints[num];
                int num2 = 0;
                while (true)
                {
                    if (num2 >= this.InterastingPoints.Count)
                    {
                        num++;
                        break;
                    }
                    if (num != num2)
                    {
                        bool flag = true;
                        ADNode node2 = this.InterastingPoints[num2];
                        Edge edge = new Edge(node, node2) {
                            platform_1 = node.platform,
                            platform_2 = node2.platform
                        };
                        foreach (Edge edge2 in this.Edge_set)
                        {
                            if (edge.col(edge2) != null)
                            {
                                flag = false;
                            }
                        }
                        bool flag2 = true;
                        Edge edge3 = new Edge(node - new CVector2(0f, Parameter.THICKNESS_HEIGHT), node2 - new CVector2(0f, Parameter.THICKNESS_HEIGHT));
                        foreach (Edge edge4 in this.Edge_set)
                        {
                            if (edge3.col(edge4) != null)
                            {
                                flag2 = false;
                            }
                        }
                        if ((node.y < 0f) || (node2.y < 0f))
                        {
                            flag = false;
                        }
                        if (flag && flag2)
                        {
                            ActionPoint point = new ActionPoint(node, node2) {
                                platform_st = node.platform,
                                platform_ed = node2.platform
                            };
                            map.AddAP(point);
                        }
                    }
                    num2++;
                }
            }
            return map;
        }

        public int getPlatform_idx(CVector2 vec)
        {
            for (int i = 0; i < this.Box_set.Count; i++)
            {
                Vector4 vector = this.Box_set[i];
                float x = vector.X;
                float y = vector.Y;
                float num4 = vector.Z / 2f;
                float num5 = vector.W / 2f;
                if (((((x - num4) + Parameter.EDGE_X_BUFFER) == 0f) && (((y - num5) - Parameter.EDGE_Y_BUFFER) == 0f)) || ((((x + num4) - Parameter.EDGE_X_BUFFER) == 0f) && (((y - num5) - Parameter.EDGE_Y_BUFFER) == 0f)))
                {
                    return i;
                }
            }
            return -1;
        }

        public void print()
        {
            foreach (ADNode local1 in this.InterastingPoints)
            {
            }
            foreach (Edge local2 in this.Edge_set)
            {
            }
        }

        public int removeAllAssistPoint()
        {
            foreach (ADNode node in this.AssistantPoints)
            {
                this.InterastingPoints.Remove(node);
            }
            this.AssistantPoints.Clear();
            return 0;
        }
    }
}

