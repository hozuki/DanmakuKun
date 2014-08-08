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
using Jint;
using Jint.Parser;
using Jint.Parser.Ast;

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

        public static DependencyProperty SenserActivatedProperty = DependencyProperty.Register("SenserActive", typeof(bool), typeof(MainWindow));
        public static DependencyProperty CaretLocationProperty = DependencyProperty.Register("CaretLocation", typeof(ICSharpCode.AvalonEdit.Document.TextLocation), typeof(MainWindow));

        public bool SenserActive
        {
            get
            {
                return (bool)GetValue(SenserActivatedProperty);
            }
            set
            {
                SetValue(SenserActivatedProperty, value);
                mnuToolsToggleSenser.IsChecked = value;
            }
        }

        public ICSharpCode.AvalonEdit.Document.TextLocation CaretLocation
        {
            get
            {
                return (ICSharpCode.AvalonEdit.Document.TextLocation)GetValue(CaretLocationProperty);
            }
            set
            {
                editor.TextArea.Caret.Location = value;
                SetValue(CaretLocationProperty, value);
            }
        }

        public MainWindow()
        {
            //splashScreen = new SplashScreen("resources/images/splash.png");
            //splashScreen.Show(false, true);

            InitializeComponent();

            this.Loaded += MainWindow_Loaded;
            this.Closing += MainWindow_Closing;
            editor.TextArea.TextEntered += editor_TextArea_TextEntered;
            editor.TextArea.TextEntering += editor_TextArea_TextEntering;
            editor.TextArea.Caret.PositionChanged += editor_Caret_PositionChanged;
            editor.PreviewKeyDown += editor_KeyDown;
            editor.MouseRightButtonUp += editor_MouseRightButtonUp;
        }

        private void PrivateInitialize()
        {
            sbiCaretPosition.Content = editor.TextArea.Caret.Location;
            editor.ShowLineNumbers = true;
        }

        private void Command_NewFile()
        {
            editor.Document = null;
            editor.Document = new ICSharpCode.AvalonEdit.Document.TextDocument();
        }

        private void Command_OpenFile()
        {
            using (System.Windows.Forms.OpenFileDialog dialog = new System.Windows.Forms.OpenFileDialog())
            {
                dialog.AddExtension = true;
                dialog.DefaultExt = ".js";
                dialog.DereferenceLinks = true;
                dialog.Filter = "ECMA-262 常见脚本 (*.js;*.as;*.mx)|*.js;*.as;*.mx|JavaScript 脚本 (*.js)|*.js|ActionScript 脚本 (*.as;*.mx)|*.as;*.mx|所有文件 (*)|*";
                dialog.FilterIndex = 0;
                dialog.SupportMultiDottedExtensions = true;
                dialog.ValidateNames = true;
                dialog.Multiselect = false;
                dialog.ShowReadOnly = false;
                dialog.CheckFileExists = true;
                if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.Cancel)
                {
                    if (!string.IsNullOrEmpty(dialog.FileName))
                    {
                        editor.Load(dialog.FileName);
                    }
                }
            }
        }

        private void Command_SaveFileAs()
        {
            using (System.Windows.Forms.SaveFileDialog dialog = new System.Windows.Forms.SaveFileDialog())
            {
                dialog.AddExtension = true;
                dialog.DefaultExt = ".js";
                dialog.DereferenceLinks = true;
                dialog.Filter = "ECMA-262 常见脚本 (*.js;*.as;*.mx)|*.js;*.as;*.mx|JavaScript 脚本 (*.js)|*.js|ActionScript 脚本 (*.as;*.mx)|*.as;*.mx|所有文件 (*)|*";
                dialog.FilterIndex = 0;
                dialog.SupportMultiDottedExtensions = true;
                dialog.ValidateNames = true;
                dialog.OverwritePrompt = true;
                if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.Cancel)
                {
                    if (!string.IsNullOrEmpty(dialog.FileName))
                    {
                        editor.Save(dialog.FileName);
                    }
                }
            }
        }

        void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!editor.Document.UndoStack.IsOriginalFile)
            {
                MessageBoxResult result = MessageBoxResult.Cancel;
                result = MessageBox.Show("文档已修改，是否保存？", string.Empty, MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
                switch (result)
                {
                    case MessageBoxResult.Yes:
                        mnuFileSave.Command.Execute(null);
                        if (!editor.Document.UndoStack.IsOriginalFile)
                        {
                            // 如果用户最终取消了保存，那么就还是不关闭窗口
                            e.Cancel = true;
                        }
                        break;
                    case MessageBoxResult.No:
                        break;
                    case MessageBoxResult.Cancel:
                        e.Cancel = true;
                        break;
                }
            }
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
            SenserActive = true;
            editor.Focus();
            ICSharpCode.AvalonEdit.TextEditorOptions options = editor.Options;
            options.EnableHyperlinks = true;
            options.EnableEmailHyperlinks = true;
            options.EnableTextDragDrop = true;
            options.HighlightCurrentLine = true;
            options.IndentationSize = 4;
            options.ConvertTabsToSpaces = true;
            options.WordWrapIndentation = 4;
            editor.TextArea.IndentationStrategy = new ICSharpCode.AvalonEdit.Indentation.CSharp.CSharpIndentationStrategy(options);

            BiliLists.Initialize();
            CompletionItemImages.Initialize();

            CompletionListReader.Read("resources/comp-funcs_props-js.xml", BiliLists.PresetFuncAndProp);
            CompletionListReader.Read("resources/comp-funcs_props.xml", BiliLists.PresetFuncAndProp);
            InsightListReader.Read("resources/insights-js.xml", BiliLists.PresetInsight);
            InsightListReader.Read("resources/insights.xml", BiliLists.PresetInsight);
            CompletionListReader.Read("resources/comp-keywords-js.xml", BiliLists.PresetGlobalStatic);
            CompletionListReader.Read("resources/comp-objfields.xml", BiliLists.PresetGlobalStatic);
            CompletionListReader.Read("resources/comp-classes.xml", BiliLists.PresetGlobalStatic);
            SnippetListReader.Read("resources/comp-snippets.xml", BiliLists.PresetGlobalStatic);
            // 开始首次合并
            ListImmigrator.Concat<CompletionList, CompletionData>(BiliLists.LocalFuncAndProp, BiliLists.PresetFuncAndProp, BiliLists.UserFuncAndProp);
            ListImmigrator.Concat<FunctionInsightList, FunctionInsightData>(BiliLists.LocalInsight, BiliLists.PresetInsight, BiliLists.UserInsight);

            PrivateInitialize();

            //splashScreen.Close(TimeSpan.Zero);
            //splashScreen = null;
        }

        void editor_TextArea_TextEntered(object sender, TextCompositionEventArgs e)
        {
            if (SenserActive)
            {
                // 如果 isInString 成为了常量，请使用 Regex.Match(string, int, int)
                const bool isInString = false;
                string s = null;
                switch (e.Text)
                {
                    case ".":
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
                                    System.Diagnostics.Debug.Print("Dot caller: " + callerObjectFullName);
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
                            break;
                        }
                    case "(":
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
                                Match match;
                                match = RegexHelper.FunctionDefinitionIdentifierRegexRTL.Match(s);
                                if (match.Success)
                                {
                                    // 不要捕获函数定义语句
                                    // 现在的两次判断这个方法有点笨，能用是能用
                                    break;
                                }
                                match = RegexHelper.FunctionCallIdentifierRegexRTL.Match(s);
                                if (match.Success)
                                {
                                    var functionName = match.Value;
                                    functionName = functionName.TrimStart();
                                    if (functionName.Length > 0 && !char.IsLetterOrDigit(functionName[0]) && functionName[0] != '_' && functionName[0] != '$')
                                    {
                                        functionName = functionName.Substring(1, functionName.Length - 1);
                                    }
                                    if (functionName == "function")
                                    {
                                        // 不要捕获匿名函数
                                        break;
                                    }

                                    if (!string.IsNullOrEmpty(functionName))
                                    {
                                        ItemModifiers functionFilter = DV.DefaultModifiers;
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
                                            // 用户自定义的静态全局函数，和成员函数，怎么区分？无法区分。所以不用管这个 filter 了。
                                            //functionFilter = ~FunctionModifiers.Static; // 既然全局静态函数库找不到，那么就不能是静态函数，肯定是成员函数
                                        }
                                        if (funcs != null)
                                        {
                                            if (insightWindow != null)
                                            {
                                                insightWindow.Close();
                                            }
                                            insightWindow = funcs.GetInsightWindow(editor.TextArea, functionFilter);
                                            if (insightWindow != null)
                                            {
                                                insightWindow.Show();
                                            }
                                        }
                                    }
                                }
                            }
                            break;
                        }
                    case ")":
                        {
                            if (insightWindow != null)
                            {
                                insightWindow.Close();
                            }
                            break;
                        }
                    case "@":
                        {
                            if (completionWindow != null)
                            {
                                completionWindow.Close();
                            }
                            completionWindow = new CompletionWindow(editor.TextArea);
                            completionWindow.Closed += (xs, xe) =>
                            {
                                completionWindow = null;
                            };
                            // 这里的替换很特殊，没有修改 completionWindow.StartOffset，而是由 ObjectFieldCompletionData 重写了 Complete() 函数
                            Utils.CombineListsToOne(completionWindow.CompletionList.CompletionData, true,
                                BiliLists.PresetGlobalStatic[DV.ObjectFieldListName]);
                            completionWindow.Show();
                            break;
                        }
                    case "{":
                    case "}":
                    case ";":
                        {
                            editor.TextArea.IndentationStrategy.IndentLine(editor.Document, editor.Document.GetLineByOffset(editor.CaretOffset));
                            break;
                        }
                    default:
                        break;
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
                        return;
                    }
                }
            }
        }

        void editor_KeyDown(object sender, KeyEventArgs e)
        {
            if (SenserActive)
            {
                if (e.Key == Key.Tab && e.KeyboardDevice.Modifiers == ModifierKeys.None)
                {
                    var match = RegexHelper.IdentifierRegexRTL.Match(editor.Text, 0, editor.CaretOffset);
                    if (match.Success)
                    {
                        var probableSnippetName = match.Value;
                        var list = BiliLists.PresetGlobalStatic[DV.SnippetListName];
                        foreach (var item in list)
                        {
                            if (item.Text.Length > 2)
                            {
                                if (item.Text.Substring(0, item.Text.Length - 2) == probableSnippetName)
                                {
                                    var tempCWindow = new CompletionWindow(editor.TextArea);
                                    tempCWindow.StartOffset -= probableSnippetName.Length;
                                    tempCWindow.CompletionList.CompletionData.Add(item);
                                    tempCWindow.CompletionList.ListBox.SelectedIndex = 0;
                                    tempCWindow.CompletionList.RequestInsertion(e);
                                    tempCWindow = null;
                                    e.Handled = true;
                                    break;
                                }
                            }
                        }
                        return;
                    }
                }
            }
        }

        void editor_Caret_PositionChanged(object sender, EventArgs e)
        {
            sbiCaretPosition.Content = editor.TextArea.Caret.Location;
        }

        private void ToolsShowSenserList_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = completionWindow == null;
        }

        private void ToolsShowSenserList_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (completionWindow != null)
            {
                completionWindow.Close();
            }
            completionWindow = new CompletionWindow(editor.TextArea);
            IList<ICompletionData> data = completionWindow.CompletionList.CompletionData;
            Utils.CombineListsToOne(data, true,
                                        BiliLists.PresetGlobalStatic[DV.KeywordListName],
                                        BiliLists.PresetGlobalStatic[DV.SnippetListName],
                                        BiliLists.PresetGlobalStatic[DV.ClassListName]);
            completionWindow.Closed += (xs, xe) =>
            {
                completionWindow = null;
            };
            if (completionWindow != null)
            {
                completionWindow.Show();
            }
        }

        private void FileNew_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void FileNew_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MessageBoxResult result = MessageBoxResult.No;
            if (!editor.Document.UndoStack.IsOriginalFile)
            {
                result = MessageBox.Show("文档已经修改，是否保存？", string.Empty, MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
            }
            switch (result)
            {
                case MessageBoxResult.Yes:
                    mnuFileSave.Command.Execute(null);
                    if (editor.Document.UndoStack.IsOriginalFile)
                    {
                        Command_NewFile();
                    }
                    break;
                case MessageBoxResult.No:
                    Command_NewFile();
                    break;
                case MessageBoxResult.Cancel:
                    break;
            }
        }

        private void FileOpen_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void FileOpen_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MessageBoxResult result = MessageBoxResult.No;
            if (!editor.Document.UndoStack.IsOriginalFile)
            {
                result = MessageBox.Show("文档已经修改，是否保存？", string.Empty, MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
            }
            switch (result)
            {
                case MessageBoxResult.Yes:
                    mnuFileSave.Command.Execute(null);
                    if (editor.Document.UndoStack.IsOriginalFile)
                    {
                        Command_OpenFile();
                    }
                    break;
                case MessageBoxResult.No:
                    Command_OpenFile();
                    break;
                case MessageBoxResult.Cancel:
                    break;
            }
        }

        private void FileSave_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !editor.Document.UndoStack.IsOriginalFile;
        }

        private void FileSave_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (!System.IO.File.Exists(editor.Document.FileName))
            {
                Command_SaveFileAs();
            }
            else
            {
                editor.Save(editor.Document.FileName);
            }
        }

        private void FileSaveAs_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void FileSaveAs_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Command_SaveFileAs();
        }

        private void FileExit_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void FileExit_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
        }

        private void ToolsFormatDocument_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void ToolsFormatDocument_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            // 不提供撤销操作
            ICSharpCode.AvalonEdit.Document.ISegment segment = ICSharpCode.AvalonEdit.Document.TextUtilities.GetLeadingWhitespace(editor.Document, editor.Document.Lines[0]);
            if (segment.Length > 0)
            {
                editor.Document.Remove(segment);
            }
            ICSharpCode.AvalonEdit.Indentation.IIndentationStrategy ind = editor.TextArea.IndentationStrategy;
            if (ind != null)
            {
                ind.IndentLines(editor.Document, 1, editor.LineCount);
            }
        }

        private void ToolsToggleSenser_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void ToolsToggleSenser_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.SenserActive = !this.SenserActive;
        }

        private void ToolsScanSourceCode_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void ToolsScanSourceCode_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            JavaScriptParser parser = new JavaScriptParser();
            Program program;
            sbiStatus.Content = "Scanning...";
            tviDeclarations.Items.Clear();
            tviAst.Items.Clear();
            //try
            {
                program = parser.Parse(editor.Text);
                foreach (var func in program.FunctionDeclarations)
                {
                    AstHelper.ProcessFunctionDeclarations(func, tviDeclarations, editor);
                }
                foreach (var vd in program.VariableDeclarations)
                {
                    AstHelper.ProcessVariableDeclarations(vd, tviDeclarations, editor);
                }
                // Abstract Syntax Tree
                // Not fully implemented yet
                AstHelper.AstProcessProgram(program, tviAst, editor);
            }
            //catch (Exception ex)
            //{
            //    sbiStatus.Content = ex.Message;
            //}
            //finally
            //{
            //    program = null;
            //    parser = null;
            //}
            sbiStatus.Content = string.Empty;
        }

        private void mnuHelpAbout_Click(object sender, RoutedEventArgs e)
        {
            AboutWindow aboutWindow = new AboutWindow();
            aboutWindow.ShowDialog(this);
            aboutWindow = null;
        }

    }
}
