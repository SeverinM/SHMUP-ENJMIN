using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

public class Tools : EditorWindow {
    static Tools instance;
    static SerializedProperty ser;
    static SerializedObject obj;
    static Level staticLvl;

    // Liste de toutes les vagues à lancer
    public List<Wave> allWaves;

    // Liste des vagues sérializés dans l'UI
    SerializedProperty serWaves;
    public Waypoints waypoints;

    Generator currentGen;

	[MenuItem("Outils GD/Ennemies et spawner")]
    static void Init()
    {
        if (instance == null)
        {
            instance = (Tools)EditorWindow.GetWindow(typeof(Tools));
            instance.Show();
        }
    }

    int selectedNumber()
    {
        int output = 0;
        //Un ennemi est selectionné ?
        if (allWaves == null)
        {
            return output;
        }
        foreach(Wave wv in allWaves)
        {
            foreach(WaveElement we in wv.allEnnemies)
            {
                if (we.selected)
                {
                    output++;
                }
            }
        }
        return output;
    }

    private void OnGUI()
    {
        if (obj == null)
        {
            obj = new SerializedObject(this);
        }

        staticLvl = (Level)EditorGUILayout.ObjectField(staticLvl, typeof(Level));
        currentGen = (Generator)EditorGUILayout.ObjectField(currentGen, typeof(Generator));
        //Waypoints
        obj = new SerializedObject(this);
        //Liste des waypoints
        ser = obj.FindProperty("waypoints");
        SerializedProperty listWaypoints = ser.FindPropertyRelative("allWaypoints");

        EditorGUILayout.PropertyField(ser.FindPropertyRelative("loop"));
        for (int i = 0; i < listWaypoints.arraySize; i++)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(listWaypoints.GetArrayElementAtIndex(i).FindPropertyRelative("targetPosition"), new GUIContent(""));
            EditorGUILayout.PropertyField(listWaypoints.GetArrayElementAtIndex(i).FindPropertyRelative("speed"), new GUIContent("Vitesse"));
            if (i > 0 && GUILayout.Button("Up"))
            {
                listWaypoints.MoveArrayElement(i, i - 1);
            }
            if (i < listWaypoints.arraySize - 1 && GUILayout.Button("Down"))
            {
                listWaypoints.MoveArrayElement(i, i + 1);
            }

            Vector3 value = listWaypoints.GetArrayElementAtIndex(i).FindPropertyRelative("targetPosition").vector3Value;
            //Tous les waypoints sont entre -1 et 1
            listWaypoints.GetArrayElementAtIndex(i).FindPropertyRelative("targetPosition").vector3Value = new Vector3(Mathf.Clamp(value.x, -1, 1), 0, Mathf.Clamp(value.z, -1, 1));

            if (GUILayout.Button("Supprimer"))
            {
                listWaypoints.DeleteArrayElementAtIndex(i);
            }

            EditorGUILayout.EndHorizontal();
        }
        if (GUILayout.Button("Ajouter"))
        {
            listWaypoints.InsertArrayElementAtIndex(0);
        }

        EditorGUI.BeginDisabledGroup(selectedNumber() == 0);
        List<WaveElement> allElem = new List<WaveElement>();
        if (GUILayout.Button("Appliquer"))
        {
            foreach(Wave wv in allWaves)
            {
                foreach(WaveElement we in wv.allEnnemies.Where(x => x.selected))
                {
                    allElem.Add(we);
                }
            }

            //Applique les waypoints a tous les ennemies concernés
            for (int i = 0; i < allElem.Count; i++)
            {
                WaveElement elem = allElem[i];
                elem.Waypoints = waypoints.Clone();
                foreach(WaypointElement wE in elem.Waypoints.allWaypoints)
                {
                    wE.targetPosition = Tools.GetPositionAbsolute(wE.targetPosition);
                }
            }
        }
        EditorGUI.EndDisabledGroup();
        GUILayout.Space(20);

        float distance = 10;
        //distance camera / level pour l'affichage du plan
        if (Selection.GetFiltered<Level>(SelectionMode.Unfiltered).Length > 0)
        {
            distance = Vector3.Distance(Selection.GetFiltered<Level>(SelectionMode.Unfiltered)[0].transform.position, Camera.main.transform.position);
        }

        //Liste des waves
        serWaves = obj.FindProperty("allWaves");
        //Toutes les vagues
        for (int i = 0; i < serWaves.arraySize; i++)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Vague numero " + i);
            EditorGUILayout.PropertyField(serWaves.GetArrayElementAtIndex(i).FindPropertyRelative("firstIsLeader"), new GUIContent("Premier est Leader"));
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
                EditorGUILayout.PropertyField(serRel.GetArrayElementAtIndex(j).FindPropertyRelative("spawnAfter"), new GUIContent("Apparait apres (s)"));
                EditorGUILayout.PropertyField(serRel.GetArrayElementAtIndex(j).FindPropertyRelative("followPlayer"), new GUIContent("Suit le Joueur"));
                EditorGUILayout.PropertyField(serRel.GetArrayElementAtIndex(j).FindPropertyRelative("enMov"), new GUIContent("Type de Mouvement"));
                EditorGUILayout.PropertyField(serRel.GetArrayElementAtIndex(j).FindPropertyRelative("selected"), new GUIContent(" "));

