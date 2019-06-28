namespace GeometryFriendsAgents
{
    using System;

    internal class BowWow
    {
        private static BowWow _instance;
        private static readonly object padlock = new object();
        private int ping;
        public bool direct;
        public float X;
        public float Y;
        public int cmd;
        public bool isfusion;

        private BowWow()
        {
        }

        public bool IsBow() => 
            (this.ping == 0);

        public bool Isping() => 
            (this.ping == 1);

        public void setBow()
        {
            this.ping = 0;
        }

        public void setPing()
        {
            this.ping = 1;
        }

        public static BowWow Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (padlock)
                    {
                        if (_instance == null)
                        {
                            _instance = new BowWow();
                        }
                    }
                }
                return _instance;
            }
        }
    }
}

