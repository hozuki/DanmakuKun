using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace DanmakuKun
{
    public static class SnippetListReader
    {

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
                    dict.TryGetValue(DV.SnippetListName, out list);
                    if (list == null)
                    {
                        list = new CompletionList();
                        dict.Add(DV.SnippetListName, list);
                    }
                    reader.ReadStartElement("snippets");
                    while (reader.IsStartElement("snippet"))
                    {
                        name = reader.GetAttribute("name");
                        description = reader.GetAttribute("d");
                        replacing = reader.ReadElementString("snippet");
                        replacing = replacing.Trim();
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
