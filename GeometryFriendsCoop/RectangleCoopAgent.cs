﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using GeometryFriends.AI.Perceptions.Information;
using GeometryFriends.AI.ActionSimulation;
using GeometryFriends.AI;
using GeometryFriends.AI.Debug;


namespace GeometryFriendsAgents
{
    class RectangleCoopAgent
    {
        private CoopRules coopRules;
        private RectangleSingleplayer rectangleAgent;

        public RectangleCoopAgent(Rectangle area, CollectibleRepresentation[] diamonds, ObstacleRepresentation[] platforms, ObstacleRepresentation[] rectanglePlatforms, ObstacleRepresentation[] circlePlatforms, RectangleSingleplayer rectangleSingleplayer)
        {
            coopRules = new CoopRules(area, diamonds, platforms, rectanglePlatforms, circlePlatforms);
            //Splits the diamonds into each category
            //Testing
            coopRules.setRectangleDiamonds(diamonds);

            //Actual function
            //coopRules.ApplyRules();
            //coopRules.recieveRectangleDiamonds();

            rectangleAgent = rectangleSingleplayer;
        }

        public void Setup(CountInformation nI, RectangleRepresentation rI, CircleRepresentation cI, ObstacleRepresentation[] oI, ObstacleRepresentation[] rPI, ObstacleRepresentation[] cPI, Rectangle area, double timeLimit)
        {
            rectangleAgent.Setup(nI, rI, cI, oI, rPI, cPI, coopRules.getRectangleDiamonds(), area, timeLimit);
        }

        public void SensorsUpdated(int nC, RectangleRepresentation rI, CircleRepresentation cI, CollectibleRepresentation[] colI)
        {
            rectangleAgent.SensorsUpdated(nC, rI, cI, coopRules.updateRectangleDiamonds(colI));
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