using System;
using System.Drawing;
using GeometryFriends.AI.Perceptions.Information;

namespace GeometryFriendsAgents
{
    public class TightSpaceFilter : FilterRule
    {
        public TightSpaceFilter(Rectangle area, ObstacleRepresentation[] platforms, ObstacleRepresentation[] rectanglePlatforms, ObstacleRepresentation[] circlePlatforms) : base(area, platforms, rectanglePlatforms, circlePlatforms) { }

        public override ActionRule diamondFilter(RectangleRepresentation r, CircleRepresentation c, CollectibleRepresentation diamond)
        {
            ObstacleRepresentation closestAbove = new ObstacleRepresentation(diamond.X, getArea().Y, 0, 0), closestBelow = new ObstacleRepresentation(diamond.X, getArea().Height + getArea().Y, 0, 0);

            // Might be able to be changed into logarithmic complexity
            // Might not work for some combinations of Circle and regular platforms
            foreach (ObstacleRepresentation platform in getYPlatforms())
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
            if (closestBelow.Y - closestBelow.Height / 2 - closestAbove.Y - closestAbove.Height / 2 <= getMaxJump() * 2)
            {
                // If rectangle is on top of the platform below, it can get there alone
                if (r.Y + (r.Height / 2) - closestBelow.Y + closestBelow.Height / 2 <= 10)
                {
                    return null;
                }
                // Else, it needs the help from the circle
                else
                {
                    return new TightSpaceRule();
                }
            }

            return null;
        }
    }
}
