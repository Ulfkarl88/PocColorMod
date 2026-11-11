using System;

namespace PocColor.Config
{
    // Token: 0x02000011 RID: 17
    public class PocColorModConfigClan : ConfigSection, UnitsContainerSection
    {
        // Token: 0x17000033 RID: 51
        // (get) Token: 0x0600009C RID: 156 RVA: 0x000093FB File Offset: 0x000075FB
        // (set) Token: 0x0600009D RID: 157 RVA: 0x00009403 File Offset: 0x00007603
        public new int? mode { get; set; }

        // Token: 0x17000034 RID: 52
        // (get) Token: 0x0600009E RID: 158 RVA: 0x0000940C File Offset: 0x0000760C
        // (set) Token: 0x0600009F RID: 159 RVA: 0x00009414 File Offset: 0x00007614
        public new string[] color { get; set; }

        // Token: 0x17000035 RID: 53
        // (get) Token: 0x060000A0 RID: 160 RVA: 0x0000941D File Offset: 0x0000761D
        // (set) Token: 0x060000A1 RID: 161 RVA: 0x00009425 File Offset: 0x00007625
        public new string[] color2 { get; set; }

        // Token: 0x1700004C RID: 76  
        // (get) Token: 0x060000CE RID: 206
        // (set) Token: 0x060000CF RID: 207
        public bool useAdvancedConfig { get; set; }

        // Token: 0x17000036 RID: 54
        // (get) Token: 0x060000A2 RID: 162 RVA: 0x0000942E File Offset: 0x0000762E
        // (set) Token: 0x060000A3 RID: 163 RVA: 0x00009436 File Offset: 0x00007636
        public string clanBanner { get; set; }

        // Token: 0x17000037 RID: 55
        // (get) Token: 0x060000A4 RID: 164 RVA: 0x0000943F File Offset: 0x0000763F
        // (set) Token: 0x060000A5 RID: 165 RVA: 0x00009447 File Offset: 0x00007647
        public string clanShield { get; set; }

        // Token: 0x17000038 RID: 56
        // (get) Token: 0x060000A6 RID: 166 RVA: 0x00009450 File Offset: 0x00007650
        // (set) Token: 0x060000A7 RID: 167 RVA: 0x00009458 File Offset: 0x00007658
        public string primaryColor { get; set; }

        // Token: 0x17000039 RID: 57
        // (get) Token: 0x060000A8 RID: 168 RVA: 0x00009461 File Offset: 0x00007661
        // (set) Token: 0x060000A9 RID: 169 RVA: 0x00009469 File Offset: 0x00007669
        public string secondaryColor { get; set; }

        // Token: 0x1700003A RID: 58
        // (get) Token: 0x060000AA RID: 170 RVA: 0x00009472 File Offset: 0x00007672
        // (set) Token: 0x060000AB RID: 171 RVA: 0x0000947A File Offset: 0x0000767A
        public string FollowKingdomColors { get; set; }

        // Token: 0x1700003B RID: 59
        // (get) Token: 0x060000AC RID: 172 RVA: 0x00009483 File Offset: 0x00007683
        // (set) Token: 0x060000AD RID: 173 RVA: 0x0000948B File Offset: 0x0000768B
        public string FollowKingdomBackgroundOnly { get; set; }

        // Token: 0x1700003C RID: 60
        // (get) Token: 0x060000AE RID: 174 RVA: 0x00009494 File Offset: 0x00007694
        // (set) Token: 0x060000AF RID: 175 RVA: 0x0000949C File Offset: 0x0000769C
        public new string banner { get; set; }

        // Token: 0x1700003D RID: 61
        // (get) Token: 0x060000B0 RID: 176 RVA: 0x000094A5 File Offset: 0x000076A5
        // (set) Token: 0x060000B1 RID: 177 RVA: 0x000094AD File Offset: 0x000076AD
        public new string[] shields { get; set; }

        // Token: 0x1700003E RID: 62
        // (get) Token: 0x060000B2 RID: 178 RVA: 0x000094B6 File Offset: 0x000076B6
        // (set) Token: 0x060000B3 RID: 179 RVA: 0x000094BE File Offset: 0x000076BE
        public new string[] banners { get; set; }

