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

    Vector2 scrollPos = Vector2.zero;
    Vector2 scrollPosWave = Vector2.zero;

    public static Generator currentGen;
    Generator previousGen = null;

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

        GUILayout.Label("Le cadre ne s'affichera que si vous avez choisi un Level , si besoin changez la selection dans la scene", EditorStyles.boldLabel);
        staticLvl = (Level)EditorGUILayout.ObjectField(staticLvl, typeof(Level));
        currentGen = (Generator)EditorGUILayout.ObjectField(currentGen, typeof(Generator));
        if (currentGen != previousGen && currentGen != null)
        {
            allWaves = currentGen.allWaves;
            if (ToolsLock.instance != null)
                currentGen.ToList(ref ToolsLock.instance.allLocksNumber, ref ToolsLock.instance.allLocks);
        }
        previousGen = currentGen;

        //Waypoints
        obj = new SerializedObject(this);
        //Liste des waypoints
        ser = obj.FindProperty("waypoints");
        SerializedProperty listWaypoints = ser.FindPropertyRelative("allWaypoints");

        GUILayout.Label("Liste des waypoints , si la liste est finit il bougera aleatoirement à la fin", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(ser.FindPropertyRelative("loop"));
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(Mathf.Max(position.width, 300)), GUILayout.Height(Mathf.Max(position.height / 6, 10)));
        for (int i = 0; i < listWaypoints.arraySize; i++)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(listWaypoints.GetArrayElementAtIndex(i).FindPropertyRelative("targetPosition"), new GUIContent(""), GUILayout.Width(Mathf.Max(300,position.width / 3)));
            EditorGUIUtility.labelWidth = 50;
            EditorGUILayout.PropertyField(listWaypoints.GetArrayElementAtIndex(i).FindPropertyRelative("speed"), new GUIContent("Vitesse"), GUILayout.Width(200));
            if (i > 0 && GUILayout.Button("Up"))
            {
                listWaypoints.MoveArrayElement(i, i - 1);
            }
            if (i < listWaypoints.arraySize - 1 && GUILayout.Button("Down"))
            {
                listWaypoints.MoveArrayElement(i, i + 1);
            }

            Vector3 value = listWaypoints.GetArrayElementAtIndex(i).FindPropertyRelative("targetPosition").vector3Value;
            //Tous les waypoints sont entre 0 et 1
            listWaypoints.GetArrayElementAtIndex(i).FindPropertyRelative("targetPosition").vector3Value = new Vector3(Mathf.Clamp(value.x, 0, 1), 0, Mathf.Clamp(value.z, 0, 1));

            if (GUILayout.Button("Supprimer"))
            {
                listWaypoints.DeleteArrayElementAtIndex(i);
            }

            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndScrollView();
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

        GUILayout.Label("Gestion des vagues , selectionnez un ennemie pour lui attribuer un waypoint", EditorStyles.boldLabel);

        //Liste des waves
        serWaves = obj.FindProperty("allWaves");
        //Toutes les vagues
        scrollPosWave = EditorGUILayout.BeginScrollView(scrollPosWave, GUILayout.Width(Mathf.Max(position.width, 300)), GUILayout.Height(Mathf.Max(position.height / 3, 10)));
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
                GUILayout.FlexibleSpace();
                EditorGUIUtility.labelWidth = 80;
                EditorGUILayout.PropertyField(serRel.GetArrayElementAtIndex(j).FindPropertyRelative("enn"), new GUIContent("L'ennemi "), GUILayout.MaxWidth(120));
                EditorGUIUtility.labelWidth = 100;
                GUILayout.FlexibleSpace();
                EditorGUILayout.PropertyField(serRel.GetArrayElementAtIndex(j).FindPropertyRelative("spawnAfter"), new GUIContent("Apparait apres (s)"), GUILayout.MaxWidth(150));
                GUILayout.FlexibleSpace();
                EditorGUILayout.PropertyField(serRel.GetArrayElementAtIndex(j).FindPropertyRelative("followPlayer"), new GUIContent("Suit le Joueur"));
                GUILayout.FlexibleSpace();
                EditorGUILayout.PropertyField(serRel.GetArrayElementAtIndex(j).FindPropertyRelative("enMov"), new GUIContent("Type Mvmt"));
                EditorGUIUtility.labelWidth = 60;
                EditorGUILayout.PropertyField(serRel.GetArrayElementAtIndex(j).FindPropertyRelative("speed"), new GUIContent("Vitesse"));
                EditorGUIUtility.labelWidth = 45;
                EditorGUILayout.PropertyField(serRel.GetArrayElementAtIndex(j).FindPropertyRelative("life"), new GUIContent("Vie"));
                GUILayout.FlexibleSpace();
                EditorGUIUtility.labelWidth = 30;
                EditorGUILayout.PropertyField(serRel.GetArrayElementAtIndex(j).FindPropertyRelative("selected"), GUIContent.none);
                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Lire WP"))
                {
                    foreach(WaypointElement wE in allWaves[i].allEnnemies[j].Waypoints.allWaypoints)
                    {
                        wE.targetPosition = GetPositionRelative(wE.targetPosition);
                    }
                    waypoints = allWaves[i].allEnnemies[j].Waypoints;
                }

                if (GUILayout.Button("Suppr."))
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
        EditorGUILayout.EndScrollView();

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

    //Inverse de getPositionAbsolute
    public Vector3 GetPositionRelative(Vector3 input)
    {
        float coeff = Vector3.Distance(staticLvl.transform.position, Camera.main.transform.position);
        Vector3 leftBottom = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, coeff));
        Vector3 leftTop = Camera.main.ScreenToWorldPoint(new Vector3(0, Camera.main.pixelHeight, coeff));
        Vector3 rightBottom = Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.pixelWidth, 0, coeff));
        Vector3 rightTop = Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.pixelWidth, Camera.main.pixelHeight, coeff));

        float XRelative = Mathf.Abs(leftBottom.x - input.x) / Mathf.Abs(leftBottom.x - rightBottom.x);
        float ZRelative = Mathf.Abs(leftBottom.z - input.z) / Mathf.Abs(leftBottom.z - leftTop.z);
        return new Vector3(XRelative, 0, ZRelative);
    }

    [DrawGizmo(GizmoType.NotInSelectionHierarchy | GizmoType.Selected | GizmoType.NonSelected)]
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