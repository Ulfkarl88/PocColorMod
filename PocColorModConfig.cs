using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;

namespace PocColor.Config
{
    // Token: 0x02000010 RID: 16
    public class PocColorModConfig : UnitsContainerSection
    {
        // Token: 0x1700002C RID: 44
        // (get) Token: 0x0600007D RID: 125 RVA: 0x00006511 File Offset: 0x00004711
        // (set) Token: 0x0600007E RID: 126 RVA: 0x00006519 File Offset: 0x00004719
        public bool log { get; set; } = false;

        // Token: 0x1700002D RID: 45
        // (get) Token: 0x0600007F RID: 127 RVA: 0x00006522 File Offset: 0x00004722
        // (set) Token: 0x06000080 RID: 128 RVA: 0x0000652A File Offset: 0x0000472A
        public bool generateTemplate { get; set; } = false;

        // Token: 0x1700002E RID: 46
        // (get) Token: 0x06000081 RID: 129 RVA: 0x00006533 File Offset: 0x00004733
        // (set) Token: 0x06000082 RID: 130 RVA: 0x0000653B File Offset: 0x0000473B
        public PocColorModConfigColorEntry defaultConfig { get; set; }

        // Token: 0x1700002F RID: 47
        // (get) Token: 0x06000083 RID: 131 RVA: 0x00006544 File Offset: 0x00004744
        // (set) Token: 0x06000084 RID: 132 RVA: 0x0000654C File Offset: 0x0000474C
        public Map<string, PocColorModConfigKingdom> kingdoms { get; set; }

        // Token: 0x17000030 RID: 48
        // (get) Token: 0x06000085 RID: 133 RVA: 0x00006555 File Offset: 0x00004755
        // (set) Token: 0x06000086 RID: 134 RVA: 0x0000655D File Offset: 0x0000475D
        public Map<string, PocColorModConfigClan> clans { get; set; }

        // Token: 0x17000031 RID: 49
        // (get) Token: 0x06000087 RID: 135 RVA: 0x00006566 File Offset: 0x00004766
        // (set) Token: 0x06000088 RID: 136 RVA: 0x0000656E File Offset: 0x0000476E
        public Map<string, PocColorModConfigUnit> units { get; set; }

        // Token: 0x17000032 RID: 50
        // (get) Token: 0x06000089 RID: 137 RVA: 0x00006577 File Offset: 0x00004777
        // (set) Token: 0x0600008A RID: 138 RVA: 0x0000657F File Offset: 0x0000477F
        public PocColorModGroupEntry[] groups { get; set; }

        // Token: 0x0600008B RID: 139 RVA: 0x00006588 File Offset: 0x00004788
        public ValueTuple<bool, bool, string, string, string, string> GetClanConfig(string kingdom, string clan, bool isPlayerKingdom, bool isPlayerClan)
        {
            string follow = null;
            string followBG = "false";
            string banner = null;
            string shield = null;
            string primaryColor = null;
            string secondaryColor = null;
            bool success = false;
            bool successBG = false;
            try
            {
                Map<string, PocColorModConfigKingdom> kingdoms = this.kingdoms;
                PocColorModConfigKingdom kConfig = (kingdoms != null) ? kingdoms[kingdom] : null;
                bool flag = kConfig == null && isPlayerKingdom;
                if (flag)
                {
                    Map<string, PocColorModConfigKingdom> kingdoms2 = this.kingdoms;
                    kConfig = ((kingdoms2 != null) ? kingdoms2["PlayerKingdom"] : null);
                }
                Map<string, PocColorModConfigClan> clans = this.clans;
                PocColorModConfigClan cConfig = (clans != null) ? clans[clan] : null;
                bool flag2 = cConfig == null && isPlayerClan;
                if (flag2)
                {
                    Map<string, PocColorModConfigClan> clans2 = this.clans;
                    cConfig = ((clans2 != null) ? clans2["PlayerClan"] : null);
                }
                PocColorModConfigClan pocColorModConfigClan;
                if (kConfig == null)
                {
                    pocColorModConfigClan = null;
                }
                else
                {
                    Map<string, PocColorModConfigClan> clans3 = kConfig.clans;
                    pocColorModConfigClan = ((clans3 != null) ? clans3[clan] : null);
                }
                PocColorModConfigClan kcConfig = pocColorModConfigClan;
                bool flag3 = kcConfig == null && isPlayerClan;
                if (flag3)
                {
                    PocColorModConfigClan pocColorModConfigClan2;
                    if (kConfig == null)
                    {
                        pocColorModConfigClan2 = null;
                    }
                    else
                    {
                        Map<string, PocColorModConfigClan> clans4 = kConfig.clans;
                        pocColorModConfigClan2 = ((clans4 != null) ? clans4["PlayerClan"] : null);
                    }
                    kcConfig = pocColorModConfigClan2;
                }
                banner = (((kConfig != null) ? kConfig.clanBanner : null) ?? banner);
                banner = (((cConfig != null) ? cConfig.clanBanner : null) ?? banner);
                banner = (((kcConfig != null) ? kcConfig.clanBanner : null) ?? banner);
                shield = (((kConfig != null) ? kConfig.clanShield : null) ?? shield);
                shield = (((cConfig != null) ? cConfig.clanShield : null) ?? shield);
                shield = (((kcConfig != null) ? kcConfig.clanShield : null) ?? shield);
                PocColorModConfigColorEntry defaultConfig = this.defaultConfig;
                follow = (((defaultConfig != null) ? defaultConfig.FollowKingdomColors : null) ?? follow);
                follow = (((kConfig != null) ? kConfig.FollowKingdomColors : null) ?? follow);
                follow = (((cConfig != null) ? cConfig.FollowKingdomColors : null) ?? follow);
                follow = (((kcConfig != null) ? kcConfig.FollowKingdomColors : null) ?? follow);
                bool flag4 = follow == null;
                if (flag4)
                {
                    bool flag5 = banner != null;
                    if (flag5)
                    {
                        follow = "false";
                    }
                    else
                    {
                        follow = "true";
                    }
                }
                bool couldParse = bool.TryParse(follow.ToLower(), out success);
                PocColorModConfigColorEntry defaultConfig2 = this.defaultConfig;
                followBG = (((defaultConfig2 != null) ? defaultConfig2.FollowKingdomBackgroundOnly : null) ?? followBG);
                followBG = (((kConfig != null) ? kConfig.FollowKingdomBackgroundOnly : null) ?? followBG);
                followBG = (((cConfig != null) ? cConfig.FollowKingdomBackgroundOnly : null) ?? followBG);
                followBG = (((kcConfig != null) ? kcConfig.FollowKingdomBackgroundOnly : null) ?? followBG);
                couldParse = bool.TryParse(followBG.ToLower(), out successBG);
                primaryColor = (((cConfig != null) ? cConfig.primaryColor : null) ?? primaryColor);
                primaryColor = (((kcConfig != null) ? kcConfig.primaryColor : null) ?? primaryColor);
                secondaryColor = (((cConfig != null) ? cConfig.secondaryColor : null) ?? secondaryColor);
                secondaryColor = (((kcConfig != null) ? kcConfig.secondaryColor : null) ?? secondaryColor);
                PocColorModConfigColorEntry defaultConfig3 = this.defaultConfig;
                Map<string, string[]>[] vars = ((defaultConfig3 != null) ? defaultConfig3.vars : null) ?? null;
                vars = (((kConfig != null) ? kConfig.vars : null) ?? vars);
                vars = (((cConfig != null) ? cConfig.vars : null) ?? vars);
                vars = (((kcConfig != null) ? kcConfig.vars : null) ?? vars);
                Map<string, string> values = null;
                bool flag6 = vars != null;
                if (flag6)
                {
                    values = this.GetVarsValues(vars);
                }
                bool flag7 = values != null;
                if (flag7)
                {
                    banner = this.resolveBanner(banner, values);
                    shield = this.resolveBanner(shield, values);
                }
            }
            catch (Exception e)
            {
                Log.write(e.Message);
            }
            return new ValueTuple<bool, bool, string, string, string, string>(success, successBG, banner, shield, primaryColor, secondaryColor);
        }

