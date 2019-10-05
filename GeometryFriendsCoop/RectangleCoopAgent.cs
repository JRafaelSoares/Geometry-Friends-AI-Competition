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
        private List<ActionRule>.Enumerator iterator;

        private bool finished;

        public RectangleCoopAgent(Rectangle area, CollectibleRepresentation[] diamonds, ObstacleRepresentation[] platforms, ObstacleRepresentation[] rectanglePlatforms, ObstacleRepresentation[] circlePlatforms, RectangleSingleplayer rectangleSingleplayer)
        {
            coopRules = new CoopRules(area, diamonds, platforms, rectanglePlatforms, circlePlatforms);

            this.rectangleSingleplayer = rectangleSingleplayer;

            finished = false;
        }

        public void Setup(CountInformation nI, RectangleRepresentation rI, CircleRepresentation cI, ObstacleRepresentation[] oI, ObstacleRepresentation[] rPI, ObstacleRepresentation[] cPI, CollectibleRepresentation[] colI, Rectangle area, double timeLimit)
        {
            //Splits the diamonds into each category
            actionRules = coopRules.ApplyRules(cI, rI);
            iterator = actionRules.GetEnumerator();

            iterator.MoveNext();
            iterator.Current.Setup(nI, rI, cI, oI, rPI, cPI, colI, area, timeLimit);

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
                if (!iterator.Current.isFinished())
                {
                    iterator.Current.SensorsUpdate(rI, cI, colI);
                }
                else
                {
                    finished = !iterator.MoveNext();

                    if (!finished)
                    {
                        iterator.Current.Setup(nI, rI, cI, oI, rPI, cPI, colI, area, timeLimit);
                    }
                }
            }
        }

        public void ActionSimulatorUpdated(ActionSimulator updatedSimulator)
        {
            if (!finished)
            {
                iterator.Current.ActionSimulatorUpdated(updatedSimulator);
            }
        }

        public Moves GetAction()
        {
            if (!finished)
            {
                return iterator.Current.getActionRectangle();
            }

            return Moves.NO_ACTION;
        }

        public void Update(TimeSpan elapsedGameTime)
        {
            if (!finished)
            {
                iterator.Current.Update(elapsedGameTime);
            }
        }

        public DebugInformation[] GetDebugInformation()
        {
            if (!finished)
            {
                return iterator.Current.GetRectangleDebugInformation();
            }

            return new DebugInformation[0];
        }

    }
}
