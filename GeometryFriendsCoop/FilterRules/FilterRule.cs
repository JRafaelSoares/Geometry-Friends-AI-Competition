using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using GeometryFriends.AI.Perceptions.Information;

namespace GeometryFriendsAgents
{
    public abstract class FilterRule
    {
        //Game Constants (of current game patch)
        private double maxJump = 320; //322.57;
        private double maxRadius = 40;

        private Rectangle area;

        private ObstacleRepresentation[] platforms;
        private ObstacleRepresentation[] rectanglePlatforms;
        private ObstacleRepresentation[] circlePlatforms;

        //Right now it has all platforms, can be optimized to each space has the needed platforms
        public FilterRule(Rectangle area, ObstacleRepresentation[] platforms, ObstacleRepresentation[] rectanglePlatforms, ObstacleRepresentation[] circlePlatforms) {
            this.area = area;

            
        }

        public abstract ActionRule filter(RectangleRepresentation rI, CircleRepresentation cI, CollectibleRepresentation diamond);

        public Rectangle getArea()
        {
            return area;
        }

        public double getMaxJump()
        {
            return maxJump;
        }

        public double getMaxRadius()
        {
            return maxRadius;
        }
    }
}
