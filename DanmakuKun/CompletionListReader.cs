using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace DanmakuKun
{
    public static class CompletionListReader
    {

        public static void Read(string filename, IDictionary<string, CompletionList> dict)
        {
            using (XmlTextReader reader = new XmlTextReader(filename))
            {
                try
                {
                    string listName;
                    string listStatic;
                    reader.ReadStartElement("completion-lists");
                    while (reader.IsStartElement("list"))
                    {
                        listName = reader.GetAttribute("name");
                        listStatic = reader.GetAttribute("static");
                        if (!bool.Parse(listStatic))
                        {
                            listName = "$" + listName;
                        }
                        reader.ReadStartElement("list");
                        string name, type, returnType, description, source;
                        CompletionData data;
                        while (reader.IsStartElement("item"))
                        {
                            data = null;
                            name = reader.GetAttribute("name");
                            type = reader.GetAttribute("type");
                            returnType = reader.GetAttribute("return");
                            description = reader.GetAttribute("description");
                            source = reader.GetAttribute("source");
                            switch (type)
                            {
                                case "function":
                                    if (string.IsNullOrEmpty(source))
                                    {
                                        if (string.IsNullOrEmpty(description))
                                        {
                                            data = new FunctionCompletionData(name, returnType);
                                        }
                                        else
                                        {
                                            data = new FunctionCompletionData(name, returnType, description);
                                        }
                                    }
                                    else
                                    {
                                        if (string.IsNullOrEmpty(description))
                                        {
                                            data = new WithSourceFunctionCompletionData(name, returnType, source);
                                        }
                                        else
                                        {
                                            data = new WithSourceFunctionCompletionData(name, returnType, source, description);
                                        }
                                    }
                                    break;
                                case "property":
                                    ItemModifiers mod = ItemModifiers.None;
                                    string modifiers;
                                    modifiers = reader.GetAttribute("modifiers");
                                    if (!string.IsNullOrEmpty(modifiers))
                                    {
                                        mod = (ItemModifiers)Enum.Parse(typeof(ItemModifiers), modifiers);
                                    }
                                    if (string.IsNullOrEmpty(source))
                                    {
                                        if (string.IsNullOrEmpty(description))
                                        {
                                            data = new PropertyCompletionData(name, returnType, mod);
                                        }
                                        else
                                        {
                                            data = new PropertyCompletionData(name, returnType, mod, description);
                                        }
                                    }
                                    else
                                    {
                                        if (string.IsNullOrEmpty(description))
                                        {
                                            data = new WithSourcePropertyCompletionData(name, returnType, source, mod);
                                        }
                                        else
                                        {
                                            data = new WithSourcePropertyCompletionData(name, returnType, source, mod, description);
                                        }
                                    }
                                    break;
                                case "keyword":
                                    data = new KeywordCompletionData(name, description, source);
                                    break;
                            }
                            if (data != null)
                            {
                                CompletionList list;
                                dict.TryGetValue(listName, out list);
                                if (list == null)
                                {
                                    list = new CompletionList();
                                    dict.Add(listName, list);
                                }
                                list.List.Add(data);
                            }
                            reader.ReadElementString();
                        }
                        reader.ReadEndElement();
                    }
                    reader.ReadEndElement();
                }
                catch (Exception x)
                {
                    System.Diagnostics.Debug.Print(x.Message);
                }
            }
        }

    }
}
