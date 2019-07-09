using GeometryFriends;
using GeometryFriends.AI;
using GeometryFriends.AI.Communication;
using GeometryFriends.AI.Interfaces;
using GeometryFriends.AI.Perceptions.Information;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace GeometryFriendsAgents
{
    /// <summary>
    /// A rectangle agent implementation for the GeometryFriends game using A-Star search
    /// </summary>
    public class RectangleAgent : AbstractRectangleAgent
    {
        //agent implementation specificiation
        private bool implementedAgent;
        private string agentName = "AStarAgent";

        //auxiliary variables for agent action
        private Moves lastAction;
        private Moves currentAction;
        private long lastMoveTime;

        //Sensors Information
        private CountInformation numbersInfo;
        private RectangleRepresentation rectangleInfo;
        private CircleRepresentation circleInfo;
        private ObstacleRepresentation[] obstaclesInfo;
        private ObstacleRepresentation[] rectanglePlatformsInfo;
        private ObstacleRepresentation[] circlePlatformsInfo;
        private CollectibleRepresentation[] collectiblesInfo;

        private int nCollectiblesLeft;

        //Area of the game screen
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
        private AStar astar = new AStar();
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
        

        public RectangleAgent()
        {
            //Change flag if agent is not to be used
            implementedAgent = true;

            lastMoveTime = DateTime.Now.Second;
            lastAction = Moves.NO_ACTION;
            currentAction = Moves.NO_ACTION;

            astar.a_cnt++;

            if (File.Exists(@"C:\test\testPrint.txt"))
            {
                File.Delete(@"C:\test\testPrint.txt");
            }
        }

		//implements abstract rectangle interface: provides the name of the agent to the agents manager in GeometryFriends
		public override string AgentName()
		{
			return agentName;
		}

		private void AstarAction()
		{
			this.cnt++;

			this.getTopNodeYIndex();

			this.now_dist = this.rectangleInfo.X - this.start_point;
			this.now_velocity = this.now_dist - this.pre_dist;
			this.update_path();
			this.update_direct();

			switch (this.direction_switch)
			{
				case 1:
					this.currentAction = this.decide_action(this.diff_dist(this.rectangleInfo.X, (float)this.dest_x, this.predict(this.now_velocity)));

					this.astar_upstate = false;
					if ((this.obstacle_Lbottom || (this.obstacle_Lup || this.obstacle_Rbottom)) || this.obstacle_Rup)
					{
						this.hang();
					}
					break;

				case 2:
					if (this.down_check && this.down_Realcheck)
					{
						if ((((this.rectangleInfo.X - 1f) > this.dest_x) || (this.dest_x > (this.rectangleInfo.X + 1f))) || ((this.dest_y / this.cellsize) == this.y_max))
						{
							this.direction_switch = 1;
							this.currentAction = this.decide_action(this.diff_dist(this.rectangleInfo.X, (float)this.dest_x, this.predict(this.now_velocity)));
						}
						else
						{
							this.currentAction = this.down_action(this.diff_dist(this.rectangleInfo.Y - (this.rectangleInfo.Height / 2f), (this.rectangleInfo.Y + (this.rectangleInfo.Height / 2f)) - 192f));
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
					this.up = ((int)(this.rectangleInfo.Y - ((this.rectangleInfo.Height / 2f) + 16f))) / this.cellsize;
					if (!this.down_check || !this.down_Realcheck)
					{
						if (this.block(this.up))
						{
							this.direction_switch = 1;
							this.currentAction = (this.block_goX == 0) ? this.decide_action(this.diff_dist(this.rectangleInfo.X, (float)this.dest_x, this.predict(this.now_velocity))) : this.decide_action(this.diff_dist(this.rectangleInfo.X, (float)this.block_goX, this.predict(this.now_velocity)));
						}
						else
						{
							this.currentAction = this.decide_action(this.diff_dist(this.rectangleInfo.Y - (this.rectangleInfo.Height / 2f), (float)this.dest_y));
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
						this.currentAction = this.decide_action(this.diff_dist(this.rectangleInfo.Y - (this.rectangleInfo.Height / 2f), (float)this.dest_y));
						this.down_Realcheck = false;
					}
					this.hang_check = false;
					this.astar_upstate = true;
					break;

				case 4:
					if (this.rectangleInfo.Height < 188f)
					{
						this.currentAction = this.down_action(this.diff_dist(this.rectangleInfo.Y - (this.rectangleInfo.Height / 2f), (this.rectangleInfo.Y + (this.rectangleInfo.Height / 2f)) - 192f));
					}
					else
					{
						this.max_upstate = false;
						this.down_Realcheck = false;
					}
					break;

				case 5:
					if (this.rectangleInfo.Height > 53f)
					{
						this.currentAction = this.down_action(this.diff_dist(this.rectangleInfo.Y - (this.rectangleInfo.Height / 2f), (this.rectangleInfo.Y + (this.rectangleInfo.Height / 2f)) - 53f));
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
			if ((this.currentAction == Moves.MOVE_LEFT) || (this.currentAction == Moves.MOVE_RIGHT))
			{
				if ((Math.Abs(this.now_velocity) >= 0.3) || (this.lastAction != this.currentAction))
				{
					this.break_Lcount = 0;
					this.break_Rcount = 0;
					this.break_Mcount = 0;
				}
				else
				{
					if (this.currentAction == Moves.MOVE_LEFT)
					{
						this.break_Lcount++;
					}
					if (this.currentAction == Moves.MOVE_RIGHT)
					{
						this.break_Rcount++;
					}
				}
				if (this.break_Lcount >= 15)
				{
					goto BreakState;
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
					goto BreakState;
				}
			}
			return;
		BreakState:
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
			this.down_Realcheck = this.rectangleInfo.Height <= 54f;
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
			float num = 5000f / this.rectangleInfo.Height;
			this.right = ((int)((this.rectangleInfo.X + num) + 10f)) / this.cellsize;
			this.left = ((int)((this.rectangleInfo.X - num) - 10f)) / this.cellsize;
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

		private Moves decide_action(float diff_dist)
		{
			int delta = 0;
			switch (this.direction_switch)
			{
				case 1:
					return ((diff_dist < delta) ? ((diff_dist > -delta) ? Moves.NO_ACTION : Moves.MOVE_LEFT) : Moves.MOVE_RIGHT);

				case 2:
				case 3:
					return ((diff_dist < delta) ? ((diff_dist > -delta) ? Moves.NO_ACTION : Moves.MORPH_UP) : Moves.MORPH_DOWN);
			}
			return Moves.NO_ACTION;
		}

		private void detail_direct()
		{
			if (Math.Abs((float)(this.rectangleInfo.X - this.dest_x)) > Math.Abs((float)((this.rectangleInfo.Y - (this.rectangleInfo.Height / 2f)) - this.dest_y)))
			{
				this.direction_switch = 1;
			}
			else
			{
				this.direction_switch = 3;
			}
		}

		private float diff_dist(float start_y, float end_y)
		{
			return (end_y - start_y);
		}

		private float diff_dist(float start_x, float end_x, double predict_dist)
		{
			float num = (float)predict_dist;
			return (end_x - (start_x + num));
		}

		private Moves down_action(float diff_dist)
		{
			int delta = 1;
			return ((diff_dist <= delta) ? ((diff_dist >= -delta) ? 0 : Moves.MORPH_UP) : Moves.MORPH_DOWN);
		}

		private int get_LxNode()
		{
			int num = ((int)(this.rectangleInfo.X - (5000f / this.rectangleInfo.Height))) / this.cellsize;
			if (((this.rectangleInfo.X - (5000f / this.rectangleInfo.Height)) % ((float)this.cellsize)) > this.cellsize / 2)
			{
				num++;
			}
			return num;
		}

		private int get_maxyNode()
		{
			int index = ((int)(this.rectangleInfo.Y + (this.rectangleInfo.Height / 2f))) / this.cellsize;
			if (index >= Board.yCellsNum)
			{
				index = 18;
			}
			if (AStar.Nodes[this.getNodeXIndex()][index].obstacle || AStar.Nodes[this.get_RxNode()][index].obstacle)
			{
				index--;
			}
			return index;
		}

		private int get_RxNode()
		{
			int num = ((int)(this.rectangleInfo.X + (5000.0 / this.rectangleInfo.Height))) / this.cellsize;
			if(((this.rectangleInfo.X + (5000.0 / this.rectangleInfo.Height)) % ((float)this.cellsize)) < this.cellsize / 2)
			{
				num--;
			}
			return num;
		}

		private int getNodeXIndex()
		{
			return (((int)this.rectangleInfo.X) / this.cellsize);
		}
		private int getTopNodeYIndex()
		{
			return (((int)(this.rectangleInfo.Y + (this.rectangleInfo.Height / 2.0))) / this.cellsize);
		}
		private int getBottomNodeYIndex()
		{
			return (((int)(this.rectangleInfo.Y - (this.rectangleInfo.Height / 2.0))) / this.cellsize);
		}
		private void hang()
		{
			bool flag = false;
			bool flag2 = false;
			if ((this.currentAction == Moves.MOVE_LEFT) && (this.lastAction == this.currentAction))
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
			else if ((this.currentAction == Moves.MOVE_RIGHT) && (this.lastAction == this.currentAction))
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
				if (this.rectangleInfo.Height > 53f)
				{
					this.currentAction = this.down_action(this.diff_dist(this.rectangleInfo.Y - (this.rectangleInfo.Height / 2f), (this.rectangleInfo.Y + (this.rectangleInfo.Height / 2f)) - 52f));
					this.down_check = true;
					this.down_Realcheck = false;
				}
			}
			else if (flag2)
			{
				if (this.rectangleInfo.Height <= 190f)
				{
					this.currentAction = this.down_action(this.diff_dist(this.rectangleInfo.Y - (this.rectangleInfo.Height / 2f), (this.rectangleInfo.Y + (this.rectangleInfo.Height / 2f)) - 192f));
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

		private void PathFinder()
		{
			node startNode = this.astar.checkStartNode(this.rectangleInfo);
			this.astar.getUpdateEndNode(this.collectiblesInfo);

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

								for (int i = 0; i < this.collectiblesInfo.Length; i++)
								{
									CollectibleRepresentation info = this.collectiblesInfo[i];

									this.x = ((int)info.X) / this.cellsize;
									this.y = ((int)info.Y) / this.cellsize;

									if ((this.x == this.nearest[0].x) && (this.y == this.nearest[0].y))
									{
										this.dia_index = i;
										this.dia_Rx = info.X;
										this.dia_Ry = info.Y;
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

					this.astar.UpdateNew(AStar.endNodes[num][0], AStar.endNodes[num][1]);

					list.Add(this.astar.AStarFinder(startNode, AStar.endNodes[num][0], AStar.endNodes[num][1]));

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

		private void rectangleAction()
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
			else if (this.rectangleInfo.Height > 53f)
			{
				this.currentAction = this.down_action(this.diff_dist(this.rectangleInfo.Y - (this.rectangleInfo.Height / 2f), (this.rectangleInfo.Y + (this.rectangleInfo.Height / 2f)) - 52f));
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
					case Moves.MOVE_LEFT:
						this.currentAction = Moves.MOVE_RIGHT;
						return;

					case Moves.MOVE_RIGHT:
						this.currentAction = Moves.MOVE_LEFT;
						break;

					default:
						return;
				}
			}
		}

		private int update_coor(int input)
		{
			return ((this.cellsize * input) + (this.cellsize / 2));
		}

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
			this.x = this.getNodeXIndex();
			this.y = this.getBottomNodeYIndex();
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






		//implements abstract rectangle interface: used to setup the initial information so that the agent has basic knowledge about the level
		public override void Setup(CountInformation nI, RectangleRepresentation rI, CircleRepresentation cI, ObstacleRepresentation[] oI, ObstacleRepresentation[] rPI, ObstacleRepresentation[] cPI, CollectibleRepresentation[] colI, Rectangle area, double timeLimit)
        {
            numbersInfo = nI;
            nCollectiblesLeft = nI.CollectiblesCount;
			this.collect_count = this.nCollectiblesLeft;
			rectangleInfo = rI;
            circleInfo = cI;
            obstaclesInfo = oI;
            rectanglePlatformsInfo = rPI;
            circlePlatformsInfo = cPI;
            collectiblesInfo = colI;
            this.area = area;

			//DebugSensorsInfo();

			this.start_point = this.rectangleInfo.X;
			this.astar.Access();
			this.astar.getBoardValue(this.obstaclesInfo);
			this.cellsize = Board.cellMinSize;
			this.PathFinder();
			this.astar.a_cnt++;
		}

        //implements abstract rectangle interface: registers updates from the agent's sensors that it is up to date with the latest environment information
        public override void SensorsUpdated(int nC, RectangleRepresentation rI, CircleRepresentation cI, CollectibleRepresentation[] colI)
        {
            nCollectiblesLeft = nC;

            rectangleInfo = rI;
            circleInfo = cI;
            collectiblesInfo = colI;
        }

        //implements abstract rectangle interface: signals if the agent is actually implemented or not
        public override bool ImplementedAgent()
        {
            return implementedAgent;
        }

        //implements abstract rectangle interface: GeometryFriends agents manager gets the current action intended to be actuated in the enviroment for this agent
        public override Moves GetAction()
        {
            return currentAction;
        }

        //implements abstract rectangle interface: updates the agent state logic and predictions
        public override void Update(TimeSpan elapsedGameTime)
        {
			this.rectangleAction();

            if (lastMoveTime == 60)
                lastMoveTime = 0;

            if ((lastMoveTime) <= (DateTime.Now.Second) && (lastMoveTime < 60))
            {
                if (!(DateTime.Now.Second == 59))
                {
					this.check();
                    lastMoveTime += 1;

					if(this.astar_check){

						this.PathFinder();
						this.astar_check = false;
					}
                }
                else
                    lastMoveTime = 60;
            }
        }

        //typically used console debugging used in previous implementations of GeometryFriends
        protected void DebugSensorsInfo()
        {
            Log.LogInformation("Rectangle Aagent - " + numbersInfo.ToString());

            Log.LogInformation("Rectangle Aagent - " + rectangleInfo.ToString());

            Log.LogInformation("Rectangle Aagent - " + circleInfo.ToString());

            foreach (ObstacleRepresentation i in obstaclesInfo)
            {
                Log.LogInformation("Rectangle Aagent - " + i.ToString("Obstacle"));
            }

            foreach (ObstacleRepresentation i in rectanglePlatformsInfo)
            {
                Log.LogInformation("Rectangle Aagent - " + i.ToString("Rectangle Platform"));
            }

            foreach (ObstacleRepresentation i in circlePlatformsInfo)
            {
                Log.LogInformation("Rectangle Aagent - " + i.ToString("Circle Platform"));
            }

            foreach (CollectibleRepresentation i in collectiblesInfo)
            {
                Log.LogInformation("Rectangle Aagent - " + i.ToString());
            }
        }

        //implements abstract rectangle interface: signals the agent the end of the current level
        public override void EndGame(int collectiblesCaught, int timeElapsed)
        {
            Log.LogInformation("RECTANGLE - Collectibles caught = " + collectiblesCaught + ", Time elapsed - " + timeElapsed);
        }

        //implememts abstract agent interface: receives messages from the circle agent
        public override void HandleAgentMessages(List<GeometryFriends.AI.Communication.AgentMessage> newMessages)
        {
            foreach (AgentMessage item in newMessages)
            {
                Log.LogInformation("Rectangle: received message from circle: " + item.Message);
                if (item.Attachment != null)
                {
                    Log.LogInformation("Received message has attachment: " + item.Attachment.ToString());
                    if (item.Attachment.GetType() == typeof(Pen))
                    {
                        Log.LogInformation("The attachment is a pen, let's see its color: " + ((Pen)item.Attachment).Color.ToString());
                    }
                }
            }
        }
    }
}