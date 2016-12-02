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
        public string text = "Default dialogue text.";

        public Rect rect = new Rect(0, 0, 300, 200);

        [System.Serializable]
        public class Option
        {
            public string text;
            public int target;

            public Option(string text, int target)
            {
                this.text = text;
                this.target = target;
            }
        }

        public List<Option> options = new List<Option>();

        public DialogueGraphNode(string text)
        {
            this.text = text;
        }

        public DialogueGraphNode(string text, List<Option> options)
        {
            this.text = text;
            this.options = options;
        }
    }
}