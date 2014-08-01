using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.AvalonEdit.CodeCompletion;

namespace DanmakuKun
{
    public sealed class BiliLists
    {

        //public readonly static Dictionary<string, IList<ICompletionData>> Lists;
        public readonly static IDictionary<string, IList<CompletionData>> Completion;
        public readonly static IDictionary<string, IDictionary<string, FunctionInsightData>> Function;

        private static bool _initialized = false;

        private BiliLists()
        {
        }

        static BiliLists()
        {
            Completion = new Dictionary<string, IList<CompletionData>>();
            Function = new Dictionary<string, IDictionary<string, FunctionInsightData>>();
        }

        private static void InitializeCompletionList()
        {
            var OCommentFieldList = new List<CompletionData>();
            OCommentFieldList.Add(new WithSourcePropertyCompletionData("alwaysShowSelection", "Boolean", "CommentField"));
            OCommentFieldList.Add(new WithSourceFunctionCompletionData("appendText", "void", "CommentField"));
            OCommentFieldList.Add(new WithSourcePropertyCompletionData("background", "Boolean", "CommentField"));
            OCommentFieldList.Add(new WithSourcePropertyCompletionData("backgroundColor", "uint", "CommentField"));
            OCommentFieldList.Add(new WithSourcePropertyCompletionData("bold", "Boolean", "CommentField"));
            OCommentFieldList.Add(new WithSourcePropertyCompletionData("border", "Boolean", "CommentField"));
            OCommentFieldList.Add(new WithSourcePropertyCompletionData("borderColor", "uint", "CommentField"));
            OCommentFieldList.Add(new WithSourcePropertyCompletionData("bottomScrollV", "int", "CommentField", PropertyModifiers.ReadOnly));
            OCommentFieldList.Add(new WithSourcePropertyCompletionData("condenseWhite", "Boolean", "CommentField"));
            OCommentFieldList.Add(new WithSourcePropertyCompletionData("defaultTextFormat", "TextFormat", "CommentField"));
            OCommentFieldList.Add(new WithSourcePropertyCompletionData("fontSize", "Number", "CommentField"));
            OCommentFieldList.Add(new WithSourcePropertyCompletionData("gridFitType", "String", "CommentField"));
            OCommentFieldList.Add(new WithSourcePropertyCompletionData("length", "int", "CommentField", PropertyModifiers.ReadOnly));
            OCommentFieldList.Add(new WithSourcePropertyCompletionData("htmlText", "String", "CommentField"));
            OCommentFieldList.Add(new WithSourcePropertyCompletionData("multiline", "Boolean", "CommentField"));
            OCommentFieldList.Add(new WithSourcePropertyCompletionData("numLines", "int", "CommentField", PropertyModifiers.ReadOnly));
            OCommentFieldList.Add(new WithSourcePropertyCompletionData("restrict", "String", "CommentField"));
            OCommentFieldList.Add(new WithSourcePropertyCompletionData("sharpness", "Number", "CommentField"));
            OCommentFieldList.Add(new WithSourcePropertyCompletionData("text", "String", "CommentField"));
            OCommentFieldList.Add(new WithSourcePropertyCompletionData("textColor", "uint", "CommentField"));
            OCommentFieldList.Add(new WithSourcePropertyCompletionData("textHeight", "Number", "CommentField", PropertyModifiers.ReadOnly));
            OCommentFieldList.Add(new WithSourcePropertyCompletionData("textWidth", "Number", "CommentField", PropertyModifiers.ReadOnly));
            OCommentFieldList.Add(new WithSourcePropertyCompletionData("thickness", "Number", "CommentField"));
            OCommentFieldList.Add(new WithSourcePropertyCompletionData("wordWrap", "Boolean", "CommentField"));
            Completion.Add("$CommentField", OCommentFieldList);
            var OCommentDataList = new List<CompletionData>();
            OCommentDataList.Add(new WithSourcePropertyCompletionData("color", "uint", "CommentData"));
            OCommentDataList.Add(new WithSourcePropertyCompletionData("fontSize", "uint", "CommentData"));
            OCommentDataList.Add(new WithSourcePropertyCompletionData("mode", "int", "CommentData"));
            OCommentDataList.Add(new WithSourcePropertyCompletionData("pool", "int", "CommentData"));
            OCommentDataList.Add(new WithSourcePropertyCompletionData("time", "Number", "CommentData"));
            OCommentDataList.Add(new WithSourcePropertyCompletionData("txt", "String", "CommentData"));
            Completion.Add("$CommentData", OCommentDataList);
            var OGraphicsList = new List<CompletionData>();
            OGraphicsList.Add(new FunctionCompletionData("beginFill", "void"));
            OGraphicsList.Add(new FunctionCompletionData("beginGradientFill", "void"));
            OGraphicsList.Add(new FunctionCompletionData("clear", "void"));
            OGraphicsList.Add(new FunctionCompletionData("curveTo", "void"));
            OGraphicsList.Add(new FunctionCompletionData("drawCircle", "void"));
            OGraphicsList.Add(new FunctionCompletionData("drawEllipse", "void"));
            OGraphicsList.Add(new FunctionCompletionData("drawRect", "void"));
            OGraphicsList.Add(new FunctionCompletionData("drawRoundRect", "void"));
            OGraphicsList.Add(new FunctionCompletionData("endFill", "void"));
            OGraphicsList.Add(new FunctionCompletionData("lineGradientStyle", "void"));
            OGraphicsList.Add(new FunctionCompletionData("lineStyle", "void"));
            OGraphicsList.Add(new FunctionCompletionData("lineTo", "void"));
            OGraphicsList.Add(new FunctionCompletionData("moveTo", "void"));
            Completion.Add("$Graphics", OGraphicsList);
            var OITweenList = new List<CompletionData>();
            OITweenList.Add(new WithSourceFunctionCompletionData("gotoAndPlay", "void", "ITween"));
            OITweenList.Add(new WithSourceFunctionCompletionData("gotoAndStop", "void", "ITween"));
            OITweenList.Add(new WithSourceFunctionCompletionData("play", "void", "ITween"));
            OITweenList.Add(new WithSourceFunctionCompletionData("stop", "void", "ITween"));
            OITweenList.Add(new WithSourceFunctionCompletionData("togglePause", "void", "ITween"));
            OITweenList.Add(new WithSourcePropertyCompletionData("stopOnComplete", "Boolean", "ITween"));
            Completion.Add("$ITween", OITweenList);
            var OShapeList = new List<CompletionData>();
            OShapeList.Add(new WithSourcePropertyCompletionData("graphics", "Graphics", "Shape", PropertyModifiers.ReadOnly));
            Completion.Add("$Shape", OShapeList);
            var OObjectList = new List<CompletionData>();
            OObjectList.Add(new WithSourceFunctionCompletionData("hasOwnProperty", "Boolean", "Object"));
            OObjectList.Add(new WithSourceFunctionCompletionData("isPrototypeOf", "Boolean", "Object"));
            OObjectList.Add(new WithSourceFunctionCompletionData("propertyIsEnumerable", "Boolean", "Object"));
            OObjectList.Add(new WithSourceFunctionCompletionData("setPropertyIsEnumerable", "void", "Object"));
            OObjectList.Add(new WithSourceFunctionCompletionData("toLocaleString", "String", "Object"));
            OObjectList.Add(new WithSourceFunctionCompletionData("toString", "String", "Object"));
            OObjectList.Add(new WithSourceFunctionCompletionData("valueOf", "Object", "Object"));
            Completion.Add("$Object", OObjectList);
            var OTimerList = new List<CompletionData>();
            OTimerList.Add(new WithSourceFunctionCompletionData("stop", "void", "Timer"));
            Completion.Add("$Timer", OTimerList);
            var OStringList = new List<CompletionData>();
            OStringList.Add(new WithSourcePropertyCompletionData("length", "int", "String"));
            OStringList.Add(new WithSourceFunctionCompletionData("charAt", "String", "String"));
            OStringList.Add(new WithSourceFunctionCompletionData("charCodeAt", "Number", "String"));
            OStringList.Add(new WithSourceFunctionCompletionData("concat", "String", "String"));
            OStringList.Add(new WithSourceFunctionCompletionData("indexOf", "int", "String"));
            OStringList.Add(new WithSourceFunctionCompletionData("lastIndexOf", "int", "String"));
            OStringList.Add(new WithSourceFunctionCompletionData("localeCompare", "int", "String"));
            OStringList.Add(new WithSourceFunctionCompletionData("match", "Array", "String"));
            OStringList.Add(new WithSourceFunctionCompletionData("replace", "String", "String"));
            OStringList.Add(new WithSourceFunctionCompletionData("search", "int", "String"));
            OStringList.Add(new WithSourceFunctionCompletionData("slice", "String", "String"));
            OStringList.Add(new WithSourceFunctionCompletionData("split", "Array", "String"));
            OStringList.Add(new WithSourceFunctionCompletionData("substr", "String", "String"));
            OStringList.Add(new WithSourceFunctionCompletionData("substring", "String", "String"));
            OStringList.Add(new WithSourceFunctionCompletionData("toLocaleLowerCase", "String", "String"));
            OStringList.Add(new WithSourceFunctionCompletionData("toLocaleUpperCase", "String", "String"));
            OStringList.Add(new WithSourceFunctionCompletionData("toLowerCase", "String", "String"));
            OStringList.Add(new WithSourceFunctionCompletionData("toUpperCase", "String", "String"));
            OStringList.Add(new WithSourceFunctionCompletionData("valueOf", "String", "String"));
            Completion.Add("$String", OStringList);
        }

