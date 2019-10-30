using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using GeometryFriends.AI;
using GeometryFriends.AI.Perceptions.Information;

namespace GeometryFriendsAgents.ActionStates
{
    class RectangleGoToAndStay : ActionState
    {

        float x, y;

        RectangleRepresentation currentState;

        public RectangleGoToAndStay(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public override Moves getAction()
        {
            float dif = x - currentState.X;

            if (dif > 0.5)
            {
                return Moves.MOVE_RIGHT;
            }
            else if(dif < -0.5)
            {
                return Moves.MOVE_LEFT;
            }

            return Moves.NO_ACTION;
        }

        public override void SensorsUpdate(RectangleRepresentation rI, CircleRepresentation cI, CollectibleRepresentation[] colI)
        {
            currentState = rI;
        }

    }
}
