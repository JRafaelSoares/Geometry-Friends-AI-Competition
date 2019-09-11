using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using GeometryFriends;
using GeometryFriends.AI.Perceptions.Information;
using GeometryFriends.AI.Communication;

namespace GeometryFriendsAgents
{
    public class CoopRules
    {
        //private int pixelSize = 40;
        //private List<List<PixelType>> levelInfo;

        private double maxJump = 320; //322.57;
        private double maxRadius = 40;

        private List<ObstacleRepresentation> yPlatforms;
        //private List<ObstacleRepresentation> xPlatforms;
        private CollectibleRepresentation[] diamonds;

        private Rectangle levelArea;
        private List<AgentMessage> messages = new List<AgentMessage>();

        private CircleSingleplayer circleSingleplayer;
        private RectangleSingleplayer rectangleSingleplayer;

        //list of filters

        private List<FilterRule> filters;
        public CoopRules(Rectangle area, CollectibleRepresentation[] diamonds, ObstacleRepresentation[] platforms, ObstacleRepresentation[] rectanglePlatforms, ObstacleRepresentation[] circlePlatforms)
        {
            this.diamonds = diamonds;
            levelArea = area;

            //xPlatforms = new List<ObstacleRepresentation>(platforms);
            yPlatforms = new List<ObstacleRepresentation>(platforms);

            //xPlatforms = xPlatforms.OrderBy(o => o.X).ToList();
            yPlatforms = yPlatforms.OrderBy(o => o.Y).ToList();

            circleSingleplayer = new CircleSingleplayer(true, true, true);

            FilterRule[] filterList = {
                //new TightSpaceFilter(area, platforms, rectanglePlatforms, circlePlatforms),
                new HeightFilter(area, platforms, rectanglePlatforms, circlePlatforms)
            };

            filters = new List<FilterRule>(filterList);
        }

        public List<ActionRule> ApplyRules(CircleRepresentation c, RectangleRepresentation r)
        {
            List<ActionRule> actionRules = new List<ActionRule>(); 

            foreach(CollectibleRepresentation diamond in diamonds)
            {
                foreach(FilterRule filter in filters)
                {
                    ActionRule rule = filter.filter(r, c, diamond, circleSingleplayer, rectangleSingleplayer);
                    if (rule != null)
                    {
                        actionRules.Add(rule);
                    }
                }
            }

            return actionRules;
        }

        public int unreachableByJump(float dX, float dY, CircleRepresentation c, RectangleRepresentation r)
        {
            float varY = dY;
            ObstacleRepresentation previousPlatform = new ObstacleRepresentation(-10f, -10f, 0, 0);

            foreach (ObstacleRepresentation platform in yPlatforms)
            {
                if (c.Y - varY < maxJump)
                {
                    return 0;
                }

                if (varY >= platform.Y)
                {
                    continue;
                }
                else if (platform.Y - varY < maxJump)
                {
                    if(previousPlatform.X >= 0)
                    {
                        if(Math.Max(previousPlatform.X - previousPlatform.Width / 2, levelArea.X) - Math.Max(platform.X - platform.Width / 2, levelArea.X) > maxRadius / 2 || Math.Min(platform.X + platform.Width / 2, levelArea.Width + levelArea.X) - Math.Min(previousPlatform.X + previousPlatform.Width / 2, levelArea.Width + levelArea.X) > maxRadius / 2)
                        {
                            previousPlatform = platform;
                            varY = platform.Y;
                        }
                    }
                    else if(dX - platform.X + platform.Width / 2 >= 0 && platform.X + platform.Width / 2 - dX >= 0)
                    {
                        varY = platform.Y;
                        previousPlatform = platform;
                    }

                }
                else
                {
                    return 2;
                }
            }

            return 2;
        }

        public int unreachableBetweenPlatforms(float dX, float dY, CircleRepresentation c, RectangleRepresentation r)
        {
            ObstacleRepresentation closestAbove = new ObstacleRepresentation(dX, levelArea.Y, 0, 0), closestBelow = new ObstacleRepresentation(dX, levelArea.Height + levelArea.Y, 0, 0);

            // Might be able to be changed into logarithmic complexity
            // Might not work for some combinations of Circle and regular platforms
            foreach (ObstacleRepresentation platform in yPlatforms)
            {
                // Check that diamond is above or below platform (same X)
                if (platform.X - platform.Width / 2 < dX && dX < platform.X + platform.Width / 2)
                {
                    // Since its ordered the first below is the closest
                    if (platform.Y > dY && platform.Y < closestBelow.Y)
                    {
                        closestBelow = platform;
                        break;
                    }
                    else if (platform.Y < dY && platform.Y > closestAbove.Y)
                    {
                        closestAbove = platform;
                    }
                }
            }

            // Either rectangle or coop
            if (closestBelow.Y - closestBelow.Height / 2 - closestAbove.Y - closestAbove.Height / 2 <= maxRadius * 2)
            {
                // If rectangle is on top of the platform below, it can get there alone
                if (r.Y + (r.Height / 2) - closestBelow.Y + closestBelow.Height / 2  <= 10)
                {
                    return 1;
                }
                // Else, it needs the help from the circle
                else
                {
                    return 2;
                }
            }

            return 0;
        }

        public override String ToString()
        {
            String result = "";

            return result;
        }
    }

    
}
