using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueGraph
{
    public string speakerName = "Name";

    public List<DialogueGraphNode> nodes = new List<DialogueGraphNode>();

    [System.Serializable]
    public class DialogueGraphNode
    {
        public int id;

        public string text = "Default dialogue text.";

        public Rect rect = new Rect(0, 0, 300, 200);

        [System.Serializable]
        public class Option
        {
            public string text;
            public int target;

            public Option(string text)
            {
                this.text = text;
                this.target = -1;
            }

            public Option(string text, int target)
            {
                this.text = text;
                this.target = target;
            }
        }

        public List<Option> options = new List<Option>();

        public DialogueGraphNode(int id, string text)
        {
            this.text = text;
            this.id = id;
        }

        public DialogueGraphNode(int id, string text, List<Option> options)
        {
            this.id = id;
            this.text = text;
            this.options = options;
        }
    }

    public DialogueGraphNode GetNode(int id)
    {
        DialogueGraphNode node = null;

        for (int i = 0; i < nodes.Count; i++)
        {
            if (nodes[i].id == id)
                node = nodes[i];
        }

        return node;
    }

    public int GetNewID()
    {
        int id = 0;

        for (int i = 0; i < nodes.Count; i++)
        {
            if (nodes[i].id >= id)
                id = nodes[i].id + 1;
        }

        return id;
    }
}