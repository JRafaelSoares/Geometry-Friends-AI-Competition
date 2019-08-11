using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryFriends.AI;
using GeometryFriends.AI.ActionSimulation;
using GeometryFriends.AI.Debug;

namespace GeometryFriendsAgents
{
    public class NodeMPGS : NodeMP
    {
        ActionSimulator simulator;

        public NodeMPGS(NodeMP p, StateMP s, Moves[] action, ActionSimulator predictor, List<Moves[]> moves) : base(p, s, action, moves)
        {
            simulator = predictor;
        }

        public ActionSimulator getSimulator()
        {
            return simulator;
        }

        public NodeMPGS clone()
        {
            NodeMPGS newNode = new NodeMPGS(getParent(), getState(), getActions(), simulator, getRemainingMoves());
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
