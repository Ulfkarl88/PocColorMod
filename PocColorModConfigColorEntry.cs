using System;

namespace PocColor.Config
{
    // Token: 0x0200000C RID: 12
    public class PocColorModConfigColorEntry
    {
        // Token: 0x1700001F RID: 31
        // (get) Token: 0x0600005E RID: 94 RVA: 0x000062F2 File Offset: 0x000044F2
        // (set) Token: 0x0600005F RID: 95 RVA: 0x000062FA File Offset: 0x000044FA
        public int? mode { get; set; }

        // Token: 0x17000020 RID: 32
        // (get) Token: 0x06000060 RID: 96 RVA: 0x00006303 File Offset: 0x00004503
        // (set) Token: 0x06000061 RID: 97 RVA: 0x0000630B File Offset: 0x0000450B
        public string[] color { get; set; }

        // Token: 0x17000021 RID: 33
        // (get) Token: 0x06000062 RID: 98 RVA: 0x00006314 File Offset: 0x00004514
        // (set) Token: 0x06000063 RID: 99 RVA: 0x0000631C File Offset: 0x0000451C
        public string[] color2 { get; set; }

        // Token: 0x17000022 RID: 34
        // (get) Token: 0x06000064 RID: 100 RVA: 0x00006325 File Offset: 0x00004525
        // (set) Token: 0x06000065 RID: 101 RVA: 0x0000632D File Offset: 0x0000452D
        public string FollowKingdomColors { get; set; }

        // Token: 0x17000023 RID: 35
        // (get) Token: 0x06000066 RID: 102 RVA: 0x00006336 File Offset: 0x00004536
        // (set) Token: 0x06000067 RID: 103 RVA: 0x0000633E File Offset: 0x0000453E
        public string FollowKingdomBackgroundOnly { get; set; }

        // Token: 0x17000024 RID: 36
        // (get) Token: 0x06000068 RID: 104 RVA: 0x00006347 File Offset: 0x00004547
        // (set) Token: 0x06000069 RID: 105 RVA: 0x0000634F File Offset: 0x0000454F
        public string BearRulerColors { get; set; }

        // Token: 0x17000025 RID: 37
        // (get) Token: 0x0600006A RID: 106 RVA: 0x00006358 File Offset: 0x00004558
        // (set) Token: 0x0600006B RID: 107 RVA: 0x00006360 File Offset: 0x00004560
        public string BearRulerBanner { get; set; }

        // Token: 0x17000026 RID: 38
        // (get) Token: 0x0600006C RID: 108 RVA: 0x00006369 File Offset: 0x00004569
        // (set) Token: 0x0600006D RID: 109 RVA: 0x00006371 File Offset: 0x00004571
        public string BearRulerShield { get; set; }

        // Token: 0x17000027 RID: 39
        // (get) Token: 0x0600006E RID: 110 RVA: 0x0000637A File Offset: 0x0000457A
        // (set) Token: 0x0600006F RID: 111 RVA: 0x00006382 File Offset: 0x00004582
        public Map<string, string[]>[] vars { get; set; }
    }
}