        // Token: 0x0600008C RID: 140 RVA: 0x00006908 File Offset: 0x00004B08
        public ValueTuple<string, string, string, string> GetKingdomConfig(string kingdom, bool isPlayerKingdom)
        {
            string banner = null;
            string shield = null;
            string primaryColor = null;
            string secondaryColor = null;
            try
            {
                Map<string, PocColorModConfigKingdom> kingdoms = this.kingdoms;
                PocColorModConfigKingdom kConfig = (kingdoms != null) ? kingdoms[kingdom] : null;
                bool flag = kConfig == null && isPlayerKingdom;
                if (flag)
                {
                    Map<string, PocColorModConfigKingdom> kingdoms2 = this.kingdoms;
                    kConfig = ((kingdoms2 != null) ? kingdoms2["PlayerKingdom"] : null);
                }
                banner = (((kConfig != null) ? kConfig.kingdomBanner : null) ?? banner);
                shield = (((kConfig != null) ? kConfig.kingdomShield : null) ?? shield);
                primaryColor = (((kConfig != null) ? kConfig.primaryColor : null) ?? primaryColor);
                secondaryColor = (((kConfig != null) ? kConfig.secondaryColor : null) ?? secondaryColor);
                PocColorModConfigColorEntry defaultConfig = this.defaultConfig;
                Map<string, string[]>[] vars = ((defaultConfig != null) ? defaultConfig.vars : null) ?? null;
                vars = (((kConfig != null) ? kConfig.vars : null) ?? vars);
                Map<string, string> values = null;
                bool flag2 = vars != null;
                if (flag2)
                {
                    values = this.GetVarsValues(vars);
                }
                bool flag3 = values != null;
                if (flag3)
                {
                    banner = this.resolveBanner(banner, values);
                    shield = this.resolveBanner(shield, values);
                }
            }
            catch (Exception e)
            {
                Log.write(e.Message);
            }
            return new ValueTuple<string, string, string, string>(banner, shield, primaryColor, secondaryColor);
        }

        // Token: 0x0600008D RID: 141 RVA: 0x00006A44 File Offset: 0x00004C44
        public ValueTuple<bool, int, string[], string[], string[], string[]> GetRulerConfig(string clan, int modeOrig, string[] colorOrig, string[] colorOrig2, string[] bannerOrig, string[] shieldOrig, PocColorModConfigKingdom kConfig, PocColorModConfigClan kcConfig, PocColorModConfigUnit kuConfig, PocColorModConfigUnit kcuConfig, PocColorModConfigClan cConfig, PocColorModConfigUnit cuConfig, PocColorModConfigUnit uConfig)
        {
            int mode = modeOrig;
            string[] color = colorOrig;
            string[] color2 = colorOrig2;
            string[] banners = bannerOrig;
            string[] shields = shieldOrig;
            PocColorModConfigColorEntry defaultConfig = this.defaultConfig;
            string bearRulerColorsStr = ((defaultConfig != null) ? defaultConfig.BearRulerColors : null) ?? "true";
            PocColorModConfigColorEntry defaultConfig2 = this.defaultConfig;
            string bearRulerBannerStr = ((defaultConfig2 != null) ? defaultConfig2.BearRulerBanner : null) ?? "true";
            PocColorModConfigColorEntry defaultConfig3 = this.defaultConfig;
            string bearRulerShieldStr = ((defaultConfig3 != null) ? defaultConfig3.BearRulerShield : null) ?? "true";
            bool isRuler = false;
            string kingdomBanner = null;
            try
            {
                bool flag = clan != null && !clan.Equals("");
                if (flag)
                {
                    foreach (Clan clanObj in Campaign.Current.Clans)
                    {
                        bool flag2 = clanObj.Name.ToString().Equals(clan);
                        if (flag2)
                        {
                            Kingdom kingdom = clanObj.Kingdom;
                            bool flag3 = ((kingdom != null) ? kingdom.RulingClan : null) == clanObj;
                            if (flag3)
                            {
                                isRuler = true;
                                kingdomBanner = PocColorMod.SerializeBanner(clanObj.Kingdom.Banner);
                            }
                        }
                    }
                }
                bool flag4 = !isRuler;
                if (flag4)
                {
                    return new ValueTuple<bool, int, string[], string[], string[], string[]>(false, mode, color, color2, banners, shields);
                }
                bearRulerColorsStr = (((kConfig != null) ? kConfig.BearRulerColors : null) ?? bearRulerColorsStr);
                bearRulerColorsStr = (((cConfig != null) ? cConfig.BearRulerColors : null) ?? bearRulerColorsStr);
                bearRulerColorsStr = (((kcConfig != null) ? kcConfig.BearRulerColors : null) ?? bearRulerColorsStr);
                bearRulerColorsStr = (((uConfig != null) ? uConfig.BearRulerColors : null) ?? bearRulerColorsStr);
                bearRulerColorsStr = (((kuConfig != null) ? kuConfig.BearRulerColors : null) ?? bearRulerColorsStr);
                bearRulerColorsStr = (((cuConfig != null) ? cuConfig.BearRulerColors : null) ?? bearRulerColorsStr);
                bearRulerColorsStr = (((kcuConfig != null) ? kcuConfig.BearRulerColors : null) ?? bearRulerColorsStr);
                bool bearRulerColors;
                bool.TryParse(bearRulerColorsStr.ToLower(), out bearRulerColors);
                bearRulerBannerStr = (((kConfig != null) ? kConfig.BearRulerBanner : null) ?? bearRulerBannerStr);
                bearRulerBannerStr = (((cConfig != null) ? cConfig.BearRulerBanner : null) ?? bearRulerBannerStr);
                bearRulerBannerStr = (((kcConfig != null) ? kcConfig.BearRulerBanner : null) ?? bearRulerBannerStr);
                bearRulerBannerStr = (((uConfig != null) ? uConfig.BearRulerBanner : null) ?? bearRulerBannerStr);
                bearRulerBannerStr = (((kuConfig != null) ? kuConfig.BearRulerBanner : null) ?? bearRulerBannerStr);
                bearRulerBannerStr = (((cuConfig != null) ? cuConfig.BearRulerBanner : null) ?? bearRulerBannerStr);
                bearRulerBannerStr = (((kcuConfig != null) ? kcuConfig.BearRulerBanner : null) ?? bearRulerBannerStr);
                bool bearRulerBanner;
                bool.TryParse(bearRulerBannerStr.ToLower(), out bearRulerBanner);
                bearRulerShieldStr = (((kConfig != null) ? kConfig.BearRulerShield : null) ?? bearRulerShieldStr);
                bearRulerShieldStr = (((cConfig != null) ? cConfig.BearRulerShield : null) ?? bearRulerShieldStr);
                bearRulerShieldStr = (((kcConfig != null) ? kcConfig.BearRulerShield : null) ?? bearRulerShieldStr);
                bearRulerShieldStr = (((uConfig != null) ? uConfig.BearRulerShield : null) ?? bearRulerShieldStr);
                bearRulerShieldStr = (((kuConfig != null) ? kuConfig.BearRulerShield : null) ?? bearRulerShieldStr);
                bearRulerShieldStr = (((cuConfig != null) ? cuConfig.BearRulerShield : null) ?? bearRulerShieldStr);
                bearRulerShieldStr = (((kcuConfig != null) ? kcuConfig.BearRulerShield : null) ?? bearRulerShieldStr);
                bool bearRulerShield;
                bool.TryParse(bearRulerShieldStr.ToLower(), out bearRulerShield);
                bool flag5 = bearRulerColors;
                if (flag5)
                {
                    mode = (((kConfig != null) ? kConfig.rulerMode : null) ?? 2);
                    mode = (((cConfig != null) ? cConfig.rulerMode : null) ?? mode);
                    mode = (((kcConfig != null) ? kcConfig.rulerMode : null) ?? mode);
                    mode = (((uConfig != null) ? uConfig.rulerMode : null) ?? mode);
                    mode = (((kuConfig != null) ? kuConfig.rulerMode : null) ?? mode);
                    mode = (((cuConfig != null) ? cuConfig.rulerMode : null) ?? mode);
                    mode = (((kcuConfig != null) ? kcuConfig.rulerMode : null) ?? mode);
                    color = ((kConfig != null) ? kConfig.rulerColor : null);
                    color = (((cConfig != null) ? cConfig.rulerColor : null) ?? color);
                    color = (((kcConfig != null) ? kcConfig.rulerColor : null) ?? color);
                    color = (((uConfig != null) ? uConfig.rulerColor : null) ?? color);
                    color = (((kuConfig != null) ? kuConfig.rulerColor : null) ?? color);
                    color = (((cuConfig != null) ? cuConfig.rulerColor : null) ?? color);
                    color = (((kcuConfig != null) ? kcuConfig.rulerColor : null) ?? color);
                    color2 = ((kConfig != null) ? kConfig.rulerColor2 : null);
                    color2 = (((cConfig != null) ? cConfig.rulerColor2 : null) ?? color2);
                    color2 = (((kcConfig != null) ? kcConfig.rulerColor2 : null) ?? color2);
                    color2 = (((uConfig != null) ? uConfig.rulerColor2 : null) ?? color2);
                    color2 = (((kuConfig != null) ? kuConfig.rulerColor2 : null) ?? color2);
                    color2 = (((cuConfig != null) ? cuConfig.rulerColor2 : null) ?? color2);
                    color2 = (((kcuConfig != null) ? kcuConfig.rulerColor2 : null) ?? color2);
                }
                bool flag6 = bearRulerBanner;
                if (flag6)
                {
                    string banner = kingdomBanner;
                    banner = (((kConfig != null) ? kConfig.rulerBanner : null) ?? banner);
                    banner = (((cConfig != null) ? cConfig.rulerBanner : null) ?? banner);
                    banner = (((kcConfig != null) ? kcConfig.rulerBanner : null) ?? banner);
                    banner = (((uConfig != null) ? uConfig.rulerBanner : null) ?? banner);
                    banner = (((kuConfig != null) ? kuConfig.rulerBanner : null) ?? banner);
                    banner = (((cuConfig != null) ? cuConfig.rulerBanner : null) ?? banner);
                    banner = (((kcuConfig != null) ? kcuConfig.rulerBanner : null) ?? banner);
                    banners = new string[]
                    {
                        banner
                    };
                }
                bool flag7 = bearRulerShield;
                if (flag7)
                {
                    string shield = kingdomBanner;
                    shield = (((kConfig != null) ? kConfig.rulerShield : null) ?? shield);
                    shield = (((cConfig != null) ? cConfig.rulerShield : null) ?? shield);
                    shield = (((kcConfig != null) ? kcConfig.rulerShield : null) ?? shield);
                    shield = (((uConfig != null) ? uConfig.rulerShield : null) ?? shield);
                    shield = (((kuConfig != null) ? kuConfig.rulerShield : null) ?? shield);
                    shield = (((cuConfig != null) ? cuConfig.rulerShield : null) ?? shield);
                    shield = (((kcuConfig != null) ? kcuConfig.rulerShield : null) ?? shield);
                    shields = new string[]
                    {
                        shield
                    };
                }
            }
            catch (Exception e)
            {
                Log.write(e.Message);
            }
            return new ValueTuple<bool, int, string[], string[], string[], string[]>(true, mode, color, color2, banners, shields);
        }

