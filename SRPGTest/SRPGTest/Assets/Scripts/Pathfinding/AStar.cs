using System.Collections;
using System.Collections.Generic;
using System;

public static class AStar
{
    /// <summary>
    /// A generic AStar Implementation that takes a heuristic and an adjacency function and returns a path if one is found.
    /// The adj function should return a list of all nodes adjacent to the given node
    /// The cost function should return the cost to traverse from the first argument to the second (assuming they are adjacent)
    /// Heur should reuturn an estimation of the distance between two nodes. heur is admissable if it is always <= actual cost
    /// </summary>
    /// <returns> A list representing a path from start to goal if one is found. If not path is found, returns null </returns>
    public static List<T> Pathfind<T>(T start, T goal, Func<T,List<T>> adj, Func<T, T, float> cost, Func<T,T,float> heur) where T : IEquatable<T>
    {
        var dist = new Dictionary<T, float>();
        var visited = new HashSet<T>();
        var backPointers = new Dictionary<T, T>();
        PriorityQueue<T> q = new PriorityQueue<T>((int)Math.Floor(heur(start,goal)));

        dist.Add(start, 0);
        q.Enqueue(start, heur(start,goal));

        while(!q.Empty)
        {
            T node = q.Dequeue();
            // Ignore nodes we've already visited
            // Potential optimization: make priority q where DecreaseKey is not O(n) due to needing to find the item
            while(visited.Contains(node))
            {
                if (q.Empty)
                    return null;
                node = q.Dequeue();
            }
            // If we've found the goal, return the path
            if (node.Equals(goal))
            {
                List<T> path = new List<T>();
                T curr = node;
                path.Add(curr);
                while (backPointers.ContainsKey(curr))
                {
                    curr = backPointers[curr];
                    path.Add(curr);
                }
                path.Reverse();
                return path;
            }
            // Get current distance and mark current node as visited
            float currDist = dist[node];
            visited.Add(node);
            // Get adjacent nodes
            var adjacentNodes = adj(node);
            foreach(var adjNode in adjacentNodes)
            {
                // Ignore the node if it has alread been visited (A* assures nodes will be visited at the shortest path when admissable)
                if (visited.Contains(adjNode))
                    continue;
                float newDist = currDist + cost(node, adjNode);
                if(!dist.ContainsKey(adjNode))
                {
                    dist.Add(adjNode, newDist);
                    q.Enqueue(adjNode, newDist + heur(adjNode, goal));
                    backPointers.AddOrReplace(adjNode, node);
                }
                else if(dist[adjNode] > newDist)
                {
                    dist[adjNode] = newDist;
                    q.Enqueue(adjNode, newDist + heur(adjNode, goal));
                    backPointers.AddOrReplace(adjNode, node);
                }
            }
        }
        return null;
    }
}
