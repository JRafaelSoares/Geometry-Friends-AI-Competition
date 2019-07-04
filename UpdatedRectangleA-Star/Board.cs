namespace GeometryFriendsAgents
{
    using System;
    using System.Collections.Generic;
	using GeometryFriends.AI.Perceptions.Information;

	internal class Board
    {
        public static int xCellsNum;
        public static int yCellsNum;
        public static int top_limit;
        protected int width;
        protected int height;
        protected int maxXCellsNum;
        protected int maxYCellsNum;
        public static int cellMinSize;
        protected ObstacleRepresentation[] tempObstacle;
        protected CollectibleRepresentation[] tempDiamonds;
        public static int Acount;
        public static int sub_count;

        public void Ball_MakeSemi(node startNode, float[] circleInfor)
        {
            float num = circleInfor[1] + circleInfor[4];
            int num2 = 0;
            for (int i = 0; i < yCellsNum; i++)
            {
                if (((num - 438f) >= (cellMinSize * i)) && ((num - 438f) < (cellMinSize * (i + 1))))
                {
                    num2 = i;
                }
            }
            int num4 = 0;
            while (num4 < xCellsNum)
            {
                int index = 0;
                while (true)
                {
                    if (index >= yCellsNum)
                    {
                        num4++;
                        break;
                    }
                    if ((index > startNode.y) || (index < num2))
                    {
                        AStar.Nodes[num4][index].semi_block = true;
                    }
                    index++;
                }
            }
        }

		public node checkStartNode(CircleRepresentation info)
        {
            top_limit = 5 - ((int) Math.Ceiling((double) (((double) info.Radius) / 40.0)));
            return new node { 
                x = ((int) info.X) / cellMinSize,
                y = ((int) (info.Y - (info.Radius / 2f))) / cellMinSize
            };
        }

		public node checkStartNode(RectangleRepresentation info)
		{
			top_limit = 5 - ((int)Math.Ceiling((double)(((double)info.Height) / 40.0)));
			return new node
			{
				x = ((int)info.X) / cellMinSize,
				y = ((int)(info.Y - (info.Height / 2f))) / cellMinSize
			};
		}

		public void ClearSemi()
        {
            int num = 0;
            while (num < xCellsNum)
            {
                int index = 0;
                while (true)
                {
                    if (index >= yCellsNum)
                    {
                        num++;
                        break;
                    }
                    AStar.Nodes[num][index].semi_block = false;
                    index++;
                }
            }
        }

        public void CreateBoard(int w, int h)
        {
            this.width = w;
            this.height = h;
            cellMinSize = 40;
            this.maxXCellsNum = this.width / cellMinSize;
            this.maxYCellsNum = this.height / cellMinSize;
            xCellsNum = this.maxXCellsNum;
            yCellsNum = this.maxYCellsNum;
            Acount = 0;
            sub_count = 0;
        }

        private int get_DyNode(float y, float height, bool single)
        {
            if (single)
            {
                return (((int) (y + ((height / 2f) - 5f))) / cellMinSize);
            }
            int num = ((int) (y + (height / 2f))) / cellMinSize;
            if ((num != (((int) y) / cellMinSize)) && (((y + (height / 2f)) % ((float) cellMinSize)) < 20f))
            {
                num--;
            }
            return num;
        }

        private int get_LxNode(float x, float width, bool single)
        {
            if (single)
            {
                return (((int) (x - ((width / 2f) - 5f))) / cellMinSize);
            }
            int num = ((int) (x - (width / 2f))) / cellMinSize;
            if ((num != (((int) x) / cellMinSize)) && (((x - (width / 2f)) % ((float) cellMinSize)) > 20f))
            {
                num++;
            }
            return num;
        }

        private int get_RxNode(float x, float width, bool single)
        {
            if (single)
            {
                return (((int) (x + ((width / 2f) - 5f))) / cellMinSize);
            }
            int num = ((int) (x + (width / 2f))) / cellMinSize;
            if ((num != (((int) x) / cellMinSize)) && (((x + (width / 2f)) % ((float) cellMinSize)) < 20f))
            {
                num--;
            }
            return num;
        }

        private int get_UyNode(float y, float height, bool single)
        {
            if (single)
            {
                return (((int) (y - ((height / 2f) - 5f))) / cellMinSize);
            }
            int num = ((int) (y - (height / 2f))) / cellMinSize;
            if ((num != (((int) y) / cellMinSize)) && (((y - (height / 2f)) % ((float) cellMinSize)) > 20f))
            {
                num++;
            }
            return num;
        }

        public void getBoardValue(ObstacleRepresentation[] obstaclesInfo)
        {
            this.tempObstacle = obstaclesInfo;
            int length = obstaclesInfo.Length;
            int num12 = 0;

            while (num12 < xCellsNum)
            {
                int index = 0;
                while (true)
                {
                    if (index >= yCellsNum)
                    {
                        num12++;
                        break;
                    }
                    if (((num12 == 0) || (index == 0)) || (num12 == (xCellsNum - 1)))
                    {
                        AStar.Nodes[num12][index].obstacle = true;
                    }
                    index++;
                }
            }

            if (length > 0)
            {
                for (int i = 0; i < length; i++)
                {
                    float x = this.tempObstacle[i].X;
                    float y = this.tempObstacle[i].Y;
                    float height = this.tempObstacle[i].Height;
                    float width = this.tempObstacle[i].Width;

                    int num5 = this.get_LxNode(x, width, true);
                    int num6 = this.get_RxNode(x, width, true);
                    int num8 = this.get_UyNode(y, height, true);
                    int num9 = this.get_DyNode(y, height, true);

                    int index = ((int) y) / cellMinSize;
                    int num7 = ((int) x) / cellMinSize;

                    if (width > height)
                    {
                        if (height < cellMinSize)
                        {
                            for (int j = num5; j <= num6; j++)
                            {
                                AStar.Nodes[j][index].obstacle = true;
                            }
                        }
                        else
                        {
                            int num16 = num5;
                            while (num16 <= num6)
                            {
                                int num17 = this.get_UyNode(y, height, false);
                                while (true)
                                {
                                    if (num17 > this.get_DyNode(y, height, false))
                                    {
                                        num16++;
                                        break;
                                    }
                                    AStar.Nodes[num16][num17].obstacle = true;
                                    num17++;
                                }
                            }
                        }
                    }
                    else if (width >= height)
                    {
                        int num21 = num5;
                        while (num21 <= num6)
                        {
                            int num22 = num8;
                            while (true)
                            {
                                if (num22 > num9)
                                {
                                    num21++;
                                    break;
                                }
                                AStar.Nodes[num21][num22].obstacle = true;
                                num22++;
                            }
                        }
                    }
                    else if (width < cellMinSize)
                    {
                        for (int j = num8; j <= num9; j++)
                        {
                            AStar.Nodes[num7][j].obstacle = true;
                        }
                    }
                    else
                    {
                        int num19 = this.get_LxNode(x, width, false);
                        while (num19 <= this.get_RxNode(x, width, false))
                        {
                            int num20 = num8;
                            while (true)
                            {
                                if (num20 > num9)
                                {
                                    num19++;
                                    break;
                                }
                                AStar.Nodes[num19][num20].obstacle = true;
                                num20++;
                            }
                        }
                    }
                }
            }
        }

        public void getUpdateEndNode(CollectibleRepresentation[] collectiblesInfo)
        {
            int length = collectiblesInfo.Length;
            AStar.endNodes = new List<int[]>();

            if (length >= 1)
            {
				foreach (CollectibleRepresentation info in collectiblesInfo)
				{
					int x = ((int)info.X) / cellMinSize;
					int y = ((int)info.Y) / cellMinSize;

					if (!AStar.Nodes[x][y].semi_block)
					{
						AStar.endNodes.Add(new int[] { x, y });
						AStar.Nodes[x][y].obstacle = false;
					}
				}
            }
        }

        public void Square_MakeSemi(node startNode, float[] squareInfo)
        {
            float num = squareInfo[1] + (squareInfo[4] / 2f);
            int num2 = 0;
            for (int i = 0; i < yCellsNum; i++)
            {
                if (((num - 227f) >= (cellMinSize * i)) && ((num - 227f) < (cellMinSize * (i + 1))))
                {
                    num2 = i;
                }
            }
            int num4 = 0;
            while (num4 < xCellsNum)
            {
                int index = 0;
                while (true)
                {
                    if (index >= yCellsNum)
                    {
                        num4++;
                        break;
                    }
                    if ((index > startNode.y) || (index < num2))
                    {
                        AStar.Nodes[num4][index].semi_block = true;
                    }
                    index++;
                }
            }
        }

        public void UpdateNew(int end_x, int end_y)
        {
            int num = 0;

            while (num < xCellsNum)
            {
                int index = 0;

                while (true)
                {
                    if (index >= yCellsNum)
                    {
                        num++;
                        break;
                    }

                    node node = new node {
                        x = num,
                        y = index,
                        obstacle = AStar.Nodes[num][index].obstacle,
                        semi_block = AStar.Nodes[num][index].semi_block
                    };

                    AStar.Nodes[num][index] = node;

                    index++;
                }
            }
        }
    }
}

