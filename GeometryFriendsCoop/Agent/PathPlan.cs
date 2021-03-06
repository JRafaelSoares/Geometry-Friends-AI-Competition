﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using GeometryFriends.AI;
using GeometryFriends.AI.Debug;
using GeometryFriends.AI.Perceptions.Information;
using System.Drawing;

namespace GeometryFriendsAgents
{
    public class PathPlan
    {

        private List<Point> pathPoints;
        private List<Point> originalCleanedPathPoints;
        private List<Point> originalPathPoints;
        private float cleanRollMargin = 50;
        private float cleanRectangleYMargin = 10;
        private float cleanJumpMargin = 10;
        private float platformYMargin = 10;
        private int totalCollectibles;
        private int currentPoint = 0;
        private int maxFailure = 2;
        private float circleMaxJump = 400;
        private bool cutPlan;
        private Utils utils;


        public PathPlan(bool c, int totalcol, Utils u)
        {
            pathPoints = new List<Point>();
            originalCleanedPathPoints = new List<Point>();
            originalPathPoints = new List<Point>();
            cutPlan = c;
            totalCollectibles = totalcol;
            utils = u;
            if(totalCollectibles == 0)
            {
                //problem
                totalCollectibles = 0;
            }
        }

        public PathPlan(bool c, int totalcol, List<Point> points, Utils u)
        {
            pathPoints = points;
            originalCleanedPathPoints = points;
            originalPathPoints = points;
            cutPlan = c;
            totalCollectibles = totalcol;
            utils = u;
            if (totalCollectibles == 0)
            {
                //problem
                totalCollectibles = 0;
            }
        }

        public void setTotalCollectibles(int col)
        {
            totalCollectibles = col;
        }

        public int getTotalCollectibles()
        {
            return totalCollectibles;
        }

        public List<Point> getPathPoints()
        {
            return pathPoints;
        }

        public void addPoint(Point point)
        {
            pathPoints.Insert(0, point);
        }

        public void addPointEnd(Point point)
        {
            pathPoints.Add(point);
        }

        //remove first point on the list after indicated that the agent passed through this point
        public void nextPoint()
        {
            if (cutPlan)
            {
                originalCleanedPathPoints[currentPoint].passedThrough();
            }
            else
            {
                originalPathPoints[currentPoint].passedThrough();
            }            
            currentPoint++;
            pathPoints.RemoveAt(0);
        }

        public void setCurrentPoint(int i)
        {
            currentPoint = i;
        }

        public int getCurrentPoint()
        {
            return currentPoint;
        }

        //check if the agent is keeps failing the action on the same point
        public bool checkIfConstantFail()
        {
            if (cutPlan)
            {
                return checkIfConstantFailAux(originalCleanedPathPoints);
            }
            else
            {
                return checkIfConstantFailAux(originalPathPoints);
            }
            
        }