                if (GUILayout.Button("Supprimer"))
                {
                    serRel.DeleteArrayElementAtIndex(j);
                }

                EditorGUILayout.EndHorizontal();
            }

            //Fin de l'indentation
            EditorGUI.indentLevel = 0;
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Ajouter (ennemie vague)"))
            {
                serRel.InsertArrayElementAtIndex(serRel.arraySize);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
        }

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Ajouter (vague)"))
        {
            serWaves.InsertArrayElementAtIndex(serWaves.arraySize);
        }
        if (serWaves.arraySize > 0 && GUILayout.Button("Supprimer (vague)"))
        {
            serWaves.DeleteArrayElementAtIndex(serWaves.arraySize - 1);
        }
        EditorGUILayout.EndHorizontal();
        EditorGUI.BeginDisabledGroup(currentGen == null);
        if (GUILayout.Button("Appliquer au spawner"))
        {
            currentGen.AllWaves = allWaves;
        }
        EditorGUI.EndDisabledGroup();

        obj.ApplyModifiedProperties();
    }

    private void OnDestroy()
    {
        instance = null;
    }

    static Vector3 GetPositionAbsolute(Vector3 input)
    {
        if (staticLvl == null)
        {
            throw new System.Exception("Vous devez avoir un level de selectionné");
        }

        float coeff = Vector3.Distance(staticLvl.transform.position, Camera.main.transform.position);
        //Recuperation des quatres coins
        Vector3 leftBottom = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, coeff));
        Vector3 leftTop = Camera.main.ScreenToWorldPoint(new Vector3(0, Camera.main.pixelHeight, coeff));
        Vector3 rightBottom = Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.pixelWidth, 0, coeff));
        Vector3 rightTop = Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.pixelWidth, Camera.main.pixelHeight, coeff));

        Vector3 xAxis = rightBottom - leftBottom;
        Vector3 yAxis = leftTop - leftBottom;
        Vector3 finalPosition = leftBottom + (xAxis * input.x) + (yAxis * input.z);

        return finalPosition;
    }

    [DrawGizmo(GizmoType.NotInSelectionHierarchy | GizmoType.Selected)]
    static void Draw(Level lvl, GizmoType type)
    {
        if (instance != null && lvl == staticLvl)
        {
            //Distance par rapport a la camera
            float coeff = Vector3.Distance(lvl.transform.position, Camera.main.transform.position);
            Vector3 previousPosition = Vector3.zero;
            Vector3 firstPosition = Vector3.zero;
            Vector3 lastPosition = Vector3.zero;

            float value;

            SerializedProperty allWaypoints = ser.FindPropertyRelative("allWaypoints");
            for (int i = 0; i < allWaypoints.arraySize; i++)
            {
                value = (float)i / (allWaypoints.arraySize - 1);
                Gizmos.color = Color.Lerp(Color.green, Color.red, value);
                Vector3 finalPosition = GetPositionAbsolute(allWaypoints.GetArrayElementAtIndex(i).FindPropertyRelative("targetPosition").vector3Value);

                Gizmos.DrawSphere(finalPosition, 1);
                if (i > 0)
                {
                    Gizmos.DrawLine(previousPosition, finalPosition);
                }
                //A chaque fois , dessine un trait avec la position precedente
                previousPosition = finalPosition;

                if (i == 0)
                {
                    firstPosition = finalPosition;
                }

                if (i == allWaypoints.arraySize - 1)
                {
                    lastPosition = finalPosition;
                }
            }

            //On trace un trait violet si on a choist de loop
            if(ser.FindPropertyRelative("loop").boolValue)
            {
                Gizmos.color = Color.magenta;
                Gizmos.DrawLine(firstPosition, lastPosition);
            }

            Gizmos.color = Color.white;

            //On trace un plan sur la scene
            Gizmos.DrawLine(GetPositionAbsolute(new Vector3(1, 0, 1)), GetPositionAbsolute(new Vector3(1, 0, 0)));
            Gizmos.color = Color.red;
            Gizmos.DrawLine(GetPositionAbsolute(new Vector3(1, 0, 0)), GetPositionAbsolute(new Vector3(0, 0, 0)));
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(GetPositionAbsolute(new Vector3(0, 0, 0)), GetPositionAbsolute(new Vector3(0, 0, 1)));
            Gizmos.color = Color.white;
            Gizmos.DrawLine(GetPositionAbsolute(new Vector3(1, 0, 1)), GetPositionAbsolute(new Vector3(0, 0, 1)));
        }
    }
}