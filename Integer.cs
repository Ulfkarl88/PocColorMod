using System;

namespace PocColor.Config
{
    // Token: 0x0200000D RID: 13
    public class Integer
    {
        // Token: 0x17000028 RID: 40
        // (get) Token: 0x06000071 RID: 113 RVA: 0x00006394 File Offset: 0x00004594
        // (set) Token: 0x06000072 RID: 114 RVA: 0x0000639C File Offset: 0x0000459C
        public uint value { get; set; }

        // Token: 0x06000073 RID: 115 RVA: 0x000063A5 File Offset: 0x000045A5
        public Integer(uint value)
        {
            this.value = value;
        }
    }
}
