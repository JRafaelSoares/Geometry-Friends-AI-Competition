using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using GeometryFriends.AI.Perceptions.Information;
using GeometryFriends.AI.ActionSimulation;
using GeometryFriends.AI;
using GeometryFriends.AI.Debug;
using System.Diagnostics;


namespace GeometryFriendsAgents
{
    class RectangleCoopAgent
    {
        private CoopRules coopRules;
        private RectangleSingleplayer rectangleSingleplayer;

        protected CountInformation nI;
        protected RectangleRepresentation rI;
        protected CircleRepresentation cI;
        protected ObstacleRepresentation[] oI;
        protected ObstacleRepresentation[] rPI;
        protected ObstacleRepresentation[] cPI;
        protected CollectibleRepresentation[] colI;
        protected Rectangle area;
        protected double timeLimit;

        private List<ActionRule> actionRules;
        private int currentAction;
        protected TimeSpan currentRuleTime;

        private bool finished;

        public RectangleCoopAgent(Rectangle area, CollectibleRepresentation[] diamonds, ObstacleRepresentation[] platforms, ObstacleRepresentation[] rectanglePlatforms, ObstacleRepresentation[] circlePlatforms, RectangleSingleplayer rectangleSingleplayer)
        {
            coopRules = new CoopRules(area, diamonds, platforms, rectanglePlatforms, circlePlatforms);

            this.rectangleSingleplayer = rectangleSingleplayer;

            currentRuleTime = new TimeSpan(0);

            finished = false;

            currentAction = 0;
        }

        public void Setup(CountInformation nI, RectangleRepresentation rI, CircleRepresentation cI, ObstacleRepresentation[] oI, ObstacleRepresentation[] rPI, ObstacleRepresentation[] cPI, CollectibleRepresentation[] colI, Rectangle area, double timeLimit)
        {
            //Splits the diamonds into each category
            actionRules = coopRules.ApplyRules(cI, rI);

            if (actionRules == null || actionRules.Count == 0)
            {
                finished = true;
                return;
            }

            actionRules[currentAction].Setup(nI, rI, cI, oI, rPI, cPI, colI, area, timeLimit);

            this.nI = nI;
            this.rI = rI;
            this.cI = cI;
            this.oI = oI;
            this.rPI = rPI;
            this.cPI = cPI;
            this.colI = colI;
            this.area = area;
            this.timeLimit = timeLimit;
        }

        public void SensorsUpdated(int nC, RectangleRepresentation rI, CircleRepresentation cI, CollectibleRepresentation[] colI)
        {
            if (!finished)
            {
                if (!actionRules[currentAction].isFinished() && (currentRuleTime.TotalSeconds < 30 || currentAction == actionRules.Count))
                {
                    actionRules[currentAction].SensorsUpdate(rI, cI, colI);
                }
                else
                {
                    currentAction++;
                    finished = (currentAction >= actionRules.Count);

                    currentRuleTime = new TimeSpan(0);

                    if (!finished)
                    {
                        actionRules[currentAction].Setup(nI, rI, cI, oI, rPI, cPI, colI, area, timeLimit);
                    }
                }
            }
        }

        public void ActionSimulatorUpdated(ActionSimulator updatedSimulator)
        {
            if (!finished)
            {
                actionRules[currentAction].ActionSimulatorUpdated(updatedSimulator);
            }
        }

        public Moves GetAction()
        {
            if (!finished)
            {
                return actionRules[currentAction].getActionRectangle();
            }

            return Moves.NO_ACTION;
        }

        public void Update(TimeSpan elapsedGameTime)
        {
            if (!finished)
            {
                currentRuleTime = currentRuleTime.Add(elapsedGameTime);
                actionRules[currentAction].Update(elapsedGameTime);
            }
        }

        public DebugInformation[] GetDebugInformation()
        {
            if (!finished)
            {
                return actionRules[currentAction].GetRectangleDebugInformation();
            }

            return new DebugInformation[0];
        }

    }
}
