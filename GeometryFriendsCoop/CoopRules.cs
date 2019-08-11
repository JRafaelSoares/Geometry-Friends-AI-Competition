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
    class CoopRules
    {
        private int pixelSize = 40;
        List<List<PixelType>> levelInfo;

        public CoopRules(Rectangle area, CollectibleRepresentation[] diamonds, ObstacleRepresentation[] platforms, ObstacleRepresentation[] rectanglePlatforms, ObstacleRepresentation[] circlePlatforms)
        {
            levelInfo = new List<List<PixelType>>((int)(area.Height / pixelSize));

            for(int y = 0; y < (int)(area.Height / pixelSize); y++)
            {
                levelInfo.Add(new List<PixelType>((int)(area.Width / pixelSize)));

                for(int x = 0; x < (int)(area.Width / pixelSize); x++)
                {
                    levelInfo[y].Add(PixelType.NONE);
                }
            }

            Debug.Print(levelInfo.Count.ToString());

            foreach (ObstacleRepresentation platform in platforms)
            {
                int startX = (int)(platform.X / pixelSize);
                int endX = (int)Math.Ceiling((platform.X + platform.Width) / pixelSize);

                int startY = (int)(platform.Y / pixelSize);
                int endY = (int)Math.Ceiling((platform.Y + platform.Height) / pixelSize);

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
                int startX = (int)(platform.X / pixelSize);
                int endX = (int)Math.Ceiling((platform.X + platform.Width) / pixelSize);

                int startY = (int)(platform.Y / pixelSize);
                int endY = (int)Math.Ceiling((platform.Y + platform.Height) / pixelSize);

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
                int startX = (int)(platform.X / pixelSize);
                int endX = (int)Math.Ceiling((platform.X + platform.Width) / pixelSize);

                int startY = (int)(platform.Y / pixelSize);
                int endY = (int)Math.Ceiling((platform.Y + platform.Height) / pixelSize);

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
                levelInfo[(int)(diamond.X / pixelSize)][(int)(diamond.Y / pixelSize)] = PixelType.DIAMOND;
            }
        }

        override
        public String ToString()
        {
            String result = "";

            for (int y = 0; y < levelInfo.Count; y++) 
            {
                for (int x = 0; x < levelInfo[0].Count; x++)
                {
                    result += levelInfo[y][x] + " ";
                }

                result += "\n";
            }

            return result;
        }
    }
}
