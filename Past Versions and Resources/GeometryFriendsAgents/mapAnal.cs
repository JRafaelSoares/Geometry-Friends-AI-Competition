namespace GeometryFriendsAgents
{
    //using Microsoft.Xna.Framework;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Numerics;
    using GeometryFriends;

    using GeometryFriends.AI.Perceptions.Information;

    internal class mapAnal
    {
        private List<Edge> Edge_set = new List<Edge>();
        private List<ADNode> InterastingPoints = new List<ADNode>();
        private List<ADNode> AssistantPoints = new List<ADNode>();
        private ObstacleRepresentation[] obstaclesInfo;

        public int addAssistPoint(CVector2 in_vec)
        {
            this.InterastingPoints.Add(new ADNode(in_vec.x, in_vec.y, -1));
            this.AssistantPoints.Add(this.InterastingPoints.Last<ADNode>());
            float num = 50000;
            Edge edge = new Edge(in_vec, new CVector2(in_vec.x, in_vec.y + num));
            int num2 = -1;
            foreach (Edge edge2 in this.Edge_set)
            {
                CVector2 vector = edge.col(edge2);
                if ((vector != null) && (vector.y < edge.posPlatform_2.y))
                {
                    edge.posPlatform_2.y = vector.y;
                    num2 = edge2.platform_2;
                }
            }
            if (edge.length() < 500)
            {
                edge.posPlatform_2.y += 50;
                this.InterastingPoints.Add(new ADNode(edge.posPlatform_2.x, edge.posPlatform_2.y, num2));
                this.AssistantPoints.Add(this.InterastingPoints.Last<ADNode>());
            }
            return 0;
        }

        public void addObstacleInfo(ObstacleRepresentation[] obstaclesInfo)
        {
            this.obstaclesInfo = obstaclesInfo;
        }

        public int Analysis()
        {
            int num = -1;
            foreach (ObstacleRepresentation obstacle in this.obstaclesInfo)
            {
                float x = obstacle.X;
                float y = obstacle.Y;
                float height = obstacle.Height / 2;
                float width = obstacle.Width / 2;
                num++;
                addInterestPoints(x, y, height, width, num);

            }

            //Final obstacled added by the submission (to be determined what is it)
            //num++;
            //addInterestPoints(600, 800, 1000, 64, num);

            return 0;
        }

        public void addInterestPoints(float x, float y, float height, float width, int num)
        {
            this.Edge_set.Add(new Edge(x - width, y - height, x + width, y - height, num, num));
            this.Edge_set.Add(new Edge(x - width, y + height, x + width, y + height, num, num));
            this.Edge_set.Add(new Edge(x - width, y - height, x - width, y + height, num, num));
            this.Edge_set.Add(new Edge(x + width, y - height, x + width, y + height, num, num));
            //Why y-height? Should it not be y + height so it is on top?
            this.InterastingPoints.Add(new ADNode((x - width) + Parameter.EDGE_X_BUFFER, (y - height) - Parameter.EDGE_Y_BUFFER, num));
            if (this.InterastingPoints.Last<ADNode>().x < Parameter.SCREEN_X_MIN)
            {
                this.InterastingPoints.Last<ADNode>().x = Parameter.SCREEN_X_MIN;
            }
            if (this.InterastingPoints.Last<ADNode>().x > Parameter.SCREEN_X_MAX)
            {
                this.InterastingPoints.Last<ADNode>().x = Parameter.SCREEN_X_MAX;
            }
            this.InterastingPoints.Add(new ADNode((x + width) - Parameter.EDGE_X_BUFFER, (y - height) - Parameter.EDGE_Y_BUFFER, num));
            if (this.InterastingPoints.Last<ADNode>().x < Parameter.SCREEN_X_MIN)
            {
                this.InterastingPoints.Last<ADNode>().x = Parameter.SCREEN_X_MIN;
            }
            if (this.InterastingPoints.Last<ADNode>().x > Parameter.SCREEN_X_MAX)
            {
                this.InterastingPoints.Last<ADNode>().x = Parameter.SCREEN_X_MAX;
            }
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
            for (int i = 0; i < this.obstaclesInfo.Length; i++)
            {
                ObstacleRepresentation obstacle = this.obstaclesInfo[i];
                float x = obstacle.X;
                float y = obstacle.Y;
                float height = obstacle.Height / 2;
                float width = obstacle.Width / 2;
                if (((((x - width) + Parameter.EDGE_X_BUFFER) == 0) && (((y - height) - Parameter.EDGE_Y_BUFFER) == 0)) || ((((x + width) - Parameter.EDGE_X_BUFFER) == 0) && (((y - height) - Parameter.EDGE_Y_BUFFER) == 0)))
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

