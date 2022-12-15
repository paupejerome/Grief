using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(RandomSpawner))]

public class SpawnerCustomInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        var Spawner = target as RandomSpawner;


        Spawner.RandomY = GUILayout.Toggle(Spawner.RandomY, "Random Y position");
        if (Spawner.RandomY)
        {
            Spawner.MinY = EditorGUILayout.FloatField("Minimum Y (relative)", Spawner.MinY);
            Spawner.MaxY = EditorGUILayout.FloatField("Maximum Y (relative)", Spawner.MaxY);
        }

        Spawner.RandomSpeed = GUILayout.Toggle(Spawner.RandomSpeed, "Random Speed");
        if (Spawner.RandomSpeed)
        {
            Spawner.MinSpeed = EditorGUILayout.FloatField("Minimum Speed", Spawner.MinSpeed);
            Spawner.MaxSpeed = EditorGUILayout.FloatField("Maximum Speed", Spawner.MaxSpeed);
        }
    }
}