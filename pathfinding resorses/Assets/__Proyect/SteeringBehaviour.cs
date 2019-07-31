using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public static class SteeringBehaviour
{
    public enum Behaviour
    {
        FOLLOW_PATH,
        COHESION,
        VELOCITY_MATCH,
        SEEK
    }

    public static Vector2 GetDesiredPosition(Behaviour behaviour, Vector2 position, AIAgent requester, float deltaTime)
    {
        
        switch (behaviour)
        {
            case Behaviour.FOLLOW_PATH:
                return FollowPath(requester);
            case Behaviour.COHESION:
                return Cohesion(requester, position, deltaTime);
            case Behaviour.VELOCITY_MATCH:
                return VelocityMatch(requester, position);
            case Behaviour.SEEK:
                return Seek(requester, position, deltaTime);
            default:
                Debug.LogError("missing behaviour");
                return position;
        }
    }
    public static Vector2? Separation(AIAgent requester, Vector2 position, Vector2 desiredDirection, out AIAgent closestSiblin, out Vector2 directionToClosestSiblin)
    {
        List<AIAgent> siblins = requester.parent.children;

        float closestSiblinSqrDistance;

        if (!GetClosestAgent(requester, position, siblins, out closestSiblin, out closestSiblinSqrDistance))
        {
            closestSiblin = null;
            directionToClosestSiblin = Vector2.zero;
            return null;
        }


        float repulsionMaxSqrRange = Mathf.Pow(requester.data.radious * requester.data.separationRangeInRadious, 2);
        directionToClosestSiblin = ((Vector2)closestSiblin.transform.position - position).normalized;

        if (repulsionMaxSqrRange < closestSiblinSqrDistance)
            return null;

        Vector2 perpendicular = Vector2Utilities.VectorPerpendicularToDesired(desiredDirection, directionToClosestSiblin);
        return position + perpendicular * requester.data.maxSpeed * Time.deltaTime;

    }
    public static Vector2? ObstacleAvoidance(AIAgent requester, Vector2 position, Vector2 desiredDirection, out AIAgent avoidanceObjective)
    {
        if (requester.parent.ClosestUnit == null)
        {
            avoidanceObjective = null;
            return null;
        }

        float objectiveSqrDistance;
        if (requester.Target is AIAgent)
        {
            if (!GetClosestAgent(requester, position, requester.parent.ClosestUnit.children,
                out avoidanceObjective, out objectiveSqrDistance, (AIAgent)requester.Target))
                return null;
        }
        else
        {
            if (!GetClosestAgent(requester, position, requester.parent.ClosestUnit.children,
                out avoidanceObjective, out objectiveSqrDistance))
                return null;
        }
        Vector2 objectiveDirection = ((Vector2)avoidanceObjective.transform.position - position).normalized;

        Vector2 perpendicular = Vector2Utilities.VectorPerpendicularToDesired(desiredDirection, objectiveDirection);
        return position + perpendicular * requester.data.maxSpeed * Time.deltaTime;

    }
    public static Vector2? ObstacleAvoidance(AIAgent requester, Vector2 position, out AIAgent avoidanceObjective)
    {
        if (requester.parent.ClosestUnit == null)
        {
            avoidanceObjective = null;
            return null;
        }

        float objectiveSqrDistance;
        if (requester.Target is AIAgent)
        {
            if (!GetClosestAgent(requester, position, requester.parent.ClosestUnit.children,
                out avoidanceObjective, out objectiveSqrDistance, (AIAgent)requester.Target))
                return null;
        }
        else
        {
            if (!GetClosestAgent(requester, position, requester.parent.ClosestUnit.children,
                out avoidanceObjective, out objectiveSqrDistance))
                return null;
        }
        Vector2 objectiveDirection = ((Vector2)avoidanceObjective.transform.position - position).normalized;

        return position - objectiveDirection * requester.data.maxSpeed * Time.deltaTime;

    }

    private static Vector2 FollowPath(AIAgent requester)
    {
        requester.AI.destination = requester.Destination;
        //modificar variables del requester acá esta mal pero ALV :v
        //si no lo hago tendria que modificar todo el sistema y no vale la pena.
        if (requester.repathTimer >= requester.timeToRepath)
        {
            requester.AI.SearchPath();
            requester.repathTimer = 0;
        }
        requester.AI.MovementUpdate(Time.deltaTime, out Vector3 nextPos, out Quaternion nextRotation);
        return nextPos;    
    }
    private static Vector2 Cohesion(AIAgent requester, Vector2 position, float deltaTime)
    {
        return Arribe(requester, position, requester.Destination, deltaTime);
    }
    public static Vector2 Arribe(AIAgent requester, Vector2 position, Vector2 destinationPoint, float deltaTime, bool decelerate = true, bool useStopRadious = true)
    {
        AIAgentData requesterData = requester.data;

        float sqrDist = Vector2Utilities.SqrDistance(destinationPoint, position);

        //in stop radious
        if (sqrDist <= Mathf.Pow(requesterData.stopRadious, 2) && useStopRadious)
        {
            return position;
        }
        //in slow down radious
        else if (sqrDist < Mathf.Pow(requesterData.slowDownStartRadious, 2) && decelerate)
        {
            Vector2 direction = (destinationPoint - position).normalized;

            float dist = Mathf.Sqrt(sqrDist);
            float speedMult = dist / requesterData.slowDownStartRadious;
            float movementMagnitude = requesterData.maxSpeed * speedMult * deltaTime;
            //prevent overshooting
            if (dist <= movementMagnitude)  return destinationPoint;
            return position + direction * movementMagnitude;
        }
        else
        {
            float dist = Mathf.Sqrt(sqrDist);
            float movementMagnitude = requesterData.maxSpeed * deltaTime;
            if (dist <= movementMagnitude)  return destinationPoint;

            Vector2 direction = (destinationPoint - position) / dist;
            return position + direction * movementMagnitude;
        }
    }
    private static Vector2 VelocityMatch(AIAgent requester, Vector2 position)
    {
        Vector2 velocityToMatch = requester.parent.movementAI.velocity;

        return (Vector2)position + velocityToMatch * Time.fixedDeltaTime;
    }
    private static Vector2 Seek(AIAgent requester, Vector2 position, float deltaTime)
    {
        return Arribe(requester, position, requester.Destination, deltaTime, false, false);
    }


    public static bool GetClosestAgent(AIAgent requester, Vector2 requesterPosition, List<AIAgent> options, out AIAgent closestSiblin, out float closestSiblinSqrDistance, AIAgent exception = null)
    {
        bool areClosestAgent = false;
        closestSiblin = null;
        closestSiblinSqrDistance = float.MaxValue;
        for (int i = 0; i < options.Count; i++)
        {
            AIAgent tempAgent = options[i];

            if (tempAgent == requester || tempAgent == exception)
                continue;

            float tempSqrDistance = Vector2Utilities.SqrDistance(requesterPosition, tempAgent.transform.position);
            if (tempSqrDistance < closestSiblinSqrDistance)
            {
                areClosestAgent = true;
                closestSiblin = tempAgent;
                closestSiblinSqrDistance = tempSqrDistance;
            }
        }
        return areClosestAgent;
    }
}
