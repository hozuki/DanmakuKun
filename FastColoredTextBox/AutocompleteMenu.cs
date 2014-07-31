using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Text.RegularExpressions;

namespace FastColoredTextBoxNS
{
    /// <summary>
    /// Popup menu for autocomplete
    /// </summary>
    [Browsable(false)]
    [ToolboxItem(false)]
    public class AutocompleteMenu : ToolStripDropDown
    {
        AutocompleteListView listView;
        protected ToolStripControlHost host;
        public Range Fragment { get; internal set; }
        public FastColoredTextBox TextBox { get; private set; }

        /// <summary>
        /// Regex pattern for serach fragment around caret
        /// </summary>
        public string SearchPattern { get; set; }
        /// <summary>
        /// Minimum fragment length for popup
        /// </summary>
        public int MinFragmentLength { get; set; }
        /// <summary>
        /// User selects item
        /// </summary>
        public event EventHandler<SelectingEventArgs> Selecting;
        /// <summary>
        /// It fires after item inserting
        /// </summary>
        public event EventHandler<SelectedEventArgs> Selected;
        /// <summary>
        /// Occurs when popup menu is opening
        /// </summary>
        public event EventHandler<CancelEventArgs> Opening;
        /// <summary>
        /// Allow TAB for select menu item
        /// </summary>
        public bool AllowTabKey { get { return listView.AllowTabKey; } set { listView.AllowTabKey = value; } }
        ///// <summary>
        ///// Interval of menu appear (ms)
        ///// </summary>
        //public int AppearInterval { get { return listView.AppearInterval; } set { listView.AppearInterval = value; } }
        /// <summary>
        /// The character used for replacing. e.g., dot(.).
        /// </summary>
        public char ReplaceChar { get; set; }

        public AutocompleteMenu(FastColoredTextBox tb)
        {
            // create a new popup and add the list view to it 
            AutoClose = false;
            AutoSize = false;
            Margin = Padding.Empty;
            Padding = Padding.Empty;
            listView = new AutocompleteListView(tb);
            host = new ToolStripControlHost(listView);
            host.Margin = new Padding(2, 2, 2, 2);
            host.Padding = Padding.Empty;
            host.AutoSize = false;
            host.AutoToolTip = false;
            TextBox = tb;
            CalcSize();
            base.Items.Add(host);
            listView.Parent = this;
            // 默认的（只适用于"."操作）
            //SearchPattern = @"[\w\.]";
            // 以下搜索无效，因为 FCTB 会自动定位至找到的匹配之前的空格。
            //SearchPattern = @"[\s]*[\w]+[\.]";
            SearchPattern = @"[\S]";
            MinFragmentLength = 2;
            //listView.Font = new Font("Courier New", SystemFonts.MessageBoxFont.Size);
        }

        internal new void OnOpening(CancelEventArgs args)
        {
            if (Opening != null)
                Opening(this, args);
        }

        public new void Close()
        {
            listView.toolTip.Hide(listView);
            base.Close();
        }

        internal void CalcSize()
        {
            host.Size = listView.Size;
            Size = new System.Drawing.Size(listView.Size.Width + 3, listView.Size.Height + 3);
        }

        public virtual void OnSelecting()
        {
            listView.OnSelecting();
        }

        public void SelectNext(int shift)
        {
            listView.SelectNext(shift);
        }

        internal void OnSelecting(SelectingEventArgs args)
        {
            if (Selecting != null)
                Selecting(this, args);
        }

        public void OnSelected(SelectedEventArgs args)
        {
            if (Selected != null)
                Selected(this, args);
        }

        public new AutocompleteListView Items
        {
            get { return listView; }
        }

        /// <summary>
        /// Shows popup menu immediately
        /// </summary>
        /// <param name="forced">If True - MinFragmentLength will be ignored</param>
        public void Show(bool forced)
        {
            Items.DoAutocomplete(forced);
        }

    }

}
