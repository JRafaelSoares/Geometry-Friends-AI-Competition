namespace GeometryFriendsAgents
{
    using System;

    internal class Edge
    {
        public CVector2 pt_1;
        public CVector2 pt_2;
        public int platform_1;
        public int platform_2;

        public Edge(CVector2 vec_st, CVector2 vec_ed)
        {
            this.platform_1 = -1;
            this.platform_2 = -1;
            this.pt_1 = vec_st;
            this.pt_2 = vec_ed;
        }

        public Edge(float x1, float y1, float x2, float y2, int in_platform_1, int in_platform_2)
        {
            this.platform_1 = -1;
            this.platform_2 = -1;
            this.pt_1 = new CVector2(x1, y1);
            this.pt_2 = new CVector2(x2, y2);
            this.platform_1 = in_platform_1;
            this.platform_2 = in_platform_2;
        }

        public CVector2 col(Edge in_edge)
        {
            CVector2[] vectorArray = nearraytoline(new CVector2(this.pt_2 - this.pt_1), new CVector2(in_edge.pt_1 - this.pt_1), new CVector2(in_edge.pt_2 - this.pt_1));
            if (vectorArray == null)
            {
                return null;
            }
            CVector2 pt = vectorArray[0];
            pt.add(this.pt_1);
            if (!this.col_PointtoBox(pt, this.pt_1, this.pt_2) || !this.col_PointtoBox(pt, in_edge.pt_1, in_edge.pt_2))
            {
                return null;
            }
            return pt;
        }

        public bool col_PointtoBox(CVector2 pt, CVector2 Box_pt1, CVector2 Box_pt2)
        {
            CVector2 vector = new CVector2(Math.Min(Box_pt1.x, Box_pt2.x), Math.Min(Box_pt1.y, Box_pt2.y));
            CVector2 vector2 = new CVector2(Math.Max(Box_pt1.x, Box_pt2.x), Math.Max(Box_pt1.y, Box_pt2.y));
            return ((pt.x >= vector.x) && ((vector2.x >= pt.x) && ((pt.y >= vector.y) && (vector2.y >= pt.y))));
        }

        public float length() => 
            (this.pt_2 - this.pt_1).length();

        public static CVector2[] nearraytoline(CVector2 ray, CVector2 Lb_s, CVector2 Lb_e)
        {
            CVector2 vector = Lb_e - Lb_s;
            CVector2 vector2 = Lb_s;
            float num3 = (vector.y * ray.x) - (vector.x * ray.y);
            if (num3 == 0f)
            {
                return null;
            }
            float num = ((vector.y * vector2.x) - (vector.x * vector2.y)) / num3;
            float num2 = ((ray.x * num) - vector2.x) / vector.x;
            return new CVector2[] { new CVector2(ray.x * num, ray.y * num), new CVector2((vector.x * num2) + vector2.x, (vector.y * num2) + vector2.y) };
        }
    }
}

