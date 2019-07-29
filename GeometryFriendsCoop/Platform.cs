using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryFriendsAgents
{
    public class Platform
    {
        private float posX;
        private float posY;
        private float width;
        private float height;
        private PlatformType type;
        private List<int> diamondsOn;

        public Platform(float x, float y, float w, float h, PlatformType t)
        {
            posX = x;
            posY = y;
            width = w;
            height = h;
            type = t;
            diamondsOn = new List<int>();
        }

        public float getX()
        {
            return posX;
        }

        public float getY()
        {
            return posY;
        }

        public float getWidth()
        {
            return width;
        }

        public float getHeight()
        {
            return height;
        }

        public List<int> getDiamondsOn()
        {
            return diamondsOn;
        }

        public void addDiamondOn(int diamond)
        {
            diamondsOn.Add(diamond);
        }

        public PlatformType getType()
        {
            return type;
        }
    }
}
