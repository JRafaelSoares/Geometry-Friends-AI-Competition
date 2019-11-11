namespace GeometryFriendsAgents
{
    using System;

    internal class CVector2
    {
        public float x;
        public float y;

        public CVector2()
        {
            this.x = 0;
            this.y = 0;
        }

        public CVector2(CVector2 vec)
        {
            this.x = vec.x;
            this.y = vec.y;
        }

        public CVector2(float in_x, float in_y)
        {
            this.x = in_x;
            this.y = in_y;
        }

        public void add(CVector2 vec)
        {
            this.x += vec.x;
            this.y += vec.y;
        }

        public bool equal(CVector2 vec) => 
            ((this.x == vec.x) && (this.y == vec.y));

        public float length() => 
            ((float) Math.Sqrt((double) ((this.x * this.x) + (this.y * this.y))));

        public static CVector2 operator +(CVector2 vec1, CVector2 vec2) => 
            new CVector2(vec1.x + vec2.x, vec1.y + vec2.y);

        public static CVector2 operator -(CVector2 vec1, CVector2 vec2) => 
            new CVector2(vec1.x - vec2.x, vec1.y - vec2.y);

        public string tostring() => 
            (this.x + "," + this.y);
    }
}

