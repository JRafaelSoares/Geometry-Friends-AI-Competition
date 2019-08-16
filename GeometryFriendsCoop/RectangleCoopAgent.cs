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
        private RectangleSingleplayer rectangleAgent;

        public RectangleCoopAgent(Rectangle area, CollectibleRepresentation[] diamonds, ObstacleRepresentation[] platforms, ObstacleRepresentation[] rectanglePlatforms, ObstacleRepresentation[] circlePlatforms, RectangleSingleplayer rectangleSingleplayer)
        {
            coopRules = new CoopRules(area, diamonds, platforms, rectanglePlatforms, circlePlatforms);

            rectangleAgent = rectangleSingleplayer;
        }

        public void Setup(CountInformation nI, RectangleRepresentation rI, CircleRepresentation cI, ObstacleRepresentation[] oI, ObstacleRepresentation[] rPI, ObstacleRepresentation[] cPI, Rectangle area, double timeLimit)
        {
            //Splits the diamonds into each category
            coopRules.ApplyRules(cI, rI);
            rectangleAgent.Setup(nI, rI, cI, oI, rPI, cPI, coopRules.getRectangleDiamonds(), area, timeLimit);
        }

        public void SensorsUpdated(int nC, RectangleRepresentation rI, CircleRepresentation cI, CollectibleRepresentation[] colI)
        {
            CollectibleRepresentation[] rectDiamonds = coopRules.updateRectangleDiamonds(colI);
            rectangleAgent.SensorsUpdated(rectDiamonds.Count(), rI, cI, rectDiamonds);
        }

        public void ActionSimulatorUpdated(ActionSimulator updatedSimulator)
        {
            rectangleAgent.ActionSimulatorUpdated(updatedSimulator);
        }

        public Moves GetAction()
        {
            return rectangleAgent.GetAction();
        }

        public void Update(TimeSpan elapsedGameTime)
        {
            //All the special things will be implemented here
            rectangleAgent.Update(elapsedGameTime);
        }

        public DebugInformation[] GetDebugInformation()
        {
            return rectangleAgent.GetDebugInformation();
        }

    }
}
