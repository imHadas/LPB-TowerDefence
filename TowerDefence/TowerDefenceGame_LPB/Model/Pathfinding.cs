using System;
using System.Collections.Generic;
using TowerDefenceBackend.Persistence;

namespace TowerDefenceBackend.Model
{
    public abstract class IPathfinder
    {
        /// <summary>
        /// Finds the shortest unobstructed path between <c>from</c> and <c>to</c>.
        /// Fields specified in <c>except</c> will also be excluded from the search.
        /// </summary>
        /// <param name="from">Coordinate of start (inclusive)</param>
        /// <param name="to">Coordinate of destination (inclusive)</param>
        /// <param name="except">Fields to exclude from search</param>
        /// <returns>List of coordinates in order from <c>from</c> to <c>to</c>. Empty if path does not exist</returns>
        public abstract IList<(uint, uint)> FindPath((uint, uint) from, (uint, uint) to, ICollection<Field>? except = null);
        /// <summary>
        /// Method to change the inner state of the pathfinder without constructing a new one.
        /// </summary>
        /// <param name="changedField">The <c>Field</c> which's state has changed</param>
        public abstract void ChangeState(Field changedField);
    }

    /// <summary>
    /// AStar implementaion of pathfinding.
    /// </summary>
    public class AStar : IPathfinder
    {
        /// <summary>
        /// All <c>Fields</c> stored as <c>Nodes</c>
        /// </summary>
        private HashSet<Node> allNodes;


        /// <summary>
        /// Construct pathfinder
        /// </summary>
        /// <param name="table">The <c>Table</c> on which pathfinding will be done</param>
        public AStar(Table table)
        {
            allNodes = new();
            foreach (Field field in table)
            {
                allNodes.Add(new Node(field));
            }
        }

        /// <summary>
        /// Implemention of changing a <c>Field</c>'s state
        /// </summary>
        /// <param name="changedField"><c>Field</c> to change</param>
        public override void ChangeState(Field changedField)
        {
            Node node = new(changedField);
            allNodes.Remove(node);  // equality only checks the coordinate
            allNodes.Add(node);
        }

        /// <summary>
        /// Implementation of pathfinding method with an AStar algorithm
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="except"></param>
        /// <returns>The shortest path according to the AStar algorithm, using Manhattan distances</returns>
        public override IList<(uint, uint)> FindPath((uint, uint) from, (uint, uint) to, ICollection<Field>? except = null)
        {
            HashSet<Node> exceptNodes = new();
            if(except is not null) foreach (var field in except)
            {
                exceptNodes.Add(new Node(field));
            }

            HashSet<Node> closedSet = new();
            List<WeightedNode> openSet = new();
            LinkedList<Node> path = new();

            Node end = getNodeByCoord(to.Item1, to.Item2);
            WeightedNode start;
            {
                Node temp = getNodeByCoord(from.Item1, from.Item2);
                start = new(temp, 0, Heuristic(temp, end));
            }

            openSet.Add(start);
            WeightedNodeComparer comp = new WeightedNodeComparer();
            while (openSet.Count != 0)
            {
                openSet.Sort(comp);

                WeightedNode current = openSet[0];
                openSet.Remove(current);
                closedSet.Add(current);
                if (current.Equals(end))
                {
                    while (!(current.Equals(start) || current is null))
                    {
                        path.AddFirst(current);
                        current = (WeightedNode) current.Parent;
                    }
                    path.AddFirst(start);
                    return NodesToCoords(path);
                }

                foreach (var node in Neighbours(current))
                {
                    if (!node.Equals(end) && !exceptNodes.Contains(node) && (!node.Walkable || closedSet.Contains(node)))
                        continue;
                    WeightedNode wnode = new(node, current.Distance + 1, Heuristic(node, end));
                    wnode.Parent = current;
                    if (!openSet.Contains(wnode))
                    {
                        openSet.Add(wnode);
                    }
                    else
                    {
                        if (openSet[openSet.IndexOf(wnode)].Distance > current.Distance + 1)
                        {
                            openSet.Remove(wnode);
                            openSet.Add(wnode);
                        }
                    }

                }
            }

            return new List<(uint, uint)>();
        }

        /// <summary>
        /// Extracts the coordinates from a collection of <c>Node</c>s
        /// </summary>
        /// <param name="nodes">The <c>Collection</c> of <c>Nodes</c> from which the coordinates are extracted</param>
        /// <returns>A <c>List</c> of coordinates (as uint tuples)</returns>
        private List<(uint, uint)> NodesToCoords(ICollection<Node> nodes)
        {
            List<(uint, uint)> result = new List<(uint, uint)>();
            foreach (Node node in nodes)
            {
                result.Add(node.Coords);
            }
            return result;
        }