        // Token: 0x0600008E RID: 142 RVA: 0x000071A8 File Offset: 0x000053A8
        public ValueTuple<int, string[], string[], string[], string[]> GetConfig(string kingdom, string clan, string unit, bool isPlayerKingdom, bool isPlayerClan, bool isPlayer, bool isKing, bool isLeader, bool isHero, bool isMounted, bool isRanged, int tier, string culture)
        {
            int mode = 0;
            string[] color = null;
            string[] color2 = null;
            string[] shields = null;
            string[] bannersFull = null;
            PocColorModConfigKingdom kConfig = null;
            PocColorModConfigClan kcConfig = null;
            PocColorModConfigUnit kuConfig = null;
            PocColorModConfigUnit kcuConfig = null;
            PocColorModConfigClan cConfig = null;
            PocColorModConfigUnit cuConfig = null;
            PocColorModConfigUnit uConfig = null;
            try
            {
                ValueTuple<PocColorModConfigKingdom, PocColorModConfigClan, PocColorModConfigUnit, PocColorModConfigUnit, PocColorModConfigClan, PocColorModConfigUnit, PocColorModConfigUnit> configScopes = this.getConfigScopes(kConfig, kcConfig, kuConfig, kcuConfig, cConfig, cuConfig, uConfig, kingdom, clan, unit, isPlayerKingdom, isPlayerClan, isPlayer, isKing, isLeader, isHero, isMounted, isRanged, tier, culture);
                kConfig = configScopes.Item1;
                kcConfig = configScopes.Item2;
                kuConfig = configScopes.Item3;
                kcuConfig = configScopes.Item4;
                cConfig = configScopes.Item5;
                cuConfig = configScopes.Item6;
                uConfig = configScopes.Item7;
                PocColorModConfigColorEntry defaultConfig = this.defaultConfig;
                Map<string, string[]>[] vars = ((defaultConfig != null) ? defaultConfig.vars : null) ?? null;
                vars = (((kConfig != null) ? kConfig.vars : null) ?? vars);
                vars = (((cConfig != null) ? cConfig.vars : null) ?? vars);
                vars = (((kcConfig != null) ? kcConfig.vars : null) ?? vars);
                vars = (((uConfig != null) ? uConfig.vars : null) ?? vars);
                vars = (((kuConfig != null) ? kuConfig.vars : null) ?? vars);
                vars = (((cuConfig != null) ? cuConfig.vars : null) ?? vars);
                vars = (((kcuConfig != null) ? kcuConfig.vars : null) ?? vars);
                PocColorModConfigColorEntry defaultConfig2 = this.defaultConfig;
                mode = (((defaultConfig2 != null) ? defaultConfig2.mode : null) ?? 2);
                mode = (((kConfig != null) ? kConfig.mode : null) ?? mode);
                mode = (((cConfig != null) ? cConfig.mode : null) ?? mode);
                mode = (((kcConfig != null) ? kcConfig.mode : null) ?? mode);
                mode = (((uConfig != null) ? uConfig.mode : null) ?? mode);
                mode = (((kuConfig != null) ? kuConfig.mode : null) ?? mode);
                mode = (((cuConfig != null) ? cuConfig.mode : null) ?? mode);
                mode = (((kcuConfig != null) ? kcuConfig.mode : null) ?? mode);
                PocColorModConfigColorEntry defaultConfig3 = this.defaultConfig;
                color = ((defaultConfig3 != null) ? defaultConfig3.color : null);
                color = (((kConfig != null) ? kConfig.color : null) ?? color);
                color = (((cConfig != null) ? cConfig.color : null) ?? color);
                color = (((kcConfig != null) ? kcConfig.color : null) ?? color);
                color = (((uConfig != null) ? uConfig.color : null) ?? color);
                color = (((kuConfig != null) ? kuConfig.color : null) ?? color);
                color = (((cuConfig != null) ? cuConfig.color : null) ?? color);
                color = (((kcuConfig != null) ? kcuConfig.color : null) ?? color);
                PocColorModConfigColorEntry defaultConfig4 = this.defaultConfig;
                color2 = ((defaultConfig4 != null) ? defaultConfig4.color2 : null);
                color2 = (((kConfig != null) ? kConfig.color2 : null) ?? color2);
                color2 = (((cConfig != null) ? cConfig.color2 : null) ?? color2);
                color2 = (((kcConfig != null) ? kcConfig.color2 : null) ?? color2);
                color2 = (((uConfig != null) ? uConfig.color2 : null) ?? color2);
                color2 = (((kuConfig != null) ? kuConfig.color2 : null) ?? color2);
                color2 = (((cuConfig != null) ? cuConfig.color2 : null) ?? color2);
                color2 = (((kcuConfig != null) ? kcuConfig.color2 : null) ?? color2);
                string banner = (kConfig != null) ? kConfig.banner : null;
                banner = (((cConfig != null) ? cConfig.banner : null) ?? banner);
                banner = (((kcConfig != null) ? kcConfig.banner : null) ?? banner);
                banner = (((uConfig != null) ? uConfig.banner : null) ?? banner);
                banner = (((kuConfig != null) ? kuConfig.banner : null) ?? banner);
                banner = (((cuConfig != null) ? cuConfig.banner : null) ?? banner);
                banner = (((kcuConfig != null) ? kcuConfig.banner : null) ?? banner);
                shields = ((kConfig != null) ? kConfig.shields : null);
                shields = (((cConfig != null) ? cConfig.shields : null) ?? shields);
                shields = (((kcConfig != null) ? kcConfig.shields : null) ?? shields);
                shields = (((uConfig != null) ? uConfig.shields : null) ?? shields);
                shields = (((kuConfig != null) ? kuConfig.shields : null) ?? shields);
                shields = (((cuConfig != null) ? cuConfig.shields : null) ?? shields);
                shields = (((kcuConfig != null) ? kcuConfig.shields : null) ?? shields);
                string[] banners = (kConfig != null) ? kConfig.banners : null;
                banners = (((cConfig != null) ? cConfig.banners : null) ?? banners);
                banners = (((kcConfig != null) ? kcConfig.banners : null) ?? banners);
                banners = (((uConfig != null) ? uConfig.banners : null) ?? banners);
                banners = (((kuConfig != null) ? kuConfig.banners : null) ?? banners);
                banners = (((cuConfig != null) ? cuConfig.banners : null) ?? banners);
                banners = (((kcuConfig != null) ? kcuConfig.banners : null) ?? banners);
                ValueTuple<Map<string, string[]>[], int, string[], string[], string, string[], string[], ValueTuple<string[], string[]>> valueTuple = this.parseUnitsDetails(uConfig, isMounted, isRanged, tier, culture, vars, mode, color, color2, banner, shields, banners, null, null);
                vars = valueTuple.Item1;
                mode = valueTuple.Item2;
                color = valueTuple.Item3;
                color2 = valueTuple.Item4;
                banner = valueTuple.Item5;
                shields = valueTuple.Item6;
                banners = valueTuple.Item7;
                string[] combatShields = valueTuple.Rest.Item1;
                string[] combatBanners = valueTuple.Rest.Item2;
                ValueTuple<Map<string, string[]>[], int, string[], string[], string, string[], string[], ValueTuple<string[], string[]>> valueTuple2 = this.parseUnitsDetails(kuConfig, isMounted, isRanged, tier, culture, vars, mode, color, color2, banner, shields, banners, null, null);
                vars = valueTuple2.Item1;
                mode = valueTuple2.Item2;
                color = valueTuple2.Item3;
                color2 = valueTuple2.Item4;
                banner = valueTuple2.Item5;
                shields = valueTuple2.Item6;
                banners = valueTuple2.Item7;
                combatShields = valueTuple2.Rest.Item1;
                combatBanners = valueTuple2.Rest.Item2;
                ValueTuple<Map<string, string[]>[], int, string[], string[], string, string[], string[], ValueTuple<string[], string[]>> valueTuple3 = this.parseUnitsDetails(cuConfig, isMounted, isRanged, tier, culture, vars, mode, color, color2, banner, shields, banners, null, null);
                vars = valueTuple3.Item1;
                mode = valueTuple3.Item2;
                color = valueTuple3.Item3;
                color2 = valueTuple3.Item4;
                banner = valueTuple3.Item5;
                shields = valueTuple3.Item6;
                banners = valueTuple3.Item7;
                combatShields = valueTuple3.Rest.Item1;
                combatBanners = valueTuple3.Rest.Item2;
                ValueTuple<Map<string, string[]>[], int, string[], string[], string, string[], string[], ValueTuple<string[], string[]>> valueTuple4 = this.parseUnitsDetails(kcuConfig, isMounted, isRanged, tier, culture, vars, mode, color, color2, banner, shields, banners, null, null);
                vars = valueTuple4.Item1;
                mode = valueTuple4.Item2;
                color = valueTuple4.Item3;
                color2 = valueTuple4.Item4;
                banner = valueTuple4.Item5;
                shields = valueTuple4.Item6;
                banners = valueTuple4.Item7;
                combatShields = valueTuple4.Rest.Item1;
                combatBanners = valueTuple4.Rest.Item2;
                bool flag = !string.IsNullOrEmpty(banner) && banners != null && banners.Length != 0;
                if (flag)
                {
                    bannersFull = new string[banners.Length + 1];
                    banners.CopyTo(bannersFull, 0);
                    bannersFull[banners.Length] = banner;
                }
                else
                {
                    bool flag2 = banners != null && banners.Length != 0;
                    if (flag2)
                    {
                        bannersFull = banners;
                    }
                    else
                    {
                        bool flag3 = !string.IsNullOrEmpty(banner);
                        if (flag3)
                        {
                            bannersFull = new string[]
                            {
                                banner
                            };
                        }
                    }
                }
                ValueTuple<bool, int, string[], string[], string[], string[]> rulerConfig = this.GetRulerConfig(clan, mode, color, color2, bannersFull, shields, kConfig, kcConfig, kuConfig, kcuConfig, cConfig, cuConfig, uConfig);
                bool isRuler = rulerConfig.Item1;
                mode = rulerConfig.Item2;
                color = rulerConfig.Item3;
                color2 = rulerConfig.Item4;
                bannersFull = rulerConfig.Item5;
                shields = rulerConfig.Item6;
                Map<string, string> values = null;
                bool flag4 = vars != null;
                if (flag4)
                {
                    values = this.GetVarsValues(vars);
                }
                bool flag5 = values != null;
                if (flag5)
                {
                    bannersFull = Array.ConvertAll<string, string>(bannersFull, (string x) => this.resolveBanner(x, values));
                    shields = Array.ConvertAll<string, string>(shields, (string x) => this.resolveBanner(x, values));
                }
            }
            catch (Exception e)
            {
                Log.write("Error while retrieving config: " + e.Message);
            }
            return new ValueTuple<int, string[], string[], string[], string[]>(mode, color, color2, bannersFull, shields);
        }

