using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ICSharpCode.AvalonEdit.CodeCompletion;

namespace DanmakuKun
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {

        private CompletionWindow completionWindow;
        private InsightWindow insightWindow;
        public bool IntellisenseActivated { get; set; }
        private int lastLine = -1;

        public MainWindow()
        {
            InitializeComponent();

            editor.TextArea.TextEntered += editor_TextArea_TextEntered;
            editor.TextArea.TextEntering += editor_TextArea_TextEntering;
            this.Loaded += MainWindow_Loaded;
            //editor.TextArea.Caret.PositionChanged += editor_TextArea_Caret_PositionChanged;

            BiliLists.Initialize();
            CompletionItemImages.Initialize();
            RegexHelper.Initialize();

            CompletionListReader.Read("completion.xml", BiliLists.Completion);
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            IntellisenseActivated = true;
            editor.Focus();
            editor.SyntaxHighlighting = ICSharpCode.AvalonEdit.Highlighting.HighlightingManager.Instance.GetDefinition("JavaScript");
        }

        //void editor_TextArea_Caret_PositionChanged(object sender, EventArgs e)
        //{
        //    ICSharpCode.AvalonEdit.Editing.Caret caret = editor.TextArea.Caret;
        //    if (caret.Line != lastLine && insightWindow != null)
        //    {
        //        insightWindow.Close();
        //    }
        //    System.Diagnostics.Debug.Print("Last line: " + lastLine.ToString());
        //    lastLine = caret.Line;
        //}

        void editor_TextArea_TextEntered(object sender, TextCompositionEventArgs e)
        {
            if (IntellisenseActivated)
            {
                // 如果 isInString 成为了常量，请使用 Regex.Match(string, int, int)
                const bool isInString = false;
                string s = null;
                if (string.Compare(e.Text, ".") == 0)
                {
                    s = editor.Text.Substring(0, editor.CaretOffset > 0 ? editor.CaretOffset - 1 : 0);
                    // 还是不要了，类似“"this\\"”这样的字符串无法解决统计问题
                    //isInString = s.CountCharCStyle('"') % 2 == 1 || s.CountCharCStyle('\'') % 2 == 1;
                    if (!isInString)
                    {
                        //System.Diagnostics.Debug.Print(s);
                        Match ident;
                        ident = RegexHelper.DollarOrLeveledIdentifierRegexRTL.Match(s);
                        if (!ident.Success)
                        {
                            ident = RegexHelper.LeveledIdentifierAndArrayRegexRTL.Match(s);
                        }
                        // TODO: 应该再加入类似 (obj.func()).prop 或者 (obj.func1()).func2() 这样的匹配
                        if (ident.Success)
                        {
                            bool showCompletionWindow = true;
                            string callerObjectFullName = ident.Value.Trim();
                            if (callerObjectFullName.Length > 0)
                            {
                                // 为了过滤类似 a+$.fullScreenWidth 这样的字符串
                                if (!char.IsLetterOrDigit(callerObjectFullName[0]) && callerObjectFullName[0] != '_' && callerObjectFullName[0] != '$')
                                {
                                    callerObjectFullName = callerObjectFullName.Substring(1, callerObjectFullName.Length - 1);
                                }
                                // 前面确保了字符串是以字母、数字、$ 开头的
                                if (callerObjectFullName.Length > 1)
                                {
                                    // $:Display $G:Global
                                    if (callerObjectFullName[0] == '$' && callerObjectFullName[1] != '.' && callerObjectFullName[1] != 'G')
                                    {
                                        showCompletionWindow = false;
                                    }
                                }
                            }
                            System.Diagnostics.Debug.Print(callerObjectFullName);
                            //ICSharpCode.AvalonEdit.CodeCompletion.InsightWindow w = new InsightWindow(editor.TextArea);
                            //w.Content= "<b>content</b>\nSo here is the \"Insight\", right?";
                            //w.Show();
                            //w.Closed += (xs, xe) => w = null;
                            //switch (callerObjectFullName)
                            //{
                            //    case "$":
                            //    case "Utils":
                            //    case "Player":
                            //        showCompletionWindow = true;
                            //        break;
                            //    default:
                            //        showCompletionWindow = false;
                            //        break;
                            //}
                            if (showCompletionWindow)
                            {
                                if (completionWindow != null)
                                {
                                    completionWindow.Close();
                                }
                                completionWindow = new CompletionWindow(editor.TextArea);
                                completionWindow.FontFamily = new System.Windows.Media.FontFamily("Courier New");
                                IList<ICompletionData> data = completionWindow.CompletionList.CompletionData;
                                if (callerObjectFullName == "$")
                                {
                                    Utils.OneListToAnother(BiliLists.Completion["Display"], data);
                                }
                                else if (callerObjectFullName == "$G")
                                {
                                    Utils.OneListToAnother(BiliLists.Completion["Global"], data);
                                }
                                else if (!callerObjectFullName.StartsWith("$") && BiliLists.Completion.Keys.Contains(callerObjectFullName))
                                {
                                    //case "Display":
                                    //case "Utils":
                                    //case "Math":
                                    //case "Tween":
                                    //case "Global":
                                    //case "Player":
                                    //case "ScriptManager":
                                    //case "String":
                                    //case "Bitmap":  // External libraries required
                                    //case "Storage": // External libraries required
                                    Utils.OneListToAnother(BiliLists.Completion[callerObjectFullName], data);
                                }
                                else
                                {
                                    bool isGraphicsObject = false;
                                    if (callerObjectFullName.Length > 9)
                                    {
                                        //if (callerObjectFullName.Substring(callerObjectFullName.Length - 9, 9) == ".graphics")
                                        //{
                                        //    isGraphicsObject = true;
                                        //}
                                        var match = RegexHelper.LeveledIdentifierRegexRTLInString.Match(callerObjectFullName);
                                        if (match.Success)
                                        {
                                            // 在 LeveledIdentifierPatternRTLInString v1.0 中会处于最后一个匹配项的最后一个位置
                                            // 由于应用了 right-to-left, 在 LeveledIdentifierPatternRTLInString 为 v1.1 时，
                                            // 如果确实是在访问 Graphics 对象，则"graphics"会处于 Groups[2].Capture[0] 中
                                            //if (match.Groups.Count > 0)
                                            //{
                                            //    var g = match.Groups[match.Groups.Count - 1];
                                            //    if (g.Captures.Count > 0)
                                            //    {
                                            //        //if (g.Captures[g.Captures.Count - 1].Value == "graphics")
                                            //        if (g.Captures[0].Value == "graphics")
                                            //        {
                                            //            isGraphicsObject = true;
                                            //        }
                                            //    }
                                            //}
                                            if (match.Groups.Count > 2)
                                            {
                                                var g = match.Groups[2];
                                                if (g.Captures.Count > 0)
                                                {
                                                    isGraphicsObject = g.Captures[0].Value == "graphics";
                                                }
                                            }
                                        }
                                    }
                                    if (isGraphicsObject)
                                    {
                                        Utils.OneListToAnother(BiliLists.Completion["$Graphics"], data);
                                    }
                                    else
                                    {
                                        //Util.OneListToAnother(BiliCompletionLists.Lists["CommentField"], data);
                                        //Util.OneListToAnother(BiliCompletionLists.Lists["Shape"], data);
                                        // 使用排序合并很耗时间，所以如果能确定列表项目类型，就不需要使用
                                        Utils.CombineListsToOne(data, true,
                                            BiliLists.Completion["$CommentField"],
                                            BiliLists.Completion["$Shape"],
                                            BiliLists.Completion["$Object"],
                                            BiliLists.Completion["$CommentData"],
                                            BiliLists.Completion["$ITween"],
                                            BiliLists.Completion["$Timer"],
                                            BiliLists.Completion["$String"]);
                                    }
                                }
                                completionWindow.SizeToContent = System.Windows.SizeToContent.WidthAndHeight;
                                //completionWindow.SizeToContent = System.Windows.SizeToContent.Manual;
                                completionWindow.Closed += (xsender, xe) => completionWindow = null;
                                completionWindow.Show();
                            }
                        }
                    }
                }
                else if (string.Compare(e.Text, "(") == 0)
                {
                    s = editor.Text.Substring(0, editor.CaretOffset > 0 ? editor.CaretOffset - 1 : 0);
                    //isInString = s.CountCharCStyle('"') % 2 == 1 || s.CountCharCStyle('\'') % 2 == 1;
                    if (!isInString)
                    {
                        // 为了捕获两个或者三个 Group，只好采用现在的写法
                        // 注意形如 "([a-zA-Z_][\w]*)[\s]*)" 这个捕获是专门为了提取而设置的，请在之后的代码中遍历所有捕获结果
                        // 暂时未捕获类似 $.createBlurFilter() 这样的形式 - 2014.08.01
                        //Regex regex = new Regex(@"(?:^|[^\.])[^\.]*\b(?:(\$|[a-zA-Z_][\w]*)[\s]*)(?:\.[\s]*([a-zA-Z_][\w]*)[\s]*)*$", RegexOptions.None);
                        Regex regex = new Regex(@"(?:^|\b|[^\w\$]?)(?:(?:\$[G]?|[a-zA-Z_][\w]*)[\s]*)(?:\.[\s]*([a-zA-Z_][\w]*)[\s]*)*$", RegexOptions.None);
                        //Regex regex = new Regex(@"(?:^|[^\.])\b([a-zA-Z_][\w]*[\s]*)(\.[\s]*[a-zA-Z_][\w]*[\s]*)(\.[\s]*[a-zA-Z_][\w]*[\s]*)?$", RegexOptions.None);
                        var match = regex.Match(s);
                        if (match.Success)
                        {
                            //    System.Diagnostics.Debug.Print("Capture count: " + match.Captures.Count.ToString());
                            //    System.Diagnostics.Debug.Print("Group count: " + match.Groups.Count.ToString());

                            //int i = 0;
                            //int j = 0;
                            //foreach (Group item in match.Groups)
                            //{
                            //    j = 0;
                            //    foreach (Capture cap in item.Captures)
                            //    {
                            //        System.Diagnostics.Debug.Print("G" + i.ToString() + "C" + j.ToString() + " " + cap.Value);
                            //        j++;
                            //    }
                            //    i++;
                            //}

                            //string functionName = null;
                            //if (match.Groups.Count > 2 && match.Groups[2].Captures.Count > 0)
                            //{
                            //    functionName = match.Groups[2].Captures[match.Groups[2].Captures.Count - 1].Value;
                            //}
                            var functionName = match.Value;
                            functionName = functionName.TrimStart();
                            if (functionName.Length > 0 && !char.IsLetterOrDigit(functionName[0]) && functionName[0] != '_' && functionName[0] != '$')
                            {
                                functionName = functionName.Substring(1, functionName.Length - 1);
                            }

                            if (!string.IsNullOrEmpty(functionName))
                            {
                                if (functionName.StartsWith("$."))
                                {
                                    functionName = "Display." + (functionName.Length > 2 ? functionName.Substring(2, functionName.Length - 2) : string.Empty);
                                }
                                else if (functionName.StartsWith("$G."))
                                {
                                    functionName = "Global." + (functionName.Length > 3 ? functionName.Substring(3, functionName.Length - 3) : string.Empty);
                                }
                                System.Diagnostics.Debug.Print("function: " + functionName);
                                IDictionary<string, FunctionInsightData> lists;
                                FunctionInsightData func;
                                BiliLists.Function.TryGetValue("$", out lists);
                                if (lists != null)
                                {
                                    lists.TryGetValue(functionName, out func);
                                    if (func != null)
                                    {
                                        if (insightWindow != null)
                                        {
                                            insightWindow.Close();
                                        }
                                        insightWindow = new InsightWindow(editor.TextArea);
                                        //TextBlock txt = new TextBlock();
                                        //txt.FontSize = 12;
                                        //txt.Inlines.Add("This is some ");
                                        //txt.Inlines.Add(new Italic(new Run("italic")));
                                        //txt.Inlines.Add(" text, and this is some ");
                                        //txt.Inlines.Add(new Bold(new Run("bold")));
                                        //txt.Inlines.Add(" text, and let's cap it off with some ");
                                        //txt.Inlines.Add(new Bold(new Italic(new Run("bold italic"))));
                                        //txt.Inlines.Add(" text.");
                                        //txt.TextWrapping = TextWrapping.Wrap;
                                        //inw.Content = txt;
                                        insightWindow.Content = func.ToContent();
                                        insightWindow.Closed += delegate
                                        {
                                            insightWindow = null;
                                            //txt = null;
                                        };
                                        insightWindow.Show();
                                    }
                                }
                            }
                        }
                    }
                }
                else if (string.Compare(e.Text, ")") == 0)
                {
                    if (insightWindow != null)
                    {
                        insightWindow.Close();
                    }
                }
                if (s != null)
                {
                    s = null;
                }
            }
        }

        void editor_TextArea_TextEntering(object sender, TextCompositionEventArgs e)
        {
            if (e.Text.Length > 0)
            {
                if (completionWindow != null)
                {
                    if (!char.IsLetterOrDigit(e.Text[0]) && e.Text[0] != '@')
                    {
                        // Whenever a non-letter is typed while the completion window is open,
                        // insert the currently selected element.
                        completionWindow.CompletionList.RequestInsertion(e);
                    }
                }
                //else
                //{
                //    //string s = editor.Text.Substring(0, editor.CaretOffset > 0 ? editor.CaretOffset - 1 : 0);
                //    //bool isInString = s.CountChar('"') % 2 == 1;
                //    //if (!isInString && (char.IsLetter(e.Text[0]) || (s.Length > 0 && char.IsLetterOrDigit(s[s.Length - 1]))))
                //    //if (!isInString && char.IsLetter(e.Text[0]))
                //    if (!char.IsDigit(e.Text[0]) && !char.IsWhiteSpace(e.Text[0]) && !char.IsPunctuation(e.Text[0]))
                //    {
                //        completionWindow = new CompletionWindow(editor.TextArea);
                //        IList<ICompletionData> data = completionWindow.CompletionList.CompletionData;
                //        data.Add(new KeywordCompletionData("this"));
                //        data.Add(new KeywordCompletionData("null"));
                //        data.Add(new KeywordCompletionData("var"));
                //        data.Add(new KeywordCompletionData("int"));
                //        data.Add(new KeywordCompletionData("char"));
                //        data.Add(new KeywordCompletionData("string"));
                //        data.Add(new KeywordCompletionData("float"));
                //        data.Add(new KeywordCompletionData("double"));
                //        data.Add(new KeywordCompletionData("long"));
                //        data.Add(new KeywordCompletionData("new"));
                //        data.Add(new KeywordCompletionData("object"));
                //        data.Add(new KeywordCompletionData("interface"));
                //        data.Add(new KeywordCompletionData("class"));
                //        completionWindow.Closed += (xsender, xe) => completionWindow = null;
                //        completionWindow.Show();
                //    }
                //    //s = null;
                //}
            }
        }
    }
}
