using System;
using System.Collections.Generic;
using TowerDefenceGame_LPB.Persistence;

namespace TowerDefenceGame_LPB.Model
{
    internal abstract class IPathfinder
    {
        public abstract IList<(uint, uint)> FindPath((uint, uint) from, (uint, uint) to);
    }

    internal class AStar : IPathfinder
    {
        private HashSet<Node> allNodes;

        public AStar(Table table)
        {
            allNodes = new();
            foreach (Field field in table)
            {
                allNodes.Add(new Node(field));
            }
        }

        public override IList<(uint, uint)> FindPath((uint, uint) from, (uint, uint) to)
        {
            HashSet<Node> closedSet = new();
            SortedList<WeightedNode, uint> openSet = new();
            LinkedList<Node> path = new();

            Node end = getNodeByCoord(to.Item1, to.Item2);
            WeightedNode start;
            {
                Node temp = getNodeByCoord(from.Item1, from.Item2);
                start = new(temp, 0, Heuristic(temp, end));
            }

            openSet.Add(start, start.Value);
            while(openSet.Count != 0)
            {
                WeightedNode current = openSet.Keys[0];
                openSet.Remove(current);
                closedSet.Add(current);
                if(current.Equals(end))
                {
                    while (!(current.Equals(start) || current is null))
                    {
                        path.AddFirst(current);
                        current = current.Parent as WeightedNode ?? throw new Exception("Parent of node was not weighted");
                    }
                    return NodesToCoords(path);
                }

                foreach (var node in Neighbours(current))
                {
                    if (!node.Walkable || closedSet.Contains(node))
                        continue;
                    WeightedNode wnode = new(node, current.Distance, Heuristic(node, end));
                    wnode.Parent = current;
                    if(!openSet.ContainsKey(wnode))
                    {
                        openSet.Add(wnode, wnode.Value);
                    }
                    else if(openSet[wnode] > current.Distance + 1 + Heuristic(current, end))
                    {
                        openSet.Remove(wnode);
                        openSet.Add(wnode, wnode.Value);
                    }

                }
            }

            return new List<(uint, uint)>();
        }

        private List<(uint, uint)> NodesToCoords(ICollection<Node> nodes)
        {
            List<(uint, uint)> result = new List<(uint, uint)>();
            foreach (Node node in nodes)
            {
                result.Add(node.Coords);
            }
            return result;
        }

        private uint Heuristic(Node from, Node to)
        {
            return (uint)(Math.Abs(from.Coords.x - to.Coords.x) + Math.Abs(from.Coords.y - to.Coords.y)); //ki hitte volna, hogy a nummod hasznos lesz? (delta x + delta y) aka Manhattan norma
        }

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

        private Node? getNodeByCoord(uint x, uint y)
        {
            Node? result = null;
            allNodes.TryGetValue(new(new(x, y)), out result);
            return result;
        }
    }

    class Node : IEquatable<Node>, ICloneable
    {
        public Node? Parent { get; set; }
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
    }

    class WeightedNode : Node
    {
        public uint Distance { get; private set; } //distance from start
        public uint Heuristic { get; private set; } //distance from end
        public uint Value => Distance + Heuristic;

        public WeightedNode(Field field, uint distance, uint heuristic) : base(field)
        {
            Distance = distance;
            Heuristic = heuristic;
        }

        public WeightedNode(Node node, uint distance, uint heuristic) : this(node.Field, distance, heuristic) { }
    }
}
