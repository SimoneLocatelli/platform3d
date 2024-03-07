using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class TagsExtensions
{
    public static bool HasTag(this GameObject obj, string tag)
    {
        if (obj == null) return false;

        var multitags = obj.GetComponent<Multitags>();

        if (multitags == null) return false;

        return multitags.Contains(tag);
    }

    public static bool HasAnyTag(this GameObject obj, List<string> tags)
    {
        if (obj == null) return false;

        var multitags = obj.GetComponent<Multitags>();

        if (multitags == null) return false;

        return multitags.ContainsAny(tags);
    }
}
