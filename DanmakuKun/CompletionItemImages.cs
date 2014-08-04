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
        public readonly static BitmapImage FieldItemIcon;
        public readonly static BitmapImage ConstantItemIcon;

        static CompletionItemImages()
        {
            string currentPath = @"pack://application:,,,/resources/images/";
            Uri uri;
            uri = new Uri(currentPath + "classitemicon.png");
            ClassItemIcon = new BitmapImage(uri);
            uri = new Uri(currentPath + @"functionitemicon.png");
            FunctionItemIcon = new BitmapImage(uri);
            uri = new Uri(currentPath + @"keyworditemicon.png");
            KeywordItemIcon = new BitmapImage(uri);
            uri = new Uri(currentPath + @"propertyitemicon.png");
            PropertyItemIcon = new BitmapImage(uri);
            uri = new Uri(currentPath + @"propertyitemiconreadonly.png");
            PropertyItemIconReadOnly = new BitmapImage(uri);
            uri = new Uri(currentPath + @"propertyitemiconwriteonly.png");
            PropertyItemIconWriteOnly = new BitmapImage(uri);
            uri = new Uri(currentPath + @"snippetitemicon.png");
            SnippetItemIcon = new BitmapImage(uri);
            uri = new Uri(currentPath + @"fielditemicon.png");
            FieldItemIcon = new BitmapImage(uri);
            uri = new Uri(currentPath + @"constantitemicon.png");
            ConstantItemIcon = new BitmapImage(uri);
        }

        /// <summary>
        /// 无用函数，只是为了静态初始化。
        /// </summary>
        public static void Initialize()
        {
        }

    }
}
