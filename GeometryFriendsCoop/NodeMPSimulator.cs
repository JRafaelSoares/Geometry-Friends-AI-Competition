using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryFriends.AI;
using GeometryFriends.AI.ActionSimulation;
using GeometryFriends.AI.Debug;

namespace GeometryFriendsAgents
{
    public class NodeMPSimulator : NodeMP
    {
        Simulator simulator;

        public NodeMPSimulator(NodeMP p, StateMP s, Moves[] action, Simulator predictor, List<Moves[]> moves) : base(p, s, action, moves)
        {
            simulator = predictor;
        }

        public Simulator getSimulator()
        {
            return simulator;
        }

        public NodeMPSimulator clone()
        {
            NodeMPSimulator newNode = new NodeMPSimulator(getParent(), getState(), getActions(), simulator, getRemainingMoves());
            foreach (NodeMP child in getChildren())
            {
                newNode.addChild(child);
            }
            newNode.setTreeDepht(this.getTreeDepth());
            newNode.setRABool(anyRemainingSTPActions());
            if (newNode.getChildren().Count != 0)
            {
                newNode.nonLeaft();
            }
            return newNode;
        }
    }
}
