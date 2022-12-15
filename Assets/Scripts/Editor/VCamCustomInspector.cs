//using UnityEngine;
//using UnityEditor;


//[CustomEditor(typeof(vcamFollow))]

//public class VCamCustomInspector : Editor
//{
//    public override void OnInspectorGUI()
//    {
//        DrawDefaultInspector();
//        var follow = target as vcamFollow;


//        follow.MaxCoords = GUILayout.Toggle(follow.MaxCoords, "Max coords");
//        if (follow.MaxCoords)
//        {
//            follow.minX = EditorGUILayout.FloatField("Minimum X", follow.minX);
//            follow.maxX = EditorGUILayout.FloatField("Maximum X", follow.maxX);
//            follow.minY = EditorGUILayout.FloatField("Minimum Y", follow.minY);
//            follow.maxY = EditorGUILayout.FloatField("Maximum Y", follow.maxY);
//        }
//    }
//}