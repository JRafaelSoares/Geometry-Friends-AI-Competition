using System;
using System.Collections.Generic;
using System.Drawing;
using GeometryFriends.AI;
using GeometryFriends.AI.ActionSimulation;
using GeometryFriends.AI.Perceptions.Information;

namespace GeometryFriendsAgents
{
    public abstract class ActionRule
    {
        protected CountInformation nI;
        protected RectangleRepresentation rI;
        protected CircleRepresentation cI;
        protected ObstacleRepresentation[] oI;
        protected ObstacleRepresentation[] rPI;
        protected ObstacleRepresentation[] cPI;
        protected CollectibleRepresentation[] colI;
        protected Rectangle area;

        protected List<ActionState> actionStatesCircle = null;
        protected List<ActionState> actionStatesRectangle = null;

        protected int currentStateCircle;
        protected int currentStateRectangle;

        private bool finished = false;
        
        private CollectibleRepresentation diamond;

        public ActionRule()
        {
            currentStateCircle = 0;
            currentStateRectangle = 0;
        }

        public virtual Moves getActionCircle()
        {
            if(actionStatesCircle != null && currentStateCircle < actionStatesCircle.Count)
            {
                return actionStatesCircle[currentStateCircle].getAction();
            }
            else
            {
                return Moves.NO_ACTION;
            }
        }

        public virtual Moves getActionRectangle()
        {
            if (actionStatesRectangle != null && currentStateRectangle < actionStatesRectangle.Count)
            {
                return actionStatesRectangle[currentStateRectangle].getAction();
            }
            else
            {
                return Moves.NO_ACTION;
            }
        }

        public virtual void Update(TimeSpan elapsedGameTime)
        {
            if (actionStatesRectangle != null && currentStateRectangle < actionStatesRectangle.Count)
            {
                actionStatesCircle[currentStateCircle].Update(elapsedGameTime);
                actionStatesRectangle[currentStateRectangle].Update(elapsedGameTime);
            }
        }

        /*
         
            Does only independent linear progressions between states (So if, for example, there is a circle state that to finish depends on a rectangle state, then this function must be overriden)
             
        */
        public virtual void SensorsUpdate(RectangleRepresentation rI, CircleRepresentation cI, CollectibleRepresentation[] colI)
        {
            if (actionStatesRectangle == null || (actionStatesRectangle != null && currentStateRectangle < actionStatesRectangle.Count))
            {
                return;
            }

            if (isFinished())
            {
                return;
            }

            this.rI = rI;
            this.cI = cI;
            this.colI = colI;

            //Caution: its each state's responsibility to filter the information sent to it by the ation rule
            if (!actionStatesCircle[currentStateCircle].isFinished())
            {
                actionStatesCircle[currentStateCircle].SensorsUpdate(rI, cI, colI);
            }
            else
            {
                if(currentStateCircle < actionStatesCircle.Count - 1)
                {
                    currentStateCircle++;
                    actionStatesCircle[currentStateCircle].Setup(nI, rI, cI, oI, rPI, cPI, colI, area, 100.0);
                }
                else if(currentStateRectangle >= actionStatesRectangle.Count - 1 && actionStatesRectangle[currentStateRectangle].isFinished())
                {
                    setFinished();
                    return;
                }
            }

            if (!actionStatesRectangle[currentStateRectangle].isFinished())
            {
                actionStatesRectangle[currentStateRectangle].SensorsUpdate(rI, cI, colI);
            }
            else
            {
                if (currentStateRectangle < actionStatesRectangle.Count - 1)
                {
                    currentStateRectangle++;
                    actionStatesRectangle[currentStateRectangle].Setup(nI, rI, cI, oI, rPI, cPI, colI, area, 100.0);
                }
            }
        }

        public void setFinished()
        {
            finished = true;
        }

        public bool isFinished()
        {
            return finished;
        }

        public virtual void Setup(CountInformation nI, RectangleRepresentation rI, CircleRepresentation cI, ObstacleRepresentation[] oI, ObstacleRepresentation[] rPI, ObstacleRepresentation[] cPI, CollectibleRepresentation[] colI, Rectangle area, double timeLimit)
        {
            this.nI = nI;
            this.rI = rI;
            this.cI = cI;
            this.oI = oI;
            this.rPI = rPI;
            this.cPI = cPI;
            this.colI = colI;
            this.area = area;
        }

        public virtual void ActionSimulatorUpdated(ActionSimulator updatedSimulator)
        {
            if (actionStatesRectangle != null && currentStateRectangle < actionStatesRectangle.Count)
            {
                actionStatesCircle[currentStateCircle].ActionSimulatorUpdated(updatedSimulator);
                actionStatesRectangle[currentStateRectangle].ActionSimulatorUpdated(updatedSimulator);
            }
        }
    }
}
