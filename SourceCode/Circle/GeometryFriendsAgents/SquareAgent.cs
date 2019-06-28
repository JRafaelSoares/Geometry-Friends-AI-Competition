namespace GeometryFriendsAgents
{
    using GeometryFriends;
    using GeometryFriends.AI.Interfaces;
    using Microsoft.Xna.Framework;
    using System;
    using System.Linq;

    internal class SquareAgent : ISquareAgent, IAgent
    {
        private bool implementedAgent;
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
        private string agentName = "RandRect";
        protected Rectangle area;

        public SquareAgent()
        {
            this.SetImplementedAgent(true);
            this.lastMoveTime = DateTime.Now.Second;
            this.lastAction = 0;
            this.currentAction = 0;
            this.rnd = new Random();
        }

        public string AgentName() => 
            this.agentName;

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

        public int GetAction() => 
            this.currentAction;

        public bool ImplementedAgent() => 
            this.implementedAgent;

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
                    return;

                case 6:
                    this.SetAction(Moves.MOVE_RIGHT);
                    return;

                case 7:
                    this.SetAction(Moves.MORPH_UP);
                    return;

                case 8:
                    this.SetAction(Moves.MORPH_DOWN);
                    return;
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
        }

        public void toggleDebug()
        {
        }

        public void Update(TimeSpan elapsedGameTime)
        {
            if (this.lastMoveTime == 60)
            {
                this.lastMoveTime = 0L;
            }
            if ((this.lastMoveTime <= DateTime.Now.Second) && (this.lastMoveTime < 60))
            {
                if (DateTime.Now.Second != 0x3b)
                {
                    this.RandomAction();
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

