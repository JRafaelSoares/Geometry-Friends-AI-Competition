namespace GeometryFriendsAgents
{
    using System;

    internal class ActionNode
    {
        public int cmd;
        public float posX;
        public float posY;
        public float action;
        public int platform;

        public ActionNode()
        {
            this.platform = -1;
        }

        public ActionNode(int in_cmd, float in_posX)
        {
            this.platform = -1;
            this.cmd = in_cmd;
            this.posX = in_posX;
            this.action = 0f;
        }
    }
}

