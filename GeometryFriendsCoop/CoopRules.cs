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
        private CollectibleRepresentation[] diamonds;

        List<SortedDiamond> circleDiamonds;
        List<SortedDiamond> rectangleDiamonds;
        List<SortedDiamond> coopDiamonds;

        private Rectangle levelArea;
        private List<AgentMessage> messages = new List<AgentMessage>();

        public CoopRules(Rectangle area, CollectibleRepresentation[] diamonds, ObstacleRepresentation[] platforms, ObstacleRepresentation[] rectanglePlatforms, ObstacleRepresentation[] circlePlatforms)
        {
            this.diamonds = diamonds;
            levelArea = area;

            yPlatforms = new List<ObstacleRepresentation>(platforms);

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
                // Rules return 0 for no match and 1 for match

                
            }
        }

        int unreachableByJump(float dX, float dY, float cStartHeight)
        {
            if(cStartHeight - dY < maxJump)
            {
                return 0;
            }

            float varX = dX, varY = dY;

            foreach (ObstacleRepresentation platform in yPlatforms)
            {
                if(varY <= cStartHeight)
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
                    return 1;
                }
            }

            return 0;
        }

        int unreachableBetweenPlatforms(float dX, float dY, float rStartHeight)
        {
            int x = Math.Min((int)((dX - levelArea.X) / pixelSize), levelArea.Width / pixelSize);
            int y = Math.Min((int)((dY - levelArea.Y) / pixelSize), levelArea.Height / pixelSize);

            int dUp, dDown;

            for(dUp = 1; y - dUp >= 0; dUp++)
            {
                if(levelInfo[y - dUp][x] == PixelType.PLATFORM || levelInfo[y - dUp][x] == PixelType.CIRCLE_PLATFORM)
                {
                    break;
                }
            }

            for (dDown = 1; y + dDown < levelInfo.Count; dDown++)
            {
                if (levelInfo[y + dDown][x] == PixelType.PLATFORM || levelInfo[y + dDown][x] == PixelType.CIRCLE_PLATFORM)
                {
                    break;
                }
            }

            if((dUp + dDown - 1) * pixelSize < maxRadius * 2)
            {
                // The space between the platforms is too narrow for the circle
                // But the rectangle might still be able to reach it if it's at a height above or equal to the first platform
                if((y + dDown) * pixelSize + levelArea.Y <= rStartHeight)
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

        public CollectibleRepresentation[] getCircleDiamonds()
        {
            return circleDiamonds;
        }

        public CollectibleRepresentation[] getRectangleDiamonds()
        {
            return rectangleDiamonds;
        }

        public CollectibleRepresentation[] getCoopDiamonds()
        {
            return coopDiamonds;
        }

        /********************************************/
        /***************** SETTERS ******************/
        /********************************************/

        public void setCircleDiamonds(CollectibleRepresentation[] diamondInfo)
        {
            circleDiamonds = diamondInfo;
        }

        public void setRectangleDiamonds(CollectibleRepresentation[] diamondInfo)
        {
            rectangleDiamonds = diamondInfo;
        }
        public void setCoopDiamonds(CollectibleRepresentation[] diamondInfo)
        {
            coopDiamonds = diamondInfo;
        }

        /**********************************/
        /********* DIAMOND UPDATES ********/
        /**********************************/

        //Since SensorUpdate gets all Diamonds, we need to keep filtering the diamonds to see which ones got caught
        public CollectibleRepresentation[] updateCircleDiamonds(CollectibleRepresentation[] diamondInfo)
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
