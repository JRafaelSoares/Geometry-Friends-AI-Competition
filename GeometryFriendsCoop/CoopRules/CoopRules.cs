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

            FilterRule[] filterList = {
                new TightSpaceFilter(area, platforms, rectanglePlatforms, circlePlatforms),
                new HeightFilter(area, platforms, rectanglePlatforms, circlePlatforms),
                new RectangleFilter(area, platforms, rectanglePlatforms, circlePlatforms)
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
                    ActionRule rule = filter.filter(r, c, diamond);
                    if (rule != null)
                    {
                        actionRules.Add(rule);
                        break;
                    }
                }
            }

            return actionRules;
        }

        public override String ToString()
        {
            String result = "";

            return result;
        }
    }

    
}
