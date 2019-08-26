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
        private int state;
        private SortedDiamond currentCoop; 

        public RectangleCoopAgent(Rectangle area, CollectibleRepresentation[] diamonds, ObstacleRepresentation[] platforms, ObstacleRepresentation[] rectanglePlatforms, ObstacleRepresentation[] circlePlatforms, RectangleSingleplayer rectangleSingleplayer)
        {
            coopRules = new CoopRules(area, diamonds, platforms, rectanglePlatforms, circlePlatforms);

            state = 0; // Getting singleplayer diamonds

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
            switch (state)
            {
                case 0:
                    CollectibleRepresentation[] singleplayerDiamonds = coopRules.updateRectangleDiamonds(colI);

                    if (singleplayerDiamonds.Count() == 0)
                    {
                        state = 1; // Coop State
                    }

                    rectangleAgent.SensorsUpdated(nC, rI, cI, singleplayerDiamonds);
                    break;
                // Waiting for circle to begin Coop
                case 1: 
                    currentCoop = coopRules.getCoopDiamonds()[0];

                    if(coopRules.updateCircleDiamonds(colI).Count() == 0)
                    {
                        state = 2;
                    }

                    break;
                case 2:
                    int previousCount = coopRules.getCoopDiamonds().Count();

                    List<SortedDiamond> coopDiamonds = coopRules.updateCoopDiamonds(colI);

                    if(previousCount > coopDiamonds.Count() && coopDiamonds.Count() > 0)
                    {
                        currentCoop = coopDiamonds[0];
                    }

                    break;
            }
        }

        public void ActionSimulatorUpdated(ActionSimulator updatedSimulator)
        {
            rectangleAgent.ActionSimulatorUpdated(updatedSimulator);
        }

        public Moves GetAction()
        {
            switch (state)
            {
                case 0:
                    return rectangleAgent.GetAction();
                default:
                    return Moves.NO_ACTION;
            }
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
