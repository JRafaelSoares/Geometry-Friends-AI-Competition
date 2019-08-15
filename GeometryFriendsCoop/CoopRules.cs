using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using GeometryFriends;
using GeometryFriends.AI.Perceptions.Information;

namespace GeometryFriendsAgents
{
    public class CoopRules
    {
        private int pixelSize = 40;
        private List<List<PixelType>> levelInfo;
        private CollectibleRepresentation[] diamonds;
        CollectibleRepresentation[] circleDiamonds;
        CollectibleRepresentation[] rectangleDiamonds;
        CollectibleRepresentation[] coopDiamonds;
        private Rectangle levelArea;

        public CoopRules(Rectangle area, CollectibleRepresentation[] diamonds, ObstacleRepresentation[] platforms, ObstacleRepresentation[] rectanglePlatforms, ObstacleRepresentation[] circlePlatforms)
        {
            this.diamonds = diamonds;
            levelArea = area;

            foreach(ObstacleRepresentation platform in platforms)
            {
                Debug.Print(platform.ToString() + "\n");
            }

            Debug.Print("\n\n\n");

            levelInfo = new List<List<PixelType>>((int)(area.Height / pixelSize + 1));

            for(int y = 0; y <= (int)(area.Height / pixelSize); y++)
            {
                levelInfo.Add(new List<PixelType>((int)(area.Width / pixelSize + 1)));

                for(int x = 0; x <= (int)(area.Width / pixelSize); x++)
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

            foreach(ObstacleRepresentation platform in rectanglePlatforms)
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

            foreach(ObstacleRepresentation platform in circlePlatforms)
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

            foreach(CollectibleRepresentation diamond in diamonds)
            {
                int x = Math.Min((int)((diamond.X - area.X) / pixelSize), area.Width / pixelSize);
                int y = Math.Min((int)((diamond.Y - area.Y) / pixelSize), area.Height / pixelSize);

                levelInfo[y][x] = PixelType.DIAMOND;
            }
        }

        public void ApplyRules()
        {
            List<CollectibleRepresentation> tmpCircleDiamonds = new List<CollectibleRepresentation>();
            List<CollectibleRepresentation> tmpRectangleDiamonds = new List<CollectibleRepresentation>();
            List<CollectibleRepresentation> tmpCoopDiamonds = new List<CollectibleRepresentation>();

            foreach(CollectibleRepresentation diamond in diamonds)
            {
                // Rules return 0 for circleDiamonds, 1 for rectangleDiamonds, 2 for coopDiamonds and 3 for no decision

                
            }
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
    }
}
