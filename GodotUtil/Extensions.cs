using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SouchyUtil.Godot;

public static class Extensions
{
    public static T? GetFirstChildOfType<T>(this Node node) where T : Node
    {
        var children = node.GetChildren();
        foreach(var child in children)
        {
            if(child is T t)
                return t;
            var possible = GetFirstChildOfType<T>(child);
            if(possible != null)
                return possible;
        }
        return null;
    }
}
