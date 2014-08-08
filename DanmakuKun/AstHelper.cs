using System;
using System.Linq;
using System.Windows.Input;
using System.Windows.Controls;
using Jint;
using Jint.Parser;
using Jint.Parser.Ast;
using ICSharpCode.AvalonEdit;

namespace DanmakuKun
{
    public static class AstHelper
    {

        static void treeViewNode_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            //TreeViewItem tvi = sender as TreeViewItem;
            //if (tvi != null)
            //{
            //    var offset = tvi.Tag as int?;
            //    if (offset.HasValue)
            //    {
            //        var location = editor.Document.GetLocation(offset.Value);
            //        editor.ScrollTo(location.Line, location.Column);
            //        editor.TextArea.Caret.Line = location.Line;
            //        editor.TextArea.Caret.Column = location.Column;
            //        editor.Focus();
            //    }
            //}
        }

        public static void ProcessFunctionDeclarations(IFunctionDeclaration func, TreeViewItem tvi, TextEditor editor)
        {
            if (func != null)
            {
                var t = new TreeViewItem();
                t.Header = GetFuncStringExpression(func);
                var funcSeg = new ICSharpCode.AvalonEdit.Document.TextSegment();
                funcSeg.StartOffset = func.Body.Range[0];
                funcSeg.EndOffset = func.Body.Range[1];
                var ti = new TreeViewItem();
                var tic = new TreeViewItem();
                ti.Header = "(语句)";
                tic.Header = editor.Document.GetText(funcSeg);
                ti.Items.Add(tic);
                t.Items.Add(ti);
                tvi.Items.Add(t);
                foreach (var f in func.FunctionDeclarations)
                {
                    ProcessFunctionDeclarations(f, t, editor);
                }
                foreach (var vd in func.VariableDeclarations)
                {
                    ProcessVariableDeclarations(vd, t, editor);
                }
            }
        }

        public static void ProcessVariableDeclarations(VariableDeclaration vd, TreeViewItem tvi, TextEditor editor)
        {
            if (vd != null)
            {
                TreeViewItem t;
                string s;

                foreach (var item in vd.Declarations)
                {
                    t = new TreeViewItem();
                    s = vd.Kind + " ";
                    s += item.Id.Name;
                    var initText = GetSyntaxNodeText(item.Init, editor.Document, editor);
                    if (!string.IsNullOrEmpty(initText))
                    {
                        if (initText.StartsWith("="))
                        {
                            initText = " = " + initText.Substring(1, initText.Length - 1);
                        }
                        else
                        {
                            initText = " = " + initText.Trim();
                        }
                        s += initText;
                    }
                    t.Header = s;
                    tvi.Items.Add(t);
                }
            }
        }

        private static string GetSyntaxNodeText(SyntaxNode node, ICSharpCode.AvalonEdit.Document.TextDocument document, TextEditor editor)
        {
            if (node != null)
            {
                var segment = new ICSharpCode.AvalonEdit.Document.TextSegment();
                segment.StartOffset = editor.Document.GetOffset(node.Location.Start.Line, node.Location.Start.Column);
                segment.EndOffset = editor.Document.GetOffset(node.Location.End.Line, node.Location.End.Column);
                segment.EndOffset++;
                var initText = editor.Document.GetText(segment);
                return initText;
            }
            else
            {
                return "#ERROR";
            }
        }

        private static void ProcessCommentDeclarations(Comment comment, TreeViewItem tvi)
        {
            if (comment != null)
            {
                TreeViewItem t = new TreeViewItem();
                string s;
                s = "[" + comment.Range[0].ToString();
                s += "," + comment.Range[1].ToString() + "] ";
                s += comment.Value;
                t.Header = s;
                tvi.Items.Add(t);
            }
        }

        private static string GetFuncStringExpression(IFunctionDeclaration func)
        {
            if (func != null)
            {
                string ret = "function " + func.Id.Name + "(";
                var len = func.Parameters.Count();
                int i = 0;
                foreach (var param in func.Parameters)
                {
                    ret += param.Name;
                    if (i < len - 1)
                    {
                        ret += ", ";
                    }
                    i++;
                }
                ret += ")";
                return ret;
            }
            else
            {
                return "#ERROR";
            }
        }

        public static void AstProcessProgram(Program program, TreeViewItem tvi, TextEditor editor)
        {
            if (program != null)
            {
                //foreach (var func in program.FunctionDeclarations)
                //{
                //    AstProcessFunction(func, tvi);
                //}
                //foreach (var vd in program.VariableDeclarations)
                //{
                //    AstProcessVariable(vd, tvi);
                //}
                foreach (var stmt in program.Body)
                {
                    AstProcessStatement(stmt, tvi, editor);
                }
            }
        }

        private static void AstProcessFunction(FunctionDeclaration func, TreeViewItem tvi, TextEditor editor)
        {
            if (func != null)
            {
                TreeViewItem t = new TreeViewItem();
                tvi.Items.Add(t);
                t.Header = "function " + func.Id.Name;
                AstProcessStatement(func.Body, t, editor);
                foreach (var f in func.FunctionDeclarations)
                {
                    AstProcessFunction(f, t, editor);
                }
            }
        }

