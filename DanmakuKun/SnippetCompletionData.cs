using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;

namespace DanmakuKun
{
    public class SnippetCompletionData : CompletionData
    {

        public SnippetCompletionData(string name, string replacing)
            : this(name, null, replacing)
        {
        }

        public SnippetCompletionData(string name, string description, string replacing)
            : base(name, description, null, DV.DefaultModifiers, replacing)
        {
            _source = DV.SnippetListName;
        }

        public override ImageSource Image
        {
            get
            {
                return CompletionItemImages.SnippetItemIcon;
            }
        }

        public override void Complete(TextArea textArea, ISegment completionSegment, EventArgs insertionRequestEventArgs)
        {
            base.Complete(textArea, completionSegment, insertionRequestEventArgs);
            // Indentation may take slightly longer time.
            if (textArea.IndentationStrategy != null)
            {
                var beginLineIndex = textArea.Document.GetLineByOffset(completionSegment.Offset).LineNumber;
                var endLineIndex = textArea.Document.GetLineByOffset(completionSegment.EndOffset).LineNumber;
                textArea.IndentationStrategy.IndentLines(textArea.Document, beginLineIndex, endLineIndex);
            }
        }

    }
}
