using System;
using System.Collections.Generic;
using System.Drawing;
using GeometryFriends.AI;
using GeometryFriends.AI.ActionSimulation;
using GeometryFriends.AI.Perceptions.Information;

namespace GeometryFriendsAgents
{
    public class CircleSingleplayerRule : ActionRule
    {
        protected CircleSingleplayer circleSingleplayer;
        private bool setup;

        CollectibleRepresentation[] objectiveDiamond;

        public CircleSingleplayerRule(CircleSingleplayer circleSingleplayer, CollectibleRepresentation objectiveDiamond) : base()
        {
            this.circleSingleplayer = circleSingleplayer;

            this.objectiveDiamond = new CollectibleRepresentation[1];

            this.objectiveDiamond[0] = objectiveDiamond;
            
            setup = false;
        }

        public override Moves getActionCircle()
        {
            return circleSingleplayer.GetAction();
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
                if (diamond.X == objectiveDiamond[0].X && diamond.Y == objectiveDiamond[0].Y)
                {
                    setFinished();

                    objectiveDiamond = new CollectibleRepresentation[0];
                }
            }
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

    }
}
