namespace GeometryFriendsAgents
{
    using System;

    internal class ActionPoint
    {
        public ADNode start_point;
        public ADNode end_point;
        public CVector2 direction;
        public float distance;
        public int platform_st = -1;
        public int platform_ed = -1;

        public ActionPoint(ADNode pt_1, ADNode pt_2)
        {
            this.start_point = pt_1;
            this.end_point = pt_2;
            this.direction = new CVector2();
            this.direction.x = pt_2.x - pt_1.x;
            this.direction.y = pt_2.y - pt_1.y;
            this.distance = this.direction.length();
        }

        public float getweight() => 
            this.distance;

        public void print()
        {
        }
    }
}

