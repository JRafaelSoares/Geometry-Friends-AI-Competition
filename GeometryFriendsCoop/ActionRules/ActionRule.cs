using System;
using System.Collections.Generic;
using System.Drawing;
using GeometryFriends.AI;
using GeometryFriends.AI.Perceptions.Information;

namespace GeometryFriendsAgents
{
    public abstract class ActionRule
    {
        private List<ActionState> actionStates;
        private List<ActionState>.Enumerator state;
        private int diamondType;
        private bool finished = false;
        
        //Diamond types
        private CollectibleRepresentation diamond;
        public ActionRule() { }

        public Moves getAction()
        {
            return state.Current.getAction();
        }

        public ActionState getCurrentState()
        {
            return state.Current;
        }

        public bool nextState()
        {
            return state.MoveNext();
        }

        public void Update(TimeSpan elapsedGameTime)
        {
            state.Current.Update(elapsedGameTime);
        }

        public void SensorsUpdate(RectangleRepresentation rI, CircleRepresentation cI, CollectibleRepresentation[] colI)
        {
            if (!state.Current.isFinished())
            {
                //Caution: collectibles sent may not be the ones to be caught
                state.Current.SensorsUpdate(rI, cI, colI);
            }
            else
            {
                finished = state.MoveNext();
            }
        }

        public bool hasFinished()
        {
            return finished;
        }

        public void Setup(CountInformation nI, RectangleRepresentation rI, CircleRepresentation cI, ObstacleRepresentation[] oI, ObstacleRepresentation[] rPI, ObstacleRepresentation[] cPI, CollectibleRepresentation[] colI, Rectangle area, double timeLimit)
        {
            state.Current.Setup(nI, rI, cI, oI, rPI, cPI, colI, area, timeLimit);
        }
        
    }
}
