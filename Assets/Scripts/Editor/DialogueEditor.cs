using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class DialogueEditor : EditorWindow
{
    TextAsset textAsset;
    DialogueGraph graph = null;

    List<Rect> windows = new List<Rect>();
    List<int> markedForDeletion = new List<int>();

    DialogueGraph.DialogueGraphNode tempNode = null;
    DialogueGraph.DialogueGraphNode.Option tempOption = null;

    Vector2 scrollPos;

    [MenuItem("Jack's Custom Tools/Dialogue Editor")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(DialogueEditor), false, "Dialogue Editor", true);
    }

    public static void ShowWindow(TextAsset asset)
    {
        DialogueEditor editor = (DialogueEditor)EditorWindow.GetWindow(typeof(DialogueEditor), false, "Dialogue Editor", true);
        editor.textAsset = asset;

        editor.graph = JsonUtility.FromJson<DialogueGraph>(editor.textAsset.text);

        editor.windows.Clear();

        for (int i = 0; i < editor.graph.nodes.Count; i++)
            editor.CreateNode(i);
    }

    private void OnGUI()
    {
        for (int i = 0; i < markedForDeletion.Count; i++)
        {
            int id = markedForDeletion[i];

            windows.RemoveAt(graph.nodes.IndexOf(graph.GetNode(id)));
            graph.nodes.Remove(graph.GetNode(id));
        }

        markedForDeletion.Clear();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.BeginVertical(GUILayout.Width(300));

        EditorGUILayout.LabelField("Dialogue Options", EditorStyles.boldLabel);

        textAsset = (TextAsset)EditorGUILayout.ObjectField("Dialogue Text Asset", textAsset, typeof(TextAsset), false);
        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Load"))
        {
            //Deserialise json into graph
            graph = JsonUtility.FromJson<DialogueGraph>(textAsset.text);

            windows.Clear();

            for(int i = 0; i < graph.nodes.Count; i++)
                CreateNode(i);
        }

        if (GUILayout.Button("Save"))
        {
            string json = JsonUtility.ToJson(graph);
            System.IO.File.WriteAllText(AssetDatabase.GetAssetPath(textAsset), json);
            AssetDatabase.Refresh();
        }

        if (GUILayout.Button("Create New"))
        {
            if(EditorUtility.DisplayDialog(
                "Create new dialogue graph?",
                "A new file called \"NewDialog.json\" will be created in the resources folder",
                "OK", "Cancel"))
            {
                graph = new DialogueGraph();

                System.IO.File.WriteAllText("Assets/Data/Dialogue/NewDialogue.json", JsonUtility.ToJson(graph));
                AssetDatabase.Refresh();

                textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/Data/Dialogue/NewDialogue.json");

                windows.Clear();

                for (int i = 0; i < graph.nodes.Count; i++)
                    CreateNode(i);
            }
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("Dialogue Graph", EditorStyles.boldLabel);

        if (graph == null || graph.nodes.Count <= 0)
        {
            EditorGUILayout.HelpBox("No dialogue graph has been loaded.", MessageType.Warning);
        }
        else
        {
            graph.speakerName = EditorGUILayout.TextField("Speaker Name", graph.speakerName);

            if (GUILayout.Button("Create New"))
            {
                graph.nodes.Add(new DialogueGraph.DialogueGraphNode(graph.GetNewID(), ""));
                CreateNode(graph.nodes.Count - 1);
            }

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
                windows[i] = GUILayout.Window(graph.nodes[i].id, windows[i], DrawNodeWindow, "Node" + graph.nodes[i].id);
            }
            EndWindows();

            for (int i = 0; i < windows.Count; i++)
            {
                for (int j = 0; j < graph.nodes[i].options.Count; j++)
                {
                    Rect startRect = windows[i];
                    startRect.height = EditorGUIUtility.singleLineHeight;
                    startRect.y += EditorGUIUtility.singleLineHeight * (j + 5) + EditorGUIUtility.singleLineHeight / 2;

                    DialogueGraph.DialogueGraphNode node = graph.GetNode(graph.nodes[i].options[j].target);

                    if (node != null)
                        //DrawNodeCurve(startRect, windows[graph.nodes[i].options[j].target]);
                        DrawNodeCurve(startRect, node.rect);
                }
            }

            GUI.EndScrollView();
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
    }

    private void CreateNode(int index)
    {
        DialogueGraph.DialogueGraphNode node = graph.nodes[index];

        Rect rect = node.rect;
        rect.height = 0;

        windows.Add(rect);
    }

    void DrawNodeWindow(int id)
    {
        DialogueGraph.DialogueGraphNode node = graph.GetNode(id);

        EditorGUILayout.PrefixLabel("Text");
        node.text = EditorGUILayout.TextArea(node.text, GUILayout.Height(EditorGUIUtility.singleLineHeight * 3));

        int optionToDelete = -1;

        for (int i = 0; i < node.options.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            node.options[i].text = EditorGUILayout.TextField(node.options[i].text);

            if (GUILayout.Button("Remove"))
            {
                optionToDelete = i;
            }

            if (graph.GetNode(node.options[i].target) == null)
            {
                if (node.options[i] != tempOption)
                {
                    if (GUILayout.Button("+", GUILayout.Width(30)))
                    {
                        tempOption = node.options[i];
                        tempNode = node;
                    }
                }
                else
                {
                    if (GUILayout.Button("x", GUILayout.Width(30)))
                    {
                        tempOption = null;
                        tempNode = null;
                    }
                }
            }
            else
            {
                if (GUILayout.Button("-", GUILayout.Width(30)))
                {
                    node.options[i].target = -1;
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        if(optionToDelete >= 0)
            node.options.RemoveAt(optionToDelete);

        if (GUILayout.Button("Add Option"))
        {
            node.options.Add(new DialogueGraph.DialogueGraphNode.Option("New Option"));
        }

        if (GUILayout.Button("Delete Node"))
        {
            markedForDeletion.Add(id);
        }

        node.rect = windows[graph.nodes.IndexOf(graph.GetNode(id))];

        Event e = Event.current;

        if (node != tempNode && tempOption != null && e.type == EventType.MouseDown && e.button == 0 && (e.mousePosition.x < node.rect.width && e.mousePosition.y < node.rect.height))
        {
            tempOption.target = id;

            tempNode = null;
            tempOption = null;
        }

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

    [UnityEditor.Callbacks.OnOpenAsset(1)]
    public static bool OnOpenAsset(int instanceID, int line)
    {
        if (Selection.activeObject.GetType() == typeof(TextAsset) && EditorUtility.DisplayDialog(
                "Open in Dialogue Editor?",
                "Any non-dialogue JSON files may cause errors - please make sure this is a dialogue file.",
                "Open in Dialogue Editor", "Open in Text Editor"))
        {
            ShowWindow((TextAsset)Selection.activeObject);

            return true;
        }

        return false;
    }
}
