namespace GeometryFriendsAgents
{
    using GeometryFriends;
    using GeometryFriends.AI.Interfaces;
    using Microsoft.Xna.Framework;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    internal class SquareAgent : ISquareAgent, IAgent
    {
        private bool implementedAgent;
        private string agentName = "RandRect";

        private int lastAction;
        private int currentAction;
        private long lastMoveTime;
        private Random rnd;

        private int[] numbersInfo;
        private float[] squareInfo;
        private float[] circleInfo;
        private float[] obstaclesInfo;
        private float[] squarePlatformsInfo;
        private float[] circlePlatformsInfo;
        private float[] collectiblesInfo;

        private int nCollectiblesLeft;


        protected Rectangle area;

        private int cellsize;
        private float start_point;
        private float pre_velocity;
        private float pre_dist;
        private float now_dist;
        private float now_velocity;
        private float now_accel;
        private int cnt;
        private int x;
        private int y;
        private int x_right;
        private int x_left;
        private int collect_count;
        private int break_Rcount;
        private int break_Lcount;
        private int break_Mcount;
        private int block_goX;
        private int hang_time;
        private int direction_switch;
        private int bottom;
        private int right;
        private int left;
        private int up;
        private bool astar_check;
        private bool down_check;
        private bool down_Realcheck;
        private bool astar_upstate;
        private bool max_upstate;
        private bool max_downstate;
        private bool obstacle_Rtop;
        private bool obstacle_Ltop;
        private bool obstacle_Rup;
        private bool obstacle_Lup;
        private bool obstacle_Rbottom;
        private bool obstacle_Lbottom;
        private bool break_state;
        private bool break_around;
        private bool hang_check;
        private AStar S_astar = new AStar();
        private string[] S_Path_print;
        private string[] EndNode_print;
        private List<node> nearest;
        private int y_max;
        private int dest_x;
        private int dest_y;
        private int _x;
        private int _y;
        private int dia_index;
        private int dia_x;
        private int dia_y;
        private float dia_Rx;
        private float dia_Ry;
        private List<Vector4> obs = new List<Vector4>();

        public SquareAgent()
        {
            this.SetImplementedAgent(true);
            this.lastMoveTime = DateTime.Now.Second;
            this.lastAction = 0;
            this.currentAction = 0;
            this.rnd = new Random();
            this.S_astar.a_cnt++;
            if (File.Exists(@"C:\test\testPrint.txt"))
            {
                File.Delete(@"C:\test\testPrint.txt");
            }
        }

        public string AgentName() => 
            this.agentName;

        private void AstarAction()
        {
            this.cnt++;
            this.get_yDNode();
            this.now_dist = this.squareInfo[0] - this.start_point;
            this.now_velocity = this.now_dist - this.pre_dist;
            this.update_path();
            this.update_direct();
            switch (this.direction_switch)
            {
                case 1:
                    this.currentAction = this.decide_action(this.diff_dist(this.squareInfo[0], (float) this.dest_x, this.predict(this.now_velocity)));
                    this.astar_upstate = false;
                    if ((this.obstacle_Lbottom || (this.obstacle_Lup || this.obstacle_Rbottom)) || this.obstacle_Rup)
                    {
                        this.hang();
                    }
                    break;

                case 2:
                    if (this.down_check && this.down_Realcheck)
                    {
                        if ((((this.squareInfo[0] - 1f) > this.dest_x) || (this.dest_x > (this.squareInfo[0] + 1f))) || ((this.dest_y / this.cellsize) == this.y_max))
                        {
                            this.direction_switch = 1;
                            this.currentAction = this.decide_action(this.diff_dist(this.squareInfo[0], (float) this.dest_x, this.predict(this.now_velocity)));
                        }
                        else
                        {
                            this.currentAction = this.down_action(this.diff_dist(this.squareInfo[1] - (this.squareInfo[4] / 2f), (this.squareInfo[1] + (this.squareInfo[4] / 2f)) - 192f));
                            this.astar_upstate = true;
                            this.max_upstate = true;
                        }
                    }
                    else if (!this.obstacle_Rup || !this.obstacle_Lup)
                    {
                        this.currentAction = 0;
                    }
                    else
                    {
                        this.currentAction = 0;
                        if (this.obstacle_Ltop && this.obstacle_Rtop)
                        {
                            this.max_downstate = true;
                        }
                    }
                    break;

                case 3:
                    this.up = ((int) (this.squareInfo[1] - ((this.squareInfo[4] / 2f) + 16f))) / this.cellsize;
                    if (!this.down_check || !this.down_Realcheck)
                    {
                        if (this.block(this.up))
                        {
                            this.direction_switch = 1;
                            this.currentAction = (this.block_goX == 0) ? this.decide_action(this.diff_dist(this.squareInfo[0], (float) this.dest_x, this.predict(this.now_velocity))) : this.decide_action(this.diff_dist(this.squareInfo[0], (float) this.block_goX, this.predict(this.now_velocity)));
                        }
                        else
                        {
                            this.currentAction = this.decide_action(this.diff_dist(this.squareInfo[1] - (this.squareInfo[4] / 2f), (float) this.dest_y));
                            this.down_Realcheck = false;
                        }
                    }
                    else if (((this.y <= this.dia_y) || ((this.dia_x - 1) > this.x)) || (this.x > (this.dia_x + 1)))
                    {
                        this.currentAction = 0;
                        this.astar_check = true;
                    }
                    else
                    {
                        this.currentAction = this.decide_action(this.diff_dist(this.squareInfo[1] - (this.squareInfo[4] / 2f), (float) this.dest_y));
                        this.down_Realcheck = false;
                    }
                    this.hang_check = false;
                    this.astar_upstate = true;
                    break;

                case 4:
                    if (this.squareInfo[4] < 188f)
                    {
                        this.currentAction = this.down_action(this.diff_dist(this.squareInfo[1] - (this.squareInfo[4] / 2f), (this.squareInfo[1] + (this.squareInfo[4] / 2f)) - 192f));
                    }
                    else
                    {
                        this.max_upstate = false;
                        this.down_Realcheck = false;
                    }
                    break;

                case 5:
                    if (this.squareInfo[4] > 53f)
                    {
                        this.currentAction = this.down_action(this.diff_dist(this.squareInfo[1] - (this.squareInfo[4] / 2f), (this.squareInfo[1] + (this.squareInfo[4] / 2f)) - 53f));
                    }
                    else
                    {
                        this.max_downstate = false;
                    }
                    break;

                default:
                    break;
            }
            this.pre_dist = this.now_dist;
            this.pre_velocity = this.now_velocity;
            this.lastAction = this.currentAction;
        }

        private bool block(int y)
        {
            bool flag = false;
            int num = this.x_left;
            int num2 = this.x_right;
            int num3 = 0;
            int num4 = 0;
            if (this.x_left < this.x)
            {
                num++;
            }
            if (this.x < this.x_right)
            {
                num2--;
            }
            for (int i = num; i <= num2; i++)
            {
                if (AStar.Nodes[i][y].obstacle)
                {
                    num3 = i;
                    num4++;
                    flag = true;
                }
            }
            if ((num4 != 1) || (num2 != (num + 2)))
            {
                this.block_goX = 0;
            }
            else if (num3 == num)
            {
                this.block_goX = num2;
            }
            else if (num3 == num2)
            {
                this.block_goX = num;
            }
            if (((this.dest_x / this.cellsize) > num3) && (this.block_goX > num3))
            {
                this.block_goX = this.update_coor(this.block_goX);
            }
            else if (((this.dest_x / this.cellsize) >= num3) || (this.block_goX >= num3))
            {
                this.block_goX = 0;
            }
            else
            {
                this.block_goX = this.update_coor(this.block_goX);
            }
            return flag;
        }

        private void break_motion()
        {
            if ((this.currentAction == 5) || (this.currentAction == 6))
            {
                if ((Math.Abs(this.now_velocity) >= 0.3) || (this.lastAction != this.currentAction))
                {
                    this.break_Lcount = 0;
                    this.break_Rcount = 0;
                    this.break_Mcount = 0;
                }
                else
                {
                    if (this.currentAction == 5)
                    {
                        this.break_Lcount++;
                    }
                    if (this.currentAction == 6)
                    {
                        this.break_Rcount++;
                    }
                }
                if (this.break_Lcount >= 15)
                {
                    goto TR_0000;
                }
                else if (this.break_Rcount < 15)
                {
                    if (Math.Abs(this.now_velocity) > 0.7)
                    {
                        this.break_state = false;
                    }
                }
                else
                {
                    goto TR_0000;
                }
            }
            return;
        TR_0000:
            this.break_state = true;
        }

        private void check()
        {
            this.bottom = this.get_maxyNode() + 1;
            int num = 0;
            if (!this.down_check && (this.bottom < Board.yCellsNum))
            {
                for (int i = 0; i < Board.xCellsNum; i++)
                {
                    if (!AStar.Nodes[i][this.bottom].obstacle)
                    {
                        num++;
                    }
                    if (num > 3)
                    {
                        break;
                    }
                }
            }
            if ((0 >= num) || (num >= 3))
            {
                this.down_check = false;
            }
            else
            {
                this.down_check = true;
            }
            this.down_Realcheck = this.squareInfo[4] <= 54f;
            if ((!this.obstacle_Lbottom && !this.obstacle_Rbottom) && ((this.cnt - this.hang_time) >= 130))
            {
                this.hang_check = false;
            }
            else if ((this.cnt - this.hang_time) >= 200)
            {
                this.hang_check = false;
            }
        }

        private void check_surround()
        {
            float num = 5000f / this.squareInfo[4];
            this.right = ((int) ((this.squareInfo[0] + num) + 10f)) / this.cellsize;
            this.left = ((int) ((this.squareInfo[0] - num) - 10f)) / this.cellsize;
            this.obstacle_Lbottom = false;
            this.obstacle_Rbottom = false;
            this.obstacle_Rup = false;
            this.obstacle_Lup = false;
            this.obstacle_Rtop = false;
            this.obstacle_Ltop = false;
            if ((this.right != 0x1f) && AStar.Nodes[this.right][this.y + 1].obstacle)
            {
                this.obstacle_Rtop = true;
            }
            if ((this.left != 0) && AStar.Nodes[this.left][this.y + 1].obstacle)
            {
                this.obstacle_Ltop = true;
            }
            for (int i = this.y; i < this.y_max; i++)
            {
                if ((this.right != 0x1f) && AStar.Nodes[this.right][i].obstacle)
                {
                    this.obstacle_Rup = true;
                }
                if ((this.left != 0) && AStar.Nodes[this.left][i].obstacle)
                {
                    this.obstacle_Lup = true;
                }
            }
            if ((this.right != 0x1f) && AStar.Nodes[this.right][this.y_max].obstacle)
            {
                this.obstacle_Rbottom = true;
            }
            if ((this.left != 0) && AStar.Nodes[this.left][this.y_max].obstacle)
            {
                this.obstacle_Lbottom = true;
            }
        }

        public Vector4 Col_obs(float in_x, float in_y)
        {
            using (List<Vector4>.Enumerator enumerator = this.obs.GetEnumerator())
            {
                while (true)
                {
                    if (!enumerator.MoveNext())
                    {
                        break;
                    }
                    Vector4 current = enumerator.Current;
                    float x = current.X;
                    float y = current.Y;
                    float num3 = current.Z / 2f;
                    float num4 = current.W / 2f;
                    if (((x - num3) <= in_x) && ((in_x <= (x + num3)) && (((y - num4) <= in_y) && (in_y <= (y + num4)))))
                    {
                        return current;
                    }
                }
            }
            return new Vector4();
        }

        protected void DebugSensorsInfo()
        {
            int num = 0;
            Console.WriteLine("SQUARE - collectibles left - {0}", this.nCollectiblesLeft);
            Console.WriteLine("SQUARE - collectibles info size - {0}", this.collectiblesInfo.Count<float>());
            num = 0;
            foreach (float num2 in this.collectiblesInfo)
            {
                Console.WriteLine("SQUARE - Collectibles info - {0} - {1}", num, num2);
                num++;
            }
        }

        private int decide_action(float diff_dist)
        {
            int num = 0;
            switch (this.direction_switch)
            {
                case 1:
                    return ((diff_dist < num) ? ((diff_dist > -num) ? 0 : 5) : 6);

                case 2:
                case 3:
                    return ((diff_dist < num) ? ((diff_dist > -num) ? 0 : 7) : 8);
            }
            return 0;
        }

        private void detail_direct()
        {
            if (Math.Abs((float) (this.squareInfo[0] - this.dest_x)) > Math.Abs((float) ((this.squareInfo[1] - (this.squareInfo[4] / 2f)) - this.dest_y)))
            {
                this.direction_switch = 1;
            }
            else
            {
                this.direction_switch = 3;
            }
        }

        private float diff_dist(float start_y, float end_y) => 
            (end_y - start_y);

        private float diff_dist(float start_x, float end_x, double predict_dist)
        {
            float num = (float) predict_dist;
            return (end_x - (start_x + num));
        }

        private int down_action(float diff_dist)
        {
            int num = 1;
            return ((diff_dist <= num) ? ((diff_dist >= -num) ? 0 : 7) : 8);
        }

        private int get_LxNode()
        {
            int num = ((int) (this.squareInfo[0] - (5000f / this.squareInfo[4]))) / this.cellsize;
            if (((this.squareInfo[0] - (5000f / this.squareInfo[4])) % ((float) this.cellsize)) > 20f)
            {
                num++;
            }
            return num;
        }

        private int get_maxyNode()
        {
            int index = ((int) (this.squareInfo[1] + (this.squareInfo[4] / 2f))) / this.cellsize;
            if (index >= Board.yCellsNum)
            {
                index = 0x12;
            }
            if (AStar.Nodes[this.get_xNode()][index].obstacle || AStar.Nodes[this.get_RxNode()][index].obstacle)
            {
                index--;
            }
            return index;
        }

        private int get_RxNode()
        {
            int num = ((int) (this.squareInfo[0] + (5000f / this.squareInfo[4]))) / this.cellsize;
            if (((this.squareInfo[0] + (5000f / this.squareInfo[4])) % ((float) this.cellsize)) < 20f)
            {
                num--;
            }
            return num;
        }

        private int get_xNode() => 
            (((int) this.squareInfo[0]) / this.cellsize);

        private int get_yDNode() => 
            (((int) (this.squareInfo[1] + (this.squareInfo[4] / 2f))) / this.cellsize);

        private int get_yNode() => 
            (((int) (this.squareInfo[1] - (this.squareInfo[4] / 2f))) / this.cellsize);

        public int GetAction() => 
            this.currentAction;

        private void hang()
        {
            bool flag = false;
            bool flag2 = false;
            if ((this.currentAction == 5) && (this.lastAction == this.currentAction))
            {
                if (this.obstacle_Lup)
                {
                    flag = true;
                }
                else if (this.obstacle_Lbottom)
                {
                    flag2 = true;
                }
            }
            else if ((this.currentAction == 6) && (this.lastAction == this.currentAction))
            {
                if (this.obstacle_Rup)
                {
                    flag = true;
                }
                else if (this.obstacle_Rbottom)
                {
                    flag2 = true;
                }
            }
            if (flag)
            {
                if (this.squareInfo[4] > 53f)
                {
                    this.currentAction = this.down_action(this.diff_dist(this.squareInfo[1] - (this.squareInfo[4] / 2f), (this.squareInfo[1] + (this.squareInfo[4] / 2f)) - 52f));
                    this.down_check = true;
                    this.down_Realcheck = false;
                }
            }
            else if (flag2)
            {
                if (this.squareInfo[4] <= 190f)
                {
                    this.currentAction = this.down_action(this.diff_dist(this.squareInfo[1] - (this.squareInfo[4] / 2f), (this.squareInfo[1] + (this.squareInfo[4] / 2f)) - 192f));
                    this.astar_upstate = true;
                    this.down_Realcheck = false;
                    this.hang_check = true;
                    this.hang_time = this.cnt;
                }
                else
                {
                    this.hang_check = false;
                    this.astar_upstate = false;
                }
            }
        }

        public bool ImplementedAgent() => 
            this.implementedAgent;

        private void PathFinder()
        {
            node startNode = this.S_astar.checkStartNode(this.squareInfo);
            this.S_astar.getUpdateEndNode(this.collectiblesInfo);
            if (AStar.endNodes.Count != 0)
            {
                this.nearest = new List<node>();
                List<List<node>> list = new List<List<node>>();
                int num = 0;
                while (true)
                {
                    if (num >= AStar.endNodes.Count)
                    {
                        this.nearest = list[0];
                        int num2 = 0;
                        while (true)
                        {
                            if (num2 >= AStar.endNodes.Count)
                            {
                                for (int i = 0; i < this.collectiblesInfo.Length; i += 2)
                                {
                                    this.x = ((int) this.collectiblesInfo[i]) / this.cellsize;
                                    this.y = ((int) this.collectiblesInfo[i + 1]) / this.cellsize;
                                    if ((this.x == this.nearest[0].x) && (this.y == this.nearest[0].y))
                                    {
                                        this.dia_index = i;
                                        this.dia_Rx = this.collectiblesInfo[i];
                                        this.dia_Ry = this.collectiblesInfo[i + 1];
                                        this.dia_x = this.x;
                                        this.dia_y = this.y;
                                        return;
                                    }
                                }
                                break;
                            }
                            if (this.nearest[0].f >= list[num2][0].f)
                            {
                                this.nearest = list[num2];
                            }
                            num2++;
                        }
                        break;
                    }
                    Board.Acount++;
                    this.S_astar.UpdateNew(AStar.endNodes[num][0], AStar.endNodes[num][1]);
                    list.Add(this.S_astar.AStarFinder(startNode, AStar.endNodes[num][0], AStar.endNodes[num][1]));
                    num++;
                }
            }
        }

        private double predict(float velocity)
        {
            double num = 0.0;
            num = (33f * velocity) * velocity;
            return ((velocity > 0f) ? num : -num);
        }

        private void RandomAction()
        {
            this.currentAction = this.rnd.Next(5, 9);
            if (this.currentAction == this.lastAction)
            {
                this.currentAction = (this.currentAction != 8) ? (this.currentAction + 1) : this.rnd.Next(5, 8);
            }
            switch (this.currentAction)
            {
                case 5:
                    this.SetAction(Moves.MOVE_LEFT);
                    break;

                case 6:
                    this.SetAction(Moves.MOVE_RIGHT);
                    break;

                case 7:
                    this.SetAction(Moves.MORPH_UP);
                    break;

                case 8:
                    this.SetAction(Moves.MORPH_DOWN);
                    break;

                default:
                    break;
            }
            using (StreamWriter writer = new StreamWriter(@"C:\test\square_F.txt", true))
            {
                writer.WriteLine(this.currentAction);
                object[] arg = new object[] { this.squareInfo[0], this.squareInfo[1], this.squareInfo[2], this.squareInfo[3] };
                writer.WriteLine("{1},{2},{3},{4}", arg);
                writer.WriteLine("accel:{0}", this.squareInfo[2] - this.pre_velocity);
            }
        }

        private void SetAction(int a)
        {
            this.currentAction = a;
        }

        public void setAgentPane(AgentDebugPane aP)
        {
        }

        private void SetImplementedAgent(bool b)
        {
            this.implementedAgent = b;
        }

        public void Setup(int[] nI, float[] sI, float[] cI, float[] oI, float[] sPI, float[] cPI, float[] colI, Rectangle a)
        {
            this.area = a;
            this.numbersInfo = new int[4];
            for (int i = 0; i < nI.Length; i++)
            {
                this.numbersInfo[i] = nI[i];
            }
            this.nCollectiblesLeft = nI[3];
            this.collect_count = this.nCollectiblesLeft;
            this.squareInfo = new float[] { sI[0], sI[1], sI[2], sI[3], sI[4] };
            this.circleInfo = new float[] { cI[0], cI[1], cI[2], cI[3], cI[4] };
            this.obstaclesInfo = (this.numbersInfo[0] <= 0) ? new float[4] : new float[this.numbersInfo[0] * 4];
            int num = 1;
            if (nI[0] <= 0)
            {
                this.obstaclesInfo[0] = oI[0];
                this.obstaclesInfo[1] = oI[1];
                this.obstaclesInfo[2] = oI[2];
                this.obstaclesInfo[3] = oI[3];
            }
            else
            {
                while (num <= nI[0])
                {
                    this.obstaclesInfo[(num * 4) - 4] = oI[(num * 4) - 4];
                    this.obstaclesInfo[(num * 4) - 3] = oI[(num * 4) - 3];
                    this.obstaclesInfo[(num * 4) - 2] = oI[(num * 4) - 2];
                    this.obstaclesInfo[(num * 4) - 1] = oI[(num * 4) - 1];
                    num++;
                }
            }
            this.squarePlatformsInfo = (this.numbersInfo[1] <= 0) ? new float[4] : new float[this.numbersInfo[1] * 4];
            num = 1;
            if (nI[1] <= 0)
            {
                this.squarePlatformsInfo[0] = sPI[0];
                this.squarePlatformsInfo[1] = sPI[1];
                this.squarePlatformsInfo[2] = sPI[2];
                this.squarePlatformsInfo[3] = sPI[3];
            }
            else
            {
                while (num <= nI[1])
                {
                    this.squarePlatformsInfo[(num * 4) - 4] = sPI[(num * 4) - 4];
                    this.squarePlatformsInfo[(num * 4) - 3] = sPI[(num * 4) - 3];
                    this.squarePlatformsInfo[(num * 4) - 2] = sPI[(num * 4) - 2];
                    this.squarePlatformsInfo[(num * 4) - 1] = sPI[(num * 4) - 1];
                    num++;
                }
            }
            this.circlePlatformsInfo = (this.numbersInfo[2] <= 0) ? new float[4] : new float[this.numbersInfo[2] * 4];
            num = 1;
            if (nI[2] <= 0)
            {
                this.circlePlatformsInfo[0] = cPI[0];
                this.circlePlatformsInfo[1] = cPI[1];
                this.circlePlatformsInfo[2] = cPI[2];
                this.circlePlatformsInfo[3] = cPI[3];
            }
            else
            {
                while (num <= nI[2])
                {
                    this.circlePlatformsInfo[(num * 4) - 4] = cPI[(num * 4) - 4];
                    this.circlePlatformsInfo[(num * 4) - 3] = cPI[(num * 4) - 3];
                    this.circlePlatformsInfo[(num * 4) - 2] = cPI[(num * 4) - 2];
                    this.circlePlatformsInfo[(num * 4) - 1] = cPI[(num * 4) - 1];
                    num++;
                }
            }
            this.collectiblesInfo = new float[this.numbersInfo[3] * 2];
            for (num = 1; num <= nI[3]; num++)
            {
                this.collectiblesInfo[(num * 2) - 2] = colI[(num * 2) - 2];
                this.collectiblesInfo[(num * 2) - 1] = colI[(num * 2) - 1];
            }
            this.DebugSensorsInfo();
            this.start_point = this.squareInfo[0];
            this.S_astar.Access();
            this.S_astar.getBoardValue(this.obstaclesInfo);
            this.cellsize = Board.cellMinSize;
            this.PathFinder();
            this.S_astar.a_cnt++;
        }

        private void Sqaure_Action()
        {
            if ((this.cnt % 5) == 0)
            {
                this.check_surround();
            }
            if (this.nCollectiblesLeft != this.collect_count)
            {
                this.PathFinder();
                this.collect_count = this.nCollectiblesLeft;
            }
            else if (!this.down_check || this.down_Realcheck)
            {
                this.AstarAction();
            }
            else if (this.astar_upstate || this.hang_check)
            {
                this.AstarAction();
            }
            else if (this.squareInfo[4] > 53f)
            {
                this.currentAction = this.down_action(this.diff_dist(this.squareInfo[1] - (this.squareInfo[4] / 2f), (this.squareInfo[1] + (this.squareInfo[4] / 2f)) - 52f));
            }
            else
            {
                this.down_Realcheck = true;
            }
            if ((this.cnt % 7) == 0)
            {
                this.break_motion();
            }
            if (this.break_state)
            {
                this.break_Mcount++;
                switch (this.currentAction)
                {
                    case 5:
                        this.currentAction = Moves.MOVE_RIGHT;
                        return;

                    case 6:
                        this.currentAction = Moves.MOVE_LEFT;
                        break;

                    default:
                        return;
                }
            }
        }

        public void toggleDebug()
        {
        }

        public void Update(TimeSpan elapsedGameTime)
        {
            this.Sqaure_Action();
            if (this.lastMoveTime == 60)
            {
                this.lastMoveTime = 0L;
            }
            if ((this.lastMoveTime <= DateTime.Now.Second) && (this.lastMoveTime < 60))
            {
                if (DateTime.Now.Second == 0x3b)
                {
                    this.lastMoveTime = 60;
                }
                else
                {
                    this.check();
                    this.lastMoveTime += 1L;
                    if (this.astar_check)
                    {
                        this.PathFinder();
                        this.astar_check = false;
                    }
                }
            }
        }

        private int update_coor(int input) => 
            ((this.cellsize * input) + (this.cellsize / 2));

        private void update_direct()
        {
            this._x = this.dest_x / this.cellsize;
            this._y = this.dest_y / this.cellsize;
            if ((this.x == this._x) && (this.y == this._y))
            {
                this.detail_direct();
            }
            else if ((this.y < this._y) && (this.x == this._x))
            {
                this.direction_switch = 2;
            }
            else if ((this.y <= this._y) && (this._y <= this.y_max))
            {
                this.direction_switch = 1;
            }
            else if ((this.y > this._y) && (this.x == this._x))
            {
                this.direction_switch = 3;
            }
            else if ((this.x_left <= this._x) && (this._x <= this.x_right))
            {
                if (this.y < this._y)
                {
                    this.direction_switch = 2;
                }
                else if (this.y > this._y)
                {
                    this.direction_switch = 3;
                }
            }
            if (this.max_downstate)
            {
                this.direction_switch = 5;
            }
            if (this.max_upstate)
            {
                this.direction_switch = 4;
            }
        }

        private void update_path()
        {
            this.x = this.get_xNode();
            this.y = this.get_yNode();
            this.y_max = this.get_maxyNode();
            this.x_right = this.get_RxNode();
            this.x_left = this.get_LxNode();
            int num = this.nearest.Count - 1;
            if (this.nearest.Count != 0)
            {
                int x = this.nearest[num].x;
                int y = this.nearest[num].y;
                int num4 = this.nearest[this.nearest.Count - 1].x;
                int num5 = this.nearest[this.nearest.Count - 1].y;
                bool flag = false;
                bool flag2 = false;
                bool flag3 = false;
                if (this.nearest[num].x != 0x7cf)
                {
                    if (((this.x == x) && (this.y <= y)) && (y <= this.y_max))
                    {
                        flag = true;
                        if (num > 0)
                        {
                            for (int i = this.nearest.Count - 2; ((i >= 0) && ((this.x == this.nearest[i].x) && (this.y <= this.nearest[i].y))) && (this.nearest[i].y <= this.y_max); i--)
                            {
                                this.dest_x = this.update_coor(this.nearest[i].x);
                                this.dest_y = this.update_coor(this.nearest[i].y);
                                this.nearest.Remove(this.nearest[i + 1]);
                                num4 = this.nearest[this.nearest.Count - 1].x;
                                num5 = this.nearest[this.nearest.Count - 1].y;
                            }
                        }
                        if (num > 0)
                        {
                            int num7 = this.nearest.Count - 2;
                            while (true)
                            {
                                if ((num7 < 0) || (num5 != this.nearest[num7].y))
                                {
                                    if (!flag3)
                                    {
                                        for (int i = this.nearest.Count - 2; (i >= 0) && (num4 == this.nearest[i].x); i--)
                                        {
                                            this.dest_x = this.update_coor(this.nearest[i].x);
                                            this.dest_y = this.update_coor(this.nearest[i].y);
                                            this.nearest.Remove(this.nearest[i + 1]);
                                        }
                                    }
                                    break;
                                }
                                this.dest_x = this.update_coor(this.nearest[num7].x);
                                this.dest_y = this.update_coor(this.nearest[num7].y);
                                flag3 = true;
                                this.nearest.Remove(this.nearest[num7 + 1]);
                                num7--;
                            }
                        }
                        if (num == (this.nearest.Count - 1))
                        {
                            if (num > 0)
                            {
                                this.dest_x = this.update_coor(this.nearest[num - 1].x);
                                this.dest_y = this.update_coor(this.nearest[num - 1].y);
                            }
                            this.nearest.Remove(this.nearest[num]);
                        }
                    }
                    num = this.nearest.Count - 1;
                    num4 = this.nearest[this.nearest.Count - 1].x;
                    num5 = this.nearest[this.nearest.Count - 1].y;
                    if (((this.x_left <= num4) && ((num4 <= this.x_right) && ((num4 != this.x) && (this.y <= num5)))) && (num5 <= this.y_max))
                    {
                        flag2 = true;
                        if (num > 0)
                        {
                            for (int i = this.nearest.Count - 2; ((i >= 0) && ((this.x_left <= this.nearest[i + 1].x) && ((this.nearest[i + 1].x <= this.x_right) && ((this.nearest[i + 1].x != this.x) && ((this.y <= this.nearest[i + 1].y) && (this.nearest[i + 1].y <= this.y_max)))))) && (this.nearest[i].x == this.nearest[i + 1].x); i--)
                            {
                                this.dest_x = this.update_coor(this.nearest[i].x);
                                this.dest_y = this.update_coor(this.nearest[i].y);
                                this.nearest.Remove(this.nearest[i + 1]);
                            }
                        }
                    }
                    if (!flag && !flag2)
                    {
                        this.dest_x = this.update_coor(x);
                        this.dest_y = this.update_coor(y);
                    }
                }
                if ((this.nearest.Count == 1) && (this.collectiblesInfo.Length != 0))
                {
                    this.dest_x = this.update_coor(this.nearest[0].x);
                    this.dest_y = this.update_coor(this.nearest[0].y);
                    this.nearest.Remove(this.nearest[0]);
                }
                this.collect_count = this.nCollectiblesLeft;
            }
        }

        public void UpdateSensors(int nC, float[] sI, float[] cI, float[] colI)
        {
            this.nCollectiblesLeft = nC;
            this.squareInfo[0] = sI[0];
            this.squareInfo[1] = sI[1];
            this.squareInfo[2] = sI[2];
            this.squareInfo[3] = sI[3];
            this.squareInfo[4] = sI[4];
            this.circleInfo[0] = cI[0];
            this.circleInfo[1] = cI[1];
            this.circleInfo[2] = cI[2];
            this.circleInfo[3] = cI[3];
            this.circleInfo[4] = cI[4];
            Array.Resize<float>(ref this.collectiblesInfo, this.nCollectiblesLeft * 2);
            for (int i = 1; i <= this.nCollectiblesLeft; i++)
            {
                this.collectiblesInfo[(i * 2) - 2] = colI[(i * 2) - 2];
                this.collectiblesInfo[(i * 2) - 1] = colI[(i * 2) - 1];
            }
        }
    }
}

