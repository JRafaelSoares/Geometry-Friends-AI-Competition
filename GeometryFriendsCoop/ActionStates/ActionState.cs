using System;
using System.Collections.Generic;
using System.Drawing;
using GeometryFriends.AI;
using GeometryFriends.AI.ActionSimulation;
using GeometryFriends.AI.Debug;
using GeometryFriends.AI.Perceptions.Information;

namespace GeometryFriendsAgents
{
    public class ActionState
    {
        private bool finished = false;

        public ActionState()
        {

        }

        public virtual Moves getAction()
        {
            return Moves.NO_ACTION;
        }

        public virtual void Update(TimeSpan elapsedGameTime)
        {
            // Don't do anything as default
        }

        public virtual void SensorsUpdate(RectangleRepresentation rI, CircleRepresentation cI, CollectibleRepresentation[] colI)
        {

        }

        public virtual void Setup(CountInformation nI, RectangleRepresentation rI, CircleRepresentation cI, ObstacleRepresentation[] oI, ObstacleRepresentation[] rPI, ObstacleRepresentation[] cPI, CollectibleRepresentation[] colI, Rectangle area, double timeLimit)
        {

        }
       
        public bool isFinished()
        {
            return finished;
        }

        protected void setFinished()
        {
            finished = true;
        }

        public virtual void ActionSimulatorUpdated(ActionSimulator updatedSimulator)
        {

        }

        public virtual DebugInformation[] GetDebugInformation()
        {
            return new DebugInformation[0];
        }
    }
}
