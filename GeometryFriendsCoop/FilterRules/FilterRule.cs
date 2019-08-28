using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using GeometryFriends.AI.Perceptions.Information;

namespace GeometryFriendsAgents
{
    public abstract class FilterRule
    {
        private Rectangle area;
        
        //Game Constants (of current game patch)
        private double maxJump = 320; //322.57;
        private double maxRadius = 40;

        //List of platforms in the Y axis
        private List<ObstacleRepresentation> yPlatforms;

        //private List<ObstacleRepresentation> xPlatforms;

        //Right now it has all platforms, can be optimized to each space has the needed platforms
        public FilterRule(Rectangle area, ObstacleRepresentation[] platforms, ObstacleRepresentation[] rectanglePlatforms, ObstacleRepresentation[] circlePlatforms) {
            this.area = area;

            //xPlatforms = new List<ObstacleRepresentation>(platforms);
            yPlatforms = new List<ObstacleRepresentation>(platforms);

            //xPlatforms = xPlatforms.OrderBy(o => o.X).ToList();
            yPlatforms = yPlatforms.OrderBy(o => o.Y).ToList();
        }

        public abstract ActionRule diamondFilter(RectangleRepresentation rI, CircleRepresentation cI, CollectibleRepresentation diamond);

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

        public List<ObstacleRepresentation> getYPlatforms()
        {
            return yPlatforms;
        }
    }
}
