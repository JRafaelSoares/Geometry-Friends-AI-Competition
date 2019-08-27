using System;
using System.Collections.Generic;
using System.Drawing;
using GeometryFriends.AI;
using GeometryFriends.AI.Perceptions.Information;

namespace GeometryFriendsAgents
{
    public abstract class ActionState
    {
        private bool finished = false;
        public ActionState() { }

        public abstract Moves getAction();

        public abstract void Update(TimeSpan elapsedGameTime);

        public abstract void SensorsUpdate(RectangleRepresentation rI, CircleRepresentation cI, CollectibleRepresentation[] colI);

        public abstract void Setup(CountInformation nI, RectangleRepresentation rI, CircleRepresentation cI, ObstacleRepresentation[] oI, ObstacleRepresentation[] rPI, ObstacleRepresentation[] cPI, CollectibleRepresentation[] colI, Rectangle area, double timeLimit);
        
            public bool isFinished()
        {
            return finished;
        }
    }
}
