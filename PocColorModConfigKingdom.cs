using System;

namespace PocColor.Config
{
    // Token: 0x02000012 RID: 18
    public class PocColorModConfigKingdom : ConfigSection, UnitsContainerSection
    {
        // Token: 0x1700004C RID: 76
        // (get) Token: 0x060000CF RID: 207 RVA: 0x000095AD File Offset: 0x000077AD
        // (set) Token: 0x060000D0 RID: 208 RVA: 0x000095B5 File Offset: 0x000077B5
        public new int? mode { get; set; }

        // Token: 0x1700004D RID: 77
        // (get) Token: 0x060000D1 RID: 209 RVA: 0x000095BE File Offset: 0x000077BE
        // (set) Token: 0x060000D2 RID: 210 RVA: 0x000095C6 File Offset: 0x000077C6
        public new string[] color { get; set; }

        // Token: 0x1700004E RID: 78
        // (get) Token: 0x060000D3 RID: 211 RVA: 0x000095CF File Offset: 0x000077CF
        // (set) Token: 0x060000D4 RID: 212 RVA: 0x000095D7 File Offset: 0x000077D7
        public new string[] color2 { get; set; }

        // Token: 0x1700004F RID: 79
        // (get) Token: 0x060000D5 RID: 213 RVA: 0x000095E0 File Offset: 0x000077E0
        // (set) Token: 0x060000D6 RID: 214 RVA: 0x000095E8 File Offset: 0x000077E8
        public string clanBanner { get; set; }

        // Token: 0x17000050 RID: 80
        // (get) Token: 0x060000D7 RID: 215 RVA: 0x000095F1 File Offset: 0x000077F1
        // (set) Token: 0x060000D8 RID: 216 RVA: 0x000095F9 File Offset: 0x000077F9
        public string clanShield { get; set; }

        // Token: 0x17000051 RID: 81
        // (get) Token: 0x060000D9 RID: 217 RVA: 0x00009602 File Offset: 0x00007802
        // (set) Token: 0x060000DA RID: 218 RVA: 0x0000960A File Offset: 0x0000780A
        public string kingdomBanner { get; set; }

        // Token: 0x17000052 RID: 82
        // (get) Token: 0x060000DB RID: 219 RVA: 0x00009613 File Offset: 0x00007813
        // (set) Token: 0x060000DC RID: 220 RVA: 0x0000961B File Offset: 0x0000781B
        public string kingdomShield { get; set; }

        // Token: 0x17000053 RID: 83
        // (get) Token: 0x060000DD RID: 221 RVA: 0x00009624 File Offset: 0x00007824
        // (set) Token: 0x060000DE RID: 222 RVA: 0x0000962C File Offset: 0x0000782C
        public string primaryColor { get; set; }

        // Token: 0x17000054 RID: 84
        // (get) Token: 0x060000DF RID: 223 RVA: 0x00009635 File Offset: 0x00007835
        // (set) Token: 0x060000E0 RID: 224 RVA: 0x0000963D File Offset: 0x0000783D
        public string secondaryColor { get; set; }

        // Token: 0x17000055 RID: 85
        // (get) Token: 0x060000E1 RID: 225 RVA: 0x00009646 File Offset: 0x00007846
        // (set) Token: 0x060000E2 RID: 226 RVA: 0x0000964E File Offset: 0x0000784E
        public string FollowKingdomColors { get; set; }

        // Token: 0x17000056 RID: 86
        // (get) Token: 0x060000E3 RID: 227 RVA: 0x00009657 File Offset: 0x00007857
        // (set) Token: 0x060000E4 RID: 228 RVA: 0x0000965F File Offset: 0x0000785F
        public string FollowKingdomBackgroundOnly { get; set; }

        // Token: 0x17000057 RID: 87
        // (get) Token: 0x060000E5 RID: 229 RVA: 0x00009668 File Offset: 0x00007868
        // (set) Token: 0x060000E6 RID: 230 RVA: 0x00009670 File Offset: 0x00007870
        public new string banner { get; set; }

        // Token: 0x17000058 RID: 88
        // (get) Token: 0x060000E7 RID: 231 RVA: 0x00009679 File Offset: 0x00007879
        // (set) Token: 0x060000E8 RID: 232 RVA: 0x00009681 File Offset: 0x00007881
        public new string[] shields { get; set; }

        // Token: 0x17000059 RID: 89
        // (get) Token: 0x060000E9 RID: 233 RVA: 0x0000968A File Offset: 0x0000788A
        // (set) Token: 0x060000EA RID: 234 RVA: 0x00009692 File Offset: 0x00007892
        public new string[] banners { get; set; }

