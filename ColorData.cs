using System;

namespace PocColor.Config
{
    // Token: 0x02000005 RID: 5
    internal class ColorData
    {
        // Token: 0x17000004 RID: 4
        // (get) Token: 0x06000020 RID: 32 RVA: 0x00005F67 File Offset: 0x00004167
        // (set) Token: 0x06000021 RID: 33 RVA: 0x00005F6F File Offset: 0x0000416F
        public int mode { get; set; }

        // Token: 0x17000005 RID: 5
        // (get) Token: 0x06000022 RID: 34 RVA: 0x00005F78 File Offset: 0x00004178
        // (set) Token: 0x06000023 RID: 35 RVA: 0x00005F80 File Offset: 0x00004180
        public uint? c1 { get; set; }

        // Token: 0x17000006 RID: 6
        // (get) Token: 0x06000024 RID: 36 RVA: 0x00005F89 File Offset: 0x00004189
        // (set) Token: 0x06000025 RID: 37 RVA: 0x00005F91 File Offset: 0x00004191
        public uint? c2 { get; set; }

        // Token: 0x17000007 RID: 7
        // (get) Token: 0x06000026 RID: 38 RVA: 0x00005F9A File Offset: 0x0000419A
        // (set) Token: 0x06000027 RID: 39 RVA: 0x00005FA2 File Offset: 0x000041A2
        public string newbanner { get; set; }

        // Token: 0x17000008 RID: 8
        // (get) Token: 0x06000028 RID: 40 RVA: 0x00005FAB File Offset: 0x000041AB
        // (set) Token: 0x06000029 RID: 41 RVA: 0x00005FB3 File Offset: 0x000041B3
        public string newshield { get; set; }

        // Token: 0x0600002A RID: 42 RVA: 0x00005FBC File Offset: 0x000041BC
        public ColorData(int mode, uint? c1, uint? c2, string newbanner, string newshield)
        {
            this.mode = mode;
            this.c1 = c1;
            this.c2 = c2;
            this.newbanner = newbanner;
            this.newshield = newshield;
        }
    }
}