        // Token: 0x0600008F RID: 143 RVA: 0x00007A74 File Offset: 0x00005C74
        private ValueTuple<Map<string, string[]>[], int, string[], string[], string, string[], string[], ValueTuple<string[], string[]>> parseUnitsDetails(ConfigSectionUnit unitsConfig, bool isMounted, bool isRanged, int tier, string culture, Map<string, string[]>[] vars, int mode, string[] color, string[] color2, string banner, string[] shields, string[] banners, string[] combatshields, string[] combatbanners)
        {
            bool flag = unitsConfig == null;
            ValueTuple<Map<string, string[]>[], int, string[], string[], string, string[], string[], ValueTuple<string[], string[]>> result;
            if (flag)
            {
                result = new ValueTuple<Map<string, string[]>[], int, string[], string[], string, string[], string[], ValueTuple<string[], string[]>>(vars, mode, color, color2, banner, shields, banners, new ValueTuple<string[], string[]>(combatshields, combatbanners));
            }
            else
            {
                bool flag2 = unitsConfig.cultures != null && culture != null;
                if (flag2)
                {
                    ValueTuple<Map<string, string[]>[], int, string[], string[], string, string[], string[], ValueTuple<string[], string[]>> valueTuple = this.parseCriteria(unitsConfig.cultures[culture], vars, mode, color, color2, banner, shields, banners, combatshields, combatbanners);
                    vars = valueTuple.Item1;
                    mode = valueTuple.Item2;
                    color = valueTuple.Item3;
                    color2 = valueTuple.Item4;
                    banner = valueTuple.Item5;
                    shields = valueTuple.Item6;
                    banners = valueTuple.Item7;
                    combatshields = valueTuple.Rest.Item1;
                    combatbanners = valueTuple.Rest.Item2;
                }
                ValueTuple<Map<string, string[]>[], int, string[], string[], string, string[], string[], ValueTuple<string[], string[]>> valueTuple2 = this.parseTypeCriteria(unitsConfig, isMounted, isRanged, vars, mode, color, color2, banner, shields, banners, combatshields, combatbanners);
                vars = valueTuple2.Item1;
                mode = valueTuple2.Item2;
                color = valueTuple2.Item3;
                color2 = valueTuple2.Item4;
                banner = valueTuple2.Item5;
                shields = valueTuple2.Item6;
                banners = valueTuple2.Item7;
                combatshields = valueTuple2.Rest.Item1;
                combatbanners = valueTuple2.Rest.Item2;
                bool flag3 = unitsConfig.tiers != null;
                if (flag3)
                {
                    ValueTuple<Map<string, string[]>[], int, string[], string[], string, string[], string[], ValueTuple<string[], string[]>> valueTuple3 = this.parseCriteria(unitsConfig.tiers[tier], vars, mode, color, color2, banner, shields, banners, combatshields, combatbanners);
                    vars = valueTuple3.Item1;
                    mode = valueTuple3.Item2;
                    color = valueTuple3.Item3;
                    color2 = valueTuple3.Item4;
                    banner = valueTuple3.Item5;
                    shields = valueTuple3.Item6;
                    banners = valueTuple3.Item7;
                    combatshields = valueTuple3.Rest.Item1;
                    combatbanners = valueTuple3.Rest.Item2;
                }
                bool flag4 = unitsConfig.cultures != null && culture != null && unitsConfig.cultures[culture] != null;
                if (flag4)
                {
                    ValueTuple<Map<string, string[]>[], int, string[], string[], string, string[], string[], ValueTuple<string[], string[]>> valueTuple4 = this.parseTypeCriteria(unitsConfig.cultures[culture], isMounted, isRanged, vars, mode, color, color2, banner, shields, banners, combatshields, combatbanners);
                    vars = valueTuple4.Item1;
                    mode = valueTuple4.Item2;
                    color = valueTuple4.Item3;
                    color2 = valueTuple4.Item4;
                    banner = valueTuple4.Item5;
                    shields = valueTuple4.Item6;
                    banners = valueTuple4.Item7;
                    combatshields = valueTuple4.Rest.Item1;
                    combatbanners = valueTuple4.Rest.Item2;
                    bool flag5 = unitsConfig.cultures[culture].tiers != null;
                    if (flag5)
                    {
                        ValueTuple<Map<string, string[]>[], int, string[], string[], string, string[], string[], ValueTuple<string[], string[]>> valueTuple5 = this.parseCriteria(unitsConfig.cultures[culture].tiers[tier], vars, mode, color, color2, banner, shields, banners, combatshields, combatbanners);
                        vars = valueTuple5.Item1;
                        mode = valueTuple5.Item2;
                        color = valueTuple5.Item3;
                        color2 = valueTuple5.Item4;
                        banner = valueTuple5.Item5;
                        shields = valueTuple5.Item6;
                        banners = valueTuple5.Item7;
                        combatshields = valueTuple5.Rest.Item1;
                        combatbanners = valueTuple5.Rest.Item2;
                    }
                }
                bool flag6 = unitsConfig.tiers != null;
                if (flag6)
                {
                    ValueTuple<Map<string, string[]>[], int, string[], string[], string, string[], string[], ValueTuple<string[], string[]>> valueTuple6 = this.parseTypeCriteria(unitsConfig.tiers[tier], isMounted, isRanged, vars, mode, color, color2, banner, shields, banners, combatshields, combatbanners);
                    vars = valueTuple6.Item1;
                    mode = valueTuple6.Item2;
                    color = valueTuple6.Item3;
                    color2 = valueTuple6.Item4;
                    banner = valueTuple6.Item5;
                    shields = valueTuple6.Item6;
                    banners = valueTuple6.Item7;
                    combatshields = valueTuple6.Rest.Item1;
                    combatbanners = valueTuple6.Rest.Item2;
                }
                bool flag7 = unitsConfig.cultures != null && culture != null && unitsConfig.cultures[culture] != null;
                if (flag7)
                {
                    bool flag8 = unitsConfig.cultures[culture].tiers != null;
                    if (flag8)
                    {
                        ValueTuple<Map<string, string[]>[], int, string[], string[], string, string[], string[], ValueTuple<string[], string[]>> valueTuple7 = this.parseTypeCriteria(unitsConfig.cultures[culture].tiers[tier], isMounted, isRanged, vars, mode, color, color2, banner, shields, banners, combatshields, combatbanners);
                        vars = valueTuple7.Item1;
                        mode = valueTuple7.Item2;
                        color = valueTuple7.Item3;
                        color2 = valueTuple7.Item4;
                        banner = valueTuple7.Item5;
                        shields = valueTuple7.Item6;
                        banners = valueTuple7.Item7;
                        combatshields = valueTuple7.Rest.Item1;
                        combatbanners = valueTuple7.Rest.Item2;
                    }
                }
                result = new ValueTuple<Map<string, string[]>[], int, string[], string[], string, string[], string[], ValueTuple<string[], string[]>>(vars, mode, color, color2, banner, shields, banners, new ValueTuple<string[], string[]>(combatshields, combatbanners));
            }
            return result;
        }

