namespace GeometryFriendsAgents
{
    using GeometryFriends.AI.Interfaces;
    using Microsoft.Xna.Framework;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal class BallAgent : IBallAgent, IAgent
    {
        private bool implementedAgent;
        private int lastAction;
        private int currentAction;
        private long lastMoveTime;
        private Random rnd;
        private string agentName = "RandBall";
        private Queue<ActionNode> ActionPlan = new Queue<ActionNode>();
        private mapAnal BmapA = new mapAnal();
        private ActionMap AM;
        private int old_nC = -1;
        private int[] numbersInfo;
        private float[] squareInfo;
        private float[] circleInfo;
        private float[] obstaclesInfo;
        private float[] squarePlatformsInfo;
        private float[] circlePlatformsInfo;
        private float[] collectiblesInfo;
        private int nCollectiblesLeft;
        protected Rectangle area;

        public BallAgent()
        {
            this.SetImplementedAgent(true);
            this.lastMoveTime = DateTime.Now.Second;
            this.lastAction = 0;
            this.currentAction = 0;
            this.rnd = new Random();
        }

        private void Action()
        {
            ActionNode node;
            BowWow instance = BowWow.Instance;
            if (instance.Isping())
            {
                switch (instance.cmd)
                {
                    case 1:
                        node = new ActionNode {
                            cmd = 2,
                            posX = this.circleInfo[0],
                            posY = this.circleInfo[1]
                        };
                        this.ActionPlan.Enqueue(node);
                        break;

                    case 2:
                        this.ActionPlan.Clear();
                        node = new ActionNode {
                            cmd = 3,
                            instant = true
                        };
                        this.ActionPlan.Enqueue(node);
                        break;

                    case 3:
                    {
                        int num = this.getNearlestC(this.circleInfo[0], this.circleInfo[1]);
                        if (num != -1)
                        {
                            instance.direct = true;
                            instance.X = this.collectiblesInfo[num * 2];
                            instance.Y = this.collectiblesInfo[(num * 2) + 1];
                        }
                        else
                        {
                            Queue<ActionNode> queue = this.planning(false, true, null);
                            instance.X = queue.Peek().posX;
                            instance.Y = queue.Peek().posY;
                            instance.direct = false;
                        }
                        instance.isfusion = true;
                        if ((this.circleInfo[1] + 200f) < this.squareInfo[1])
                        {
                            instance.isfusion = false;
                        }
                        break;
                    }
                    case 4:
                        this.ActionPlan = this.planning_st(true, true, new CVector2(this.squareInfo[0], this.squareInfo[1]));
                        break;

                    case 6:
                        this.ActionPlan.Clear();
                        node = new ActionNode {
                            cmd = 1,
                            posX = instance.X,
                            posY = instance.Y
                        };
                        this.ActionPlan.Enqueue(node);
                        break;

                    default:
                        break;
                }
                instance.setBow();
            }
            if (this.ActionPlan.Count<ActionNode>() != 0)
            {
                node = this.ActionPlan.Peek();
                switch (node.cmd)
                {
                    case 0:
                        this.SetAction(0);
                        break;

                    case 1:
                        this.SetAction(this.stay(node));
                        break;

                    case 2:
                        this.SetAction(this.jump(node));
                        break;

                    case 3:
                        this.SetAction(this.standup());
                        break;

                    default:
                        break;
                }
                if (this.ActionPlan.Count<ActionNode>() > 1)
                {
                    if (((node.posX - 2f) <= this.circleInfo[0]) && (this.circleInfo[0] <= (node.posX + 2f)))
                    {
                        this.ActionPlan.Dequeue();
                    }
                    if (node.instant)
                    {
                        this.ActionPlan.Dequeue();
                    }
                }
            }
        }

        public string AgentName() => 
            this.agentName;

        protected void DebugSensorsInfo()
        {
            int num = 0;
            Console.WriteLine("BALL - collectibles left - {0}", this.nCollectiblesLeft);
            Console.WriteLine("BALL - collectibles info size - {0}", this.collectiblesInfo.Count<float>());
            num = 0;
            foreach (float num2 in this.collectiblesInfo)
            {
                Console.WriteLine("BALL - Collectibles info - {0} - {1}", num, num2);
                num++;
            }
        }

        public int GetAction() => 
            this.currentAction;

        public int getNearlestC(float x, float y)
        {
            Edge edge = new Edge(x, y, 0f, 0f, 0, 0);
            int num = -1;
            float num2 = -1f;
            for (int i = 0; i < this.nCollectiblesLeft; i++)
            {
                int index = i * 2;
                edge.pt_2.x = this.collectiblesInfo[index];
                edge.pt_2.y = this.collectiblesInfo[index + 1];
                if (!this.BmapA.collision(edge))
                {
                    float num5 = edge.length();
                    if ((num2 == -1f) || (num5 < num2))
                    {
                        num = i;
                        num2 = num5;
                    }
                }
            }
            return num;
        }

        public bool ImplementedAgent() => 
            this.implementedAgent;

        private int jump(ActionNode act)
        {
            CVector2 vec = new CVector2(act.posX - this.circleInfo[0], act.posY - this.circleInfo[1]);
            float num = (this.circleInfo[0] + this.predict_jump(this.circleInfo[2], vec)) - act.posX;
            if (((act.action != 0f) || (-5f > num)) || (num > 5f))
            {
                return ((num <= 0f) ? ((num >= 0f) ? 0 : Moves.ROLL_RIGHT) : Moves.ROLL_LEFT);
            }
            return Moves.JUMP;
        }

        private Queue<ActionNode> planning(bool addsquare, bool addcollectabile, CVector2 target)
        {
            this.BmapA.removeAllAssistPoint();
            if (addcollectabile)
            {
                for (int j = 0; j < this.nCollectiblesLeft; j++)
                {
                    int num2 = j * 2;
                    this.BmapA.addAssistPoint(new CVector2(this.collectiblesInfo[num2], this.collectiblesInfo[num2 + 1]));
                }
            }
            if (addsquare)
            {
                this.BmapA.addAssistPoint(new CVector2(this.squareInfo[0], this.squareInfo[1]));
            }
            this.BmapA.print();
            this.AM = this.BmapA.Generating();
            this.AM.RemoveDuplicate();
            ActionMapDijkstra dijkstra = new ActionMapDijkstra(this.AM);
            dijkstra.print();
            dijkstra.process(dijkstra.getTNnear(new CVector2(this.circleInfo[0], this.circleInfo[1]), this.BmapA));
            if (!addcollectabile)
            {
                if (addsquare)
                {
                    return dijkstra.smoothing(dijkstra.getActionPlan(dijkstra.getTNnear(new CVector2(this.squareInfo[0], this.squareInfo[1]), this.BmapA)));
                }
                if (target == null)
                {
                    return new Queue<ActionNode>();
                }
                return dijkstra.smoothing(dijkstra.getActionPlan(dijkstra.getTNnear(target, this.BmapA)));
            }
            int index = 0;
            float num4 = dijkstra.getActionDistance(dijkstra.getTNnear(new CVector2(this.collectiblesInfo[0], this.collectiblesInfo[1]), this.BmapA));
            for (int i = 1; i < this.nCollectiblesLeft; i++)
            {
                int num6 = i * 2;
                float num7 = dijkstra.getActionDistance(dijkstra.getTNnear(new CVector2(this.collectiblesInfo[num6], this.collectiblesInfo[num6 + 1]), this.BmapA));
                if (num7 < num4)
                {
                    index = i;
                    num4 = num7;
                }
            }
            return dijkstra.smoothing(dijkstra.getActionPlan(dijkstra.getTNnear(new CVector2(this.collectiblesInfo[index], this.collectiblesInfo[index + 1]), this.BmapA)));
        }

        private Queue<ActionNode> planning_st(bool addsquare, bool addcollectabile, CVector2 target)
        {
            this.BmapA.removeAllAssistPoint();
            if (addcollectabile)
            {
                for (int i = 0; i < this.nCollectiblesLeft; i++)
                {
                    int index = i * 2;
                    this.BmapA.addAssistPoint(new CVector2(this.collectiblesInfo[index], this.collectiblesInfo[index + 1]));
                }
            }
            if (addsquare)
            {
                this.BmapA.addAssistPoint(new CVector2(this.squareInfo[0], this.squareInfo[1]));
            }
            this.BmapA.print();
            this.AM = this.BmapA.Generating();
            this.AM.RemoveDuplicate();
            ActionMapDijkstra dijkstra = new ActionMapDijkstra(this.AM);
            dijkstra.print();
            dijkstra.process(dijkstra.getTNnear(new CVector2(this.circleInfo[0], this.circleInfo[1]), this.BmapA));
            return dijkstra.smoothing(dijkstra.getActionPlan(dijkstra.getTNnear(target, this.BmapA)));
        }

        private float predict(float velocity) => 
            (velocity / 1.3f);

        private float predict_jump(float Vx, CVector2 vec) => 
            (Vx * (3f + (0.5f * (vec.y / 300f))));

        private void RandomAction()
        {
            this.currentAction = this.rnd.Next(1, 4);
            if (this.currentAction == this.lastAction)
            {
                this.currentAction = (this.currentAction != 3) ? (this.currentAction + 1) : this.rnd.Next(1, 3);
            }
            switch (this.currentAction)
            {
                case 1:
                    this.SetAction(Moves.ROLL_LEFT);
                    return;

                case 2:
                    this.SetAction(Moves.ROLL_RIGHT);
                    return;

                case 3:
                    this.SetAction(Moves.JUMP);
                    return;

                case 4:
                    this.SetAction(Moves.GROW);
                    return;
            }
        }

        private void SetAction(int a)
        {
            this.currentAction = a;
        }

        private void SetImplementedAgent(bool b)
        {
            this.implementedAgent = b;
        }

        public void Setup(int[] nI, float[] sI, float[] cI, float[] oI, float[] sPI, float[] cPI, float[] colI, Rectangle a)
        {
            int num = nI[0];
            for (int i = 0; i < num; i++)
            {
                int index = i * 4;
                float num4 = oI[index];
                float num5 = oI[index + 1];
                float num6 = oI[index + 3];
                float num7 = oI[index + 2];
                this.BmapA.addBox(new Vector4(num4, num5, num6, num7));
            }
            this.BmapA.addBox(new Vector4(600f, 800f, 1000f, 64f));
            this.BmapA.Analysis();
            this.BmapA.print();
            ActionNode item = new ActionNode {
                cmd = 1,
                posX = cI[0]
            };
            this.ActionPlan.Enqueue(item);
            this.area = a;
            this.numbersInfo = new int[4];
            for (int j = 0; j < nI.Length; j++)
            {
                this.numbersInfo[j] = nI[j];
            }
            this.nCollectiblesLeft = nI[3];
            this.squareInfo = new float[] { sI[0], sI[1], sI[2], sI[3], sI[4] };
            this.circleInfo = new float[] { cI[0], cI[1], cI[2], cI[3], cI[4] };
            this.obstaclesInfo = (this.numbersInfo[0] <= 0) ? new float[4] : new float[this.numbersInfo[0] * 4];
            int num8 = 1;
            if (nI[0] <= 0)
            {
                this.obstaclesInfo[0] = oI[0];
                this.obstaclesInfo[1] = oI[1];
                this.obstaclesInfo[2] = oI[2];
                this.obstaclesInfo[3] = oI[3];
            }
            else
            {
                while (num8 <= nI[0])
                {
                    this.obstaclesInfo[(num8 * 4) - 4] = oI[(num8 * 4) - 4];
                    this.obstaclesInfo[(num8 * 4) - 3] = oI[(num8 * 4) - 3];
                    this.obstaclesInfo[(num8 * 4) - 2] = oI[(num8 * 4) - 2];
                    this.obstaclesInfo[(num8 * 4) - 1] = oI[(num8 * 4) - 1];
                    num8++;
                }
            }
            this.squarePlatformsInfo = (this.numbersInfo[1] <= 0) ? new float[4] : new float[this.numbersInfo[1] * 4];
            num8 = 1;
            if (nI[1] <= 0)
            {
                this.squarePlatformsInfo[0] = sPI[0];
                this.squarePlatformsInfo[1] = sPI[1];
                this.squarePlatformsInfo[2] = sPI[2];
                this.squarePlatformsInfo[3] = sPI[3];
            }
            else
            {
                while (num8 <= nI[1])
                {
                    this.squarePlatformsInfo[(num8 * 4) - 4] = sPI[(num8 * 4) - 4];
                    this.squarePlatformsInfo[(num8 * 4) - 3] = sPI[(num8 * 4) - 3];
                    this.squarePlatformsInfo[(num8 * 4) - 2] = sPI[(num8 * 4) - 2];
                    this.squarePlatformsInfo[(num8 * 4) - 1] = sPI[(num8 * 4) - 1];
                    num8++;
                }
            }
            this.circlePlatformsInfo = (this.numbersInfo[2] <= 0) ? new float[4] : new float[this.numbersInfo[2] * 4];
            num8 = 1;
            if (nI[2] <= 0)
            {
                this.circlePlatformsInfo[0] = cPI[0];
                this.circlePlatformsInfo[1] = cPI[1];
                this.circlePlatformsInfo[2] = cPI[2];
                this.circlePlatformsInfo[3] = cPI[3];
            }
            else
            {
                while (num8 <= nI[2])
                {
                    this.circlePlatformsInfo[(num8 * 4) - 4] = cPI[(num8 * 4) - 4];
                    this.circlePlatformsInfo[(num8 * 4) - 3] = cPI[(num8 * 4) - 3];
                    this.circlePlatformsInfo[(num8 * 4) - 2] = cPI[(num8 * 4) - 2];
                    this.circlePlatformsInfo[(num8 * 4) - 1] = cPI[(num8 * 4) - 1];
                    num8++;
                }
            }
            this.collectiblesInfo = new float[this.numbersInfo[3] * 2];
            for (num8 = 1; num8 <= nI[3]; num8++)
            {
                this.collectiblesInfo[(num8 * 2) - 2] = colI[(num8 * 2) - 2];
                this.collectiblesInfo[(num8 * 2) - 1] = colI[(num8 * 2) - 1];
            }
            this.DebugSensorsInfo();
            this.UpdateSensors(nI[3], sI, cI, colI);
        }

        private int standup()
        {
            float num = (this.circleInfo[0] + this.predict(this.circleInfo[2] - this.squareInfo[2])) - this.squareInfo[0];
            return ((num <= 0f) ? ((num >= 0f) ? 0 : Moves.ROLL_RIGHT) : Moves.ROLL_LEFT);
        }

        private int stay(ActionNode act)
        {
            float num = (this.circleInfo[0] + this.predict(this.circleInfo[2])) - act.posX;
            return ((num <= 0f) ? ((num >= 0f) ? 0 : Moves.ROLL_RIGHT) : Moves.ROLL_LEFT);
        }

        public void toggleDebug()
        {
        }

        public void Update(TimeSpan elapsedGameTime)
        {
            this.Action();
            if (this.lastMoveTime == 60)
            {
                this.lastMoveTime = 0L;
            }
            if ((this.lastMoveTime <= DateTime.Now.Second) && (this.lastMoveTime < 60))
            {
                if (DateTime.Now.Second != 0x3b)
                {
                    this.lastMoveTime += 1L;
                }
                else
                {
                    this.lastMoveTime = 60;
                }
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

