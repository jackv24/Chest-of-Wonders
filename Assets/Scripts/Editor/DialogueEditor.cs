using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class DialogueEditor : EditorWindow
{
    TextAsset textAsset;
    DialogueGraph graph = null;

    List<Rect> windows = new List<Rect>();
    List<int> attachedWindows = new List<int>();

    Vector2 scrollPos;

    [MenuItem("Custom/Dialogue Editor")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(DialogueEditor), false, "Dialogue Editor", true);
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Dialogue Asset", EditorStyles.boldLabel);

        textAsset = (TextAsset)EditorGUILayout.ObjectField("Dialogue Text Asset", textAsset, typeof(TextAsset), false);
        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Load"))
        {
            //Deserialise json into graph
            graph = JsonUtility.FromJson<DialogueGraph>(textAsset.text);

            CreateNode(0, -1);
        }

        if (GUILayout.Button("Save"))
        {
            string json = JsonUtility.ToJson(graph);
            System.IO.File.WriteAllText(AssetDatabase.GetAssetPath(textAsset), json);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.LabelField("Dialogue Graph", EditorStyles.boldLabel);

        if (graph == null || graph.nodes.Count <= 0)
        {
            EditorGUILayout.HelpBox("No dialogue graph has been loaded.", MessageType.Warning);
        }
        else
        {
            graph.speakerName = EditorGUILayout.TextField("Speaker Name", graph.speakerName);

            //if (GUILayout.Button("Create Node"))
            //{
            //    windows.Add(new Rect(10, 10, 100, 100));
            //}

            Rect scrollSize = new Rect();

            for (int i = 0; i < graph.nodes.Count; i++)
            {
                Rect nodeRect = graph.nodes[i].rect;

                if (nodeRect.width + nodeRect.x > scrollSize.width)
                    scrollSize.width = nodeRect.width + nodeRect.x;

                if (nodeRect.height + nodeRect.y > scrollSize.height)
                    scrollSize.height = nodeRect.height + nodeRect.y;
            }

            EditorGUILayout.BeginVertical();
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndVertical();

            scrollPos = GUI.BeginScrollView(GUILayoutUtility.GetLastRect(), scrollPos, scrollSize, true, true);

            BeginWindows();
            for (int i = 0; i < windows.Count; i++)
            {
                windows[i] = GUI.Window(i, windows[i], DrawNodeWindow, "Window " + i);
            }
            EndWindows();

            if (attachedWindows.Count >= 2)
            {
                for (int i = 0; i < attachedWindows.Count; i += 2)
                {
                    DrawNodeCurve(windows[attachedWindows[i]], windows[attachedWindows[i + 1]]);
                }
            }

            GUI.EndScrollView();
        }
    }

    private void CreateNode(int index, int fromIndex)
    {
        DialogueGraph.DialogueGraphNode node = graph.nodes[index];

        windows.Add(node.rect);

        for (int i = 0; i < node.options.Count; i++)
        {
            CreateNode(node.options[i].target, index);
        }

        if (fromIndex >= 0)
        {
            attachedWindows.Add(fromIndex);
            attachedWindows.Add(index);
        }
    }

    void DrawNodeWindow(int id)
    {
        DialogueGraph.DialogueGraphNode node = graph.nodes[id];

        node.text = EditorGUILayout.TextField("Text", node.text);

        for (int i = 0; i < node.options.Count; i++)
        {
            node.options[i].text = EditorGUILayout.TextField("Option", node.options[i].text);
        }

        node.rect = windows[id];

        GUI.DragWindow();
    }

    void DrawNodeCurve(Rect start, Rect end)
    {
        Vector3 startPos = new Vector3(start.x + start.width, start.y + start.height / 2, 0);
        Vector3 endPos = new Vector3(end.x, end.y + end.height / 2, 0);
        Vector3 startTan = startPos + Vector3.right * 50;
        Vector3 endTan = endPos + Vector3.left * 50;
        Color shadowCol = new Color(0, 0, 0, 0.06f);

        for (int i = 0; i < 3; i++)
        {// Draw a shadow
            Handles.DrawBezier(startPos, endPos, startTan, endTan, shadowCol, null, (i + 1) * 5);
        }

        Handles.DrawBezier(startPos, endPos, startTan, endTan, Color.black, null, 1);
    }
}
