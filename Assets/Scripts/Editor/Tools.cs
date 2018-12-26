using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public struct Wave
{
    public List<WaveElement> allEnnemies;
    public float delay;
}

[System.Serializable]
public struct WaveElement
{
    public Enemy.EnemyType enn;
    public float spawnAfter;
}

public class Tools : EditorWindow {
    static float radius = 1;
    static Tools instance;
    static SerializedProperty ser;
    static SerializedObject obj;

    public List<Wave> allWaves;
    SerializedProperty serWaves;

    public List<Vector3> allPositions;
    Level lvl;

    public enum TypeObjet
    {
        Ennemy,
        Generator
    }

    TypeObjet currentSelection = TypeObjet.Ennemy;

	[MenuItem("Outils GD/Ennemies et spawner")]
    static void Init()
    {
        if (instance == null)
        {
            instance = (Tools)EditorWindow.GetWindow(typeof(Tools));
            instance.Show();
        }
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
        obj = new SerializedObject(this);
        ser = obj.FindProperty("allPositions");
        for (int i = 0; i < ser.arraySize; i++)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(ser.GetArrayElementAtIndex(i),new GUIContent(""));
            if (i > 0 && GUILayout.Button("Up"))
            {
                ser.MoveArrayElement(i, i - 1);
            }
            if (i < ser.arraySize - 1 && GUILayout.Button("Down"))
            {
                ser.MoveArrayElement(i, i + 1);
            }

            Vector3 value = ser.GetArrayElementAtIndex(i).vector3Value;
            ser.GetArrayElementAtIndex(i).vector3Value = new Vector3(Mathf.Clamp(value.x, -1, 1), 0, Mathf.Clamp(value.z, -1, 1));
            
            EditorGUILayout.EndHorizontal();
        }
        if (GUILayout.Button("Ajouter"))
        {
            ser.InsertArrayElementAtIndex(0);
        }
        obj.ApplyModifiedProperties();
        Vector3 positionScreen = Vector3.zero;
        float distance = 10;
        if (Selection.GetFiltered<Level>(SelectionMode.Unfiltered).Length > 0)
        {
            distance = Vector3.Distance(Selection.GetFiltered<Level>(SelectionMode.Unfiltered)[0].transform.position, Camera.main.transform.position);
        }
    }

    void Generator()
    {
        //Liste des waves
        serWaves = obj.FindProperty("allWaves");
        //Toutes les vagues
        for (int i = 0; i < serWaves.arraySize; i++)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Vague numero " + i);
            EditorGUILayout.PropertyField(serWaves.GetArrayElementAtIndex(i).FindPropertyRelative("delay"), new GUIContent("Commence apres (s)"));
            EditorGUILayout.EndHorizontal();
            SerializedProperty serRel = serWaves.GetArrayElementAtIndex(i).FindPropertyRelative("allEnnemies");

            //Debut de l'indentation
            EditorGUI.indentLevel = 1;

            //Tous les ennemis d'une vague
            for (int j = 0; j < serRel.arraySize; j++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(serRel.GetArrayElementAtIndex(j).FindPropertyRelative("enn"), new GUIContent("L'ennemi "));
                EditorGUILayout.PropertyField(serRel.GetArrayElementAtIndex(j).FindPropertyRelative("spawnAfter"),new GUIContent("Apparait apres (s)"));
                EditorGUILayout.EndHorizontal();

                if ((j == serRel.arraySize - 1) && GUILayout.Button("Ajouter (ennemie vague)"))
                {
                    serRel.InsertArrayElementAtIndex(serRel.arraySize);
                }

                if (j == serRel.arraySize - 1 && GUILayout.Button("Supprimer (ennemie vague)"))
                {
                    serRel.DeleteArrayElementAtIndex(serRel.arraySize - 1);
                }
            }

            //Fin de l'indentation
            EditorGUI.indentLevel = 0;

            //Aucun ennemi dans la vague
            if (serRel.arraySize == 0 && GUILayout.Button("Ajouter (ennemie vague)"))
            {
                serRel.InsertArrayElementAtIndex(serRel.arraySize);
            }
            EditorGUILayout.Space();
        }

        if (GUILayout.Button("Ajouter (vague)"))
        {
            serWaves.InsertArrayElementAtIndex(serWaves.arraySize);
        }
        if (serWaves.arraySize > 0 && GUILayout.Button("Supprimer (vague)"))
        {
            serWaves.DeleteArrayElementAtIndex(serWaves.arraySize - 1);
        }
        obj.ApplyModifiedProperties();
    }

    [DrawGizmo(GizmoType.Selected)]
    static void Draw(Level lvl, GizmoType type)
    {
        if (instance != null)
        {
            float coeff = Vector3.Distance(lvl.transform.position, Camera.main.transform.position);
            Vector3 leftBottom = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, coeff));
            Vector3 leftTop = Camera.main.ScreenToWorldPoint(new Vector3(0, Camera.main.pixelHeight, coeff));
            Vector3 rightBottom = Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.pixelWidth, 0, coeff));
            Vector3 rightTop = Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.pixelWidth, Camera.main.pixelHeight, coeff));
            Vector3 pos = lvl.transform.position;
            leftBottom = new Vector3(leftBottom.x, 0, leftBottom.z);
            leftTop = new Vector3(leftTop.x, 0, leftTop.z);
            rightBottom = new Vector3(rightBottom.x, 0, rightBottom.z);
            rightTop = new Vector3(rightTop.x, 0, rightTop.z);
            Vector3 previousPosition = Vector3.zero;
            float value;

            try
            {
                for (int i = 0; i < ser.arraySize; i++)
                {
                    value = (float)i / (ser.arraySize - 1);
                    Gizmos.color = Color.Lerp(Color.green, Color.red, value);
                    Debug.Log(i + " / " + (ser.arraySize - 1) + " = "  + value);
                    Vector3 position = ser.GetArrayElementAtIndex(i).vector3Value;
                    Vector3 xAxis = rightBottom - leftBottom;
                    Vector3 yAxis = leftTop - leftBottom;
                    Vector3 finalPosition = leftBottom + (xAxis * position.x) + (yAxis * position.z);
                    finalPosition = new Vector3(finalPosition.x, lvl.transform.position.y, finalPosition.z);
                    Gizmos.DrawSphere(finalPosition, 0.2f);
                    if (i > 0)
                    {
                        Gizmos.DrawLine(previousPosition, finalPosition);
                    }
                    previousPosition = finalPosition;
                }               
            }
            catch
            {
                Debug.LogWarning("Ce message n'est censé etre affiché qu'une fois , sinon c'est un bug");
            }
            Gizmos.color = Color.white;

            //On trace un plan sur la scene
            Gizmos.DrawLine(pos + rightTop, pos + rightBottom);
            Gizmos.color = Color.red;
            Gizmos.DrawLine(pos + rightBottom, pos + leftBottom);
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(pos + leftBottom, pos + leftTop);
            Gizmos.color = Color.white;
            Gizmos.DrawLine(pos + rightTop, pos + leftTop);
        }
    }
}