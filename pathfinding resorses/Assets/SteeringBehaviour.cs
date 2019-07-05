using System.Collections;
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

    //quizas sea mejor quitarle el nullable vector2
    public static Vector2? GetDesiredPosition(Behaviour behaviour, Vector2 position, AIAgent requester)
    {
        switch (behaviour)
        {
            case Behaviour.FOLLOW_PATH:
                return FollowPath(requester);
            case Behaviour.SEPARATION:
                return null;
                //return Separation(requester);
            case Behaviour.COHESION:
                return Cohesion(requester, position);
            case Behaviour.VELOCITY_MATCH:
                return VelocityMatch(requester, position);
            case Behaviour.SEEK:
                return Seek(requester, position);
            //case Behaviour.AVOIDANCE:
            //    return ObstacleAvoidance(requester);
            default:
                return null;
        }
    }
    

    //ok
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

    public static Vector2? ObstacleAvoidance(AIAgent requester, Vector2 position, Vector2 desiredDirection, out AIAgent avoidanceObjective)
    {
        if (requester.parent.ClosestUnit == null)
        {
            avoidanceObjective = null;
            return null;
        }
        float objectiveSqrDistance;
        if (requester.target is AIAgent)
        {
            if (! GetClosestAgent(requester, position, requester.parent.ClosestUnit.children,
                out avoidanceObjective, out objectiveSqrDistance, (AIAgent)requester.target))
                    return null;
        }
        else
        {
            if (!GetClosestAgent(requester, position, requester.parent.ClosestUnit.children,
                out avoidanceObjective, out objectiveSqrDistance))
                    return null;
        }
        Vector2 objectiveDirection = ((Vector2)avoidanceObjective.transform.position - position).normalized;

        Vector2 perpendicular = VectorPerpendicularToDesired(desiredDirection, objectiveDirection);
        return position + perpendicular * requester.data.maxSpeed * Time.deltaTime;

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

        Vector2 perpendicular = VectorPerpendicularToDesired(desiredDirection, directionToClosestSiblin);
        return position + perpendicular * requester.data.maxSpeed* Time.deltaTime;

    }

    private static Vector2 VectorPerpendicularToDesired(Vector2 desiredNormalized, Vector2 referenceVectorNormalized)
    {
        float angle = Vector2.SignedAngle(desiredNormalized, referenceVectorNormalized);
        if (angle > 0)
        {
            return  - Vector2.Perpendicular(desiredNormalized);
        }
        else
        {
            return   Vector2.Perpendicular(desiredNormalized);
        }
    }


    //public static Vector2? Separation(AIAgent requester, out AIAgent closestSiblin)
    //{
    //    Vector2 position = requester.transform.position;
    //    List<AIAgent> siblins = requester.parent.children;

    //    float closestSiblinSqrDistance;

    //    GetClosestAgent(requester, position, siblins, out closestSiblin, out closestSiblinSqrDistance);

    //    float repulsionMaxSqrRange = Mathf.Pow(requester.data.radious * requester.data.separationRangeInRadious, 2);
    //    if (repulsionMaxSqrRange < closestSiblinSqrDistance)
    //        return null;

    //    Vector2 directionToClosestSiblin = ((Vector2)closestSiblin.transform.position - position).normalized;

    //    Vector2 desiredPos = position + -directionToClosestSiblin * requester.data.maxSpeed * Time.deltaTime;
    //    return desiredPos;
    //}
    //public static Vector2? Separation(AIAgent requester, AIAgent closestSiblin)
    //{
    //    Vector2 position = requester.transform.position;
    //    Vector2 closestSiblinPos = closestSiblin.transform.position;
    //    List<AIAgent> siblins = requester.parent.children;

    //    float closestSiblinSqrDistance = (position - closestSiblinPos).sqrMagnitude;
    //    float repulsionMaxSqrRange = Mathf.Pow(requester.data.radious * requester.data.separationRangeInRadious, 2);
    //    if (repulsionMaxSqrRange < closestSiblinSqrDistance)
    //        return null;

    //    Vector2 directionToClosestSiblin = ((Vector2)closestSiblin.transform.position - position).normalized;

    //    Vector2 desiredPos = position + -directionToClosestSiblin * requester.data.maxSpeed * Time.deltaTime;
    //    return desiredPos;
    //}
    ////ok
    private static Vector2 Cohesion(AIAgent requester, Vector2 position)
    {
        Vector2 cohesionPos = requester.parent.GetCohesionPosition(requester);
        return Arribe(requester, position, cohesionPos);
    }
    //ok
    private static Vector2 Arribe(AIAgent requester, Vector2 position, Vector2 destinationPoint)
    {
        AIAgentData requesterData = requester.data;

        float sqrDist = Vector2Utilities.SqrDistance(destinationPoint, position);

        //in stop radious
        if (sqrDist <= Mathf.Pow(requesterData.stopRadious, 2))
        {
            return position;
        }
        //in slow down radious
        else if (sqrDist < Mathf.Pow(requesterData.slowDownStartRadious, 2))
        {
            Vector2 direction = (destinationPoint - position).normalized;

            float dist = Mathf.Sqrt(sqrDist);
            float speedMult = dist / requesterData.slowDownStartRadious;

            float movementMagnitude = requesterData.maxSpeed * speedMult * Time.deltaTime;
            //prevent overshooting
            if (dist <= movementMagnitude)
                return destinationPoint;
            return position + direction * movementMagnitude;
        }
        else
        {
            Vector2 direction = (destinationPoint - position).normalized;
            return position + direction * requesterData.maxSpeed * Time.deltaTime;
        }
    }
    //ok pero podria usarse una interfaz para generalizar el lugar de donde se consigue la velocidad a imitar.
    private static Vector2 VelocityMatch(AIAgent requester, Vector2 position)
    {
        Vector2 velocityToMatch = requester.parent.movementAI.velocity;

        return (Vector2)position + velocityToMatch * Time.fixedDeltaTime;
    }
    //ok
    private static Vector2 Seek(AIAgent requester, Vector2 position)
    {
        AIAgentData requesterData = requester.data;
        IEntity target = requester.target;

        Vector2 targetPos = target.GameObject.transform.position;
        Vector2 directionTowardsPosition = (position - targetPos).normalized;

        float requesterRadiousOffset = requesterData.shape == Shape.CIRCULAR ? requesterData.radious :
            Vector2Utilities.GetDistanceOfSquareEdgeAndCenterFromDirection(requesterData.radious, directionTowardsPosition);

        float targetRadiousOffset = target.BodyShape == Shape.CIRCULAR ? target.Radious :
            Vector2Utilities.GetDistanceOfSquareEdgeAndCenterFromDirection(target.Radious, directionTowardsPosition);

        Vector2 destinationPoint = targetPos + directionTowardsPosition * (requesterRadiousOffset + targetRadiousOffset);

        return Arribe(requester, position, destinationPoint);
    }
    //private static Vector2 ObstacleAvoidance(AIAgent requester)
    //{

    //}



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
