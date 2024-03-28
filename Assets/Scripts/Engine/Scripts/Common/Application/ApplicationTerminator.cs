using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
public static class ApplicationTerminator
{


    public static void ExitGame()
    {
        Application.Quit();
        ExitUnityEditor();
    }

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void ExitUnityEditor()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
