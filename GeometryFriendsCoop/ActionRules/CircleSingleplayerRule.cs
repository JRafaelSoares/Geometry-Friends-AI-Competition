using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using GeometryFriends.AI;
using GeometryFriends.AI.ActionSimulation;
using GeometryFriends.AI.Debug;
using GeometryFriends.AI.Perceptions.Information;

namespace GeometryFriendsAgents
{
    public class CircleSingleplayerRule : ActionRule
    {
        protected CircleSingleplayer circleSingleplayer;
        private bool setup;

        CollectibleRepresentation[] objectiveDiamond;

        public CircleSingleplayerRule(CollectibleRepresentation objectiveDiamond) : base()
        {
            circleSingleplayer = new CircleSingleplayer(true, true, true);

            this.objectiveDiamond = new CollectibleRepresentation[1];

            this.objectiveDiamond[0] = objectiveDiamond;
            
            setup = false;
        }

        public override Moves getActionCircle()
        {
            Moves action = circleSingleplayer.GetAction();

            return action;
        }

        public override Moves getActionRectangle()
        {
            return Moves.NO_ACTION;
        }

        public override void SensorsUpdate(RectangleRepresentation rI, CircleRepresentation cI, CollectibleRepresentation[] colI)
        {
            if (isFinished())
            {
                return;
            }

            circleSingleplayer.SensorsUpdated(objectiveDiamond.Length, rI, cI, objectiveDiamond);

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
            circleSingleplayer.Setup(nI, rI, cI, oI, rPI, cPI, objectiveDiamond, area, timeLimit);
            setup = true;
        }

        public override void Update(TimeSpan elapsedGameTime)
        {
            if (setup)
            {
                circleSingleplayer.Update(elapsedGameTime);
            }
        }

        public override void ActionSimulatorUpdated(ActionSimulator updatedSimulator)
        {
            circleSingleplayer.ActionSimulatorUpdated(updatedSimulator);
        }

        public override DebugInformation[] GetCircleDebugInformation()
        {
            return circleSingleplayer.GetDebugInformation();
        }

    }
}
