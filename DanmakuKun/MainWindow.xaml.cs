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
using CompList = System.Collections.Generic.IDictionary<string, DanmakuKun.ObjectList<DanmakuKun.CompletionData>>;
using InsList = System.Collections.Generic.IDictionary<string, DanmakuKun.ObjectList<DanmakuKun.FunctionInsightData>>;

namespace DanmakuKun
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {

        private CompletionWindow completionWindow;
        private InsightWindow insightWindow;
        SplashScreen splashScreen;

        public bool IntellisenseActivated
        {
            get;
            set;
        }

        public MainWindow()
        {
            splashScreen = new SplashScreen("resources/images/splash.png");
            splashScreen.Show(false, true);

            InitializeComponent();

            editor.TextArea.TextEntered += editor_TextArea_TextEntered;
            editor.TextArea.TextEntering += editor_TextArea_TextEntering;
            editor.MouseRightButtonUp += editor_MouseRightButtonUp;
            this.Loaded += MainWindow_Loaded;
        }

        void editor_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (e.RightButton == MouseButtonState.Released)
            {
                ContextMenu contextMenu = Resources["popupMenu"] as ContextMenu;
                if (contextMenu != null)
                {
                    contextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.MousePoint;
                    contextMenu.IsOpen = true;
                }
            }
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            IntellisenseActivated = true;
            editor.Focus();
            editor.SyntaxHighlighting = ICSharpCode.AvalonEdit.Highlighting.HighlightingManager.Instance.GetDefinition("JavaScript");

            //editor.TextArea.Caret.PositionChanged += editor_TextArea_Caret_PositionChanged;

            BiliLists.Initialize();
            CompletionItemImages.Initialize();

            //            Jint.Parser.JavaScriptParser parser = new Jint.Parser.JavaScriptParser(true);
            //            try
            //            {
            //                Jint.Parser.Ast.Program program = parser.Parse(
            //                @"function createArrow(g,width,height,arrow_height)
            // {
            // 	var w=width/2;
            // 	g.graphics.moveTo(w,0);
            // 	g.graphics.lineTo(0,arrow_height);
            // 	g.graphics.lineTo(width,arrow_height);
            // 	g.graphics.lineTo(w,0);
            // 	g.graphics.drawRect(w/2,arrow_height,w,height);
            // }
            //var x = 3;
            //functi.pointofless=4;
            //");
            //            }
            //            catch (Exception ex)
            //            {
            //                System.Diagnostics.Debug.Print(ex.Message);
            //            }

            CompletionListReader.Read("resources/func_prop-js.xml", BiliLists.PresetFuncAndProp);
            CompletionListReader.Read("resources/func_prop.xml", BiliLists.PresetFuncAndProp);
            InsightListReader.Read("resources/insight-js.xml", BiliLists.PresetInsight);
            InsightListReader.Read("resources/insight.xml", BiliLists.PresetInsight);
            // 开始首次合并
            ListImmigrator.Concat<CompletionList, CompletionData>(BiliLists.LocalFuncAndProp, BiliLists.PresetFuncAndProp, BiliLists.UserFuncAndProp);
            ListImmigrator.Concat<FunctionInsightList, FunctionInsightData>(BiliLists.LocalInsight, BiliLists.PresetInsight, BiliLists.UserInsight);

            splashScreen.Close(TimeSpan.Zero);
            splashScreen = null;
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
                        //ident = RegexHelper.DollarOrLeveledIdentifierRegexRTL.Match(s);
                        //if (!ident.Success)
                        //{
                        ident = RegexHelper.DollarOrLeveledIdentifierAndArrayRegexRTL.Match(s);
                        //}
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
                                IList<ICompletionData> data = completionWindow.CompletionList.CompletionData;
                                if (callerObjectFullName == "$")
                                {
                                    //Utils.OneListToAnother(BiliLists.PresetFuncAndProp["Display"], data);
                                    Utils.CombineListsToOne(data, false, BiliLists.LocalFuncAndProp["Display"]);
                                }
                                else if (callerObjectFullName == "$G")
                                {
                                    //Utils.OneListToAnother(BiliLists.PresetFuncAndProp["Global"], data);
                                    Utils.CombineListsToOne(data, false, BiliLists.LocalFuncAndProp["Global"]);
                                }
                                else if (!callerObjectFullName.StartsWith("$") && BiliLists.LocalFuncAndProp.Keys.Contains(callerObjectFullName))
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
                                    //Utils.OneListToAnother(BiliLists.PresetFuncAndProp[callerObjectFullName], data);
                                    Utils.CombineListsToOne(data, false, BiliLists.LocalFuncAndProp[callerObjectFullName]);
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
                                        var match = RegexHelper.LeveledIdentifierAndArrayRegexRTLInString.Match(callerObjectFullName);
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
                                    //Util.OneListToAnother(BiliCompletionLists.Lists["CommentField"], data);
                                    //Util.OneListToAnother(BiliCompletionLists.Lists["Shape"], data);
                                    // 使用排序合并很耗时间，所以如果能确定列表项目类型，就不需要使用
                                    Utils.CombineListsToOne(data, true,
                                        BiliLists.LocalFuncAndProp["$CommentField"],
                                        BiliLists.LocalFuncAndProp["$Shape"],
                                        BiliLists.LocalFuncAndProp["$Object"],
                                        BiliLists.LocalFuncAndProp["$CommentData"],
                                        BiliLists.LocalFuncAndProp["$ITween"],
                                        BiliLists.LocalFuncAndProp["$Timer"],
                                        BiliLists.LocalFuncAndProp["$String"]);
                                    if (isGraphicsObject)
                                    {
                                        //Utils.OneListToAnother(BiliLists.PresetFuncAndProp["$Graphics"], data);
                                        Utils.CombineListsToOne(data, true, BiliLists.LocalFuncAndProp["$Graphics"]);
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
                        //Regex regex = new Regex(@"(?:^|[^\.])\b([a-zA-Z_][\w]*[\s]*)(\.[\s]*[a-zA-Z_][\w]*[\s]*)(\.[\s]*[a-zA-Z_][\w]*[\s]*)?$", RegexOptions.None);
                        var match = RegexHelper.FunctionCallIdentifierRegexRTL.Match(s);
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
                                FunctionModifiers functionFilter = FunctionModifiers.None;
                                if (functionName.StartsWith("$."))
                                {
                                    functionName = "Display." + (functionName.Length > 2 ? functionName.Substring(2, functionName.Length - 2) : string.Empty);
                                }
                                else if (functionName.StartsWith("$G."))
                                {
                                    functionName = "Global." + (functionName.Length > 3 ? functionName.Substring(3, functionName.Length - 3) : string.Empty);
                                }
                                System.Diagnostics.Debug.Print("function: " + functionName);
                                FunctionInsightList funcs;
                                BiliLists.LocalInsight.TryGetValue(functionName, out funcs);
                                if (funcs == null)
                                {
                                    BiliLists.LocalInsight.TryGetValue(functionName, out funcs);
                                }
                                if (funcs == null)
                                {
                                    string shortFunctionName;
                                    int lastDot = functionName.LastIndexOf('.');
                                    if (lastDot < 0)
                                    {
                                        shortFunctionName = functionName;
                                    }
                                    else
                                    {
                                        shortFunctionName = functionName.Substring(lastDot + 1, functionName.Length - lastDot - 1);
                                    }
                                    BiliLists.LocalInsight.TryGetValue(shortFunctionName, out funcs);
                                    functionFilter = ~FunctionModifiers.Static; // 既然全局静态函数库找不到，那么就不能是静态函数，肯定是成员函数
                                }
                                if (funcs != null)
                                {
                                    if (insightWindow != null)
                                    {
                                        insightWindow.Close();
                                    }
                                    insightWindow = funcs.GetInsightWindow(editor.TextArea, functionFilter);
                                    insightWindow.Show();
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
                    if (!char.IsLetterOrDigit(e.Text[0]) && e.Text[0] != '_')
                    {
                        // Whenever a non-letter is typed while the completion window is open,
                        // insert the currently selected element.
                        completionWindow.CompletionList.RequestInsertion(e);
                    }
                }
            }
        }

    }
}
