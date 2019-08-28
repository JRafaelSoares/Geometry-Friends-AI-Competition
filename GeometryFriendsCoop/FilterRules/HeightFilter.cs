using System;
using System.Drawing;
using GeometryFriends.AI.Perceptions.Information;

namespace GeometryFriendsAgents
{
    public class HeightFilter : FilterRule
    {
        public HeightFilter(Rectangle area, ObstacleRepresentation[] platforms, ObstacleRepresentation[] rectanglePlatforms, ObstacleRepresentation[] circlePlatforms) : base(area, platforms, rectanglePlatforms, circlePlatforms) { }

        public override ActionRule diamondFilter(RectangleRepresentation r, CircleRepresentation c, CollectibleRepresentation diamond)
        {
            float varY = diamond.Y;
            ObstacleRepresentation previousPlatform = new ObstacleRepresentation(-10f, -10f, 0, 0);

            foreach (ObstacleRepresentation platform in getYPlatforms())
            {
                if (c.Y - varY < getMaxJump())
                {
                    return null;
                }

                if (varY >= platform.Y)
                {
                    continue;
                }
                else if (platform.Y - varY < getMaxJump())
                {
                    if (previousPlatform.X >= 0)
                    {
                        if (Math.Max(previousPlatform.X - previousPlatform.Width / 2, getArea().X) - Math.Max(platform.X - platform.Width / 2, getArea().X) > getMaxRadius() / 2 || Math.Min(platform.X + platform.Width / 2, getArea().Width + getArea().X) - Math.Min(previousPlatform.X + previousPlatform.Width / 2, getArea().Width + getArea().X) > getMaxRadius() / 2)
                        {
                            previousPlatform = platform;
                            varY = platform.Y;
                        }
                    }
                    else if (diamond.X - platform.X + platform.Width / 2 >= 0 && platform.X + platform.Width / 2 - diamond.X >= 0)
                    {
                        varY = platform.Y;
                        previousPlatform = platform;
                    }

                }
                else
                {
                    return new HeightRule();
                }
            }

            return new HeightRule();
        }
    }
}
