using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CenterObject : MonoBehaviour
{
    public bool Run;
    public Vector2 Offset = new Vector2(0.5f,0.5f);

    private void Update()
    {
        if (Run)
        {
            var objs = GameObject.FindObjectsOfType<CenterObject>();

            foreach (var obj in objs)
                obj.FixPosition();

            Run = false;
        }
    }

    void FixPosition()
    {
        var tr = transform;
        var pos = tr.position;
        var x = Mathf.FloorToInt(pos.x) + Offset.x;
        var y = Mathf.FloorToInt(pos.y) + Offset.y;

        tr.position = new Vector2(x, y);
    }
}
