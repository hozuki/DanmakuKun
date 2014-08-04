using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace DanmakuKun
{
    public static class SnippetListReader
    {

        private const string DefaultListName = "Snippet";

        public static void Read(string filename, IDictionary<string, CompletionList> dict)
        {
            using (var reader = new XmlTextReader(filename))
            {
                try
                {
                    string name;
                    string description;
                    string replacing;
                    SnippetCompletionData snippet;
                    CompletionList list;
                    dict.TryGetValue(DefaultListName, out list);
                    if (list == null)
                    {
                        list = new CompletionList();
                        dict.Add(DefaultListName, list);
                    }
                    reader.ReadStartElement("snippets");
                    while (reader.IsStartElement("snippet"))
                    {
                        name = reader.GetAttribute("name");
                        description = reader.GetAttribute("d");
                        replacing = reader.ReadElementString("snippet");
                        snippet = new SnippetCompletionData(name, description, replacing);
                        list.List.Add(snippet);
                    }
                    reader.ReadEndElement();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.Print(ex.Message);
                }
            }
        }

    }
}