        // Token: 0x06000090 RID: 144 RVA: 0x00007EE8 File Offset: 0x000060E8
        private ValueTuple<Map<string, string[]>[], int, string[], string[], string, string[], string[], ValueTuple<string[], string[]>> parseCriteria(ConfigSection unitsConfig, Map<string, string[]>[] vars, int mode, string[] color, string[] color2, string banner, string[] shields, string[] banners, string[] combatShields, string[] combatBanners)
        {
            bool flag = unitsConfig != null;
            if (flag)
            {
                vars = (unitsConfig.vars ?? vars);
                mode = (unitsConfig.mode ?? mode);
                color = (unitsConfig.color ?? color);
                color2 = (unitsConfig.color2 ?? color2);
                banner = (unitsConfig.banner ?? banner);
                banners = (unitsConfig.banners ?? banners);
                shields = (unitsConfig.shields ?? shields);
                combatShields = (((unitsConfig != null) ? unitsConfig.combatShields : null) ?? combatShields);
                combatBanners = (((unitsConfig != null) ? unitsConfig.combatBanners : null) ?? combatBanners);
            }
            return new ValueTuple<Map<string, string[]>[], int, string[], string[], string, string[], string[], ValueTuple<string[], string[]>>(vars, mode, color, color2, banner, shields, banners, new ValueTuple<string[], string[]>(combatShields, combatBanners));
        }

        // Token: 0x06000091 RID: 145 RVA: 0x00007FB8 File Offset: 0x000061B8
        private ValueTuple<Map<string, string[]>[], int, string[], string[], string, string[], string[], ValueTuple<string[], string[]>> parseTypeCriteria(ConfigSectionTier unitsConfig, bool isMounted, bool isRanged, Map<string, string[]>[] vars, int mode, string[] color, string[] color2, string banner, string[] shields, string[] banners, string[] combatShields, string[] combatBanners)
        {
            bool flag = unitsConfig == null || unitsConfig.types == null;
            ValueTuple<Map<string, string[]>[], int, string[], string[], string, string[], string[], ValueTuple<string[], string[]>> result;
            if (flag)
            {
                result = new ValueTuple<Map<string, string[]>[], int, string[], string[], string, string[], string[], ValueTuple<string[], string[]>>(vars, mode, color, color2, banner, shields, banners, new ValueTuple<string[], string[]>(combatShields, combatBanners));
            }
            else
            {
                if (isMounted)
                {
                    ValueTuple<Map<string, string[]>[], int, string[], string[], string, string[], string[], ValueTuple<string[], string[]>> valueTuple = this.parseCriteria(unitsConfig.types["Mounted"], vars, mode, color, color2, banner, shields, banners, combatShields, combatBanners);
                    vars = valueTuple.Item1;
                    mode = valueTuple.Item2;
                    color = valueTuple.Item3;
                    color2 = valueTuple.Item4;
                    banner = valueTuple.Item5;
                    shields = valueTuple.Item6;
                    banners = valueTuple.Item7;
                    combatShields = valueTuple.Rest.Item1;
                    combatBanners = valueTuple.Rest.Item2;
                }
                else
                {
                    ValueTuple<Map<string, string[]>[], int, string[], string[], string, string[], string[], ValueTuple<string[], string[]>> valueTuple2 = this.parseCriteria(unitsConfig.types["Foot"], vars, mode, color, color2, banner, shields, banners, combatShields, combatBanners);
                    vars = valueTuple2.Item1;
                    mode = valueTuple2.Item2;
                    color = valueTuple2.Item3;
                    color2 = valueTuple2.Item4;
                    banner = valueTuple2.Item5;
                    shields = valueTuple2.Item6;
                    banners = valueTuple2.Item7;
                    combatShields = valueTuple2.Rest.Item1;
                    combatBanners = valueTuple2.Rest.Item2;
                }
                if (isRanged)
                {
                    ValueTuple<Map<string, string[]>[], int, string[], string[], string, string[], string[], ValueTuple<string[], string[]>> valueTuple3 = this.parseCriteria(unitsConfig.types["Ranged"], vars, mode, color, color2, banner, shields, banners, combatShields, combatBanners);
                    vars = valueTuple3.Item1;
                    mode = valueTuple3.Item2;
                    color = valueTuple3.Item3;
                    color2 = valueTuple3.Item4;
                    banner = valueTuple3.Item5;
                    shields = valueTuple3.Item6;
                    banners = valueTuple3.Item7;
                    combatShields = valueTuple3.Rest.Item1;
                    combatBanners = valueTuple3.Rest.Item2;
                }
                else
                {
                    ValueTuple<Map<string, string[]>[], int, string[], string[], string, string[], string[], ValueTuple<string[], string[]>> valueTuple4 = this.parseCriteria(unitsConfig.types["Melee"], vars, mode, color, color2, banner, shields, banners, combatShields, combatBanners);
                    vars = valueTuple4.Item1;
                    mode = valueTuple4.Item2;
                    color = valueTuple4.Item3;
                    color2 = valueTuple4.Item4;
                    banner = valueTuple4.Item5;
                    shields = valueTuple4.Item6;
                    banners = valueTuple4.Item7;
                    combatShields = valueTuple4.Rest.Item1;
                    combatBanners = valueTuple4.Rest.Item2;
                }
                bool flag2 = isMounted && isRanged;
                if (flag2)
                {
                    ValueTuple<Map<string, string[]>[], int, string[], string[], string, string[], string[], ValueTuple<string[], string[]>> valueTuple5 = this.parseCriteria(unitsConfig.types["HorseArcher"], vars, mode, color, color2, banner, shields, banners, combatShields, combatBanners);
                    vars = valueTuple5.Item1;
                    mode = valueTuple5.Item2;
                    color = valueTuple5.Item3;
                    color2 = valueTuple5.Item4;
                    banner = valueTuple5.Item5;
                    shields = valueTuple5.Item6;
                    banners = valueTuple5.Item7;
                    combatShields = valueTuple5.Rest.Item1;
                    combatBanners = valueTuple5.Rest.Item2;
                }
                bool flag3 = !isMounted && isRanged;
                if (flag3)
                {
                    ValueTuple<Map<string, string[]>[], int, string[], string[], string, string[], string[], ValueTuple<string[], string[]>> valueTuple6 = this.parseCriteria(unitsConfig.types["Archer"], vars, mode, color, color2, banner, shields, banners, combatShields, combatBanners);
                    vars = valueTuple6.Item1;
                    mode = valueTuple6.Item2;
                    color = valueTuple6.Item3;
                    color2 = valueTuple6.Item4;
                    banner = valueTuple6.Item5;
                    shields = valueTuple6.Item6;
                    banners = valueTuple6.Item7;
                    combatShields = valueTuple6.Rest.Item1;
                    combatBanners = valueTuple6.Rest.Item2;
                }
                bool flag4 = isMounted && !isRanged;
                if (flag4)
                {
                    ValueTuple<Map<string, string[]>[], int, string[], string[], string, string[], string[], ValueTuple<string[], string[]>> valueTuple7 = this.parseCriteria(unitsConfig.types["Cavalry"], vars, mode, color, color2, banner, shields, banners, combatShields, combatBanners);
                    vars = valueTuple7.Item1;
                    mode = valueTuple7.Item2;
                    color = valueTuple7.Item3;
                    color2 = valueTuple7.Item4;
                    banner = valueTuple7.Item5;
                    shields = valueTuple7.Item6;
                    banners = valueTuple7.Item7;
                    combatShields = valueTuple7.Rest.Item1;
                    combatBanners = valueTuple7.Rest.Item2;
                }
                bool flag5 = !isMounted && !isRanged;
                if (flag5)
                {
                    ValueTuple<Map<string, string[]>[], int, string[], string[], string, string[], string[], ValueTuple<string[], string[]>> valueTuple8 = this.parseCriteria(unitsConfig.types["Infantry"], vars, mode, color, color2, banner, shields, banners, combatShields, combatBanners);
                    vars = valueTuple8.Item1;
                    mode = valueTuple8.Item2;
                    color = valueTuple8.Item3;
                    color2 = valueTuple8.Item4;
                    banner = valueTuple8.Item5;
                    shields = valueTuple8.Item6;
                    banners = valueTuple8.Item7;
                    combatShields = valueTuple8.Rest.Item1;
                    combatBanners = valueTuple8.Rest.Item2;
                }
                result = new ValueTuple<Map<string, string[]>[], int, string[], string[], string, string[], string[], ValueTuple<string[], string[]>>(vars, mode, color, color2, banner, shields, banners, new ValueTuple<string[], string[]>(combatShields, combatBanners));
            }
            return result;
        }

