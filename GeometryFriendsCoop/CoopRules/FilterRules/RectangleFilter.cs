using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using GeometryFriends.AI.Perceptions.Information;

namespace GeometryFriendsAgents
{
    public class RectangleFilter : FilterRule
    {
        //List of platforms or circle platforms ordered by Y value (So from top to bottom)
        private System.Collections.Generic.List<ObstacleRepresentation> yPlatforms;

        public RectangleFilter(Rectangle area, ObstacleRepresentation[] platforms, ObstacleRepresentation[] rectanglePlatforms, ObstacleRepresentation[] circlePlatforms) : base(area, platforms, rectanglePlatforms, circlePlatforms)
        {
            //xPlatforms = new List<ObstacleRepresentation>(platforms);
            yPlatforms = new List<ObstacleRepresentation>(platforms);
            yPlatforms.InsertRange(0, circlePlatforms);

            //xPlatforms = xPlatforms.OrderBy(o => o.X).ToList();
            yPlatforms = yPlatforms.OrderBy(o => o.Y).ToList();
        }

        public override ActionRule filter(RectangleRepresentation r, CircleRepresentation c, CollectibleRepresentation diamond)
        {
            ObstacleRepresentation closestAbove = new ObstacleRepresentation(diamond.X, getArea().Y, 0, 0), closestBelow = new ObstacleRepresentation(diamond.X, getArea().Height + getArea().Y, 0, 0);

            // Might be able to be changed into logarithmic complexity
            // Might not work for some combinations of Circle and regular platforms
            foreach (ObstacleRepresentation platform in yPlatforms)
            {
                // Check that diamond is above or below platform (same X)
                if (platform.X - platform.Width / 2 < diamond.X && diamond.X < platform.X + platform.Width / 2)
                {
                    // Since its ordered the first below is the closest
                    if (platform.Y > diamond.Y && platform.Y < closestBelow.Y)
                    {
                        closestBelow = platform;
                        break;
                    }
                    else if (platform.Y < diamond.Y && platform.Y > closestAbove.Y)
                    {
                        closestAbove = platform;
                    }
                }
            }

            // Either rectangle or coop
            if (closestBelow.Y - closestBelow.Height / 2 - closestAbove.Y - closestAbove.Height / 2 <= getMaxRadius() * 2)
            {
                // If rectangle is on top of the platform below, it can get there alone
                if (r.Y + (r.Height / 2) - closestBelow.Y + closestBelow.Height / 2 <= 10)
                {
                    return new RectangleSingleplayerRule(diamond);
                }
                // Else, it needs the help from the circle
                else
                {
                    return null;
                }
            }

            return new CircleSingleplayerRule(diamond);
        }
    }
}
