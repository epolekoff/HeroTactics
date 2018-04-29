using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Priority_Queue;

public static class Pathfinder
{
    // Do we want to show path debugging info?
    private static bool PathDebuggingActive = true;


    /// <summary>
    /// Get a path between two nodes.
    /// </summary>
    /// <returns></returns>
    public static List<MapTile> GetPath(MapTile start, MapTile goal)
    {
        // The set of nodes already evaluated
        List<MapTile> closedSet = new List<MapTile>();

        // The set of currently discovered nodes that are not evaluated yet.
        // Initially, only the start node is known.
        var openSet = new SimplePriorityQueue<MapTile>();
        openSet.Enqueue(start, 0);

        // For each node, which node it can most efficiently be reached from.
        // If a node can be reached from many nodes, cameFrom will eventually contain the
        // most efficient previous step.
        var cameFrom = new Dictionary<MapTile, MapTile>();

        // For each node, the cost of getting from the start node to that node.
        var gScore = new Dictionary<MapTile, float>();

        // The cost of going from start to start is zero.
        gScore[start] = 0;

        // For each node, the total cost of getting from the start node to the goal
        // by passing by that node. That value is partly known, partly heuristic.
        var fScore = new Dictionary<MapTile, float>();

        // For the first node, that value is completely heuristic.
        fScore[start] = Heuristic(start, goal);


        while(openSet.Count != 0)
        {
            // Current Node is the node in the openSet with the lowest fScore.
            MapTile current = openSet.Dequeue();
            
            // If we reached our goal, we can return the path.
            if(current == goal)
            {
                return ReconstructPathUsingCameFromMap(start, goal, cameFrom);
            }

            //openSet.Remove(current);
            closedSet.Add(current);

            List<MapTile> neighborsOfCurrent = GameManager.Instance.Map.GetValidNeighbors(current.Position);
            foreach (var neighbor in neighborsOfCurrent)
            {
                // Ignore the neighbor which is already evaluated.
                if (closedSet.Contains(neighbor))
                {
                    continue;
                }

                // The distance from start to a neighbor
                // the "dist_between" function may vary as per the solution requirements.
                float tentative_gScore = gScore[current] + RealCostToMoveBetweenTiles(current, neighbor);

                // Calculate a tentative fScore so we can enqueue this neighbor in the open set and sort it.
                float tentative_fScore = tentative_gScore + Heuristic(neighbor, goal);

                // Discover a new node.
                // Enqueue it using its fscore, so it's sorted appropriately.  	
                if (!openSet.Contains(neighbor))
                {
                    openSet.Enqueue(neighbor, tentative_fScore);
                }

                // This is not a better path.
                if (gScore.ContainsKey(neighbor) && tentative_gScore >= gScore[neighbor])
                {
                    continue;
                }

                // This path is the best until now. Record it!
                cameFrom[neighbor] = current;
                gScore[neighbor] = tentative_gScore;
                fScore[neighbor] = tentative_fScore;
            }
        }

        Debug.LogError(string.Format("Unable to find path from {0} to {1}", start.name, goal.name));
        return new List<MapTile>();
    }

    /// <summary>
    /// After generating the CameFrom map, we can construct a path from Start to End.
    /// </summary>
    /// <returns></returns>
    private static List<MapTile> ReconstructPathUsingCameFromMap(MapTile start, MapTile goal, Dictionary<MapTile, MapTile> cameFrom)
    {
        List<MapTile> path = new List<MapTile>();
        MapTile currentNodeInPath = goal;
        while (!currentNodeInPath.Equals(start))
        {
            if (!cameFrom.ContainsKey(currentNodeInPath))
            {
                Debug.LogError("Node In Pathfinding Map Does Not Have Parent.");
                return new List<MapTile>();
            }
            path.Add(currentNodeInPath);
            currentNodeInPath = cameFrom[currentNodeInPath];
        }

        // Reverse the path, since it's from Goal to Start, and return it.
        path.Reverse();

        // Draw debug lines so we can see the graph/path.
        DebugPathfinding(path, cameFrom);
        return path;
    }

    /// <summary>
    /// Currently, it's the same cost to move between each tile.
    /// </summary>
    private static float RealCostToMoveBetweenTiles(MapTile from, MapTile to)
    {
        return 1f;
    }

    /// <summary>
    /// Simple Heuristic for finding a short path.
    /// </summary>
    /// <returns></returns>
    private static float Heuristic(MapTile current, MapTile goal)
    {
        return Vector3.Distance(current.Position, goal.Position);
    }

    /// <summary>
    /// Draw a debug line along the path and all of the parent tiles.
    /// </summary>
    private static void DebugPathfinding(List<MapTile> path, Dictionary<MapTile, MapTile> cameFrom)
    {
        if(!PathDebuggingActive)
        {
            return;
        }

        float offsetY = 0;// MapTile.Height;

        // Draw connections for the Came From map.
        foreach (var kvp in cameFrom)
        {
            if (kvp.Key == null || kvp.Value == null)
            {
                continue;
            }
            Debug.DrawLine(
                kvp.Key.transform.position + Vector3.up * offsetY, 
                kvp.Value.transform.position + Vector3.up * offsetY,
                Color.yellow,
                1f);
        }

        // Early out if there's nothing to draw.
        if (path.Count == 0)
        {
            return;
        }

        // Draw connections for the path.
        for (int i = 0; i < path.Count - 1; i++)
        {
            Debug.DrawLine(
                path[i].transform.position + Vector3.up * offsetY, 
                path[i+1].transform.position + Vector3.up * offsetY, 
                Color.red,
                5f);
        }
    }
}