        // Token: 0x06000092 RID: 146 RVA: 0x00008438 File Offset: 0x00006638
        public ValueTuple<int, string[], string[], string[], string[]> GetBattleConfig(string kingdom, string clan, string unit, bool isPlayerKingdom, bool isPlayerClan, bool isPlayer, bool isKing, bool isLeader, bool isHero, bool isMounted, bool isRanged, int tier, string culture)
        {
            string[] combatBanners = null;
            PocColorModConfigKingdom kConfig = null;
            PocColorModConfigClan kcConfig = null;
            PocColorModConfigUnit kuConfig = null;
            PocColorModConfigUnit kcuConfig = null;
            PocColorModConfigClan cConfig = null;
            PocColorModConfigUnit cuConfig = null;
            PocColorModConfigUnit uConfig = null;
            ValueTuple<PocColorModConfigKingdom, PocColorModConfigClan, PocColorModConfigUnit, PocColorModConfigUnit, PocColorModConfigClan, PocColorModConfigUnit, PocColorModConfigUnit> configScopes = this.getConfigScopes(kConfig, kcConfig, kuConfig, kcuConfig, cConfig, cuConfig, uConfig, kingdom, clan, unit, isPlayerKingdom, isPlayerClan, isPlayer, isKing, isLeader, isHero, isMounted, isRanged, tier, culture);
            kConfig = configScopes.Item1;
            kcConfig = configScopes.Item2;
            kuConfig = configScopes.Item3;
            kcuConfig = configScopes.Item4;
            cConfig = configScopes.Item5;
            cuConfig = configScopes.Item6;
            uConfig = configScopes.Item7;
            ValueTuple<int, string[], string[], string[], string[]> config = this.GetConfig(kingdom, clan, unit, isPlayerKingdom, isPlayerClan, isPlayer, isKing, isLeader, isHero, isMounted, isRanged, tier, culture);
            int mode = config.Item1;
            string[] color = config.Item2;
            string[] color2 = config.Item3;
            string[] banners = config.Item4;
            string[] shields = config.Item5;
            PocColorModConfigColorEntry defaultConfig = this.defaultConfig;
            Map<string, string[]>[] vars = ((defaultConfig != null) ? defaultConfig.vars : null) ?? null;
            vars = (((kConfig != null) ? kConfig.vars : null) ?? vars);
            vars = (((cConfig != null) ? cConfig.vars : null) ?? vars);
            vars = (((kcConfig != null) ? kcConfig.vars : null) ?? vars);
            vars = (((uConfig != null) ? uConfig.vars : null) ?? vars);
            vars = (((kuConfig != null) ? kuConfig.vars : null) ?? vars);
            vars = (((cuConfig != null) ? cuConfig.vars : null) ?? vars);
            vars = (((kcuConfig != null) ? kcuConfig.vars : null) ?? vars);
            string[] combatShields = (kConfig != null) ? kConfig.combatShields : null;
            combatShields = (((cConfig != null) ? cConfig.combatShields : null) ?? combatShields);
            combatShields = (((kcConfig != null) ? kcConfig.combatShields : null) ?? combatShields);
            combatShields = (((uConfig != null) ? uConfig.combatShields : null) ?? combatShields);
            combatShields = (((kuConfig != null) ? kuConfig.combatShields : null) ?? combatShields);
            combatShields = (((cuConfig != null) ? cuConfig.combatShields : null) ?? combatShields);
            combatShields = (((kcuConfig != null) ? kcuConfig.combatShields : null) ?? combatShields);
            combatBanners = (((kConfig != null) ? kConfig.combatBanners : null) ?? combatBanners);
            combatBanners = (((cConfig != null) ? cConfig.combatBanners : null) ?? combatBanners);
            combatBanners = (((kcConfig != null) ? kcConfig.combatBanners : null) ?? combatBanners);
            combatBanners = (((uConfig != null) ? uConfig.combatBanners : null) ?? combatBanners);
            combatBanners = (((kuConfig != null) ? kuConfig.combatBanners : null) ?? combatBanners);
            combatBanners = (((cuConfig != null) ? cuConfig.combatBanners : null) ?? combatBanners);
            combatBanners = (((kcuConfig != null) ? kcuConfig.combatBanners : null) ?? combatBanners);
            ValueTuple<Map<string, string[]>[], int, string[], string[], string, string[], string[], ValueTuple<string[], string[]>> valueTuple = this.parseUnitsDetails(uConfig, isMounted, isRanged, tier, culture, vars, mode, color, color2, null, shields, banners, combatShields, combatBanners);
            vars = valueTuple.Item1;
            mode = valueTuple.Item2;
            color = valueTuple.Item3;
            color2 = valueTuple.Item4;
            string banner = valueTuple.Item5;
            shields = valueTuple.Item6;
            banners = valueTuple.Item7;
            combatShields = valueTuple.Rest.Item1;
            combatBanners = valueTuple.Rest.Item2;
            ValueTuple<Map<string, string[]>[], int, string[], string[], string, string[], string[], ValueTuple<string[], string[]>> valueTuple2 = this.parseUnitsDetails(kuConfig, isMounted, isRanged, tier, culture, vars, mode, color, color2, null, shields, banners, combatShields, combatBanners);
            vars = valueTuple2.Item1;
            mode = valueTuple2.Item2;
            color = valueTuple2.Item3;
            color2 = valueTuple2.Item4;
            banner = valueTuple2.Item5;
            shields = valueTuple2.Item6;
            banners = valueTuple2.Item7;
            combatShields = valueTuple2.Rest.Item1;
            combatBanners = valueTuple2.Rest.Item2;
            ValueTuple<Map<string, string[]>[], int, string[], string[], string, string[], string[], ValueTuple<string[], string[]>> valueTuple3 = this.parseUnitsDetails(cuConfig, isMounted, isRanged, tier, culture, vars, mode, color, color2, null, shields, banners, combatShields, combatBanners);
            vars = valueTuple3.Item1;
            mode = valueTuple3.Item2;
            color = valueTuple3.Item3;
            color2 = valueTuple3.Item4;
            banner = valueTuple3.Item5;
            shields = valueTuple3.Item6;
            banners = valueTuple3.Item7;
            combatShields = valueTuple3.Rest.Item1;
            combatBanners = valueTuple3.Rest.Item2;
            ValueTuple<Map<string, string[]>[], int, string[], string[], string, string[], string[], ValueTuple<string[], string[]>> valueTuple4 = this.parseUnitsDetails(kcuConfig, isMounted, isRanged, tier, culture, vars, mode, color, color2, null, shields, banners, combatShields, combatBanners);
            vars = valueTuple4.Item1;
            mode = valueTuple4.Item2;
            color = valueTuple4.Item3;
            color2 = valueTuple4.Item4;
            banner = valueTuple4.Item5;
            shields = valueTuple4.Item6;
            banners = valueTuple4.Item7;
            combatShields = valueTuple4.Rest.Item1;
            combatBanners = valueTuple4.Rest.Item2;
            Map<string, string> values = null;
            bool flag = vars != null;
            if (flag)
            {
                values = this.GetVarsValues(vars);
            }
            bool flag2 = combatShields != null && combatShields.Length != 0;
            if (flag2)
            {
                shields = combatShields;
            }
            bool flag3 = combatBanners != null && combatBanners.Length != 0;
            if (flag3)
            {
                banners = combatBanners;
            }
            bool flag4 = values != null;
            if (flag4)
            {
                if (banners != null)
                    banners = Array.ConvertAll<string, string>(banners, (string x) => this.resolveBanner(x, values));
                if (shields != null)
                    shields = Array.ConvertAll<string, string>(shields, (string x) => this.resolveBanner(x, values));
            }
            return new ValueTuple<int, string[], string[], string[], string[]>(mode, color, color2, banners, shields);
        }