        private static void AstProcessVariable(VariableDeclaration vd, TreeViewItem tvi)
        {
        }

        private static void AstProcessStatement(Statement st, TreeViewItem tvi, TextEditor editor)
        {
            TreeViewItem t, t1, t2, t3;
            if (st != null)
            {
                switch (st.Type)
                {
                    case SyntaxNodes.BlockStatement:
                        t = new TreeViewItem();
                        tvi.Items.Add(t);
                        t.Header = "Block Statement";
                        foreach (var item in (st as BlockStatement).Body)
                        {
                            AstProcessStatement(item, t, editor);
                        }
                        if (st.Range != null)
                        {
                            t.Tag = st.Range[0];
                        }
                        else
                        {
                            t.Tag = editor.Document.GetOffset(st.Location.Start.Line, st.Location.Start.Column);
                        }
                        t.MouseRightButtonDown += treeViewNode_MouseRightButtonDown;
                        break;
                    case SyntaxNodes.BreakStatement:
                        t = new TreeViewItem();
                        tvi.Items.Add(t);
                        t.Header = "Break Statement, label = " + (st as BreakStatement).Label;
                        if (st.Range != null)
                        {
                            t.Tag = st.Range[0];
                        }
                        else
                        {
                            t.Tag = editor.Document.GetOffset(st.Location.Start.Line, st.Location.Start.Column);
                        }
                        t.MouseRightButtonDown += treeViewNode_MouseRightButtonDown;
                        break;
                    case SyntaxNodes.CatchClause:
                        t = new TreeViewItem();
                        tvi.Items.Add(t);
                        t.Header = "Catch Clause, param = " + (st as CatchClause).Param.Name;
                        AstProcessStatement((st as CatchClause).Body, t, editor);
                        if (st.Range != null)
                        {
                            t.Tag = st.Range[0];
                        }
                        else
                        {
                            t.Tag = editor.Document.GetOffset(st.Location.Start.Line, st.Location.Start.Column);
                        }
                        t.MouseRightButtonDown += treeViewNode_MouseRightButtonDown;
                        break;
                    case SyntaxNodes.ContinueStatement:
                        t = new TreeViewItem();
                        tvi.Items.Add(t);
                        t.Header = "Continue Statement, label = " + (st as ContinueStatement).Label;
                        if (st.Range != null)
                        {
                            t.Tag = st.Range[0];
                        }
                        else
                        {
                            t.Tag = editor.Document.GetOffset(st.Location.Start.Line, st.Location.Start.Column);
                        }
                        t.MouseRightButtonDown += treeViewNode_MouseRightButtonDown;
                        break;
                    case SyntaxNodes.DebuggerStatement:
                        t = new TreeViewItem();
                        tvi.Items.Add(t);
                        t.Header = "Debugger Statement";
                        if (st.Range != null)
                        {
                            t.Tag = st.Range[0];
                        }
                        else
                        {
                            t.Tag = editor.Document.GetOffset(st.Location.Start.Line, st.Location.Start.Column);
                        }
                        t.MouseRightButtonDown += treeViewNode_MouseRightButtonDown;
                        break;
                    case SyntaxNodes.DoWhileStatement:
                        t = new TreeViewItem();
                        tvi.Items.Add(t);
                        t.Header = "DoWhile Statement";
                        AstProcessExpression((st as DoWhileStatement).Test, t, editor);
                        AstProcessStatement((st as DoWhileStatement).Body, t, editor);
                        if (st.Range != null)
                        {
                            t.Tag = st.Range[0];
                        }
                        else
                        {
                            t.Tag = editor.Document.GetOffset(st.Location.Start.Line, st.Location.Start.Column);
                        }
                        t.MouseRightButtonDown += treeViewNode_MouseRightButtonDown;
                        break;
                    case SyntaxNodes.EmptyStatement:
                        t = new TreeViewItem();
                        tvi.Items.Add(t);
                        t.Header = "(Empty Statement)";
                        if (st.Range != null)
                        {
                            t.Tag = st.Range[0];
                        }
                        else
                        {
                            t.Tag = editor.Document.GetOffset(st.Location.Start.Line, st.Location.Start.Column);
                        }
                        t.MouseRightButtonDown += treeViewNode_MouseRightButtonDown;
                        break;
                    case SyntaxNodes.ExpressionStatement:
                        t = new TreeViewItem();
                        tvi.Items.Add(t);
                        t.Header = "Expression Statement";
                        AstProcessExpression((st as ExpressionStatement).Expression, t, editor);
                        if (st.Range != null)
                        {
                            t.Tag = st.Range[0];
                        }
                        else
                        {
                            t.Tag = editor.Document.GetOffset(st.Location.Start.Line, st.Location.Start.Column);
                        }
                        t.MouseRightButtonDown += treeViewNode_MouseRightButtonDown;
                        break;
                    case SyntaxNodes.ForInStatement:
                        t = new TreeViewItem();
                        tvi.Items.Add(t);
                        if ((st as ForInStatement).Each)
                        {
                            t.Header = "ForEach (...In) Statement";
                        }
                        else
                        {
                            t.Header = "ForEach Statement";
                        }
                        t1 = new TreeViewItem();
                        t.Items.Add(t1);
                        t1.Header = "[Init]";
                        if ((st as ForStatement).Init is Statement)
                        {
                            AstProcessStatement((st as ForStatement).Init as Jint.Parser.Ast.Statement, t1, editor);
                        }
                        else if ((st as ForStatement).Init is Jint.Parser.Ast.Expression)
                        {
                            AstProcessExpression((st as ForStatement).Init as Jint.Parser.Ast.Expression, t1, editor);
                        }
                        t2 = new TreeViewItem() { Header = "[Right]" };
                        t.Items.Add(t2);
                        AstProcessExpression((st as ForInStatement).Right, t2, editor);
                        AstProcessStatement((st as ForInStatement).Body, t, editor);
                        if (st.Range != null)
                        {
                            t.Tag = st.Range[0];
                        }
                        else
                        {
                            t.Tag = editor.Document.GetOffset(st.Location.Start.Line, st.Location.Start.Column);
                        }
                        t.MouseRightButtonDown += treeViewNode_MouseRightButtonDown;
                        break;
                    case SyntaxNodes.ForStatement:
                        t = new TreeViewItem();
                        tvi.Items.Add(t);
                        t.Header = "For Statement";
                        t1 = new TreeViewItem();
                        t.Items.Add(t1);
                        t1.Header = "[Init]";
                        if ((st as ForStatement).Init is Statement)
                        {
                            AstProcessStatement((st as ForStatement).Init as Jint.Parser.Ast.Statement, t1, editor);
                        }
                        else if ((st as ForStatement).Init is Jint.Parser.Ast.Expression)
                        {
                            AstProcessExpression((st as ForStatement).Init as Jint.Parser.Ast.Expression, t1, editor);
                        }
                        t2 = new TreeViewItem();
                        t.Items.Add(t2);
                        t2.Header = "[Test]";
                        AstProcessExpression((st as ForStatement).Test, t2, editor);
                        t3 = new TreeViewItem();
                        t.Items.Add(t3);
                        t3.Header = "[Update]";
                        AstProcessExpression((st as ForStatement).Update, t3, editor);
                        AstProcessStatement((st as ForStatement).Body, t, editor);
                        if (st.Range != null)
                        {
                            t.Tag = st.Range[0];
                        }
                        else
                        {
                            t.Tag = editor.Document.GetOffset(st.Location.Start.Line, st.Location.Start.Column);
                        }
                        t.MouseRightButtonDown += treeViewNode_MouseRightButtonDown;
                        break;
                    case SyntaxNodes.FunctionDeclaration:
                        t = new TreeViewItem();
                        tvi.Items.Add(t);
                        if ((st as FunctionDeclaration).Id != null)
                        {
                            t.Header = "Function Declaration (" + (st as FunctionDeclaration).Id.Name + ")";
                        }
                        else
                        {
                            t.Header = "Function Declaration (#Lambda)";
                        }
                        t1 = new TreeViewItem();
                        t.Items.Add(t1);
                        t1.Header = "[Defaults]";
                        foreach (var item in (st as FunctionDeclaration).Defaults)
                        {
                            AstProcessExpression(item, t1, editor);
                        }
                        t2 = new TreeViewItem();
                        t.Items.Add(t2);
                        t2.Header = "[Body]";
                        AstProcessStatement((st as FunctionDeclaration).Body, t2, editor);
                        if (st.Range != null)
                        {
                            t.Tag = st.Range[0];
                        }
                        else
                        {
                            t.Tag = editor.Document.GetOffset(st.Location.Start.Line, st.Location.Start.Column);
                        }
                        t.MouseRightButtonDown += treeViewNode_MouseRightButtonDown;
                        break;
                    case SyntaxNodes.IfStatement:
                        t = new TreeViewItem();
                        tvi.Items.Add(t);
                        t.Header = "If Statement";
                        t1 = new TreeViewItem();
                        t.Items.Add(t1);
                        t1.Header = "[Test]";
                        AstProcessExpression((st as IfStatement).Test, t1, editor);
                        t2 = new TreeViewItem();
                        t.Items.Add(t2);
                        t2.Header = "[Consequence]";
                        AstProcessStatement((st as IfStatement).Consequent, t2, editor);
                        t3 = new TreeViewItem();
                        t.Items.Add(t3);
                        t2.Header = "[Alternate]";
                        AstProcessStatement((st as IfStatement).Alternate, t3, editor);
                        if (st.Range != null)
                        {
                            t.Tag = st.Range[0];
                        }
                        else
                        {
                            t.Tag = editor.Document.GetOffset(st.Location.Start.Line, st.Location.Start.Column);
                        }
                        t.MouseRightButtonDown += treeViewNode_MouseRightButtonDown;
                        break;
                    case SyntaxNodes.LabeledStatement:
                        t = new TreeViewItem();
                        tvi.Items.Add(t);
                        t.Header = "Labeled Statement, label = " + (st as LabelledStatement).Label.Name;
                        AstProcessStatement((st as LabelledStatement).Body, t, editor);
                        if (st.Range != null)
                        {
                            t.Tag = st.Range[0];
                        }
                        else
                        {
                            t.Tag = editor.Document.GetOffset(st.Location.Start.Line, st.Location.Start.Column);
                        }
                        t.MouseRightButtonDown += treeViewNode_MouseRightButtonDown;
                        break;
                    case SyntaxNodes.ReturnStatement:
                        t = new TreeViewItem();
                        tvi.Items.Add(t);
                        t.Header = "Return Statement";
                        AstProcessExpression((st as ReturnStatement).Argument, t, editor);
                        if (st.Range != null)
                        {
                            t.Tag = st.Range[0];
                        }
                        else
                        {
                            t.Tag = editor.Document.GetOffset(st.Location.Start.Line, st.Location.Start.Column);
                        }
                        t.MouseRightButtonDown += treeViewNode_MouseRightButtonDown;
                        break;
                    case SyntaxNodes.SwitchStatement:
                        t = new TreeViewItem();
                        tvi.Items.Add(t);
                        t.Header = "Switch Statement";
                        t1 = new TreeViewItem();
                        t.Items.Add(t1);
                        t1.Header = "[Discriminant]";
                        AstProcessExpression((st as SwitchStatement).Discriminant, t1, editor);
                        foreach (var scase in (st as SwitchStatement).Cases)
                        {
                            var tcase = new TreeViewItem();
                            t.Header = "[Case]";
                            t.Items.Add(tcase);
                            AstProcessExpression(scase.Test, tcase, editor);
                            foreach (var stmt in scase.Consequent)
                            {
                                AstProcessStatement(stmt, t, editor);
                            }
                        }
                        if (st.Range != null)
                        {
                            t.Tag = st.Range[0];
                        }
                        else
                        {
                            t.Tag = editor.Document.GetOffset(st.Location.Start.Line, st.Location.Start.Column);
                        }
                        t.MouseRightButtonDown += treeViewNode_MouseRightButtonDown;
                        break;
                    case SyntaxNodes.ThrowStatement:
                        t = new TreeViewItem();
                        tvi.Items.Add(t);
                        t.Header = "Throw Statement";
                        AstProcessExpression((st as ThrowStatement).Argument, t, editor);
                        if (st.Range != null)
                        {
                            t.Tag = st.Range[0];
                        }
                        else
                        {
                            t.Tag = editor.Document.GetOffset(st.Location.Start.Line, st.Location.Start.Column);
                        }
                        t.MouseRightButtonDown += treeViewNode_MouseRightButtonDown;
                        break;
                    case SyntaxNodes.TryStatement:
                        t = new TreeViewItem();
                        tvi.Items.Add(t);
                        t.Header = "Try Statement";
                        t1 = new TreeViewItem();
                        t.Items.Add(t1);
                        t1.Header = "[Handlers]";
                        foreach (var ocatch in (st as TryStatement).Handlers)
                        {
                            var tcatch = new TreeViewItem();
                            t1.Items.Add(tcatch);
                            tcatch.Header = "[Catch " + ocatch.Param.Name + "]";
                            AstProcessStatement(ocatch.Body, tcatch, editor);
                        }
                        t2 = new TreeViewItem();
                        t.Items.Add(t2);
                        t2.Header = "[Guarded Handlers]";
                        foreach (var ocatch in (st as TryStatement).GuardedHandlers)
                        {
                            AstProcessStatement(ocatch, t2, editor);
                        }
                        t3 = new TreeViewItem();
                        t.Items.Add(t3);
                        t3.Header = "[Finalizer]";
                        AstProcessStatement((st as TryStatement).Finalizer, t3, editor);
                        AstProcessStatement((st as TryStatement).Block, t, editor);
                        if (st.Range != null)
                        {
                            t.Tag = st.Range[0];
                        }
                        else
                        {
                            t.Tag = editor.Document.GetOffset(st.Location.Start.Line, st.Location.Start.Column);
                        }
                        t.MouseRightButtonDown += treeViewNode_MouseRightButtonDown;
                        break;
                    case SyntaxNodes.VariableDeclaration:
                        t = new TreeViewItem();
                        tvi.Items.Add(t);
                        t.Header = "Variable Declaration";
                        t1 = new TreeViewItem();
                        t.Items.Add(t1);
                        t1.Header = "[Kind = " + (st as VariableDeclaration).Kind + "]";
                        t2 = new TreeViewItem();
                        t.Items.Add(t2);
                        t2.Header = "[Declarations]";
                        foreach (var item in (st as VariableDeclaration).Declarations)
                        {
                            AstProcessExpression(item, t2, editor);
                        }
                        if (st.Range != null)
                        {
                            t.Tag = st.Range[0];
                        }
                        else
                        {
                            t.Tag = editor.Document.GetOffset(st.Location.Start.Line, st.Location.Start.Column);
                        }
                        t.MouseRightButtonDown += treeViewNode_MouseRightButtonDown;
                        break;
                    case SyntaxNodes.WhileStatement:
                        t = new TreeViewItem();
                        tvi.Items.Add(t);
                        t.Header = "While Statement";
                        t1 = new TreeViewItem();
                        t.Items.Add(t1);
                        t1.Header = "[Test]";
                        AstProcessExpression((st as WhileStatement).Test, t1, editor);
                        AstProcessStatement((st as WhileStatement).Body, t, editor);
                        if (st.Range != null)
                        {
                            t.Tag = st.Range[0];
                        }
                        else
                        {
                            t.Tag = editor.Document.GetOffset(st.Location.Start.Line, st.Location.Start.Column);
                        }
                        t.MouseRightButtonDown += treeViewNode_MouseRightButtonDown;
                        break;
                    case SyntaxNodes.WithStatement:
                        t = new TreeViewItem();
                        tvi.Items.Add(t);
                        t.Header = "With Statement";
                        t1 = new TreeViewItem();
                        t.Items.Add(t1);
                        t1.Header = "[Object]";
                        AstProcessExpression((st as WithStatement).Object, t1, editor);
                        AstProcessStatement((st as WithStatement).Body, t, editor);
                        if (st.Range != null)
                        {
                            t.Tag = st.Range[0];
                        }
                        else
                        {
                            t.Tag = editor.Document.GetOffset(st.Location.Start.Line, st.Location.Start.Column);
                        }
                        t.MouseRightButtonDown += treeViewNode_MouseRightButtonDown;
                        break;
                    default:
                        System.Diagnostics.Debug.Print("Statement, type " + st.Type.ToString() + ", not handled.");
                        break;
                }
            }
        }

