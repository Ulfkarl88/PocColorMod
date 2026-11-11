using System;

namespace PocColor.Config
{
    // Token: 0x0200000E RID: 14
    public class CharInfo
    {
        // Token: 0x17000029 RID: 41
        // (get) Token: 0x06000074 RID: 116 RVA: 0x000063B7 File Offset: 0x000045B7
        // (set) Token: 0x06000075 RID: 117 RVA: 0x000063BF File Offset: 0x000045BF
        public string name { get; set; }

        // Token: 0x1700002A RID: 42
        // (get) Token: 0x06000076 RID: 118 RVA: 0x000063C8 File Offset: 0x000045C8
        // (set) Token: 0x06000077 RID: 119 RVA: 0x000063D0 File Offset: 0x000045D0
        public bool isHero { get; set; }

        // Token: 0x06000078 RID: 120 RVA: 0x000063D9 File Offset: 0x000045D9
        public CharInfo(string name, bool isHero)
        {
            this.name = name;
            this.isHero = isHero;
        }
    }
}
