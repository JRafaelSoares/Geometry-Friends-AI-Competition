using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using GeometryFriends.AI;
using GeometryFriends.AI.Perceptions.Information;
using GeometryFriendsAgents.ActionStates;

namespace GeometryFriendsAgents
{
    public class HeightRule : ActionRule
    {

        float coopX;
        float coopY;

        CollectibleRepresentation diamond;

        public HeightRule(float coopX, float coopY, CollectibleRepresentation diamond) : base()
        {
            this.coopX = coopX;
            this.coopY = coopY;

            this.diamond = diamond;

            actionStatesCircle = new List<ActionState>();
            actionStatesRectangle = new List<ActionState>();

            actionStatesCircle.Add(new ActionState());
            actionStatesCircle.Add(new CircleGoTo(coopX, coopY, true));
            actionStatesCircle.Add(new CircleStay(coopX, coopY));
            actionStatesCircle.Add(new CircleGoTo(diamond, true));

            actionStatesRectangle.Add(new MorphDown());
            actionStatesRectangle.Add(new RectangleGoToAndStay(coopX, coopY));
            actionStatesRectangle.Add(new MorphUp());
            actionStatesRectangle.Add(new RectangleGoToAndStay(coopX, coopY));
        }

        public override void Setup(CountInformation nI, RectangleRepresentation rI, CircleRepresentation cI, ObstacleRepresentation[] oI, ObstacleRepresentation[] rPI, ObstacleRepresentation[] cPI, CollectibleRepresentation[] colI, Rectangle area, double timeLimit)
        {
            base.Setup(nI, rI, cI, oI, rPI, cPI, colI, area, timeLimit);

            actionStatesCircle[currentStateCircle].Setup(nI, rI, cI, oI, rPI, cPI, colI, area, timeLimit);
            actionStatesRectangle[currentStateRectangle].Setup(nI, rI, cI, oI, rPI, cPI, colI, area, timeLimit);
        }

        public override void SensorsUpdate(RectangleRepresentation rI, CircleRepresentation cI, CollectibleRepresentation[] colI)
        {
            this.rI = rI;
            this.cI = cI;
            this.colI = colI;

            switch (currentStateCircle)
            {
                case 0:
                    if (Math.Abs(rI.X - coopX) < 1 && Math.Abs(rI.VelocityX) < 20)
                    {
                        currentStateCircle++;
                        actionStatesCircle[currentStateCircle].Setup(nI, rI, cI, oI, rPI, cPI, colI, area, 100.0);
                    }
                    break;
                case 1:
                    if (actionStatesCircle[currentStateCircle].isFinished())
                    {
                        currentStateCircle++;
                        actionStatesCircle[currentStateCircle].Setup(nI, rI, cI, oI, rPI, cPI, colI, area, 100.0);
                    }
                    break;
                case 2:
                    if (currentStateRectangle == 3)
                    {
                        currentStateCircle++;
                        actionStatesCircle[currentStateCircle].Setup(nI, rI, cI, oI, rPI, cPI, colI, area, 100.0);
                    }
                    break;
                case 3:
                    if (actionStatesCircle[currentStateCircle].isFinished())
                    {
                        setFinished();
                    }
                    break;
            }

            switch (currentStateRectangle)
            {
                case 0:
                    if (actionStatesRectangle[currentStateRectangle].isFinished())
                    {
                        currentStateRectangle++;
                        actionStatesRectangle[currentStateRectangle].Setup(nI, rI, cI, oI, rPI, cPI, colI, area, 100.0);
                    }
                    break;
                case 1:
                    if (currentStateCircle == 2)
                    {
                        currentStateRectangle++;
                        actionStatesRectangle[currentStateRectangle].Setup(nI, rI, cI, oI, rPI, cPI, colI, area, 100.0);
                    }
                    break;
                case 2:
                    if (actionStatesRectangle[currentStateRectangle].isFinished())
                    {
                        currentStateRectangle++;
                        actionStatesRectangle[currentStateRectangle].Setup(nI, rI, cI, oI, rPI, cPI, colI, area, 100.0);
                    }
                    break;
                case 3:
                    break;
            }

            actionStatesCircle[currentStateCircle].SensorsUpdate(rI, cI, colI);
            actionStatesRectangle[currentStateRectangle].SensorsUpdate(rI, cI, colI);
        }
    }
}
