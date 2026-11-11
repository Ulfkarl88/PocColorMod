using System;

namespace PocColor.Config
{
    // Token: 0x0200000A RID: 10
    public class ConfigSection
    {
        // Token: 0x1700000D RID: 13
        // (get) Token: 0x06000036 RID: 54 RVA: 0x0000603E File Offset: 0x0000423E
        // (set) Token: 0x06000037 RID: 55 RVA: 0x00006046 File Offset: 0x00004246
        public int? mode { get; set; }

        // Token: 0x1700000E RID: 14
        // (get) Token: 0x06000038 RID: 56 RVA: 0x0000604F File Offset: 0x0000424F
        // (set) Token: 0x06000039 RID: 57 RVA: 0x00006057 File Offset: 0x00004257
        public string[] color { get; set; }

        // Token: 0x1700000F RID: 15
        // (get) Token: 0x0600003A RID: 58 RVA: 0x00006060 File Offset: 0x00004260
        // (set) Token: 0x0600003B RID: 59 RVA: 0x00006068 File Offset: 0x00004268
        public string[] color2 { get; set; }

        // Token: 0x17000010 RID: 16
        // (get) Token: 0x0600003C RID: 60 RVA: 0x00006071 File Offset: 0x00004271
        // (set) Token: 0x0600003D RID: 61 RVA: 0x00006079 File Offset: 0x00004279
        public string banner { get; set; }

        // Token: 0x17000011 RID: 17
        // (get) Token: 0x0600003E RID: 62 RVA: 0x00006082 File Offset: 0x00004282
        // (set) Token: 0x0600003F RID: 63 RVA: 0x0000608A File Offset: 0x0000428A
        public string[] shields { get; set; }

        // Token: 0x17000012 RID: 18
        // (get) Token: 0x06000040 RID: 64 RVA: 0x00006093 File Offset: 0x00004293
        // (set) Token: 0x06000041 RID: 65 RVA: 0x0000609B File Offset: 0x0000429B
        public string[] banners { get; set; }

        // Token: 0x17000013 RID: 19
        // (get) Token: 0x06000042 RID: 66 RVA: 0x000060A4 File Offset: 0x000042A4
        // (set) Token: 0x06000043 RID: 67 RVA: 0x000060AC File Offset: 0x000042AC
        public string BearRulerColors { get; set; }

        // Token: 0x17000014 RID: 20
        // (get) Token: 0x06000044 RID: 68 RVA: 0x000060B5 File Offset: 0x000042B5
        // (set) Token: 0x06000045 RID: 69 RVA: 0x000060BD File Offset: 0x000042BD
        public string BearRulerBanner { get; set; }

        // Token: 0x17000015 RID: 21
        // (get) Token: 0x06000046 RID: 70 RVA: 0x000060C6 File Offset: 0x000042C6
        // (set) Token: 0x06000047 RID: 71 RVA: 0x000060CE File Offset: 0x000042CE
        public string BearRulerShield { get; set; }

        // Token: 0x17000016 RID: 22
        // (get) Token: 0x06000048 RID: 72 RVA: 0x000060D7 File Offset: 0x000042D7
        // (set) Token: 0x06000049 RID: 73 RVA: 0x000060DF File Offset: 0x000042DF
        public int? rulerMode { get; set; }

        // Token: 0x17000017 RID: 23
        // (get) Token: 0x0600004A RID: 74 RVA: 0x000060E8 File Offset: 0x000042E8
        // (set) Token: 0x0600004B RID: 75 RVA: 0x000060F0 File Offset: 0x000042F0
        public string[] rulerColor { get; set; }

        // Token: 0x17000018 RID: 24
        // (get) Token: 0x0600004C RID: 76 RVA: 0x000060F9 File Offset: 0x000042F9
        // (set) Token: 0x0600004D RID: 77 RVA: 0x00006101 File Offset: 0x00004301
        public string[] rulerColor2 { get; set; }

        // Token: 0x17000019 RID: 25
        // (get) Token: 0x0600004E RID: 78 RVA: 0x0000610A File Offset: 0x0000430A
        // (set) Token: 0x0600004F RID: 79 RVA: 0x00006112 File Offset: 0x00004312
        public string rulerBanner { get; set; }

        // Token: 0x1700001A RID: 26
        // (get) Token: 0x06000050 RID: 80 RVA: 0x0000611B File Offset: 0x0000431B
        // (set) Token: 0x06000051 RID: 81 RVA: 0x00006123 File Offset: 0x00004323
        public string rulerShield { get; set; }

        // Token: 0x1700001B RID: 27
        // (get) Token: 0x06000052 RID: 82 RVA: 0x0000612C File Offset: 0x0000432C
        // (set) Token: 0x06000053 RID: 83 RVA: 0x00006134 File Offset: 0x00004334
        public string[] combatShields { get; set; }

        // Token: 0x1700001C RID: 28
        // (get) Token: 0x06000054 RID: 84 RVA: 0x0000613D File Offset: 0x0000433D
        // (set) Token: 0x06000055 RID: 85 RVA: 0x00006145 File Offset: 0x00004345
        public string[] combatBanners { get; set; }

        // Token: 0x1700001D RID: 29
        // (get) Token: 0x06000056 RID: 86 RVA: 0x0000614E File Offset: 0x0000434E
        // (set) Token: 0x06000057 RID: 87 RVA: 0x00006156 File Offset: 0x00004356
        public Map<string, string[]>[] vars { get; set; }
    }
}
