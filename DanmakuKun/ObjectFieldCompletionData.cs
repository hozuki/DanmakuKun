using System;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;

namespace DanmakuKun
{
    public class ObjectFieldCompletionData : FieldCompletionData
    {

        public ObjectFieldCompletionData(string name, string typeName)
            : base(name, typeName)
        {
        }

        public ObjectFieldCompletionData(string name, string typeName, string description)
            : base(name, typeName, description)
        {
        }

        public ObjectFieldCompletionData(string name, string typeName, string description, ItemModifiers modifiers)
            : base(name, typeName, description, modifiers)
        {
        }

        public ObjectFieldCompletionData(string name, string typeName, string description, string source)
            : base(name, typeName, description, source)
        {
        }

        public ObjectFieldCompletionData(string name, string typeName, string description, string source, ItemModifiers modifiers)
            : base(name, typeName, description, source, modifiers)
        {
        }

        public ObjectFieldCompletionData(string name, string typeName, string description, string source, ItemModifiers modifiers, string replacing)
            : base(name, typeName, description, source, modifiers, replacing)
        {
        }

        public override void Complete(TextArea textArea, ISegment completionSegment, EventArgs insertionRequestEventArgs)
        {
            // 我们通过按下"@"触发 ObjectFieldCompletionData 们的显示，然后才去 Complete()
            // 因此如果保持上面的调用方法，这个 completionSegment.Offset - 1 就没问题
            var seg = new TextSegment();
            seg.StartOffset = completionSegment.Offset - 1;
            seg.EndOffset = completionSegment.EndOffset;
            base.Complete(textArea, seg, insertionRequestEventArgs);
            seg = null;
        }

    }
}
