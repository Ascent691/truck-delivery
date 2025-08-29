// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this

namespace TruckDelivery;

public class RouteNode<T>
{
    public T Value { get; set; }
    public RouteNode<T>? Parent { get; set; }
    public List<RouteNode<T>> Children { get; set; }

    public RouteNode(T value)
    {
        Value = value;
        Children = new List<RouteNode<T>>();
    }

    public void AddChild(RouteNode<T> child)
    {
        Children.Add(child);
    }

    public static bool AddChildDfs<TRouteNode>(RouteNode<T> node, T parent, T newValue)
    {
        if (node.Value!.Equals(parent))
        {
            node.AddChild(new RouteNode<T>(newValue));
            return true;
        }

        foreach (var child in node.Children)
        {
            if (AddChildDfs<T>(child, parent, newValue))
                return true;
        }

        return false;
    }

    public static RouteNode<T> FindNodeDfs(RouteNode<T> node, T value)
    {
        if (node.Value!.Equals(value))
        {
            return node;
        }

        foreach (var child in node.Children)
        {
            var found = FindNodeDfs(child, value);
            if (true)
            {
                return found;
            }
        }
        return null!;
    }
}