        public bool checkIfConstantFailAux(List<Point> points)
        {
            //in case it only has one action
            if (points.Count == 1)
            {
                if (points[0].getTimesPassed() >= maxFailure)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            //check if stuck on other points
            for (int i = 0; i < points.Count - 1; i++)
            {
                if (points[i].getTimesPassed() - points[i + 1].getTimesPassed() >= maxFailure)
                {
                    return true;
                }
            }
            //check if stuck on the last point
            if (points[points.Count - 1].getTimesPassed() - points[points.Count - 2].getTimesPassed() >= maxFailure)
            {
                return true;
            }
            return false;
        }

        //circle 
        public List<DebugInformation> cleanPlan(ObstacleRepresentation[] platforms, List<DiamondInfo> diamonds, Rectangle area, float circleRadius, bool first, bool remFirst)
        {
            //TODO - check other cases
            bool stopFor = false;
            bool removedFirstJump = false;
            bool removeFirst = remFirst;
            int margin = 50;

            if (pathPoints.Count != 0) {
                //if the first action is JUMP and there is no diamond above, then it is not necessary 
                //int margin = 10;
                //bool above = false;
                if (first && pathPoints[0].getAction() == Moves.JUMP)
                {
                    Platform initialPlatform = utils.onPlatform(pathPoints[0].getPosX(), pathPoints[0].getPosY(), 25, platformYMargin);
                    if(initialPlatform != null && pathPoints.Count > 1)
                    {
                        int i = 1;
                        Platform auxPlatform = utils.onPlatform(pathPoints[i].getPosX(), pathPoints[i].getPosY(), 25, platformYMargin);
                        //get the first point where the agent touches the ground again
                        while (auxPlatform == null && i + 1 < pathPoints.Count)
                        {
                            i++;
                            auxPlatform = utils.onPlatform(pathPoints[i].getPosX(), pathPoints[i].getPosY(), 25, platformYMargin);
                        }
                        //if it lands without catching a diamond, then these points are useless
                        if(totalCollectibles == pathPoints[i].getUncaughtDiamonds().Count)
                        {
                            pathPoints.RemoveRange(0, i);
                            removedFirstJump = true;
                        }
                    }
                    if (!removedFirstJump)
                    {
                        removeFirst = false;
                    }
                }
                //check if agent keeps on the same platform or returning to the same platform without catching a diamond - do not count with the first point
                for (int i = 0; i < pathPoints.Count; i++)
                {
                    //get the furthest state where the agent is the same platform at the same Y level and with the same number of caught diamonds
                    for(int j = pathPoints.Count - 1; j > i; j--)
                    {
                        //if the agent has caught the same number of diamonds
                        //if the agent is at the same Y level                        
                        if(pathPoints[j].getUncaughtDiamonds().Count == pathPoints[i].getUncaughtDiamonds().Count &&
                            Math.Abs(pathPoints[j].getPosY() - pathPoints[i].getPosY()) < cleanRollMargin)
                        {
                            //if the platform is the same and there is no other blocking the way nor it is a jump
                            Platform platformJ = utils.onPlatform(pathPoints[j].getPosX(), pathPoints[j].getPosY(), margin, platformYMargin);
                            Platform platformI = utils.onPlatform(pathPoints[i].getPosX(), pathPoints[i].getPosY(), margin, platformYMargin);
                            if(platformJ != null && platformI != null &&
                                platformJ.getX() == platformI.getX() && platformJ.getY() == platformI.getY() &&
                                !utils.blockingPlatform(pathPoints[i].getPosX(), pathPoints[i].getPosY(), pathPoints[j].getPosX(), pathPoints[j].getPosY(), circleRadius) &&
                                pathPoints[j].getAction() != Moves.JUMP)
                            {
                                //then remove all points until j  -never remove the first point when it is jump - this case was already treated
                                if(i == 0 && j > 1 && !removeFirst)
                                {
                                    if (pathPoints[j].getPosY() < pathPoints[i].getPosY() && pathPoints[j].getPosY() - pathPoints[i].getPosY() < cleanRollMargin)
                                    {
                                        pathPoints[j].setY(pathPoints[i].getPosY());
                                    }
                                    i = 1;
                                    pathPoints.RemoveRange(i, Math.Abs(j - i));
                                    cleanPlan(platforms, diamonds, area, circleRadius, false, removeFirst);
                                    stopFor = true;
                                    break;
                                }
                                else if(i == 0 && j <= 1)
                                {
                                    //do nothing
                                    continue;
                                }
                                else
                                {
                                    if (pathPoints[j].getPosY() < pathPoints[i].getPosY() && pathPoints[j].getPosY() - pathPoints[i].getPosY() < cleanRollMargin)
                                    {
                                        pathPoints[j].setY(pathPoints[i].getPosY());
                                    }
                                    pathPoints.RemoveRange(i, Math.Abs(j - i));
                                    cleanPlan(platforms, diamonds, area, circleRadius, false, removeFirst);
                                    stopFor = true;
                                    break;
                                }
                                
                            }
                        }
                    }
                    if (stopFor)
                    {
                        break;
                    }
                }
            }
            return debugCleanPlan();
        }

        //rectangle
        public List<DebugInformation> cleanPlanRectangle(ObstacleRepresentation[] platforms, List<DiamondInfo> diamonds, Rectangle area, float circleRadius, bool first)
        {
            bool stopFor = false;
            int margin = 50;

            if (pathPoints.Count != 0)
            {                
                //check if agent keeps on the same platform or returning to the same platform without catching a diamond - do not count with the first point
                for (int i = 0; i < pathPoints.Count; i++)
                {
                    //get the furthest state where the agent is the same platform at the same Y level and with the same number of caught diamonds
                    for (int j = pathPoints.Count - 1; j > i; j--)
                    {
                        //if the agent has caught the same number of diamonds
                        //if the agent is at the same Y level   
                        //For Simulation/Circle version
                        if(pathPoints[j].getUncaughtDiamonds() != null)
                        {
                            if (pathPoints[j].getUncaughtDiamonds().Count == pathPoints[i].getUncaughtDiamonds().Count &&
                            Math.Abs(pathPoints[j].getPosY() - pathPoints[i].getPosY()) < cleanRectangleYMargin)
                            {
                                //if the platform is the same and there is no other blocking the way nor it is a  morph
                                Platform platformJ = utils.onPlatform(pathPoints[j].getPosX(), pathPoints[j].getPosY(), margin, platformYMargin);
                                Platform platformI = utils.onPlatform(pathPoints[i].getPosX(), pathPoints[i].getPosY(), margin, platformYMargin);
                                if (platformJ != null && platformI != null &&
                                    platformJ.getX() == platformI.getX() && platformJ.getY() == platformI.getY() &&
                                    !utils.blockingPlatform(pathPoints[i].getPosX(), pathPoints[i].getPosY(), pathPoints[j].getPosX(), pathPoints[j].getPosY(), circleRadius) &&
                                    pathPoints[j].getAction() != Moves.MORPH_UP && pathPoints[j].getAction() != Moves.MORPH_DOWN)
                                {
                                    pathPoints.RemoveRange(i, Math.Abs(j - i));
                                    cleanPlanRectangle(platforms, diamonds, area, circleRadius, false);
                                    stopFor = true;
                                    break;
                                }
                            }
                        }
                        //For Game Simulator
                        else
                        {
                            if (pathPoints[j].getUncaughtColl().Count == pathPoints[i].getUncaughtColl().Count &&
                            Math.Abs(pathPoints[j].getPosY() - pathPoints[i].getPosY()) < cleanRectangleYMargin)
                            {
                                //if the platform is the same and there is no other blocking the way nor it is a  morph
                                Platform platformJ = utils.onPlatform(pathPoints[j].getPosX(), pathPoints[j].getPosY(), margin, platformYMargin);
                                Platform platformI = utils.onPlatform(pathPoints[i].getPosX(), pathPoints[i].getPosY(), margin, platformYMargin);
                                if (platformJ != null && platformI != null &&
                                    platformJ.getX() == platformI.getX() && platformJ.getY() == platformI.getY() &&
                                    !utils.blockingPlatform(pathPoints[i].getPosX(), pathPoints[i].getPosY(), pathPoints[j].getPosX(), pathPoints[j].getPosY(), circleRadius) &&
                                    pathPoints[j].getAction() != Moves.MORPH_UP && pathPoints[j].getAction() != Moves.MORPH_DOWN)
                                {
                                    pathPoints.RemoveRange(i, Math.Abs(j - i));
                                    cleanPlanRectangle(platforms, diamonds, area, circleRadius, false);
                                    stopFor = true;
                                    break;
                                }
                            }
                        }
                        
                    }
                    if (stopFor)
                    {
                        break;
                    }
                }
            }
            return debugCleanPlan();
        }

        public List<DebugInformation> debugCleanPlan()
        {
            List<DebugInformation> debugInfo = new List<DebugInformation>();

            for(int i = 1; i < pathPoints.Count; i++)
            {
                debugInfo.Add(GeometryFriends.AI.Debug.DebugInformationFactory.CreateLineDebugInfo(new PointF(pathPoints[i - 1].getPosX(), pathPoints[i - 1].getPosY()), new PointF(pathPoints[i].getPosX(), pathPoints[i].getPosY()), new GeometryFriends.XNAStub.Color(255, 0, 255)));
            }

            return debugInfo;
        }

        public void saveOriginal()
        {
            List<Point> points;
            if (cutPlan)
            {
                points = originalCleanedPathPoints;
            }
            else
            {
                points = originalPathPoints;
            }

            foreach(Point point in pathPoints)
            {
                points.Add(point);
            }
        }

        public List<Point> getOriginalPoints()
        {
            if (cutPlan)
            {
                return originalCleanedPathPoints;
            }
            else
            {
                return originalPathPoints;
            }            
        }

        public void setOriginalPoints(List<Point> points)
        {
            if (cutPlan)
            {
                originalCleanedPathPoints = points;
            }
            else
            {
                originalPathPoints = points;
            }
        }

        public PathPlan clone()
        {
            PathPlan newPlan = new PathPlan(cutPlan, totalCollectibles, utils);

            newPlan.setTotalCollectibles(this.totalCollectibles);

            foreach(Point point in this.pathPoints)
            {
                newPlan.addPointEnd(point);
            }

            return newPlan;
        }

        public PathPlan joinPlans(PathPlan plan1, PathPlan plan2)
        {
            List<Point> newPoints = new List<Point>();
            newPoints.AddRange(plan1.getPathPoints());
            newPoints.AddRange(plan2.getPathPoints());

            PathPlan newPlan = new PathPlan(cutPlan, plan1.totalCollectibles, newPoints, utils);

            return newPlan;
        }
    }
}