        // Token: 0x1700005A RID: 90
        // (get) Token: 0x060000EB RID: 235 RVA: 0x0000969B File Offset: 0x0000789B
        // (set) Token: 0x060000EC RID: 236 RVA: 0x000096A3 File Offset: 0x000078A3
        public new string BearRulerColors { get; set; }

        // Token: 0x1700005B RID: 91
        // (get) Token: 0x060000ED RID: 237 RVA: 0x000096AC File Offset: 0x000078AC
        // (set) Token: 0x060000EE RID: 238 RVA: 0x000096B4 File Offset: 0x000078B4
        public new string BearRulerBanner { get; set; }

        // Token: 0x1700005C RID: 92
        // (get) Token: 0x060000EF RID: 239 RVA: 0x000096BD File Offset: 0x000078BD
        // (set) Token: 0x060000F0 RID: 240 RVA: 0x000096C5 File Offset: 0x000078C5
        public new string BearRulerShield { get; set; }

        // Token: 0x1700005D RID: 93
        // (get) Token: 0x060000F1 RID: 241 RVA: 0x000096CE File Offset: 0x000078CE
        // (set) Token: 0x060000F2 RID: 242 RVA: 0x000096D6 File Offset: 0x000078D6
        public new int? rulerMode { get; set; }

        // Token: 0x1700005E RID: 94
        // (get) Token: 0x060000F3 RID: 243 RVA: 0x000096DF File Offset: 0x000078DF
        // (set) Token: 0x060000F4 RID: 244 RVA: 0x000096E7 File Offset: 0x000078E7
        public new string[] rulerColor { get; set; }

        // Token: 0x1700005F RID: 95
        // (get) Token: 0x060000F5 RID: 245 RVA: 0x000096F0 File Offset: 0x000078F0
        // (set) Token: 0x060000F6 RID: 246 RVA: 0x000096F8 File Offset: 0x000078F8
        public new string[] rulerColor2 { get; set; }

        // Token: 0x17000060 RID: 96
        // (get) Token: 0x060000F7 RID: 247 RVA: 0x00009701 File Offset: 0x00007901
        // (set) Token: 0x060000F8 RID: 248 RVA: 0x00009709 File Offset: 0x00007909
        public new string rulerBanner { get; set; }

        // Token: 0x17000061 RID: 97
        // (get) Token: 0x060000F9 RID: 249 RVA: 0x00009712 File Offset: 0x00007912
        // (set) Token: 0x060000FA RID: 250 RVA: 0x0000971A File Offset: 0x0000791A
        public new string rulerShield { get; set; }

        // Token: 0x17000062 RID: 98
        // (get) Token: 0x060000FB RID: 251 RVA: 0x00009723 File Offset: 0x00007923
        // (set) Token: 0x060000FC RID: 252 RVA: 0x0000972B File Offset: 0x0000792B
        public new string[] combatShields { get; set; }

        // Token: 0x17000063 RID: 99
        // (get) Token: 0x060000FD RID: 253 RVA: 0x00009734 File Offset: 0x00007934
        // (set) Token: 0x060000FE RID: 254 RVA: 0x0000973C File Offset: 0x0000793C
        public new string[] combatBanners { get; set; }

        // Token: 0x17000064 RID: 100
        // (get) Token: 0x060000FF RID: 255 RVA: 0x00009745 File Offset: 0x00007945
        // (set) Token: 0x06000100 RID: 256 RVA: 0x0000974D File Offset: 0x0000794D
        public new Map<string, string[]>[] vars { get; set; }

        // Token: 0x17000065 RID: 101
        // (get) Token: 0x06000101 RID: 257 RVA: 0x00009756 File Offset: 0x00007956
        // (set) Token: 0x06000102 RID: 258 RVA: 0x0000975E File Offset: 0x0000795E
        public Map<string, PocColorModConfigUnit> units { get; set; }

        // Token: 0x17000066 RID: 102
        // (get) Token: 0x06000103 RID: 259 RVA: 0x00009767 File Offset: 0x00007967
        // (set) Token: 0x06000104 RID: 260 RVA: 0x0000976F File Offset: 0x0000796F
        public Map<string, PocColorModConfigClan> clans { get; set; }

        // Token: 0x17000067 RID: 103
        // (get) Token: 0x06000105 RID: 261 RVA: 0x00009778 File Offset: 0x00007978
        // (set) Token: 0x06000106 RID: 262 RVA: 0x00009780 File Offset: 0x00007980
        public PocColorModGroupEntry[] groups { get; set; }
    }
}
