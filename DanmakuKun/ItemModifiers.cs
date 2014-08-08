using System;

namespace DanmakuKun
{
    public enum ItemModifiers
    {
        None = 0x0,
        ReadOnly = 0x1,
        WriteOnly = 0x2,
        Static = 0x4,
        Params = 0x8,
        Optional = 0x10,
        // 仅用于参数，就是 hideInHeader
        Hidden = 0x20
    }
}
