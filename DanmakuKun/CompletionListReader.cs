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
                        // 默认是动态的，如果不加 static 标签指明的话
                        if (string.IsNullOrEmpty(listStatic) || !bool.Parse(listStatic))
                        {
                            // 加入 $ 以表示这是一个成员列表
                            listName = "$" + listName;
                        }
                        reader.ReadStartElement("list");
                        string name, type, returnType, description, source, modifiers;
                        ItemModifiers mod;
                        CompletionData data;
                        while (reader.IsStartElement("item"))
                        {
                            data = null;
                            name = reader.GetAttribute("name");
                            type = reader.GetAttribute("type");
                            returnType = reader.GetAttribute("return");
                            description = reader.GetAttribute("d");
                            source = reader.GetAttribute("source");
                            modifiers = reader.GetAttribute("modifiers");
                            mod = DV.DefaultModifiers;
                            if (!string.IsNullOrEmpty(modifiers))
                            {
                                mod = (ItemModifiers)Enum.Parse(typeof(ItemModifiers), modifiers, true);
                            }
                            switch (type)
                            {
                                case "function":
                                    data = new FunctionCompletionData(name, returnType, description, source, mod);
                                    break;
                                case "property":
                                    data = new PropertyCompletionData(name, returnType, description, source, mod);
                                    break;
                                case "keyword":
                                    data = new KeywordCompletionData(name, description, source);
                                    break;
                                case "class":
                                    data = new ClassCompletionData(name, description);
                                    break;
                                case "constant":
                                    data = new ConstantCompletionData(name, returnType, description, source);
                                    break;
                                case "field":
                                    data = new FieldCompletionData(name, returnType, description, source, mod);
                                    break;
                                case "objfield":
                                    data = new ObjectFieldCompletionData(name, returnType, description, source, mod);
                                    break;
                                default:
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
