using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using System.IO;

namespace DanmakuKun
{
    public sealed class CompletionItemImages
    {

        private CompletionItemImages()
        {
        }

        public readonly static BitmapImage ClassItemIcon;
        public readonly static BitmapImage FunctionItemIcon;
        public readonly static BitmapImage KeywordItemIcon;
        public readonly static BitmapImage PropertyItemIcon;
        public readonly static BitmapImage PropertyItemIconReadOnly;
        public readonly static BitmapImage PropertyItemIconWriteOnly;
        public readonly static BitmapImage SnippetItemIcon;
        public readonly static BitmapImage VariableItemIcon;

        static CompletionItemImages()
        {
            string currentPath;
            currentPath = Environment.CurrentDirectory;
            currentPath = Path.Combine(currentPath, @"images");
            Uri uri;
            uri = new Uri(Path.Combine(currentPath, "classitemicon.png"));
            ClassItemIcon = new BitmapImage(uri);
            uri = new Uri(Path.Combine(currentPath, @"functionitemicon.png"));
            FunctionItemIcon = new BitmapImage(uri);
            uri = new Uri(Path.Combine(currentPath, @"keyworditemicon.png"));
            KeywordItemIcon = new BitmapImage(uri);
            uri = new Uri(Path.Combine(currentPath, @"propertyitemicon.png"));
            PropertyItemIcon = new BitmapImage(uri);
            uri = new Uri(Path.Combine(currentPath, @"propertyitemiconreadonly.png"));
            PropertyItemIconReadOnly = new BitmapImage(uri);
            uri = new Uri(Path.Combine(currentPath, @"propertyitemiconwriteonly.png"));
            PropertyItemIconWriteOnly = new BitmapImage(uri);
            uri = new Uri(Path.Combine(currentPath, @"snippetitemicon.png"));
            SnippetItemIcon = new BitmapImage(uri);
            uri = new Uri(Path.Combine(currentPath, @"variableitemicon.png"));
            VariableItemIcon = new BitmapImage(uri);
        }

        /// <summary>
        /// 无用函数，只是为了静态初始化。
        /// </summary>
        public static void Initialize()
        {
        }

    }
}
