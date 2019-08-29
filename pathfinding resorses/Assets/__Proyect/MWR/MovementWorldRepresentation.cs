using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class MovementWorldRepresentation
{
    public int width, heigth;
    public bool[,] worldRepresentation; 
    public Vector2 origin;
    public float cellSize;

    public MovementWorldRepresentation(GridGraph graph)
    {
        Debug.Assert(graph != null, "the graph must be initialized prior to start!");
        width = graph.width;
        heigth = graph.depth;
        cellSize = graph.nodeSize;
        origin = new Vector2(graph.center.x - (width / 2 * cellSize), graph.center.y - (heigth / 2 * cellSize));

        //true if walkable/empty
        worldRepresentation = new bool[width, heigth];

        for (int y = 0; y < heigth; y++)
        {
            for (int x = 0; x < width; x++)
            {
                worldRepresentation[x,y] = graph.nodes[y * width + x].Walkable;
            }
        }
    }    

    public bool MoveToCell(IMWRUser user, Vector2Int endCell)
    {
        if (worldRepresentation[endCell.x, endCell.y])
        {
            worldRepresentation[user.CurrentCoordintates.x, user.CurrentCoordintates.y] = true;
            worldRepresentation[endCell.x, endCell.y] = false;

            user.CurrentCoordintates = endCell;
            return true;
        }
        else
        {
            return false;
        }
        
    }

    public bool TryAssignUser(IMWRUser user)
    {
        user.MWR = this;
        //revisa si es que la coordenada a la que se quiere asignar esta ocupada.
        // si es que lo esta busca la coordenada vacia más cercana. ----acá hay una pequeña posibiladad de que no haya ninguna casilla libre---ahí seria retornar false
        //si no esta ocupada, la ocupa y asigna el valor al user.
        Vector2Int bestCell = GetCell(user.GameObject.transform.position);
        if (worldRepresentation[bestCell.x, bestCell.y])
        {
            user.CurrentCoordintates = bestCell;
            worldRepresentation[bestCell.x, bestCell.y] = false;
            
            return true;
        }
        else
        {
            return false;
        }
        
    }

    public Vector2Int GetCell(Vector2 pos)
    {       
        int xIndex = GetAxisIndex(pos.x, origin.x, width);
        int yIndex = GetAxisIndex(pos.y, origin.y, heigth);

        return new Vector2Int(xIndex, yIndex);        
    }
    private int GetAxisIndex(float pos, float originPos, int lenght)
    {
        int returnIndex = int.MaxValue;

        float cellRadious = cellSize * 0.5f;
        for (int i = 0; i < lenght; i++)
        {
            float tempX = originPos + cellRadious + (cellSize * i);
            float dif = Mathf.Abs(tempX - pos);
            if (dif <= cellRadious)
            {
                returnIndex = i;
                break;
            }
        }
        //check if out of bounds
        if (returnIndex == int.MaxValue)
        {
            float minRefPos = originPos + cellRadious;
            if (pos < minRefPos) returnIndex = 0;

            float maxRefPos = originPos + cellRadious + cellSize * (lenght - 1);
            if (pos > maxRefPos) returnIndex = lenght - 1;
        }

        Debug.Assert(returnIndex != int.MaxValue, "this function have an error!");
        return returnIndex;
    }
    public Vector2 GetCellPosition(Vector2Int coords)
    {
        float cellRadious = cellSize * 0.5f;

        float xPos = coords.x * cellSize + origin.x + cellRadious;
        float yPos = coords.y * cellSize + origin.y + cellRadious;

        return new Vector2(xPos, yPos);
    }
}
