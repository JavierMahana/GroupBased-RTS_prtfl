﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public static class SteeringBehaviour
{
    public enum Behaviour
    {
        FOLLOW_PATH,
        SEPARATION,
        COHESION,
        VELOCITY_MATCH,
        SEEK,
        AVOIDANCE
    }

    public static Vector2? GetDesiredPosition(Behaviour behaviour, AIAgent requester)
    {
        switch (behaviour)
        {
            case Behaviour.FOLLOW_PATH:
                return FollowPath(requester);
            case Behaviour.SEPARATION:
                return Separation(requester);
            case Behaviour.COHESION:
                return Cohesion(requester);
            case Behaviour.VELOCITY_MATCH:
                return VelocityMatch(requester);
            case Behaviour.SEEK:
                return Seek(requester);
            case Behaviour.AVOIDANCE:
                return ObstacleAvoidance(requester);
            default:
                return Vector2.zero;
        }
    }

    private static Vector2 FollowPath(AIAgent requester)
    {
        Vector2 velocity = requester.parent.movementAI.velocity;

    }
    private static Vector2? Separation(AIAgent requester)
    {
        Vector2 position = requester.transform.position;
        List<AIAgent> siblins = requester.parent.childs;

        AIAgent closestSiblin = null; 
        float closestSiblinSqrDistance = float.MaxValue;
        for (int i = 0; i < siblins.Count; i++)
        {
            AIAgent tempAgent = siblins[i];

            if (tempAgent == requester)
                continue;

            float tempSqrDistance = Vector2Utilities.SqrDistance(position, tempAgent.transform.position);
            if (tempSqrDistance < closestSiblinSqrDistance)
            {
                closestSiblin = tempAgent;
                closestSiblinSqrDistance = tempSqrDistance;
            }
        }

        float repulsionMaxSqrRange = Mathf.Pow(requester.data.radious * requester.data.separationRangeInRadious, 2);
        if (repulsionMaxSqrRange < closestSiblinSqrDistance)
            return null;

        float speedMult = 1 - (closestSiblinSqrDistance / repulsionMaxSqrRange);
        Vector2 direction = ((Vector2)closestSiblin.transform.position - position).normalized;
        Vector2 desiredPos = position + direction * speedMult * requester.data.maxSpeed * Time.deltaTime;

        return desiredPos;
    }
        
    private static Vector2 Cohesion(AIAgent requester)
    {
        AIAgentData requesterData = requester.data;

        Vector2 position = requester.transform.position;
        Vector2 cohesionPos = requester.parent.GetCohesionPosition(requester);

        
        float sqrDist = Vector2Utilities.SqrDistance(cohesionPos, position);

        //in stop radious
        if (sqrDist <= Mathf.Pow(requesterData.stopRadious, 2))
        {
            return position;
        }
        //in slow down radious
        else if (sqrDist < Mathf.Pow(requesterData.slowDownStartRadious, 2))
        {
            Vector2 direction = (cohesionPos - position).normalized;

            float dist = Mathf.Sqrt(sqrDist);
            float speedMult = dist / requesterData.slowDownStartRadious;

            float movementMagnitude = requesterData.maxSpeed * speedMult * Time.deltaTime;
            //prevent overshooting
            if (dist <= movementMagnitude)
                return cohesionPos;
            return position + direction * movementMagnitude;
        }
        else
        {
            Vector2 direction = (cohesionPos - position).normalized;
            return position + direction * requesterData.maxSpeed * Time.deltaTime;
        }
    }
    private static Vector2 VelocityMatch(AIAgent requester)
    {
        Vector2 velocityToMatch = requester.parent.movementAI.velocity;

        return (Vector2)requester.transform.position + velocityToMatch * Time.deltaTime;
    }
    private static Vector2 Seek(AIAgent requester)
    {

    }
    private static Vector2 ObstacleAvoidance(AIAgent requester)
    {

    }

    private static Vector2 Arribe(AIAgent requester)
    {
    }
}
