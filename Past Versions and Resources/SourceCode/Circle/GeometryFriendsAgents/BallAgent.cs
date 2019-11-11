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
            if (this.ActionPlan.Count<ActionNode>() != 0)
            {
                ActionNode act = this.ActionPlan.Peek();
                switch (act.cmd)
                {
                    case 0:
                        this.SetAction(0);
                        break;

                    case 1:
                        this.SetAction(this.stay(act));
                        break;

                    case 2:
                        this.SetAction(this.jump(act));
                        break;

                    default:
                        break;
                }
                if (((act.posX - 2f) <= this.circleInfo[0]) && (this.circleInfo[0] <= (act.posX + 2f)))
                {
                    this.ActionPlan.Dequeue();
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
            if ((nC > 0) && ((this.ActionPlan.Count<ActionNode>() == 0) || (nC != this.old_nC)))
            {
                this.old_nC = nC;
                this.BmapA.removeAllAssistPoint();
                int num2 = nC;
                int num3 = 0;
                while (true)
                {
                    if (num3 >= num2)
                    {
                        this.BmapA.print();
                        this.AM = this.BmapA.Generating();
                        this.AM.RemoveDuplicate();
                        ActionMapDijkstra dijkstra = new ActionMapDijkstra(this.AM);
                        dijkstra.print();
                        dijkstra.process(dijkstra.getTNnear(new CVector2(cI[0], cI[1]), this.BmapA));
                        int num5 = 0;
                        float num6 = dijkstra.getActionDistance(dijkstra.getTNnear(new CVector2(colI[0], colI[1]), this.BmapA));
                        int num7 = 1;
                        while (true)
                        {
                            if (num7 >= nC)
                            {
                                List<ActionNode> list = dijkstra.getActionPlan(dijkstra.getTNnear(new CVector2(colI[2 * num5], colI[(2 * num5) + 1]), this.BmapA));
                                int num10 = 0;
                                while (true)
                                {
                                    if (num10 >= list.Count)
                                    {
                                        Queue<ActionNode> queue = dijkstra.smoothing(list);
                                        this.ActionPlan = queue;
                                        break;
                                    }
                                    ActionNode local1 = list[num10];
                                    num10++;
                                }
                                break;
                            }
                            int num8 = num7 * 2;
                            float num9 = dijkstra.getActionDistance(dijkstra.getTNnear(new CVector2(colI[num8], colI[num8 + 1]), this.BmapA));
                            if (num9 < num6)
                            {
                                num5 = num7;
                                num6 = num9;
                            }
                            num7++;
                        }
                        break;
                    }
                    int index = num3 * 2;
                    this.BmapA.addAssistPoint(new CVector2(colI[index], colI[index + 1]));
                    num3++;
                }
            }
        }
    }
}