        // Token: 0x06000093 RID: 147 RVA: 0x00008950 File Offset: 0x00006B50
        public Map<string, string[]>[] GetVars(string kingdom, string clan, string unit, bool isPlayerKingdom, bool isPlayerClan, bool isPlayer, bool isKing, bool isLeader, bool isHero, bool isMounted, bool isRanged, int tier, string culture)
        {
            PocColorModConfigKingdom kConfig = null;
            PocColorModConfigClan kcConfig = null;
            PocColorModConfigUnit kuConfig = null;
            PocColorModConfigUnit kcuConfig = null;
            PocColorModConfigClan cConfig = null;
            PocColorModConfigUnit cuConfig = null;
            PocColorModConfigUnit uConfig = null;
            ValueTuple<PocColorModConfigKingdom, PocColorModConfigClan, PocColorModConfigUnit, PocColorModConfigUnit, PocColorModConfigClan, PocColorModConfigUnit, PocColorModConfigUnit> configScopes = this.getConfigScopes(kConfig, kcConfig, kuConfig, kcuConfig, cConfig, cuConfig, uConfig, kingdom, clan, unit, isPlayerKingdom, isPlayerClan, isPlayer, isKing, isLeader, isHero, isMounted, isRanged, tier, culture);
            kConfig = configScopes.Item1;
            kcConfig = configScopes.Item2;
            kuConfig = configScopes.Item3;
            kcuConfig = configScopes.Item4;
            cConfig = configScopes.Item5;
            cuConfig = configScopes.Item6;
            uConfig = configScopes.Item7;
            PocColorModConfigColorEntry defaultConfig = this.defaultConfig;
            Map<string, string[]>[] vars = ((defaultConfig != null) ? defaultConfig.vars : null) ?? null;
            vars = (((kConfig != null) ? kConfig.vars : null) ?? vars);
            vars = (((cConfig != null) ? cConfig.vars : null) ?? vars);
            vars = (((kcConfig != null) ? kcConfig.vars : null) ?? vars);
            vars = (((uConfig != null) ? uConfig.vars : null) ?? vars);
            vars = (((kuConfig != null) ? kuConfig.vars : null) ?? vars);
            vars = (((cuConfig != null) ? cuConfig.vars : null) ?? vars);
            return ((kcuConfig != null) ? kcuConfig.vars : null) ?? vars;
        }

        // Token: 0x06000094 RID: 148 RVA: 0x00008A7C File Offset: 0x00006C7C
        public string[] GetVarList(Map<string, string[]>[] vars)
        {
            HashSet<string> varList = new HashSet<string>();
            foreach (Map<string, string[]> group in vars)
            {
                varList.UnionWith(group.Keys);
            }
            string[] stringArray = new string[varList.Count];
            varList.CopyTo(stringArray);
            return stringArray;
        }

        // Token: 0x06000095 RID: 149 RVA: 0x00008AD4 File Offset: 0x00006CD4
        public Map<string, string> GetVarsValues(Map<string, string[]>[] vars)
        {
            Map<string, string> values = new Map<string, string>();
            foreach (Map<string, string[]> group in vars)
            {
                int min = 100000;
                foreach (string key in group.Keys)
                {
                    string[] vals = group[key];
                    min = Math.Min(min, vals.Length);
                }
                int ind = PocColorModConfig.rnd.Next(0, min);
                foreach (string key2 in group.Keys)
                {
                    string[] vals2 = group[key2];
                    values[key2] = vals2[ind];
                }
            }
            return values;
        }

        // Token: 0x06000096 RID: 150 RVA: 0x00008BD4 File Offset: 0x00006DD4
        public string resolveBanner(string bannerText, Map<string, string> values)
        {
            string banner = bannerText;
            foreach (string key in values.Keys)
            {
                banner = banner.Replace(key, values[key]);
            }
            return banner;
        }

        // Token: 0x06000097 RID: 151 RVA: 0x00008C38 File Offset: 0x00006E38
        public ValueTuple<PocColorModConfigKingdom, PocColorModConfigClan, PocColorModConfigUnit, PocColorModConfigUnit, PocColorModConfigClan, PocColorModConfigUnit, PocColorModConfigUnit> getConfigScopes(PocColorModConfigKingdom kConfig, PocColorModConfigClan kcConfig, PocColorModConfigUnit kuConfig, PocColorModConfigUnit kcuConfig, PocColorModConfigClan cConfig, PocColorModConfigUnit cuConfig, PocColorModConfigUnit uConfig, string kingdom, string clan, string unit, bool isPlayerKingdom, bool isPlayerClan, bool isPlayer, bool isKing, bool isLeader, bool isHero, bool isMounted, bool isRanged, int tier, string culture)
        {
            Map<string, PocColorModConfigKingdom> kingdoms = this.kingdoms;
            kConfig = ((kingdoms != null) ? kingdoms[kingdom] : null);
            bool flag = kConfig == null && isPlayerKingdom;
            if (flag)
            {
                Map<string, PocColorModConfigKingdom> kingdoms2 = this.kingdoms;
                kConfig = ((kingdoms2 != null) ? kingdoms2["PlayerKingdom"] : null);
            }
            Map<string, PocColorModConfigClan> clans = this.clans;
            cConfig = ((clans != null) ? clans[clan] : null);
            bool flag2 = cConfig == null && isPlayerClan;
            if (flag2)
            {
                Map<string, PocColorModConfigClan> clans2 = this.clans;
                cConfig = ((clans2 != null) ? clans2["PlayerClan"] : null);
            }
            PocColorModConfigClan pocColorModConfigClan;
            if (kConfig == null)
            {
                pocColorModConfigClan = null;
            }
            else
            {
                Map<string, PocColorModConfigClan> clans3 = kConfig.clans;
                pocColorModConfigClan = ((clans3 != null) ? clans3[clan] : null);
            }
            kcConfig = pocColorModConfigClan;
            bool flag3 = kcConfig == null && isPlayerClan;
            if (flag3)
            {
                PocColorModConfigClan pocColorModConfigClan2;
                if (kConfig == null)
                {
                    pocColorModConfigClan2 = null;
                }
                else
                {
                    Map<string, PocColorModConfigClan> clans4 = kConfig.clans;
                    pocColorModConfigClan2 = ((clans4 != null) ? clans4["PlayerClan"] : null);
                }
                kcConfig = pocColorModConfigClan2;
            }
            uConfig = this.parseUnits(this, unit, isPlayer, isKing, isLeader, isHero);
            kuConfig = this.parseUnits(kConfig, unit, isPlayer, isKing, isLeader, isHero);
            cuConfig = this.parseUnits(cConfig, unit, isPlayer, isKing, isLeader, isHero);
            kcuConfig = this.parseUnits(kcConfig, unit, isPlayer, isKing, isLeader, isHero);
            return new ValueTuple<PocColorModConfigKingdom, PocColorModConfigClan, PocColorModConfigUnit, PocColorModConfigUnit, PocColorModConfigClan, PocColorModConfigUnit, PocColorModConfigUnit>(kConfig, kcConfig, kuConfig, kcuConfig, cConfig, cuConfig, uConfig);
        }

