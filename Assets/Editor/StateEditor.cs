using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DialogState))]
public class StateEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        //DialogState state = (DialogState)target;
        //state.name = name;
    }
}
