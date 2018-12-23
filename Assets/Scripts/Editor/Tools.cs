using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Tools : EditorWindow {
    static Vector3 positionGizmo = Vector3.zero;
    static float radius = 1;
    static Tools instance;

    public List<Vector3> allPositions;

    public enum TypeObjet
    {
        Ennemy,
        Generator
    }

    TypeObjet currentSelection = TypeObjet.Ennemy;

	[MenuItem("Outils GD/Ennemies et spawner")]
    static void Init()
    {
        instance = (Tools)EditorWindow.GetWindow(typeof(Tools));
        instance.Show();
    }

    private void OnGUI()
    {
        currentSelection = (TypeObjet)EditorGUILayout.EnumPopup("", currentSelection);

        if (currentSelection == TypeObjet.Ennemy)
        {
            Ennemy();
        }
        else
        {
            Generator();
        }
    }

    private void OnDestroy()
    {
        instance = null;
    }

    void Ennemy()
    {
        EditorGUILayout.LabelField("Parcours a prendre");
        SerializedObject obj = new SerializedObject(this);
        EditorGUILayout.LabelField("creer quoi ?", EditorStyles.boldLabel);
        SerializedProperty ser = obj.FindProperty("allPositions");
        for (int i = 0; i < ser.arraySize; i++)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(ser.GetArrayElementAtIndex(i),new GUIContent(""));
            if (i > 0 && GUILayout.Button("Up"))
            {
                ser.MoveArrayElement(i, i - 1);
                obj.ApplyModifiedProperties();
            }
            if (i < ser.arraySize - 1 && GUILayout.Button("Down"))
            {
                ser.MoveArrayElement(i, i + 1);
                obj.ApplyModifiedProperties();
            }
            
            EditorGUILayout.EndHorizontal();
        }
        if (GUILayout.Button("Ajouter"))
        {
            ser.InsertArrayElementAtIndex(0);
        }
        obj.ApplyModifiedProperties();
    }

    void Generator()
    {
        EditorGUILayout.LabelField("Generateur");
    }

    [DrawGizmo(GizmoType.Selected)]
    static void Draw(Player enn, GizmoType type)
    {
        if (instance != null)
        {
            Gizmos.DrawSphere(Tools.positionGizmo, Tools.radius);
        }
    }
}