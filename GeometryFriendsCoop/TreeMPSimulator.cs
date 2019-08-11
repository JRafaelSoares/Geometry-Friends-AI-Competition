using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryFriends.AI;
using GeometryFriends.AI.ActionSimulation;

namespace GeometryFriendsAgents
{
    //The tree used for the RRT algorithm
    public class TreeMPSimulator : TreeMP
    {
        //constructor
        public TreeMPSimulator(StateMP initialState, Simulator predictor, List<Moves[]> moves, bool BGT) : base(initialState, moves, BGT)
        {
            setRoot(new NodeMPSimulator(null, initialState, null, predictor, moves));
            addNode(getRoot());
        }

    }
}
