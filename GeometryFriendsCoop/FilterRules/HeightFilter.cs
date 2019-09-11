using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using GeometryFriends.AI.Perceptions.Information;

namespace GeometryFriendsAgents
{
    public class HeightFilter : FilterRule
    {
        //List of platforms or circle platforms ordered by Y value (So from top to bottom)
        private List<ObstacleRepresentation> yPlatforms;

        private float rectangleMinHeight = 52;

        //private List<ObstacleRepresentation> xPlatforms;

        public HeightFilter(Rectangle area, ObstacleRepresentation[] platforms, ObstacleRepresentation[] rectanglePlatforms, ObstacleRepresentation[] circlePlatforms) : base(area, platforms, rectanglePlatforms, circlePlatforms)
        {
            //xPlatforms = new List<ObstacleRepresentation>(platforms);
            yPlatforms = new List<ObstacleRepresentation>(platforms);
            yPlatforms.InsertRange(0, circlePlatforms);

            //xPlatforms = xPlatforms.OrderBy(o => o.X).ToList();
            yPlatforms = yPlatforms.OrderBy(o => o.Y).ToList();
        }

        /*
            Filtering accuracy could be improved by accounting for max sideways jump as well as max vertical jump 
        */
        public override ActionRule filter(RectangleRepresentation r, CircleRepresentation c, CollectibleRepresentation diamond, CircleSingleplayer circleSingleplayer, RectangleSingleplayer rectangleSingleplayer)
        {
            float varY = diamond.Y;
            float coopX = diamond.X, coopY = getArea().Height + getArea().Y - rectangleMinHeight - c.Radius;

            ObstacleRepresentation previousPlatform = new ObstacleRepresentation(-10f, -10f, 0, 0);

            foreach (ObstacleRepresentation platform in yPlatforms)
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
                        // If next platform down is to the left
                        if (Math.Max(previousPlatform.X - previousPlatform.Width / 2, getArea().X) - Math.Max(platform.X - platform.Width / 2, getArea().X) > getMaxRadius() / 2)
                        {
                            previousPlatform = platform;
                            varY = platform.Y;
                            coopX = previousPlatform.X - previousPlatform.Width / 2 - ((10000 / rectangleMinHeight) / 2);
                            coopY = platform.Y - platform.Height / 2 - rectangleMinHeight - c.Radius;
                        }
                        // If next platform down is to the right
                        else if (Math.Min(platform.X + platform.Width / 2, getArea().Width + getArea().X) - Math.Min(previousPlatform.X + previousPlatform.Width / 2, getArea().Width + getArea().X) > getMaxRadius() / 2)
                        {
                            previousPlatform = platform;
                            varY = platform.Y;
                            coopX = previousPlatform.X + previousPlatform.Width / 2 + ((10000 / rectangleMinHeight) / 2);
                            coopY = platform.Y - platform.Height / 2 - rectangleMinHeight - c.Radius;
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
                    break;
                }
            }

            return new HeightRule(circleSingleplayer, coopX, coopY, diamond);
        }
    }
}
