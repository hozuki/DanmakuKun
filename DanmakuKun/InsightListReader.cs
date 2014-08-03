using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace DanmakuKun
{
    public static class InsightListReader
    {

        public static void Read(string filename, IDictionary<string, FunctionInsightList> dict)
        {
            using (var reader = new XmlTextReader(filename))
            {
                try
                {
                    string funcName;
                    string funcReturn;
                    string funcDescription;
                    string funcSource;
                    string funcReturnDescription;
                    string funcRemarks;
                    string funcMod;
                    FunctionModifiers funcMod_e;
                    string argName;
                    string argType;
                    string argDefaultValue;
                    string argDescription;
                    string argHideInHeader;
                    bool argHideInHeader_b;
                    IList<ArgumentInsightData> args = new List<ArgumentInsightData>();
                    FunctionInsightData func;
                    reader.ReadStartElement("insight");
                    while (reader.IsStartElement("f"))
                    {
                        funcName = reader.GetAttribute("name");
                        funcReturn = reader.GetAttribute("type");
                        funcDescription = reader.GetAttribute("d");
                        funcSource = reader.GetAttribute("source");
                        funcReturnDescription = reader.GetAttribute("return");
                        funcRemarks = reader.GetAttribute("remarks");
                        funcMod = reader.GetAttribute("modifiers");
                        reader.ReadStartElement();
                        args.Clear();
                        while (reader.IsStartElement("a"))
                        {
                            argName = reader.GetAttribute("name");
                            argType = reader.GetAttribute("type");
                            argDefaultValue = reader.GetAttribute("default");
                            argHideInHeader = reader.GetAttribute("hideInHeader");
                            argHideInHeader_b = false;
                            argDescription = reader.GetAttribute("d");
                            if (!string.IsNullOrEmpty(argHideInHeader))
                            {
                                argHideInHeader_b = bool.Parse(argHideInHeader);
                            }
                            args.Add(new ArgumentInsightData(argName, argType, argHideInHeader_b, argDefaultValue, argDescription));
                            reader.ReadElementString();
                        }
                        reader.ReadEndElement();
                        funcMod_e = FunctionModifiers.None;
                        if (!string.IsNullOrEmpty(funcMod))
                        {
                            funcMod_e = (FunctionModifiers)Enum.Parse(typeof(FunctionModifiers), funcMod);
                        }
                        func = new FunctionInsightData(funcName, funcReturn, funcSource, funcDescription, funcMod_e, funcReturnDescription, funcRemarks, args);
                        FunctionInsightList list;
                        dict.TryGetValue(func.Name, out list);
                        if (list == null)
                        {
                            list = new FunctionInsightList();
                            dict.Add(func.Name, list);
                        }
                        list.List.Add(func);
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
