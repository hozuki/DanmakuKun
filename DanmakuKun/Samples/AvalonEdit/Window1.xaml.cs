// Copyright (c) 2009 Daniel Grunwald
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using System.Xml;

using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Highlighting;
using Microsoft.Win32;

namespace DanmakuKun.AvalonEdit.Sample
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
            propertyGridComboBox.SelectedIndex = 2;

            //textEditor.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("C#");
            //textEditor.SyntaxHighlighting = customHighlighting;
            // initial highlighting now set by XAML

            textEditor.TextArea.TextEntering += textEditor_TextArea_TextEntering;
            textEditor.TextArea.TextEntered += textEditor_TextArea_TextEntered;

            DispatcherTimer foldingUpdateTimer = new DispatcherTimer();
            foldingUpdateTimer.Interval = TimeSpan.FromSeconds(2);
            foldingUpdateTimer.Tick += foldingUpdateTimer_Tick;
            foldingUpdateTimer.Start();
        }

        string currentFileName;

        void openFileClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.CheckFileExists = true;
            if (dlg.ShowDialog() ?? false)
            {
                currentFileName = dlg.FileName;
                textEditor.Load(currentFileName);
                textEditor.SyntaxHighlighting = HighlightingManager.Instance.GetDefinitionByExtension(Path.GetExtension(currentFileName));
            }
        }

        void saveFileClick(object sender, EventArgs e)
        {
            if (currentFileName == null)
            {
                SaveFileDialog dlg = new SaveFileDialog();
                dlg.DefaultExt = ".txt";
                if (dlg.ShowDialog() ?? false)
                {
                    currentFileName = dlg.FileName;
                }
                else
                {
                    return;
                }
            }
            textEditor.Save(currentFileName);
        }

        void exitClick(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        void propertyGridComboBoxSelectionChanged(object sender, RoutedEventArgs e)
        {
            if (propertyGrid == null)
                return;
            switch (propertyGridComboBox.SelectedIndex)
            {
                case 0:
                    propertyGrid.SelectedObject = textEditor;
                    break;
                case 1:
                    propertyGrid.SelectedObject = textEditor.TextArea;
                    break;
                case 2:
                    propertyGrid.SelectedObject = textEditor.Options;
                    break;
            }
        }

        CompletionWindow completionWindow;

        private int CountQuotes(string content)
        {
            if (string.IsNullOrEmpty(content))
            {
                return 0;
            }
            int r = 0;
            foreach (var c in content)
            {
                if (c == '"') r++;
            }
            return r;
        }

        void textEditor_TextArea_TextEntered(object sender, TextCompositionEventArgs e)
        {
            if (e.Text.CompareTo(".") == 0)
            {
                var caretOffset = textEditor.CaretOffset;
                // Matches: identifier | string (C# style)
                const string matchPattern = @"("
                                            + @"(\b[@]?[a-zA-Z_]([\.]?[a-zA-Z_][\w]*)*((<[a-zA-Z_]([\.]?[a-zA-Z_][\w]*)*>)|(\[[\S]+\]))*)" + "|"   // identifier ("\b([@]?[a-zA-Z_]([\.]?[\w])*)"), plus generic or array
                                            + @"(\b[a-zA-Z_](([\.]?[a-zA-Z_][\w]*)*)((<[a-zA-Z_]([\.]?[a-zA-Z_][\w]*)*)>)?\([\S]*\)(\[[\S]]+\])*)" + "|"   //// function: my.func(params)[index] <= bugs in indexer-descripter. Input like "my.func(12)[42]" is valid, but "my.func(12)[42](2)" is not.
                                            + @"([@]?""[\s\S]*"")"  // string
                                            + @")$";
                //var regex = new System.Text.RegularExpressions.Regex(@"(\b(([@]?[a-zA-Z_]([\.]?[\w])*)(([<][a-zA-Z_]([\.]?[\w])*)[>])?)|([@]?""[\s\S]*""))$", System.Text.RegularExpressions.RegexOptions.RightToLeft | System.Text.RegularExpressions.RegexOptions.Compiled | System.Text.RegularExpressions.RegexOptions.ExplicitCapture);
                var regex = new System.Text.RegularExpressions.Regex(matchPattern, System.Text.RegularExpressions.RegexOptions.RightToLeft | System.Text.RegularExpressions.RegexOptions.Compiled | System.Text.RegularExpressions.RegexOptions.ExplicitCapture);
                // 敲入"."之后，至少有一个字符，放心地用 caretOffset - 1
                var contentBefore = textEditor.Text.Substring(0, caretOffset - 1);
                var quotesBefore = CountQuotes(contentBefore);
                var match = regex.Match(contentBefore);
                if (match != null && !string.IsNullOrEmpty(match.Value) && (quotesBefore % 2 == 0))
                {
                    System.Diagnostics.Debug.Print(match.Value);

                    // open code completion after the user has pressed dot:
                    completionWindow = new CompletionWindow(textEditor.TextArea);
                    // provide AvalonEdit with the data:
                    IList<ICompletionData> data = completionWindow.CompletionList.CompletionData;
                    data.Add(new MyCompletionData("Method1"));
                    data.Add(new MyCompletionData("Method2"));
                    data.Add(new MyCompletionData("Method3"));
                    data.Add(new MyCompletionData("Property1"));
                    completionWindow.Show();
                    completionWindow.Closed += delegate
                    {
                        completionWindow = null;
                    };
                }
                if (contentBefore.Length > 5120)
                {
                    // Free the string if it is TOO long
                    contentBefore = null;
                }
            }
        }

        void textEditor_TextArea_TextEntering(object sender, TextCompositionEventArgs e)
        {
            if (e.Text.Length > 0 && completionWindow != null)
            {
                if (!char.IsLetterOrDigit(e.Text[0]))
                {
                    // Whenever a non-letter is typed while the completion window is open,
                    // insert the currently selected element.
                    completionWindow.CompletionList.RequestInsertion(e);
                }
            }
            // do not set e.Handled=true - we still want to insert the character that was typed
        }

        #region Folding
        FoldingManager foldingManager;
        //AbstractFoldingStrategy foldingStrategy;

        void HighlightingComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (textEditor.SyntaxHighlighting == null)
            {
                //foldingStrategy = null;
            }
            else
            {
                switch (textEditor.SyntaxHighlighting.Name)
                {
                    case "XML":
                        //foldingStrategy = new XmlFoldingStrategy();
                        textEditor.TextArea.IndentationStrategy = new ICSharpCode.AvalonEdit.Indentation.DefaultIndentationStrategy();
                        break;
                    case "C#":
                    case "C++":
                    case "PHP":
                    case "Java":
                        textEditor.TextArea.IndentationStrategy = new ICSharpCode.AvalonEdit.Indentation.CSharp.CSharpIndentationStrategy(textEditor.Options);
                        //foldingStrategy = new BraceFoldingStrategy();
                        break;
                    default:
                        textEditor.TextArea.IndentationStrategy = new ICSharpCode.AvalonEdit.Indentation.DefaultIndentationStrategy();
                        //foldingStrategy = null;
                        break;
                }
            }
            //if (foldingStrategy != null)
            //{
            //    if (foldingManager == null)
            //        foldingManager = FoldingManager.Install(textEditor.TextArea);
            //    foldingStrategy.UpdateFoldings(foldingManager, textEditor.Document);
            //}
            //else
            //{
            //    if (foldingManager != null)
            //    {
            //        FoldingManager.Uninstall(foldingManager);
            //        foldingManager = null;
            //    }
            //}
        }

        void foldingUpdateTimer_Tick(object sender, EventArgs e)
        {
            //if (foldingStrategy != null)
            //{
            //    foldingStrategy.UpdateFoldings(foldingManager, textEditor.Document);
            //}
        }
        #endregion
    }
}