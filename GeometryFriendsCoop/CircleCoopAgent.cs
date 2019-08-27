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
    class CircleCoopAgent
    {
        private CoopRules coopRules;
        private CircleSingleplayer circleAgent;
        private int state;
        private List<ActionRule> actionRules;
        private List<ActionRule>.Enumerator iterator;

        public CircleCoopAgent(Rectangle area, CollectibleRepresentation[] diamonds, ObstacleRepresentation[] platforms, ObstacleRepresentation[] rectanglePlatforms, ObstacleRepresentation[] circlePlatforms, CircleSingleplayer circleSingleplayer)
        {
            state = 0; // Getting singleplayer diamonds
            coopRules = new CoopRules(area, diamonds, platforms, rectanglePlatforms, circlePlatforms, circleAgent);
        }

        public void Setup(CountInformation nI, RectangleRepresentation rI, CircleRepresentation cI, ObstacleRepresentation[] oI, ObstacleRepresentation[] rPI, ObstacleRepresentation[] cPI, CollectibleRepresentation[] colI, Rectangle area, double timeLimit)
        {
            //Splits the diamonds into each category
            actionRules = coopRules.ApplyRules(cI, rI);
            iterator = actionRules.GetEnumerator();
            iterator.MoveNext();

            iterator.Current.Setup(nI, rI, cI, oI, rPI, cPI, colI, area, timeLimit);
        }

        public void SensorsUpdated(RectangleRepresentation rI, CircleRepresentation cI, CollectibleRepresentation[] colI)
        {
            if (iterator.Current.hasFinished())
            {
                iterator.Current.SensorsUpdate(rI, cI, colI);
            }
            else
            {
                iterator.MoveNext();
            }
        }

        public void ActionSimulatorUpdated(ActionSimulator updatedSimulator)
        {
            circleAgent.ActionSimulatorUpdated(updatedSimulator);
        }

        public Moves GetAction()
        {
            switch (state)
            {
                case 0:
                    return circleAgent.GetAction();
            }

            return Moves.NO_ACTION;
        }

        public void Update(TimeSpan elapsedGameTime)
        {
            //All the special things will be implemented here
            circleAgent.Update(elapsedGameTime);
        }

        public DebugInformation[] GetDebugInformation()
        {
            return circleAgent.GetDebugInformation();
        }

    }
}
