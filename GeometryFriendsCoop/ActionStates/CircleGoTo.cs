using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using GeometryFriends.AI;
using GeometryFriends.AI.Perceptions.Information;

namespace GeometryFriendsAgents.ActionStates
{
    class CircleGoTo : ActionState
    {
        /*
        
            Can go to either diamond or position (always using singleplayer implementation for movement purposes)
        
        */

        CollectibleRepresentation[] objectiveDiamond;

        CircleSingleplayer singleplayer;

        float rectangleArea = 10000;

        bool rectangleAsPlatform;

        bool setup = false;

        public CircleGoTo(CircleSingleplayer singleplayer, CollectibleRepresentation objectiveDiamond, bool rectangleAsPlatform) : base()
        {
            this.singleplayer = new CircleSingleplayer(true, true, true);

            this.objectiveDiamond = new CollectibleRepresentation[1];

            this.objectiveDiamond[0] = objectiveDiamond;

            this.rectangleAsPlatform = rectangleAsPlatform;
        }

        public CircleGoTo(CircleSingleplayer singleplayer, CollectibleRepresentation objectiveDiamond) : this(singleplayer, objectiveDiamond, false)
        {

        }

        public CircleGoTo(CircleSingleplayer singleplayer, float x, float y) : this(singleplayer, new CollectibleRepresentation(x, y), false)
        {

        }

        public CircleGoTo(CircleSingleplayer singleplayer, float x, float y, bool rectangleAsPlatform) : this(singleplayer, new CollectibleRepresentation(x, y), rectangleAsPlatform)
        {

        }


        public override Moves getAction()
        {
            Debug.Print(singleplayer.GetAction().ToString());

            return singleplayer.GetAction();
        }

        public override void SensorsUpdate(RectangleRepresentation rI, CircleRepresentation cI, CollectibleRepresentation[] colI)
        {
            if (!isFinished() && Math.Abs(cI.X - objectiveDiamond[0].X) < 5 && Math.Abs(cI.Y - objectiveDiamond[0].Y) < 5)
            {
                setFinished();

                objectiveDiamond = new CollectibleRepresentation[0];
            }

            singleplayer.SensorsUpdated(objectiveDiamond.Length, rI, cI, objectiveDiamond);
        }

        public override void Setup(CountInformation nI, RectangleRepresentation rI, CircleRepresentation cI, ObstacleRepresentation[] oI, ObstacleRepresentation[] rPI, ObstacleRepresentation[] cPI, CollectibleRepresentation[] colI, Rectangle area, double timeLimit)
        {
            CountInformation nIFixed = new CountInformation(nI.ObstaclesCount + (rectangleAsPlatform ? 1 : 0), nI.RectanglePlatformsCount, nI.CirclePlatformsCount, 1);

            ObstacleRepresentation[] oIFixed;

            if (rectangleAsPlatform)
            {
                List<ObstacleRepresentation> oIList = new List<ObstacleRepresentation>(oI);
                oIList.Add(new ObstacleRepresentation(rI.X, rI.Y, rectangleArea / rI.Height, rI.Height));

                oIFixed = oIList.ToArray();
            }
            else
            {
                oIFixed = oI;
            }

            singleplayer.Setup(nIFixed, rI, cI, oIFixed, rPI, cPI, objectiveDiamond, area, timeLimit);
            setup = true;
        }

        public override void Update(TimeSpan elapsedGameTime)
        {
            if (setup)
            {
                singleplayer.Update(elapsedGameTime);
            }
        }
    }
}
