using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryFriends.AI;
using GeometryFriends.AI.ActionSimulation;

namespace GeometryFriendsAgents
{
    //The tree used for the RRT algorithm
    public class TreeMPGS : TreeMP
    {
        //constructor
        public TreeMPGS(StateMP initialState, ActionSimulator predictor, List<Moves[]> moves, bool BGT) : base(initialState, moves, BGT)
        {
            setRoot(new NodeMPGS(null, initialState, null, predictor, moves));
            addNode(getRoot());
        }

    }
}