        // Token: 0x1700003F RID: 63
        // (get) Token: 0x060000B4 RID: 180 RVA: 0x000094C7 File Offset: 0x000076C7
        // (set) Token: 0x060000B5 RID: 181 RVA: 0x000094CF File Offset: 0x000076CF
        public new string BearRulerColors { get; set; }

        // Token: 0x17000040 RID: 64
        // (get) Token: 0x060000B6 RID: 182 RVA: 0x000094D8 File Offset: 0x000076D8
        // (set) Token: 0x060000B7 RID: 183 RVA: 0x000094E0 File Offset: 0x000076E0
        public new string BearRulerBanner { get; set; }

        // Token: 0x17000041 RID: 65
        // (get) Token: 0x060000B8 RID: 184 RVA: 0x000094E9 File Offset: 0x000076E9
        // (set) Token: 0x060000B9 RID: 185 RVA: 0x000094F1 File Offset: 0x000076F1
        public new string BearRulerShield { get; set; }

        // Token: 0x17000042 RID: 66
        // (get) Token: 0x060000BA RID: 186 RVA: 0x000094FA File Offset: 0x000076FA
        // (set) Token: 0x060000BB RID: 187 RVA: 0x00009502 File Offset: 0x00007702
        public new int? rulerMode { get; set; }

        // Token: 0x17000043 RID: 67
        // (get) Token: 0x060000BC RID: 188 RVA: 0x0000950B File Offset: 0x0000770B
        // (set) Token: 0x060000BD RID: 189 RVA: 0x00009513 File Offset: 0x00007713
        public new string[] rulerColor { get; set; }

        // Token: 0x17000044 RID: 68
        // (get) Token: 0x060000BE RID: 190 RVA: 0x0000951C File Offset: 0x0000771C
        // (set) Token: 0x060000BF RID: 191 RVA: 0x00009524 File Offset: 0x00007724
        public new string[] rulerColor2 { get; set; }

        // Token: 0x17000045 RID: 69
        // (get) Token: 0x060000C0 RID: 192 RVA: 0x0000952D File Offset: 0x0000772D
        // (set) Token: 0x060000C1 RID: 193 RVA: 0x00009535 File Offset: 0x00007735
        public new string rulerBanner { get; set; }

        // Token: 0x17000046 RID: 70
        // (get) Token: 0x060000C2 RID: 194 RVA: 0x0000953E File Offset: 0x0000773E
        // (set) Token: 0x060000C3 RID: 195 RVA: 0x00009546 File Offset: 0x00007746
        public new string rulerShield { get; set; }

        // Token: 0x17000047 RID: 71
        // (get) Token: 0x060000C4 RID: 196 RVA: 0x0000954F File Offset: 0x0000774F
        // (set) Token: 0x060000C5 RID: 197 RVA: 0x00009557 File Offset: 0x00007757
        public new string[] combatShields { get; set; }

        // Token: 0x17000048 RID: 72
        // (get) Token: 0x060000C6 RID: 198 RVA: 0x00009560 File Offset: 0x00007760
        // (set) Token: 0x060000C7 RID: 199 RVA: 0x00009568 File Offset: 0x00007768
        public new string[] combatBanners { get; set; }

        // Token: 0x17000049 RID: 73
        // (get) Token: 0x060000C8 RID: 200 RVA: 0x00009571 File Offset: 0x00007771
        // (set) Token: 0x060000C9 RID: 201 RVA: 0x00009579 File Offset: 0x00007779
        public new Map<string, string[]>[] vars { get; set; }

        // Token: 0x1700004A RID: 74
        // (get) Token: 0x060000CA RID: 202 RVA: 0x00009582 File Offset: 0x00007782
        // (set) Token: 0x060000CB RID: 203 RVA: 0x0000958A File Offset: 0x0000778A
        public Map<string, PocColorModConfigUnit> units { get; set; }

        // Token: 0x1700004B RID: 75
        // (get) Token: 0x060000CC RID: 204 RVA: 0x00009593 File Offset: 0x00007793
        // (set) Token: 0x060000CD RID: 205 RVA: 0x0000959B File Offset: 0x0000779B
        public PocColorModGroupEntry[] groups { get; set; }
    }
}