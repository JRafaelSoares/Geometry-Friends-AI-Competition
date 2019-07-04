namespace GeometryFriendsAgents
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct node
    {
        public int x;
        public int y;
        public int parent_x;
        public int parent_y;
        public int g;
        public int h;
        public int f;
        public bool opened;
        public bool closed;
        public bool obstacle;
        public bool semi_block;
    }
}

