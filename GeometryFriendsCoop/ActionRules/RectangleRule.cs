using System;
using System.Collections.Generic;
using System.Drawing;
using GeometryFriends.AI;
using GeometryFriends.AI.Perceptions.Information;

namespace GeometryFriendsAgents
{
    public class RectangleRule : ActionRule
    {
        private RectangleSingleplayer rectangleSingleplayer;

        public RectangleRule(RectangleSingleplayer rectangleSingleplayer) : base()
        {
            this.rectangleSingleplayer = rectangleSingleplayer;
        }
        
    }
}
