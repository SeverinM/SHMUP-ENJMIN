using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ToolsLock : EditorWindow {

    public static ToolsLock instance;
    static SerializedObject obj;
    static SerializedProperty allLockSerialized;
    static SerializedProperty allLockNbSerialized;

    public List<LockWaveElement> allLocks = new List<LockWaveElement>();
    public List<int> allLocksNumber = new List<int>();

    [MenuItem("Outils GD/Gestion de verrou")]
    static void Init()
    {
        if (instance == null)
        {
            instance = (ToolsLock)EditorWindow.GetWindow(typeof(ToolsLock));
            obj = new SerializedObject(instance);
            allLockSerialized = obj.FindProperty("allLocks");
            allLockNbSerialized = obj.FindProperty("allLocksNumber");
            instance.Show();
        }
    }

    public void OnGUI()
    {
        GUILayout.Label("Attention aux blocages mutuels !",EditorStyles.boldLabel);

        for (int i = 0; i < allLockSerialized.arraySize; i++)
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(allLockNbSerialized.GetArrayElementAtIndex(i), new GUIContent("La vague numero "));
            EditorGUILayout.PropertyField(allLockSerialized.GetArrayElementAtIndex(i).FindPropertyRelative("number"), new GUIContent("Se lancera quand la vague numero "));
            EditorGUILayout.PropertyField(allLockSerialized.GetArrayElementAtIndex(i).FindPropertyRelative("generator"), new GUIContent("de ce generateur sera finit"));
            if (GUILayout.Button("Supprimer"))
            {
                allLockNbSerialized.DeleteArrayElementAtIndex(i);
                allLockSerialized.DeleteArrayElementAtIndex(i);
            }
            GUILayout.EndHorizontal();
        }

        //Tri et filtres pour s'assurer que rien ne fasse bugger
        allLocksNumber.ForEach(x => x = Mathf.Abs(x));
        allLocks.ForEach(x =>
        {
            if(Tools.currentGen != null && x.generator == Tools.currentGen)
            {
                x.generator = null;
                Debug.LogError("Un generateur ne peut pas mettre un verrou sur lui-meme, la selection a été retiré");
            }
        });

        if (GUILayout.Button("Ajouter un verrou"))
        {
            allLockSerialized.InsertArrayElementAtIndex(allLockSerialized.arraySize);
            allLockNbSerialized.InsertArrayElementAtIndex(allLockNbSerialized.arraySize);
        }

        if(Tools.currentGen != null && GUILayout.Button("Appliquer au spawner"))
        {
            Dictionary<int, List<LockWaveElement>> output = new Dictionary<int, List<LockWaveElement>>();
            for (int i = 0; i < allLockNbSerialized.arraySize; i++)
            {
                int key = allLocksNumber[i];
                if (!output.ContainsKey(key))
                    output[key] = new List<LockWaveElement>();

                output[key].Add(allLocks[i]);
            }
            Tools.currentGen.AllLocks = output;
        }
        obj.ApplyModifiedProperties();
    }
}