        // Token: 0x06000098 RID: 152 RVA: 0x00008D68 File Offset: 0x00006F68
        private PocColorModConfigUnit parseUnits(UnitsContainerSection section, string unit, bool isPlayer, bool isKing, bool isLeader, bool isHero)
        {
            PocColorModConfigUnit pocColorModConfigUnit;
            if (section == null)
            {
                pocColorModConfigUnit = null;
            }
            else
            {
                Map<string, PocColorModConfigUnit> units = section.units;
                pocColorModConfigUnit = ((units != null) ? units[unit] : null);
            }
            PocColorModConfigUnit unitConfig = pocColorModConfigUnit;
            bool flag = unitConfig == null;
            if (flag)
            {
                if (isPlayer)
                {
                    PocColorModConfigUnit pocColorModConfigUnit2;
                    if (section == null)
                    {
                        pocColorModConfigUnit2 = null;
                    }
                    else
                    {
                        Map<string, PocColorModConfigUnit> units2 = section.units;
                        pocColorModConfigUnit2 = ((units2 != null) ? units2["Player"] : null);
                    }
                    unitConfig = pocColorModConfigUnit2;
                }
                bool flag2 = unitConfig == null;
                if (flag2)
                {
                    if (isKing)
                    {
                        PocColorModConfigUnit pocColorModConfigUnit3;
                        if (section == null)
                        {
                            pocColorModConfigUnit3 = null;
                        }
                        else
                        {
                            Map<string, PocColorModConfigUnit> units3 = section.units;
                            pocColorModConfigUnit3 = ((units3 != null) ? units3["King"] : null);
                        }
                        unitConfig = pocColorModConfigUnit3;
                    }
                    bool flag3 = unitConfig == null;
                    if (flag3)
                    {
                        if (isLeader)
                        {
                            PocColorModConfigUnit pocColorModConfigUnit4;
                            if (section == null)
                            {
                                pocColorModConfigUnit4 = null;
                            }
                            else
                            {
                                Map<string, PocColorModConfigUnit> units4 = section.units;
                                pocColorModConfigUnit4 = ((units4 != null) ? units4["Leader"] : null);
                            }
                            unitConfig = pocColorModConfigUnit4;
                        }
                        bool flag4 = unitConfig == null;
                        if (flag4)
                        {
                            if (isHero)
                            {
                                PocColorModConfigUnit pocColorModConfigUnit5;
                                if (section == null)
                                {
                                    pocColorModConfigUnit5 = null;
                                }
                                else
                                {
                                    Map<string, PocColorModConfigUnit> units5 = section.units;
                                    pocColorModConfigUnit5 = ((units5 != null) ? units5["Hero"] : null);
                                }
                                unitConfig = pocColorModConfigUnit5;
                            }
                            else
                            {
                                bool flag5 = unitConfig == null && !isHero;
                                if (flag5)
                                {
                                    PocColorModConfigUnit pocColorModConfigUnit6;
                                    if (section == null)
                                    {
                                        pocColorModConfigUnit6 = null;
                                    }
                                    else
                                    {
                                        Map<string, PocColorModConfigUnit> units6 = section.units;
                                        pocColorModConfigUnit6 = ((units6 != null) ? units6["Trooper"] : null);
                                    }
                                    unitConfig = pocColorModConfigUnit6;
                                }
                            }
                        }
                    }
                }
            }
            return unitConfig;
        }

        // Token: 0x06000099 RID: 153 RVA: 0x00008E9C File Offset: 0x0000709C
        public void parseGroups()
        {
            bool flag = this.groups != null;
            if (flag)
            {
                foreach (PocColorModGroupEntry group in this.groups)
                {
                    Map<string, PocColorModConfigUnit> units = group.toUnits();
                    bool flag2 = this.units != null;
                    if (flag2)
                    {
                        foreach (string unitName in this.units.Keys)
                        {
                            units[unitName] = this.units[unitName];
                        }
                    }
                    this.units = units;
                }
            }
            bool flag3 = this.clans != null;
            if (flag3)
            {
                foreach (string clanName in this.clans.Keys)
                {
                    PocColorModConfigClan clanConfig = this.clans[clanName];
                    bool flag4 = clanConfig.groups != null;
                    if (flag4)
                    {
                        foreach (PocColorModGroupEntry group2 in clanConfig.groups)
                        {
                            Map<string, PocColorModConfigUnit> units2 = group2.toUnits();
                            bool flag5 = this.clans[clanName].units != null;
                            if (flag5)
                            {
                                foreach (string unitName2 in this.clans[clanName].units.Keys)
                                {
                                    units2[unitName2] = this.clans[clanName].units[unitName2];
                                }
                            }
                            this.clans[clanName].units = units2;
                        }
                    }
                }
            }
            bool flag6 = this.kingdoms != null;
            if (flag6)
            {
                foreach (string kingdomName in this.kingdoms.Keys)
                {
                    PocColorModConfigKingdom kingdomConfig = this.kingdoms[kingdomName];
                    bool flag7 = kingdomConfig.groups != null;
                    if (flag7)
                    {
                        foreach (PocColorModGroupEntry group3 in kingdomConfig.groups)
                        {
                            Map<string, PocColorModConfigUnit> units3 = group3.toUnits();
                            bool flag8 = this.kingdoms[kingdomName].units != null;
                            if (flag8)
                            {
                                foreach (string unitName3 in this.kingdoms[kingdomName].units.Keys)
                                {
                                    units3[unitName3] = this.kingdoms[kingdomName].units[unitName3];
                                }
                            }
                            this.kingdoms[kingdomName].units = units3;
                        }
                    }
                    bool flag9 = kingdomConfig.clans != null;
                    if (flag9)
                    {
                        foreach (string clanName2 in kingdomConfig.clans.Keys)
                        {
                            PocColorModConfigClan clanConfig2 = kingdomConfig.clans[clanName2];
                            bool flag10 = clanConfig2.groups != null;
                            if (flag10)
                            {
                                foreach (PocColorModGroupEntry group4 in clanConfig2.groups)
                                {
                                    Map<string, PocColorModConfigUnit> units4 = group4.toUnits();
                                    bool flag11 = kingdomConfig.clans[clanName2].units != null;
                                    if (flag11)
                                    {
                                        foreach (string unitName4 in kingdomConfig.clans[clanName2].units.Keys)
                                        {
                                            units4[unitName4] = kingdomConfig.clans[clanName2].units[unitName4];
                                        }
                                    }
                                    this.kingdoms[kingdomName].clans[clanName2].units = units4;
                                }
                            }
                        }
                    }
                }
            }
        }

        public bool HasConfigForClan(string clanName, bool isPlayerClan)
        {
            if (string.IsNullOrEmpty(clanName))
                return false;

            // Check si hay config específico para este clan
            if (clans != null && clans.ContainsKey(clanName))
                return true;

            // Check si hay config para PlayerClan
            if (isPlayerClan && clans != null && clans.ContainsKey("PlayerClan"))
                return true;

            return false;
        }

        public bool HasConfigForKingdom(string kingdomName, bool isPlayerKingdom)
        {
            if (string.IsNullOrEmpty(kingdomName))
                return false;

            // Check si hay config específico para este reino
            if (kingdoms != null && kingdoms.ContainsKey(kingdomName))
                return true;

            // Check si hay config para PlayerKingdom
            if (isPlayerKingdom && kingdoms != null && kingdoms.ContainsKey("PlayerKingdom"))
                return true;

            return false;
        }

        public bool HasConfigForUnit(string unitName)
        {
            if (string.IsNullOrEmpty(unitName))
                return false;

            return units != null && units.ContainsKey(unitName);
        }

        public bool HasAnyConfigFor(string kingdomName, string clanName, string unitName,
                                   bool isPlayerKingdom, bool isPlayerClan)
        {
            // Tiene config si hay CUALQUIER configuración específica
            return HasConfigForKingdom(kingdomName, isPlayerKingdom) ||
                   HasConfigForClan(clanName, isPlayerClan) ||
                   HasConfigForUnit(unitName) ||
                   defaultConfig != null; // O si hay config por defecto
        }

        // Token: 0x04000046 RID: 70
        public static Random rnd = new Random();
    }
}
