using System;
using GeometryFriends.AI.Perceptions.Information;

namespace GeometryFriendsAgents
{
    public abstract class FilterRule
    {
        public FilterRule() { }

        public abstract ActionRule diamondFilter(RectangleRepresentation rI, CircleRepresentation cI, ObstacleRepresentation diamond);
    }
}
