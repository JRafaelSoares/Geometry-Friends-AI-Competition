using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryFriends.AI.Perceptions.Information;

namespace GeometryFriendsAgents
{
    public class State
    {
        private float positionX;
        private float positionY;
        private float velocityX;
        private float velocityY;
        private float height;
        private float circleVelocityRadius;
        private List<DiamondInfo> caughtCollectibles;
        private List<DiamondInfo> uncaughtCollectibles;

        public State(float cPosX, float cPosY, float cVelX, float cVelY, float cH, float cVelRad, List<DiamondInfo> cC, List<DiamondInfo> uC)
        {
            positionX = cPosX;
            positionY = cPosY;
            velocityX = cVelX;
            velocityY = cVelY;
            height = cH;
            circleVelocityRadius = cVelRad;
            caughtCollectibles = cC;
            uncaughtCollectibles = uC;
        }

        public float getPosX()
        {
            return positionX;
        }

        public float getPosY()
        {
            return positionY;
        }

        public float getVelX()
        {
            return velocityX;
        }

        public float getVelY()
        {
            return velocityY;
        }

        public float getHeight()
        {
            return height;
        }

        public int getNumberCaughtCollectibles()
        {
            return caughtCollectibles.Count;
        }

        public int getNumberUncaughtCollectibles()
        {
            return uncaughtCollectibles.Count;
        }

        public List<DiamondInfo> getCaughtCollectibles()
        {
            return caughtCollectibles;
        }

        public List<DiamondInfo> getUncaughtCollectibles()
        {
            return uncaughtCollectibles;
        }

        public void addCaughtCollectibles(List<DiamondInfo> collectibles)
        {
            //actualizes all the collectibles that were caught without duplicating
            foreach(DiamondInfo collectible in collectibles)
            {
                if (!caughtCollectibles.Contains(collectible))
                {
                    caughtCollectibles.Add(collectible);
                }
            }
        }
    }
}
