using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using GeometryFriends.AI;
using GeometryFriends.AI.Perceptions.Information;

namespace GeometryFriendsAgents.ActionStates
{
    class MorphUp : ActionState
    {

        float previousHeight;
        int sameCount = 0;

        float maxHeight = 192.3077f;

        public override Moves getAction()
        {
            return Moves.MORPH_UP;
        }

        public override void SensorsUpdate(RectangleRepresentation rI, CircleRepresentation cI, CollectibleRepresentation[] colI)
        {
            if(maxHeight - rI.Height < 0.5 || sameCount > 10)
            {
                setFinished();
            }
            else if(rI.Height - previousHeight < 0.1)
            {
                sameCount++;
                previousHeight = rI.Height;
            }
            else
            {
                sameCount = 0;
            }


        }

        public override void Setup(CountInformation nI, RectangleRepresentation rI, CircleRepresentation cI, ObstacleRepresentation[] oI, ObstacleRepresentation[] rPI, ObstacleRepresentation[] cPI, CollectibleRepresentation[] colI, Rectangle area, double timeLimit)
        {
            previousHeight = rI.Height;
        }
    }
}
