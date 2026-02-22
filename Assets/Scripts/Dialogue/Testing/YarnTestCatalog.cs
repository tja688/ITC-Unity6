using System;
using System.Collections.Generic;
using UnityEngine;

namespace ITC.Dialogue.Testing
{
    [Serializable]
    public class YarnTestFileInfo
    {
        public string yarnFileName;
        public string startNode;
        public string endNode;
        [TextArea(2, 5)]
        public string description;
        public string commandsUsed;
        public List<string> variableNodes = new List<string>();
    }

    [CreateAssetMenu(fileName = "YarnTestCatalog", menuName = "ITC/Dialogue/Yarn Test Catalog")]
    public class YarnTestCatalog : ScriptableObject
    {
        public List<YarnTestFileInfo> files = new List<YarnTestFileInfo>();

        public List<string> GetAllJumpableNodes()
        {
            var list = new List<string>();
            foreach (var f in files)
            {
                if (!string.IsNullOrEmpty(f.startNode) && !list.Contains(f.startNode))
                {
                    list.Add(f.startNode);
                }

                if (f.variableNodes != null)
                {
                    foreach (var vNode in f.variableNodes)
                    {
                        if (!string.IsNullOrEmpty(vNode) && !list.Contains(vNode))
                            list.Add(vNode);
                    }
                }
            }
            return list;
        }
    }
}
