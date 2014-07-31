using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Linq;

namespace FastColoredTextBoxNS
{

    [ToolboxItem(false)]
    public class AutocompleteListView : UserControl
    {
        internal List<AutocompleteItem> visibleItems;
        IEnumerable<AutocompleteItem> sourceItems = new List<AutocompleteItem>();
        int selectedItemIndex = 0;
        int hoveredItemIndex = -1;
        int itemHeight;
        private AutocompleteMenu Menu { get { return Parent as AutocompleteMenu; } }
        int oldItemCount = 0;
        internal FastColoredTextBox textBox;
        internal ToolTip toolTip = new ToolTip();
        //System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();

        public bool AllowTabKey { get; set; }
        public ImageList ImageList { get; set; }
        //internal int AppearInterval { get { return timer.Interval; } set { timer.Interval = value; } }

        internal static readonly char[] _delimChars = { '(', ')', '=', ':', '+', '-', '*', '/', '?', ',', '!', '@', '#', '$', '%', '^', '&', '~', '\\', '|', '<', '>' };
        private static readonly char[] _functionalChars = { '.', ':' };

        internal AutocompleteListView(FastColoredTextBox tb)
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true);
            base.Font = new Font(FontFamily.GenericMonospace, 9);
            visibleItems = new List<AutocompleteItem>();
            itemHeight = Font.Height + 2;
            VerticalScroll.SmallChange = itemHeight;
            BackColor = Color.White;
            MaximumSize = new Size(Size.Width, 180);
            toolTip.ShowAlways = false;
            //AppearInterval = 50;
            //timer.Tick += new EventHandler(timer_Tick);

            this.textBox = tb;

            tb.KeyDown += new KeyEventHandler(tb_KeyDown);
            tb.SelectionChanged += new EventHandler(tb_SelectionChanged);
            tb.KeyPressed += new KeyPressEventHandler(tb_KeyPressed);
            tb.KeyPressing += new KeyPressEventHandler(tb_KeyPressing);

            Form form = tb.FindForm();
            if (form != null)
            {
                form.LocationChanged += (o, e) => Menu.Close();
                form.ResizeBegin += (o, e) => Menu.Close();
                form.FormClosing += (o, e) => Menu.Close();
                form.LostFocus += (o, e) => Menu.Close();
            }

            tb.LostFocus += (o, e) =>
            {
                if (!Menu.Focused) Menu.Close();
            };

