using System;

namespace PocColor.Config
{
    // Token: 0x0200000B RID: 11
    public class PocColorModGroupEntry : PocColorModConfigUnit
    {
        // Token: 0x1700001E RID: 30
        // (get) Token: 0x06000059 RID: 89 RVA: 0x00006168 File Offset: 0x00004368
        // (set) Token: 0x0600005A RID: 90 RVA: 0x00006170 File Offset: 0x00004370
        public string[] units { get; set; }

        // Token: 0x0600005B RID: 91 RVA: 0x0000617C File Offset: 0x0000437C
        public PocColorModConfigUnit toUnit()
        {
            return new PocColorModConfigUnit
            {
                mode = base.mode,
                color = base.color,
                color2 = base.color2,
                banner = base.banner,
                banners = base.banners,
                shields = base.shields,
                combatBanners = base.combatBanners,
                combatShields = base.combatShields,
                BearRulerColors = base.BearRulerColors,
                BearRulerBanner = base.BearRulerBanner,
                BearRulerShield = base.BearRulerShield,
                rulerMode = base.rulerMode,
                rulerColor = base.rulerColor,
                rulerColor2 = base.rulerColor2,
                rulerBanner = base.rulerBanner,
                rulerShield = base.rulerShield,
                cultures = base.cultures,
                tiers = base.tiers,
                types = base.types
            };
        }

        // Token: 0x0600005C RID: 92 RVA: 0x0000628C File Offset: 0x0000448C
        public Map<string, PocColorModConfigUnit> toUnits()
        {
            Map<string, PocColorModConfigUnit> unitMap = new Map<string, PocColorModConfigUnit>();
            PocColorModConfigUnit unitConfig = this.toUnit();
            bool flag = this.units != null;
            if (flag)
            {
                foreach (string unit in this.units)
                {
                    unitMap[unit] = unitConfig;
                }
            }
            return unitMap;
        }
    }
}
