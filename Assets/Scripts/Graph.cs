using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets
{
    public record Node
    {
        public int Id { get; set; }
    }

    public record Edge
    {
        public int First { get; set; }

        public int Second { get; set; }

        public int Weight { get; set; }
    }

    public record Graph
    {
        public IReadOnlyCollection<Node> Nodes { get; set; }

        public IReadOnlyCollection<Edge> Edges { get; set; }
    }

    public static class GridGraphExtensions
    {
        public static Graph Create(int size)
        {
            var random = new Random();

            var nodes = new List<Node>();
            var edges = new List<Edge>();

            foreach (var i in Enumerable.Range(0, size))
            {
                foreach (var j in Enumerable.Range(0, size))
                {
                    var thisIndex = i * size + j;

                    nodes.Add(new Node { Id = thisIndex });

                    if (i + 1 < size)
                    {
                        edges.Add(new Edge { First = thisIndex, Second = thisIndex + size, Weight = random.Next() });
                    }

                    if (j + 1 < size)
                    {
                        edges.Add(new Edge{ First = thisIndex, Second = thisIndex + 1, Weight = random.Next() });
                    }
                }
            }

            return new Graph 
            { 
                Edges = edges, 
                Nodes = nodes 
            };
        }

        public static string ToString(int size, IReadOnlyCollection<Edge> edges)
        {
            int Index(int i, int j) => i * size + j;

            bool HasLeft(int i, int j)
            {
                var first = Index(i, j - 1);
                var second = Index(i, j);
                return edges.Any(e => e.First == first && e.Second == second);
            }

            bool HasRight(int i, int j)
            {
                var first = Index(i, j);
                var second = Index(i, j + 1);
                return edges.Any(e => e.First == first && e.Second == second);
            }

            bool HasTop(int i, int j)
            {
                var first = Index(i - 1, j);
                var second = Index(i, j);
                return edges.Any(e => e.First == first && e.Second == second);
            }

            bool HasBottom(int i, int j)
            {
                var first = Index(i, j);
                var second = Index(i + 1, j);
                return edges.Any(e => e.First == first && e.Second == second);
            }

            var sb = new StringBuilder();

            foreach (var i in Enumerable.Range(0, size))
            {
                foreach (var j in Enumerable.Range(0, size))
                {
                    var hasLeft = HasLeft(i, j);
                    var hasRight = HasRight(i, j);
                    var hasTop = HasTop(i, j);
                    var hasBottom = HasBottom(i, j);

                    if (hasLeft && hasRight && hasTop && hasBottom)
                    {
                        sb.Append('┼');
                    }
                    else if (hasLeft && hasRight && hasTop)
                    {
                        sb.Append('┴');
                    }
                    else if (hasLeft && hasRight && hasBottom)
                    {
                        sb.Append('┬');
                    }
                    else if (hasLeft && hasTop && hasBottom)
                    {
                        sb.Append('┤');
                    }
                    else if (hasRight && hasTop && hasBottom)
                    {
                        sb.Append('├');
                    }
                    else if (hasLeft && hasTop)
                    {
                        sb.Append('┘');
                    }
                    else if (hasRight && hasTop)
                    {
                        sb.Append('└');
                    }
                    else if (hasLeft && hasBottom)
                    {
                        sb.Append('┐');
                    }
                    else if (hasRight && hasBottom)
                    {
                        sb.Append('┌');
                    }
                    else if (hasLeft && hasRight)
                    {
                        sb.Append('─');
                    }
                    else if (hasTop && hasBottom)
                    {
                        sb.Append('│');
                    }
                    else if (hasLeft)
                    {
                        sb.Append('╴');
                    }
                    else if (hasRight)
                    {
                        sb.Append('╶');
                    }
                    else if (hasTop)
                    {
                        sb.Append('╵');
                    }
                    else if (hasBottom)
                    {
                        sb.Append('╷');
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                }

                sb.AppendLine();
            }

            return sb.ToString();
        }
    }

    public static class GraphExtensions
    {
        public static List<Edge> MST(this Graph @this)
        {
            var subsets = @this.Nodes
                .Select(e => new HashSet<int> { e.Id })
                .ToList();

            var mst = new List<Edge>();

            var orderedEdges = @this.Edges.OrderBy(e => e.Weight);

            foreach (var edge in orderedEdges)
            {
                var firstSubsetIndex = subsets.FindIndex(e => e.Contains(edge.First));
                var secondSubsetIndex = subsets.FindIndex(e => e.Contains(edge.Second));

                if (firstSubsetIndex == secondSubsetIndex)
                {
                    continue;
                }

                var firstSubset = subsets[firstSubsetIndex];
                var secondSubset = subsets[secondSubsetIndex];

                var subsetUnion = firstSubset
                    .Union(secondSubset)
                    .ToHashSet();

                subsets.Remove(firstSubset);
                subsets.Remove(secondSubset);

                subsets.Add(subsetUnion);

                mst.Add(edge);
            }

            return mst;
        }
    }
}
