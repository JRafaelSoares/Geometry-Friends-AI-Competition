﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using GeometryFriends.AI;
using GeometryFriends.AI.ActionSimulation;
using GeometryFriends.AI.Debug;
using GeometryFriends.AI.Perceptions.Information;

namespace GeometryFriendsAgents
{
    public class RectangleSingleplayerRule : ActionRule
    {
        private RectangleSingleplayer rectangleSingleplayer;
        private bool setup;

        CollectibleRepresentation[] objectiveDiamond;

        public RectangleSingleplayerRule(CollectibleRepresentation objectiveDiamond) : base()
        {
            this.rectangleSingleplayer = new RectangleSingleplayer(true, true, true);

            this.objectiveDiamond = new CollectibleRepresentation[1];

            this.objectiveDiamond[0] = objectiveDiamond;

            setup = false;
        }

        public override Moves getActionRectangle()
        {
            Moves action = rectangleSingleplayer.GetAction();
            return action;
        }

        public override Moves getActionCircle()
        {
            return Moves.NO_ACTION;
        }

        public override void SensorsUpdate(RectangleRepresentation rI, CircleRepresentation cI, CollectibleRepresentation[] colI)
        {
            if (isFinished())
            {
                return;
            }

            rectangleSingleplayer.SensorsUpdated(objectiveDiamond.Length, rI, cI, objectiveDiamond);

            foreach (CollectibleRepresentation diamond in colI)
            {
                if (objectiveDiamond.Length > 0 && diamond.X == objectiveDiamond[0].X && diamond.Y == objectiveDiamond[0].Y)
                {
                    return;
                }
            }

            setFinished();

            objectiveDiamond = new CollectibleRepresentation[0];
        }

        public override void Setup(CountInformation nI, RectangleRepresentation rI, CircleRepresentation cI, ObstacleRepresentation[] oI, ObstacleRepresentation[] rPI, ObstacleRepresentation[] cPI, CollectibleRepresentation[] colI, Rectangle area, double timeLimit)
        {
            rectangleSingleplayer.Setup(nI, rI, cI, oI, rPI, cPI, objectiveDiamond, area, timeLimit);
            setup = true;
        }

        public override void Update(TimeSpan elapsedGameTime)
        {
            if (setup)
            {
                rectangleSingleplayer.Update(elapsedGameTime);
            }
        }

        public override void ActionSimulatorUpdated(ActionSimulator updatedSimulator)
        {
            rectangleSingleplayer.ActionSimulatorUpdated(updatedSimulator);
        }

        public override DebugInformation[] GetRectangleDebugInformation()
        {
            return rectangleSingleplayer.GetDebugInformation();
        }

    }
}