        private static void AstProcessExpression(Jint.Parser.Ast.Expression exp, TreeViewItem tvir, TextEditor editor)
        {
            // 先在 tvi 下新建一个节点 (Expression)
            if (exp != null)
            {
                TreeViewItem tvi = new TreeViewItem();
                tvi.Header = "(Expression)";
                tvir.Items.Add(tvi);
                TreeViewItem t, t1, t2, t3;

                switch (exp.Type)
                {
                    case SyntaxNodes.ArrayExpression:
                        t = new TreeViewItem();
                        tvi.Items.Add(t);
                        t.Header = "Array Expression";
                        foreach (var item in (exp as ArrayExpression).Elements)
                        {
                            AstProcessExpression(item, t, editor);
                        }
                        if (exp.Range != null)
                        {
                            t.Tag = exp.Range[0];
                        }
                        else
                        {
                            t.Tag = editor.Document.GetOffset(exp.Location.Start.Line, exp.Location.Start.Column);
                        }
                        t.MouseRightButtonDown += treeViewNode_MouseRightButtonDown;
                        break;
                    case SyntaxNodes.AssignmentExpression:
                        t = new TreeViewItem();
                        tvi.Items.Add(t);
                        t.Header = "Assignment Expression";
                        t1 = new TreeViewItem();
                        t.Items.Add(t1);
                        t1.Header = "[Left]";
                        AstProcessExpression((exp as AssignmentExpression).Left, t1, editor);
                        t2 = new TreeViewItem();
                        t.Items.Add(t2);
                        t2.Header = "[Operator = " + (exp as AssignmentExpression).Operator.ToString() + "]";
                        t3 = new TreeViewItem();
                        t.Items.Add(t3);
                        t3.Header = "[Right]";
                        AstProcessExpression((exp as AssignmentExpression).Right, t3, editor);
                        if (exp.Range != null)
                        {
                            t.Tag = exp.Range[0];
                        }
                        else
                        {
                            t.Tag = editor.Document.GetOffset(exp.Location.Start.Line, exp.Location.Start.Column);
                        }
                        t.MouseRightButtonDown += treeViewNode_MouseRightButtonDown;
                        break;
                    case SyntaxNodes.BinaryExpression:
                        t = new TreeViewItem();
                        tvi.Items.Add(t);
                        t.Header = "Binary Expression";
                        t1 = new TreeViewItem();
                        t.Items.Add(t1);
                        t1.Header = "[Left]";
                        AstProcessExpression((exp as BinaryExpression).Left, t1, editor);
                        t2 = new TreeViewItem();
                        t.Items.Add(t2);
                        t2.Header = "[Operator = " + (exp as BinaryExpression).Operator.ToString() + "]";
                        t3 = new TreeViewItem();
                        t.Items.Add(t3);
                        t3.Header = "[Right]";
                        AstProcessExpression((exp as BinaryExpression).Right, t3, editor);
                        if (exp.Range != null)
                        {
                            t.Tag = exp.Range[0];
                        }
                        else
                        {
                            t.Tag = editor.Document.GetOffset(exp.Location.Start.Line, exp.Location.Start.Column);
                        }
                        t.MouseRightButtonDown += treeViewNode_MouseRightButtonDown;
                        break;
                    case SyntaxNodes.CallExpression:
                        t = new TreeViewItem();
                        tvi.Items.Add(t);
                        t.Header = "Call Expression";
                        t1 = new TreeViewItem();
                        t.Items.Add(t1);
                        t1.Header = "[Callee]";
                        AstProcessExpression((exp as CallExpression).Callee, t1, editor);
                        t2 = new TreeViewItem();
                        t.Items.Add(t2);
                        t2.Header = "[Arguments]";
                        foreach (var oarg in (exp as CallExpression).Arguments)
                        {
                            AstProcessExpression(oarg, t2, editor);
                        }
                        if (exp.Range != null)
                        {
                            t.Tag = exp.Range[0];
                        }
                        else
                        {
                            t.Tag = editor.Document.GetOffset(exp.Location.Start.Line, exp.Location.Start.Column);
                        }
                        t.MouseRightButtonDown += treeViewNode_MouseRightButtonDown;
                        break;
                    case SyntaxNodes.ConditionalExpression:
                        t = new TreeViewItem();
                        tvi.Items.Add(t);
                        t.Header = "Conditional Expression";
                        t1 = new TreeViewItem();
                        t.Items.Add(t1);
                        t1.Header = "[Test]";
                        AstProcessExpression((exp as ConditionalExpression).Test, t1, editor);
                        t2 = new TreeViewItem();
                        t.Items.Add(t2);
                        t2.Header = "[Consequence]";
                        AstProcessExpression((exp as ConditionalExpression).Consequent, t2, editor);
                        t3 = new TreeViewItem();
                        t.Items.Add(t3);
                        t2.Header = "[Alternate]";
                        AstProcessExpression((exp as ConditionalExpression).Alternate, t3, editor);
                        if (exp.Range != null)
                        {
                            t.Tag = exp.Range[0];
                        }
                        else
                        {
                            t.Tag = editor.Document.GetOffset(exp.Location.Start.Line, exp.Location.Start.Column);
                        }
                        t.MouseRightButtonDown += treeViewNode_MouseRightButtonDown;
                        break;
                    case SyntaxNodes.FunctionExpression:
                        t = new TreeViewItem();
                        tvi.Items.Add(t);
                        if ((exp as FunctionExpression).Id != null)
                        {
                            t.Header = "Function Expression (" + (exp as FunctionExpression).Id.Name + ")";
                        }
                        else
                        {
                            t.Header = "Function Expression (#Lambda)";
                        }
                        t1 = new TreeViewItem();
                        t.Items.Add(t1);
                        t1.Header = "[Defaults]";
                        foreach (var item in (exp as FunctionExpression).Defaults)
                        {
                            AstProcessExpression(item, t1, editor);
                        }
                        t2 = new TreeViewItem();
                        t.Items.Add(t2);
                        t2.Header = "[Body]";
                        AstProcessStatement((exp as FunctionExpression).Body, t2, editor);
                        if (exp.Range != null)
                        {
                            t.Tag = exp.Range[0];
                        }
                        else
                        {
                            t.Tag = editor.Document.GetOffset(exp.Location.Start.Line, exp.Location.Start.Column);
                        }
                        t.MouseRightButtonDown += treeViewNode_MouseRightButtonDown;
                        break;
                    case SyntaxNodes.Identifier:
                        t = new TreeViewItem();
                        tvi.Items.Add(t);
                        t.Header = "Identifier, name = " + (exp as Identifier).Name;
                        if (exp.Range != null)
                        {
                            t.Tag = exp.Range[0];
                        }
                        else
                        {
                            t.Tag = editor.Document.GetOffset(exp.Location.Start.Line, exp.Location.Start.Column);
                        }
                        t.MouseRightButtonDown += treeViewNode_MouseRightButtonDown;
                        break;
                    case SyntaxNodes.Literal:
                        t = new TreeViewItem();
                        tvi.Items.Add(t);
                        t.Header = "Literal";
                        t1 = new TreeViewItem();
                        t.Items.Add(t1);
                        t1.Header = "[Raw = " + (exp as Literal).Raw + "]";
                        t2 = new TreeViewItem();
                        t.Items.Add(t2);
                        if ((exp as Literal).Value != null)
                        {
                            t2.Header = "[Value = " + (exp as Literal).Value.ToString() + "]";
                        }
                        if (exp.Range != null)
                        {
                            t.Tag = exp.Range[0];
                        }
                        else
                        {
                            t.Tag = editor.Document.GetOffset(exp.Location.Start.Line, exp.Location.Start.Column);
                        }
                        t.MouseRightButtonDown += treeViewNode_MouseRightButtonDown;
                        break;
                    case SyntaxNodes.LogicalExpression:
                        t = new TreeViewItem();
                        tvi.Items.Add(t);
                        t.Header = "Logical Expression";
                        t1 = new TreeViewItem();
                        t.Items.Add(t1);
                        t1.Header = "[Left]";
                        AstProcessExpression((exp as LogicalExpression).Left, t1, editor);
                        t2 = new TreeViewItem();
                        t.Items.Add(t2);
                        t2.Header = "[Operator = " + (exp as LogicalExpression).Operator.ToString() + "]";
                        t3 = new TreeViewItem();
                        t.Items.Add(t3);
                        t3.Header = "[Right]";
                        AstProcessExpression((exp as LogicalExpression).Right, t3, editor);
                        if (exp.Range != null)
                        {
                            t.Tag = exp.Range[0];
                        }
                        else
                        {
                            t.Tag = editor.Document.GetOffset(exp.Location.Start.Line, exp.Location.Start.Column);
                        }
                        t.MouseRightButtonDown += treeViewNode_MouseRightButtonDown;
                        break;
                    case SyntaxNodes.MemberExpression:
                        t = new TreeViewItem();
                        tvi.Items.Add(t);
                        t.Header = "Member Expression, computed = " + (exp as MemberExpression).Computed.ToString();
                        t1 = new TreeViewItem();
                        t.Items.Add(t1);
                        t1.Header = "[Object]";
                        AstProcessExpression((exp as MemberExpression).Object, t1, editor);
                        t2 = new TreeViewItem();
                        t.Items.Add(t2);
                        t2.Header = "[Property]";
                        AstProcessExpression((exp as MemberExpression).Property, t2, editor);
                        if (exp.Range != null)
                        {
                            t.Tag = exp.Range[0];
                        }
                        else
                        {
                            t.Tag = editor.Document.GetOffset(exp.Location.Start.Line, exp.Location.Start.Column);
                        }
                        t.MouseRightButtonDown += treeViewNode_MouseRightButtonDown;
                        break;
                    case SyntaxNodes.NewExpression:
                        t = new TreeViewItem();
                        tvi.Items.Add(t);
                        t.Header = "New Expression";
                        t1 = new TreeViewItem();
                        t.Items.Add(t1);
                        t1.Header = "[Callee]";
                        AstProcessExpression((exp as NewExpression).Callee, t1, editor);
                        t2 = new TreeViewItem();
                        t.Items.Add(t2);
                        t2.Header = "[Arguments]";
                        foreach (var oarg in (exp as NewExpression).Arguments)
                        {
                            AstProcessExpression(oarg, t2, editor);
                        }
                        if (exp.Range != null)
                        {
                            t.Tag = exp.Range[0];
                        }
                        else
                        {
                            t.Tag = editor.Document.GetOffset(exp.Location.Start.Line, exp.Location.Start.Column);
                        }
                        t.MouseRightButtonDown += treeViewNode_MouseRightButtonDown;
                        break;
                    case SyntaxNodes.ObjectExpression:
                        t = new TreeViewItem();
                        tvi.Items.Add(t);
                        t.Header = "Object Expression";
                        foreach (var oobject in (exp as ObjectExpression).Properties)
                        {
                            t1 = new TreeViewItem();
                            t.Items.Add(t1);
                            t1.Header = "[Property, kind = " + oobject.Kind.ToString() + "]";
                            t2 = new TreeViewItem();
                            t1.Items.Add(t2);
                            t2.Header = "[Key = " + oobject.Key.GetKey() + "]";
                            t3 = new TreeViewItem();
                            t1.Items.Add(t3);
                            t3.Header = "[Value]";
                            AstProcessExpression(oobject.Value, t3, editor);
                        }
                        if (exp.Range != null)
                        {
                            t.Tag = exp.Range[0];
                        }
                        else
                        {
                            t.Tag = editor.Document.GetOffset(exp.Location.Start.Line, exp.Location.Start.Column);
                        }
                        t.MouseRightButtonDown += treeViewNode_MouseRightButtonDown;
                        break;
                    case SyntaxNodes.Property:
                        t = new TreeViewItem();
                        tvi.Items.Add(t);
                        t.Header = "Property, kind = " + (exp as Property).Kind.ToString();
                        t1 = new TreeViewItem();
                        t.Items.Add(t1);
                        t1.Header = "[Key = " + (exp as Property).Key.GetKey() + "]";
                        t2 = new TreeViewItem();
                        t.Items.Add(t2);
                        t2.Header = "[Value]";
                        AstProcessExpression((exp as Property).Value, t2, editor);
                        if (exp.Range != null)
                        {
                            t.Tag = exp.Range[0];
                        }
                        else
                        {
                            t.Tag = editor.Document.GetOffset(exp.Location.Start.Line, exp.Location.Start.Column);
                        }
                        t.MouseRightButtonDown += treeViewNode_MouseRightButtonDown;
                        break;
                    case SyntaxNodes.RegularExpressionLiteral:
                        t = new TreeViewItem();
                        tvi.Items.Add(t);
                        t.Header = "RegExp Literal";
                        t1 = new TreeViewItem();
                        t.Items.Add(t1);
                        t1.Header = "[Raw = " + (exp as RegExpLiteral).Raw + "]";
                        t2 = new TreeViewItem();
                        t.Items.Add(t2);
                        if ((exp as Literal).Value != null)
                        {
                            t2.Header = "[Value = " + (exp as RegExpLiteral).Value.ToString() + "]";
                        }
                        t3 = new TreeViewItem();
                        t.Items.Add(t3);
                        t3.Header = "[Flags = " + (exp as RegExpLiteral).Flags + "]";
                        if (exp.Range != null)
                        {
                            t.Tag = exp.Range[0];
                        }
                        else
                        {
                            t.Tag = editor.Document.GetOffset(exp.Location.Start.Line, exp.Location.Start.Column);
                        }
                        t.MouseRightButtonDown += treeViewNode_MouseRightButtonDown;
                        break;
                    case SyntaxNodes.SequenceExpression:
                        t = new TreeViewItem();
                        tvi.Items.Add(t);
                        t.Header = "Sequence Expression";
                        foreach (var item in (exp as SequenceExpression).Expressions)
                        {
                            AstProcessExpression(item, t, editor);
                        }
                        if (exp.Range != null)
                        {
                            t.Tag = exp.Range[0];
                        }
                        else
                        {
                            t.Tag = editor.Document.GetOffset(exp.Location.Start.Line, exp.Location.Start.Column);
                        }
                        t.MouseRightButtonDown += treeViewNode_MouseRightButtonDown;
                        break;
                    case SyntaxNodes.ThisExpression:
                        t = new TreeViewItem();
                        tvi.Items.Add(t);
                        t.Header = "This Expression";
                        if (exp.Range != null)
                        {
                            t.Tag = exp.Range[0];
                        }
                        else
                        {
                            t.Tag = editor.Document.GetOffset(exp.Location.Start.Line, exp.Location.Start.Column);
                        }
                        t.MouseRightButtonDown += treeViewNode_MouseRightButtonDown;
                        break;
                    case SyntaxNodes.UnaryExpression:
                        t = new TreeViewItem();
                        tvi.Items.Add(t);
                        t.Header = "Unary Expression, prefix = " + (exp as UnaryExpression).Prefix.ToString();
                        t1 = new TreeViewItem();
                        t.Items.Add(t1);
                        t1.Header = "[Operator = " + (exp as UnaryExpression).Operator.ToString() + "]";
                        t2 = new TreeViewItem();
                        t.Items.Add(t2);
                        t2.Header = "[Argument]";
                        AstProcessExpression((exp as UnaryExpression).Argument, t2, editor);
                        if (exp.Range != null)
                        {
                            t.Tag = exp.Range[0];
                        }
                        else
                        {
                            t.Tag = editor.Document.GetOffset(exp.Location.Start.Line, exp.Location.Start.Column);
                        }
                        t.MouseRightButtonDown += treeViewNode_MouseRightButtonDown;
                        break;
                    case SyntaxNodes.UpdateExpression:
                        t = new TreeViewItem();
                        tvi.Items.Add(t);
                        t.Header = "Unary Expression, prefix = " + (exp as UpdateExpression).Prefix.ToString();
                        t1 = new TreeViewItem();
                        t.Items.Add(t1);
                        t1.Header = "[Operator = " + (exp as UpdateExpression).Operator.ToString() + "]";
                        t2 = new TreeViewItem();
                        t.Items.Add(t2);
                        t2.Header = "[Argument]";
                        AstProcessExpression((exp as UpdateExpression).Argument, t2, editor);
                        if (exp.Range != null)
                        {
                            t.Tag = exp.Range[0];
                        }
                        else
                        {
                            t.Tag = editor.Document.GetOffset(exp.Location.Start.Line, exp.Location.Start.Column);
                        }
                        t.MouseRightButtonDown += treeViewNode_MouseRightButtonDown;
                        break;
                    case SyntaxNodes.VariableDeclarator:
                        t = new TreeViewItem();
                        tvi.Items.Add(t);
                        t.Header = "Variable Declarator";
                        t1 = new TreeViewItem();
                        t.Items.Add(t1);
                        t1.Header = "[Id = " + (exp as VariableDeclarator).Id.Name + "]";
                        t2 = new TreeViewItem();
                        t.Items.Add(t2);
                        t2.Header = "[Init]";
                        AstProcessExpression((exp as VariableDeclarator).Init, t2, editor);
                        if (exp.Range != null)
                        {
                            t.Tag = exp.Range[0];
                        }
                        else
                        {
                            t.Tag = editor.Document.GetOffset(exp.Location.Start.Line, exp.Location.Start.Column);
                        }
                        t.MouseRightButtonDown += treeViewNode_MouseRightButtonDown;
                        break;
                    default:
                        System.Diagnostics.Debug.Print("Expression, type " + exp.Type.ToString() + ", not handled.");
                        break;
                }
            }
        }

    }
}