        /// <summary>
        /// Calculates the heuristic value for the <c>from</c> <c>Node</c> with the destination of <c>to</c> with Manhattan distance.
        /// </summary>
        /// <param name="from">Starting <c>Node</c></param>
        /// <param name="to">Destination <c>Node</c></param>
        /// <returns>The heuristic value (as a uint)</returns>
        private uint Heuristic(Node from, Node to)
        {
            return (uint)(Math.Abs((int)from.Coords.x - (int)to.Coords.x) + Math.Abs((int)from.Coords.y - (int)to.Coords.y)); //ki hitte volna, hogy a nummod hasznos lesz? (delta x + delta y) aka Manhattan norma
        }

        /// <summary>
        /// Finds all the neighbours of a given <c>Node</c> (if they exist)
        /// </summary>
        /// <param name="node">The <c>Node</c> in the center</param>
        /// <returns><c>Collection</c> of neighbouring <c>Node</c>s</returns>
        private ICollection<Node> Neighbours(Node node)
        {
            HashSet<Node> result = new();
            Node? current;
            if ((current = getNodeByCoord(node.Coords.x - 1, node.Coords.y)) is not null)
            {
                result.Add((Node)current.Clone());
            }
            if((current = getNodeByCoord(node.Coords.x, node.Coords.y - 1)) is not null)
            {
                result.Add((Node)current.Clone());
            }
            if ((current = getNodeByCoord(node.Coords.x + 1, node.Coords.y)) is not null)
            {
                result.Add((Node)current.Clone());
            }
            if ((current = getNodeByCoord(node.Coords.x, node.Coords.y + 1)) is not null)
            {
                result.Add((Node)current.Clone());
            }
            return result;
        }

        /// <summary>
        /// Returns the <c>Node</c> from <c>allNodes</c> which's coordinates match.
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        /// <returns><c>Node</c> with matching coordinates. Null if there is no such.</returns>
        private Node? getNodeByCoord(uint x, uint y)
        {
            Node? result = null;
            allNodes.TryGetValue(new(new(x, y)), out result);
            return result;
        }

        
    }

    /// <summary>
    /// <c>Comparer</c> for <c>WeightedNodes</c>
    /// </summary>
    class WeightedNodeComparer : IComparer<WeightedNode>
    {
        /// <summary>
        /// Compares <c>x</c> to <c>y</c>
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>The value difference (distance difference) of the two inputs</returns>
        public int Compare(WeightedNode? x, WeightedNode? y)
        {
            if (x == null) return int.MinValue;
            else if (y == null) return int.MaxValue;
            return (int)x.Value - (int)y.Value;
        }
    }

    /// <summary>
    /// Pathfinder processable encapsulation of a <c>Field</c>
    /// </summary>
    class Node : IEquatable<Node>, ICloneable, IComparable<Node>
    {
        public Node? Parent { get; set; }
        /// <summary>
        /// Encapsulated <c>Field</c>
        /// </summary>
        public Field Field { get; private set; }
        public (uint x, uint y) Coords => Field.Coords;
        public bool Walkable => Field.Placement is null;

        public Node(Field field)
        {
            Field = field;
            Parent = null;
        }

        public bool Equals(Node? other)
        {
            return this.Coords == other?.Coords;
        }

        public object Clone()
        {
            return new Node(Field);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as Node);
        }

        public override int GetHashCode()
        {
            return Coords.GetHashCode();
        }

        public int CompareTo(Node? other)
        {
            if(other == null) return int.MaxValue;
            return Math.Abs((int)this.Coords.x - (int)other.Coords.x) + Math.Abs((int)this.Coords.y - (int)other.Coords.y);
        }
    }

    /// <summary>
    /// Processed <c>Node</c>
    /// </summary>
    class WeightedNode : Node
    {
        /// <summary>
        /// Number of steps taken from the start
        /// </summary>
        public uint Distance { get; private set; }
        /// <summary>
        /// Manhattan distance from the end
        /// </summary>
        public uint Heuristic { get; private set; }
        /// <summary>
        /// Sum of <c>Distance</c> and <c>Heuristic</c>
        /// </summary>
        public uint Value => Distance + Heuristic;

        public WeightedNode(Field field, uint distance, uint heuristic) : base(field)
        {
            Distance = distance;
            Heuristic = heuristic;
        }

        public WeightedNode(Node node, uint distance, uint heuristic) : this(node.Field, distance, heuristic) { }
    }
}