        private static void InitializeFunctionList()
        {
            var PresetList = new Dictionary<string, FunctionInsightData>();
            PresetList.Add("trace", new FunctionInsightData("trace", "void", "(全局)", @"添加指定内容至日志中
参数
s:String — 要添加的内容", new ArgumentInsightData("s", "String")));
            PresetList.Add("clear", new FunctionInsightData("clear", "void", "(全局)", "清空日志内容", null));
            PresetList.Add("getTimer", new FunctionInsightData("getTimer", "int", "(全局)", "获取从启动播放器到现在经过的毫秒数", null));
            PresetList.Add("parseInt", new FunctionInsightData("parseInt", "Number", "(全局)", @"将字符串转换为整数。如果参数中指定的字符串不能转换为数字，则此函数返回 NaN。以 0x 开头的字符串被解释为十六进制数字。以 0 开头的整数不会被解释为八进制数字。必须指定 8 的基数才能解释为八进制数字。
有效整数前面的空白和 0 以及后面的非数字字符将被忽略。
参数
str:String — 要转换为整数的字符串。
radix:uint (default = 0) — 表示要分析的数字的基数（基）的整数。合法值为 2 到 36。
返回
Number — 一个数字或 NaN（非数字）。", new ArgumentInsightData("str", "String"), new ArgumentInsightData("radix", "uint", "0")));
            PresetList.Add("parseFloat", new FunctionInsightData("parseFloat", "Number", "(全局)", @"将字符串转换为浮点数。此函数读取或分析 并返回字符串中的数字，直到此函数遇到不是初始数字一部分的字符。如果字符串不是以可以分析的数字开头，parseFloat() 将返回 NaN。
有效整数前面的空白将被忽略，有效整数后面的非数字字符也将被忽略。
参数
str:String — 要读取并转换为浮点数的字符串。
返回
Number — 一个数字或 NaN（非数字）。", new ArgumentInsightData("str", "String")));
            PresetList.Add("timer", new FunctionInsightData("timer", "uint", "(全局)", @"在指定的延迟（以毫秒为单位）后运行指定的函数。
参数
closure:Function — 要执行的函数的名称。不要包括引号或圆括号，并且不要指定要调用的函数的参数。例如，使用 functionName，而不要使用 functionName() 或 functionName(param)。
delay:Number — 执行函数之前的延迟时间（以毫秒为单位）。
返回
uint — 超时进程的唯一数字标识符。使用此标识符可通过调用 clearTimeout() 方法取消进程。", new ArgumentInsightData("closure", "Function"), new ArgumentInsightData("delay", "Number")));
            PresetList.Add("interval", new FunctionInsightData("interval", "Timer", "(全局)", @"以指定的间隔（以毫秒为单位）运行函数。
参数
closure:Function — 要执行的函数的名称。不要包括引号或圆括号，并且不要指定要调用的函数的参数。例如，使用 functionName，而不要使用 functionName() 或 functionName(param)。
delay:Number — 间隔（以毫秒为单位）。
times:Number — 运行次数。
返回
Timer — 超时进程的唯一标识符。使用此标识符可通过调用 timer.stop() 方法取消进程。", new ArgumentInsightData("closure", "Function"), new ArgumentInsightData("delay", "Number"), new ArgumentInsightData("times", "Number", "1")));
            PresetList.Add("foreach", new FunctionInsightData("foreach", "void", "(全局)", @"遍历指定Object
参数
loop:Object — 被遍历的Object
f:Function — 遍历回调函数
回调函数定义
function foreachCallback(key:String,value:*):void;
====回调函数参数定义====
key:String — 键值名
value:* — 值", new ArgumentInsightData("loop", "Object"), new ArgumentInsightData("f", "Function")));
            PresetList.Add("clone", new FunctionInsightData("clone", "Object", "(全局)", @"复制指定Object
注意：此功能无法复制函数
参数
object:Object — 被复制的Object", new ArgumentInsightData("object", "Object")));
            PresetList.Add("load", new FunctionInsightData("load", "void", "(全局)", @"加载外部库
参数
library:String ─ 库名称
onComplete:Function ─ 加载完成时执行的回调函数", new ArgumentInsightData("library", "String"), new ArgumentInsightData("onComplete", "Function")));
            PresetList.Add("Player.play", new FunctionInsightData("play", "void", "Player", "开始播放媒体文件", null));
            PresetList.Add("Player.pause", new FunctionInsightData("pause", "void", "Player", "暂停视频流的回放。如果视频已经暂停，则调用此方法将不会执行任何操作。要在暂时视频后恢复播放，请调用 play()。", null));
            PresetList.Add("Player.seek", new FunctionInsightData("seek", "void", "Player", @"搜索与指定位置最接近的关键帧（在视频行业中也称为 I 帧）。关键帧位于从流的开始处算起的偏移位置（以毫秒为单位）。
视频流通常是使用以下两种类型的帧进行编码的：关键帧（或 I 帧）和 P 帧。关键帧包含完整图像；而 P 帧是一个中间帧，它在两个关键帧之间提供额外的视频信息。通常，视频流每 10 到 50 帧中有一个关键帧。
参数
offset:Number — 要在视频文件中移动到的时间近似值（以毫秒为单位）。", new ArgumentInsightData("offset", "Number")));
            PresetList.Add("Player.jump", new FunctionInsightData("jump", "void", "Player", @"跳至指定AV号指定页的视频
参数
av:String — 要跳转的视频(如av120040)。
page:Number — 要跳转的视频页数。
newwindow:Boolean — 是否打开新窗口进行跳转", new ArgumentInsightData("av", "String"), new ArgumentInsightData("page", "int", "1"), new ArgumentInsightData("newWindow", "Boolean", "false")));
            PresetList.Add("Player.commentTrigger", new FunctionInsightData("commentTrigger", "uint", "Player", @"监听发送弹幕
注意：此函数不会因播放器暂停而终止执行
回调函数定义
function commentCallback(cd:CommentData):void;
参数
f:Function — 发送弹幕时执行的回调函数
timeout:Number — 监听超时时间", new ArgumentInsightData("f", "Function"), new ArgumentInsightData("timeout", "Number", "1000")));
            PresetList.Add("Player.keyTrigger", new FunctionInsightData("keyTrigger", "uint", "Player", @"监听键盘输入
注意：
此函数不会因播放器暂停而终止执行
此函数只能监听数字键盘 0-9 及 上下左右 Home, End, Page UP, Page Down, W, S, A, D
回调函数定义
function keyCallback(key:int):void;
参数
f:Function — 键盘按下时的回调函数
timeout:Number — 监听超时时间
up:Boolean — 是否为监听keyUp事件", new ArgumentInsightData("f", "Function"), new ArgumentInsightData("timeout", "Number", "1000"), new ArgumentInsightData("up", "Boolean", "false")));
            PresetList.Add("Player.setMask", new FunctionInsightData("setMask", "void", "Player", @"设置播放器遮罩
参数
obj:DisplayObject — 作为遮罩的图形对象", new ArgumentInsightData("obj", "DisplayObject")));
            PresetList.Add("Player.createSound", new FunctionInsightData("createSound", "ScriptSound", "Player", @"建立声音元件
参数
t:String — 播放声音类型
onLoad:Function — 载入完成时的回调函数", new ArgumentInsightData("t", "String"), new ArgumentInsightData("onLoad", "Function", "null")));
            Function.Add("$", PresetList);
        }

        /// <summary>
        /// 初始化其他列表。
        /// </summary>
        public static void Initialize()
        {
            if (!_initialized)
            {
                InitializeCompletionList();
                InitializeFunctionList();  
            }
            _initialized = true;
        }

    }
}
