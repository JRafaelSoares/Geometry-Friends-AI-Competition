namespace GeometryFriendsAgents
{
    using System;

    internal class ADNode : CVector2
    {
        public int platform;

        public ADNode(ADNode in_node)
        {
            this.platform = -1;
            base.x = in_node.x;
            base.y = in_node.y;
            this.platform = in_node.platform;
        }

        public ADNode(float in_x, float in_y, int in_plat)
        {
            this.platform = -1;
            base.x = in_x;
            base.y = in_y;
            this.platform = in_plat;
        }
    }
}

