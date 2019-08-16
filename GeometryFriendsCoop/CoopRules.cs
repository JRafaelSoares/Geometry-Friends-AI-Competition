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
        private int pixelSize = 40;
        private List<List<PixelType>> levelInfo;

        private double maxJump = 320; //322.57;
        private double maxRadius = 40;

        private List<ObstacleRepresentation> yPlatforms;
        //private List<ObstacleRepresentation> xPlatforms;
        private CollectibleRepresentation[] diamonds;

        List<ObstacleRepresentation> circleDiamonds;
        List<ObstacleRepresentation> rectangleDiamonds;
        List<SortedDiamond> coopDiamonds;

        private Rectangle levelArea;
        private List<AgentMessage> messages = new List<AgentMessage>();

        public CoopRules(Rectangle area, CollectibleRepresentation[] diamonds, ObstacleRepresentation[] platforms, ObstacleRepresentation[] rectanglePlatforms, ObstacleRepresentation[] circlePlatforms)
        {
            this.diamonds = diamonds;
            levelArea = area;

            //xPlatforms = new List<ObstacleRepresentation>(platforms);
            yPlatforms = new List<ObstacleRepresentation>(platforms);

            //xPlatforms = xPlatforms.OrderBy(o => o.X).ToList();
            yPlatforms = yPlatforms.OrderBy(o => o.Y).ToList();
            
            foreach (ObstacleRepresentation platform in platforms)
            {
                Debug.Print(platform.ToString() + "\n");
            }

            Debug.Print("\n\n\n");

            levelInfo = new List<List<PixelType>>((int)(area.Height / pixelSize + 1));

            for (int y = 0; y <= (int)(area.Height / pixelSize); y++)
            {
                levelInfo.Add(new List<PixelType>((int)(area.Width / pixelSize + 1)));

                for (int x = 0; x <= (int)(area.Width / pixelSize); x++)
                {
                    levelInfo[y].Add(PixelType.NONE);
                }
            }

            Debug.Print(levelInfo.Count.ToString());

            foreach (ObstacleRepresentation platform in platforms)
            {
                int startX = (int)((platform.X - area.X - (platform.Width / 2)) / pixelSize);
                int endX = Math.Min((int)Math.Ceiling((platform.X - area.X + (platform.Width / 2)) / pixelSize), area.Width / pixelSize);

                int startY = (int)((platform.Y - area.Y - (platform.Height / 2)) / pixelSize);
                int endY = Math.Min((int)Math.Ceiling((platform.Y - area.Y + (platform.Height / 2)) / pixelSize), area.Height / pixelSize);

                for (int y = startY; y <= endY; y++)
                {
                    for (int x = startX; x <= endX; x++)
                    {
                        levelInfo[y][x] = PixelType.PLATFORM;
                    }
                }
            }

            foreach (ObstacleRepresentation platform in rectanglePlatforms)
            {
                int startX = (int)((platform.X - area.X - (platform.Width / 2)) / pixelSize);
                int endX = Math.Min((int)Math.Ceiling((platform.X - area.X + (platform.Width / 2)) / pixelSize), area.Width / pixelSize);

                int startY = (int)((platform.Y - area.Y - (platform.Height / 2)) / pixelSize);
                int endY = Math.Min((int)Math.Ceiling((platform.Y - area.Y + (platform.Height / 2)) / pixelSize), area.Height / pixelSize);

                for (int y = startY; y <= endY; y++)
                {
                    for (int x = startX; x <= endX; x++)
                    {
                        levelInfo[y][x] = PixelType.RECTANGLE_PLATFORM;
                    }
                }
            }

            foreach (ObstacleRepresentation platform in circlePlatforms)
            {
                int startX = (int)((platform.X - area.X - (platform.Width / 2)) / pixelSize);
                int endX = Math.Min((int)Math.Ceiling((platform.X - area.X + (platform.Width / 2)) / pixelSize), area.Width / pixelSize);

                int startY = (int)((platform.Y - area.Y - (platform.Height / 2)) / pixelSize);
                int endY = Math.Min((int)Math.Ceiling((platform.Y - area.Y + (platform.Height / 2)) / pixelSize), area.Height / pixelSize);

                for (int y = startY; y <= endY; y++)
                {
                    for (int x = startX; x <= endX; x++)
                    {
                        levelInfo[y][x] = PixelType.CIRCLE_PLATFORM;
                    }
                }
            }

            foreach (CollectibleRepresentation diamond in diamonds)
            {
                int x = Math.Min((int)((diamond.X - area.X) / pixelSize), area.Width / pixelSize);
                int y = Math.Min((int)((diamond.Y - area.Y) / pixelSize), area.Height / pixelSize);

                levelInfo[y][x] = PixelType.DIAMOND;
            }
            
        }

        void ApplyRules(CircleRepresentation c, RectangleRepresentation r)
        {
            List<SortedDiamond> circleDiamonds = new List<SortedDiamond>();
            List<SortedDiamond> rectangleDiamonds = new List<SortedDiamond>();
            List<SortedDiamond> coopDiamonds = new List<SortedDiamond>();

            foreach(CollectibleRepresentation diamond in diamonds)
            {
                // Rules return 0 for circle diamond, 1 for rectangle diamond and 2 for coop

                
            }
        }

        int unreachableByJump(float dX, float dY, CircleRepresentation c, RectangleRepresentation r)
        {
            if(c.Y - dY < maxJump)
            {
                return 0;
            }

            float varX = dX, varY = dY;

            foreach (ObstacleRepresentation platform in yPlatforms)
            {
                if(varY <= c.Y)
                {
                    break;
                }

                if (varY <= platform.Y)
                {
                    continue;
                }
                else if (platform.Y - varY < maxJump)
                {
                    varY = platform.Y;
                }
                else
                {
                    return 2;
                }
            }

            return 0;
        }

        int unreachableBetweenPlatforms(float dX, float dY)
        {
            ObstacleRepresentation closestAbove = new ObstacleRepresentation(dX, levelArea.Y, 0, 0), closestBelow = new ObstacleRepresentation(dX, levelArea.Height + levelArea.Y, 0, 0);

            // Might be able to be changed into logarithmic complexity
            // Might not work for some combinations of Circle and regular platforms
            foreach(ObstacleRepresentation platform in yPlatforms)
            {
                // Check that diamond is above or below platform (same X)
                if(platform.X - platform.Width / 2 < dX && dX < platform.X + platform.Width / 2)
                {
                    // Since its ordered the first below is the closest
                    if(platform.Y > dY && platform.Y < closestBelow.Y)
                    {
                        closestBelow = platform;
                        break;
                    }
                    else if(platform.Y < dY && platform.Y > closestAbove.Y)
                    {
                        closestAbove = platform;
                    }
                }
            }

            // Either rectangle or coop
            if(closestBelow.Y - closestAbove.Y <= maxRadius * 2)
            {
                
            }

            return 0;
        }

        public override String ToString()
        {
            String result = "";

            for (int y = 0; y < levelInfo[0].Count + 1; y++)
            {
                result += "++";
            }

            result += "\n";

            for (int y = 0; y < levelInfo.Count; y++) 
            {
                result += "+";
                for (int x = 0; x < levelInfo[0].Count; x++)
                {
                    result += levelInfo[y][x] == PixelType.PLATFORM ? "PP" : (levelInfo[y][x] == PixelType.DIAMOND ? "DD" : "  ");
                }

                result += "+\n";
            }

            for (int y = 0; y < levelInfo[0].Count + 1; y++)
            {
                result += "++";
            }

            result += "\n";

            foreach(SortedDiamond d in circleDiamonds)
            {
                result += d.ToString() + "\n";
            }

            foreach (SortedDiamond d in rectangleDiamonds)
            {
                result += d.ToString() + "\n";
            }

            foreach (SortedDiamond d in coopDiamonds)
            {
                result += d.ToString() + "\n";
            }

            return result;
        }

        /********************************************/
        /***************** GETTERS ******************/
        /********************************************/

        public List<SortedDiamond> getCircleDiamonds()
        {
            return circleDiamonds;
        }

        public List<SortedDiamond> getRectangleDiamonds()
        {
            return rectangleDiamonds;
        }

        public List<SortedDiamond> getCoopDiamonds()
        {
            return coopDiamonds;
        }

        /********************************************/
        /***************** SETTERS ******************/
        /********************************************/

        public void setCircleDiamonds(List<SortedDiamond> diamondInfo)
        {
            circleDiamonds = diamondInfo;
        }

        public void setRectangleDiamonds(List<SortedDiamond> diamondInfo)
        {
            rectangleDiamonds = diamondInfo;
        }
        public void setCoopDiamonds(List<SortedDiamond> diamondInfo)
        {
            coopDiamonds = diamondInfo;
        }

        /**********************************/
        /********* DIAMOND UPDATES ********/
        /**********************************/

        //Since SensorUpdate gets all Diamonds, we need to keep filtering the diamonds to see which ones got caught
        public void updateCircleDiamonds(CollectibleRepresentation[] diamondInfo)
        {
            CollectibleRepresentation[] newDiamondCollectible = new CollectibleRepresentation[circleDiamonds.Count()];
            int i = 0;

            foreach(CollectibleRepresentation diamond in diamondInfo)
            {
                //if contains work
                if (circleDiamonds.Contains(diamond))
                {
                    newDiamondCollectible[i] = diamond;
                    i++;
                }
                //if it doesnt we need to do it hardcoded by transversing the vectors
            }

            circleDiamonds = newDiamondCollectible;
            return circleDiamonds;
        }

        public CollectibleRepresentation[] updateRectangleDiamonds(CollectibleRepresentation[] diamondInfo)
        {
            CollectibleRepresentation[] newDiamondCollectible = new CollectibleRepresentation[rectangleDiamonds.Count()];
            int i = 0;

            foreach (CollectibleRepresentation diamond in diamondInfo)
            {
                if (rectangleDiamonds.Contains(diamond))
                {
                    newDiamondCollectible[i] = diamond;
                    i++;
                }
            }

            rectangleDiamonds = newDiamondCollectible;
            return rectangleDiamonds;
        }

        public CollectibleRepresentation[] updateCoopDiamonds(CollectibleRepresentation[] diamondInfo)
        {
            CollectibleRepresentation[] newDiamondCollectible = new CollectibleRepresentation[coopDiamonds.Count()];
            int i = 0;

            foreach (CollectibleRepresentation diamond in diamondInfo)
            {
                if (coopDiamonds.Contains(diamond))
                {
                    newDiamondCollectible[i] = diamond;
                    i++;
                }
            }

            coopDiamonds = newDiamondCollectible;
            return coopDiamonds;
        }

        
        public void sendRectangleDiamonds()
        {
            messages.Add(new AgentMessage("Sending RectangleDiamongs", rectangleDiamonds));
        }

        public void recieveRectangleDiamonds()
        {
            rectangleDiamonds = (CollectibleRepresentation[]) messages[0].Attachment;
        }
    }

    
}