            tb.Scroll += (o, e) => Menu.Close();
        }

        void tb_KeyPressing(object sender, KeyPressEventArgs e)
        {
            if (Menu.Visible && _delimChars.Contains(e.KeyChar))
            {
                // 若在 KeyPressed 中，就应该用 OnSelecting(+1)
                this.OnSelecting();
                return;
            }
        }

        void tb_KeyPressed(object sender, KeyPressEventArgs e)
        {
            bool backspaceORdel = e.KeyChar == '\b' || e.KeyChar == 0xff;
            bool whitespace = e.KeyChar == ' ' || e.KeyChar == '\n' || e.KeyChar == '\r' || e.KeyChar == '\t' || e.KeyChar == '\v';

            /*
            if (backspaceORdel)
                prevSelection = tb.Selection.Start;*/

            //if (!backspaceORdel && !whitespace)
            if (_functionalChars.Contains(e.KeyChar))
                if (Menu.Visible)
                    DoAutocomplete(false);
                //else
                //    ResetTimer(timer);
        }

        //void timer_Tick(object sender, EventArgs e)
        //{
        //    timer.Stop();
        //    DoAutocomplete(false);
        //}

        void ResetTimer(System.Windows.Forms.Timer timer)
        {
            timer.Stop();
            timer.Start();
        }

        internal void DoAutocomplete()
        {
            DoAutocomplete(false);
        }

        internal void DoAutocomplete(bool forced)
        {
            if (!Menu.Enabled)
            {
                Menu.Close();
                return;
            }

            visibleItems.Clear();
            selectedItemIndex = 0;
            VerticalScroll.Value = 0;
            //get fragment around caret
            Range fragment = textBox.Selection.GetLeftFragment(Menu.SearchPattern, RegexOptions.RightToLeft);
            string text = fragment.Text;
            //calc screen point for popup menu
            Point point = textBox.PlaceToPoint(fragment.End);
            point.Offset(2, textBox.CharHeight);
            try
            {
                System.Diagnostics.Debug.Print("Fragment: " + (this.Parent as AutocompleteMenu).Fragment.Text);
            }
            catch (Exception)
            {
            }
            if (forced ||
                (text.Length >= Menu.MinFragmentLength && textBox.Selection.Start == textBox.Selection.End))
            {
                Menu.Fragment = fragment;
                bool foundSelected = false;
                //build popup menu
                foreach (var item in sourceItems)
                {
                    item.Parent = Menu;
                    CompareResult res = item.Compare(text);
                    if (res != CompareResult.Hidden)
                        visibleItems.Add(item);
                    if (res == CompareResult.VisibleAndSelected && !foundSelected)
                    {
                        foundSelected = true;
                        selectedItemIndex = visibleItems.Count - 1;
                    }
                }

                if (foundSelected)
                {
                    AdjustScroll();
                    DoSelectedVisible();
                }
            }

            //show popup menu
            if (Count > 0)
            {
                if (!Menu.Visible)
                {
                    CancelEventArgs args = new CancelEventArgs();
                    Menu.OnOpening(args);
                    if (!args.Cancel)
                        Menu.Show(textBox, point);
                }
                else
                    Invalidate();
            }
            else
                Menu.Close();
        }

        void tb_SelectionChanged(object sender, EventArgs e)
        {
            /*
            FastColoredTextBox tb = sender as FastColoredTextBox;
            
            if (Math.Abs(prevSelection.iChar - tb.Selection.Start.iChar) > 1 ||
                        prevSelection.iLine != tb.Selection.Start.iLine)
                Menu.Close();
            prevSelection = tb.Selection.Start;*/
            if (Menu.Visible)
            {
                bool needClose = false;

                if (textBox.Selection.Start != textBox.Selection.End)
                    needClose = true;
                else
                    if (!Menu.Fragment.Contains(textBox.Selection.Start))
                    {
                        if (textBox.Selection.Start.iLine == Menu.Fragment.End.iLine && textBox.Selection.Start.iChar == Menu.Fragment.End.iChar + 1)
                        {
                            //user press key at end of fragment
                            char c = textBox.Selection.CharBeforeStart;
                            if (!Regex.IsMatch(c.ToString(), Menu.SearchPattern))//check char
                                needClose = true;
                        }
                        else
                            needClose = true;
                    }

                if (needClose)
                    Menu.Close();
            }

        }

        void tb_KeyDown(object sender, KeyEventArgs e)
        {
            if (Menu.Visible)
                if (ProcessKey(e.KeyCode, e.Modifiers))
                    e.Handled = true;

            if (!Menu.Visible)
                if (e.Modifiers == Keys.Control && e.KeyCode == Keys.T)
                {
                    DoAutocomplete();
                    e.Handled = true;
                }
        }

        void AdjustScroll()
        {
            if (oldItemCount == visibleItems.Count)
                return;

            int needHeight = itemHeight * visibleItems.Count + 1;
            Height = Math.Min(needHeight, MaximumSize.Height);
            Menu.CalcSize();

            AutoScrollMinSize = new Size(0, needHeight);
            oldItemCount = visibleItems.Count;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            AdjustScroll();
            int startI = VerticalScroll.Value / itemHeight - 1;
            int finishI = (VerticalScroll.Value + ClientSize.Height) / itemHeight + 1;
            startI = Math.Max(startI, 0);
            finishI = Math.Min(finishI, visibleItems.Count);
            int y = 0;
            const int leftPadding = 18;
            for (int i = startI; i < finishI; i++)
            {
                y = i * itemHeight - VerticalScroll.Value;

                if (ImageList != null && visibleItems[i].ImageIndex >= 0)
                    e.Graphics.DrawImage(ImageList.Images[visibleItems[i].ImageIndex], 1, y);

                if (i == selectedItemIndex)
                {
                    //Brush selectedBrush = new LinearGradientBrush(new Point(0, y - 3), new Point(0, y + itemHeight), Color.White, Color.Orange);
                    Brush selectedBrush = new SolidBrush(Color.FromArgb(0x33, 0x99, 0xff)) as Brush;
                    e.Graphics.FillRectangle(selectedBrush, leftPadding, y, ClientSize.Width - leftPadding - 1, itemHeight);
                    //e.Graphics.DrawRectangle(SystemPens.HighlightText, leftPadding, y, ClientSize.Width - leftPadding, itemHeight);
                }
                if (i == hoveredItemIndex)
                {
                    e.Graphics.DrawRectangle(new Pen(Color.FromArgb(0x33, 0x99, 0xff)), leftPadding, y, ClientSize.Width - leftPadding - 1, itemHeight);
                }
                if (i == selectedItemIndex)
                {
                    e.Graphics.DrawString(visibleItems[i].ToString(), Font, Brushes.White, leftPadding, y);
                }
                else
                {
                    e.Graphics.DrawString(visibleItems[i].ToString(), Font, Brushes.Black, leftPadding, y);
                }
            }
        }

        protected override void OnScroll(ScrollEventArgs se)
        {
            base.OnScroll(se);
            Invalidate();
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);

            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                selectedItemIndex = PointToItemIndex(e.Location);
                DoSelectedVisible();
                Invalidate();
            }
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            base.OnMouseDoubleClick(e);
            selectedItemIndex = PointToItemIndex(e.Location);
            Invalidate();
            OnSelecting();
        }

        internal void OnSelecting()
        {
            this.OnSelecting(0);
        }

        internal virtual void OnSelecting(int offsetAfterReplacing)
        {
            if (selectedItemIndex < 0 || selectedItemIndex >= visibleItems.Count)
                return;
            textBox.manager.BeginAutoUndoCommands();
            try
            {
                AutocompleteItem item = visibleItems[selectedItemIndex];
                SelectingEventArgs args = new SelectingEventArgs()
                {
                    Item = item,
                    SelectedIndex = selectedItemIndex
                };

                Menu.OnSelecting(args);

                if (args.Cancel)
                {
                    selectedItemIndex = args.SelectedIndex;
                    Invalidate();
                    return;
                }

                if (!args.Handled)
                {
                    var fragment = Menu.Fragment;
                    DoAutocomplete(item, fragment);
                    if (offsetAfterReplacing != 0)
                    {
                        this.textBox.SelectionStart += offsetAfterReplacing;
                    }
                }

                Menu.Close();
                //
                SelectedEventArgs args2 = new SelectedEventArgs()
                {
                    Item = item,
                    TextBox = Menu.Fragment.tb
                };
                item.OnSelected(Menu, args2);
                Menu.OnSelected(args2);
            }
            finally
            {
                textBox.manager.EndAutoUndoCommands();
            }
        }

        private void DoAutocomplete(AutocompleteItem item, Range fragment)
        {
            string newText = item.GetTextForReplace();
            //replace text of fragment
            var tb = fragment.tb;
            tb.Selection.Start = fragment.Start;
            tb.Selection.End = fragment.End;
            tb.InsertText(newText);
            tb.Focus();
        }

        int PointToItemIndex(Point p)
        {
            return (p.Y + VerticalScroll.Value) / itemHeight;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            ProcessKey(keyData, Keys.None);

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private bool ProcessKey(Keys keyData, Keys keyModifiers)
        {
            if (keyModifiers == Keys.None)
                switch (keyData)
                {
                    case Keys.Down:
                        SelectNext(+1);
                        return true;
                    case Keys.PageDown:
                        SelectNext(+10);
                        return true;
                    case Keys.Up:
                        SelectNext(-1);
                        return true;
                    case Keys.PageUp:
                        SelectNext(-10);
                        return true;
                    case Keys.Enter:
                        OnSelecting();
                        return true;
                    case Keys.Tab:
                        if (!AllowTabKey)
                            break;
                        OnSelecting();
                        return true;
                    case Keys.Escape:
                        Menu.Close();
                        return true;
                }

            return false;
        }

        public void SelectNext(int shift)
        {
            selectedItemIndex = Math.Max(0, Math.Min(selectedItemIndex + shift, visibleItems.Count - 1));
            DoSelectedVisible();
            //
            Invalidate();
        }

        private void DoSelectedVisible()
        {
            if (selectedItemIndex >= 0 && selectedItemIndex < visibleItems.Count)
                SetToolTip(visibleItems[selectedItemIndex]);

            var y = selectedItemIndex * itemHeight - VerticalScroll.Value;
            if (y < 0)
                VerticalScroll.Value = selectedItemIndex * itemHeight;
            if (y > ClientSize.Height - itemHeight)
                VerticalScroll.Value = Math.Min(VerticalScroll.Maximum, selectedItemIndex * itemHeight - ClientSize.Height + itemHeight);
            //some magic for update scrolls
            AutoScrollMinSize -= new Size(1, 0);
            AutoScrollMinSize += new Size(1, 0);
        }

        private void SetToolTip(AutocompleteItem autocompleteItem)
        {
            var title = visibleItems[selectedItemIndex].ToolTipTitle;
            var text = visibleItems[selectedItemIndex].ToolTipText;

            if (string.IsNullOrEmpty(title))
            {
                toolTip.ToolTipTitle = null;
                toolTip.SetToolTip(this, null);
                return;
            }

            if (string.IsNullOrEmpty(text))
            {
                toolTip.ToolTipTitle = null;
                toolTip.Show(title, this, Width + 3, 0, 3000);
            }
            else
            {
                toolTip.ToolTipTitle = title;
                toolTip.Show(text, this, Width + 3, 0, 3000);
            }
        }

        public int Count
        {
            get { return visibleItems.Count; }
        }

        public void SetAutocompleteItems(ICollection<string> items)
        {
            List<AutocompleteItem> list = new List<AutocompleteItem>(items.Count);
            foreach (var item in items)
                list.Add(new AutocompleteItem(item));
            SetAutocompleteItems(list);
        }

        public void SetAutocompleteItems(ICollection<AutocompleteItem> items)
        {
            sourceItems = items;
        }
    }
}
