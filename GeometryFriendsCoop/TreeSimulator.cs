using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryFriends.AI;
using GeometryFriends.AI.ActionSimulation;

namespace GeometryFriendsAgents
{
    //Tree for use with general Simulator
    public class TreeSimulator : Tree
    {

        //constructor
        public TreeSimulator(State initialState, Simulator sim, List<Moves> moves, bool BGT) : base(initialState, moves, BGT)
        {

            setRoot(new Node(null, initialState, 0, sim, moves));
            addNode(getRoot());
        }

    }
}
