namespace GeometryFriendsAgents
{
    using System;
    using System.Collections.Generic;

    internal class ActionMap
    {
        private List<ActionPoint> AP = new List<ActionPoint>();

        public int AddAP(ActionPoint _AP)
        {
            this.AP.Add(_AP);
            return 0;
        }

        public List<ActionPoint> getAPdirection(CVector2 vec)
        {
            List<ActionPoint> list = new List<ActionPoint>();
            foreach (ActionPoint point in this.AP)
            {
                if ((point.start_point.x == vec.x) && (point.start_point.y == vec.y))
                {
                    list.Add(new ActionPoint(point.start_point, point.end_point));
                    continue;
                }
                if ((point.end_point.x == vec.x) && (point.end_point.y == vec.y))
                {
                    list.Add(new ActionPoint(point.end_point, point.start_point));
                }
            }
            return list;
        }

        public List<ActionPoint> getAPs() => 
            this.AP;

        public void print()
        {
            foreach (ActionPoint point in this.AP)
            {
                point.print();
            }
        }

        public void RemoveDuplicate()
        {
            List<ActionPoint> aP = this.AP;
            int num = 0;
            while (num < aP.Count)
            {
                int count = 0;
                while (true)
                {
                    if (count >= aP.Count)
                    {
                        num++;
                        break;
                    }
                    if (((num != count) && ((aP[num].start_point.x == aP[count].end_point.x) && ((aP[num].start_point.y == aP[count].end_point.y) && (aP[num].end_point.x == aP[count].start_point.x)))) && (aP[num].end_point.y == aP[count].start_point.y))
                    {
                        aP.Remove(aP[count]);
                        count = aP.Count;
                        num--;
                    }
                    count++;
                }
            }
        }
    }
}

