using System.Collections.Generic;
using UnityEngine;

namespace Platformer
{
    public class GraphNode<T>
    {
        private T value;
        private List<GraphNode<T>> neighbors;
        public GraphNode(T value)
        {
            this.value = value;
            neighbors = new List<GraphNode<T>>();
        }
        public T Value => value;
        public IList<GraphNode<T>> Neighbors => neighbors.AsReadOnly();

        public bool AddNeighbor(GraphNode<T> neighbor)
        {
            if (neighbors.Contains(neighbor)) return false;
            else
            {
                neighbors.Add(neighbor);
                return true;
            }
        }

        public bool RemoveNeighbor(GraphNode<T> neighbor)
        {
            return neighbors.Remove(neighbor);
        }
        
        
    }
    public class GraphBase<T>
    {
        List<GraphNode<T>> nodes = new ();

        public GraphBase() { }
        
        public int Count => nodes.Count;
        public IList<GraphNode<T>> Nodes => nodes.AsReadOnly();

        public bool AddNode(T value)
        {
            if (FindNode(value) != null)
            {
                //duplicate node
                return false;
            }
            else
            {
                nodes.Add(new GraphNode<T>(value));
                return true;
            }
        }

        public bool AddEdge(T from, T to)
        {
            GraphNode<T> fromNode = FindNode(from);
            GraphNode<T> toNode = FindNode(to);
            if (fromNode == null || toNode == null) return false;   //Nodes not found
            else if (fromNode.Neighbors.Contains(toNode)) return false; //Nodes already have edge connections
            else
            {
                fromNode.AddNeighbor(toNode);
                toNode.AddNeighbor(fromNode);
                return true;
            }
        }

        public bool RemoveNode(T value)
        {
            GraphNode<T> removeNode = FindNode(value);
            if (removeNode == null) return false;
            else
            {
                nodes.Remove(removeNode);
                foreach (GraphNode<T> node in nodes)
                {
                    node.RemoveNeighbor(removeNode);
                }
                return true;
            }
        }

        public bool RemoveEdge(T from, T to)
        {
            GraphNode<T> removeFrom = FindNode(from);
            GraphNode<T> removeTo = FindNode(to);
            if (removeFrom == null || removeTo == null) return false;
            else if (!removeFrom.Neighbors.Contains(removeTo)) return false;    //there is no edge connect between these nodes
            else
            {
                removeFrom.RemoveNeighbor(removeTo);
                removeTo.RemoveNeighbor(removeFrom);
                return true;
            }
        }
        
        public GraphNode<T> FindNode(T value)
        {
            foreach (GraphNode<T> node in nodes)
            {
                if (node.Value.Equals(value))
                    return node;
            }
            return null;
        }
    }
}
