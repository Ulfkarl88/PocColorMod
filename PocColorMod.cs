using HarmonyLib;
using Newtonsoft.Json;
using PocColor.Config;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Reflection;
using System.Text;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View;

namespace PocColor
{
    public class PocColorMod : MBSubModuleBase
    {
        public static PocColorModConfig config { get; set; }
        public static Map<string, string> bannerKingdomCache { get; set; } = new Map<string, string>();
        public static Map<string, string> bannerClanCache { get; set; } = new Map<string, string>();
        public static bool doLog;
        public bool _bannerSavedForCurrentGame = false;
        private CampaignTime _campaignStartTime = CampaignTime.Zero;

        private static bool _harmonyPatched = false;
        private bool _needsKingdomColorReapply = false;
        private int _reapplyTickCounter = 0;

        public static string SerializeBanner(Banner banner)
        {
            if (banner == null)
            {
                Log.write(">>> SerializeBanner: banner is NULL");
                return "";
            }

            if (banner.BannerDataList == null)
            {
                Log.write(">>> SerializeBanner: BannerDataList is NULL");
                return "";
            }

            if (banner.BannerDataList.Count == 0)
            {
                Log.write(">>> SerializeBanner: BannerDataList is EMPTY (Count=0)");
                return "";
            }

            StringBuilder myStringBuilder = new StringBuilder();
            bool flag = true;
            foreach (BannerData bannerData in banner.BannerDataList)
            {
                try
                {
                    bool flag2 = !flag;
                    if (flag2)
                    {
                        myStringBuilder.Append(".");
                    }
                    flag = false;
                    myStringBuilder.Append(bannerData.MeshId);
                    myStringBuilder.Append('.');
                    myStringBuilder.Append(bannerData.ColorId);
                    myStringBuilder.Append('.');
                    myStringBuilder.Append(bannerData.ColorId2);
                    myStringBuilder.Append('.');
                    myStringBuilder.Append((int)bannerData.Size.x);
                    myStringBuilder.Append('.');
                    myStringBuilder.Append((int)bannerData.Size.y);
                    myStringBuilder.Append('.');
                    myStringBuilder.Append((int)bannerData.Position.x);
                    myStringBuilder.Append('.');
                    myStringBuilder.Append((int)bannerData.Position.y);
                    myStringBuilder.Append('.');
                    myStringBuilder.Append(bannerData.DrawStroke ? 1 : 0);
                    myStringBuilder.Append('.');
                    myStringBuilder.Append(bannerData.Mirror ? 1 : 0);
                    myStringBuilder.Append('.');
                    float rotationFlt = bannerData.RotationValue / 0.00278f;
                    int rotationInt = (int)short.Parse(Math.Round((double)rotationFlt, MidpointRounding.AwayFromZero).ToString());
                    myStringBuilder.Append(rotationInt);
                }
                catch (Exception e)
                {
                    Log.write(">>> SerializeBanner ERROR: " + e.Message);
                }
            }

            string result = myStringBuilder.ToString();
            if (string.IsNullOrEmpty(result))
            {
                Log.write(">>> SerializeBanner: result is EMPTY after processing");
            }

            return result;
        }

        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
            // DO NOT APPLY HARMONY HERE - Too early, causes character creation crash
            Log.write("==> POC Color Mod: SubModule loaded, waiting for campaign start...");
        }

        protected override void OnBeforeInitialModuleScreenSetAsRoot()
        {
            base.OnBeforeInitialModuleScreenSetAsRoot();
            InformationManager.DisplayMessage(new InformationMessage(
                "POC Color Mod For Bannerlord 1.3 Loaded",
                Colors.Yellow));
            // DO NOT APPLY HARMONY HERE - Still too early
        }

        public override void OnGameInitializationFinished(Game game)
        {
            Log.write("==> OnGameInitializationFinished CALLED");

            // CRITICAL: ALWAYS load config first (for both new and loaded games)
            try
            {
                PocColorMod.config = new PocColorModConfig();
                Log.write("--- POC COLOR MOD ---");
                Log.write("read config file");
                PocColorMod.config = JsonConvert.DeserializeObject<PocColorModConfig>(File.ReadAllText("..\\..\\modules\\PocColor\\config.json"));
                PocColorMod.config.parseGroups();
                PocColorMod.doLog = PocColorMod.config.log;
                Log.write("==> Config loaded successfully");
            }
            catch (FileNotFoundException e)
            {
                Log.write("config file not found: " + e.Message);
                return;
            }
            catch (Exception e2)
            {
                Log.write("Could not parse the config file :(");
                Log.write(" =====>" + e2.Message);
                return;
            }

            // Check if this is a loaded game
            bool isLoadedGame = false;
            if (game.GameType is Campaign && Campaign.Current != null)
            {
                string saveId = Campaign.Current?.UniqueGameId;
                string bannerPath = saveId != null ? $"..\\..\\modules\\PocColor\\PlayerBanners\\{saveId}.json" : null;

                if (bannerPath != null && File.Exists(bannerPath))
                {
                    Log.write("==> OnGameInitializationFinished: LOADED GAME detected");
                    isLoadedGame = true;
                }
            }

            if (!isLoadedGame)
            {
                Log.write("==> OnGameInitializationFinished: NEW GAME - initializing banners and colors");
            }

    



            Map<string, string> bannerKingdomCacheTmp = new Map<string, string>();
            Map<string, string> bannerClanCacheTmp = new Map<string, string>();

            try
            {
                PocColorMod.config = new PocColorModConfig();
                Log.write("--- POC COLOR MOD ---");
                Log.write("read config file");
                PocColorMod.config = JsonConvert.DeserializeObject<PocColorModConfig>(File.ReadAllText("..\\..\\modules\\PocColor\\config.json"));
                PocColorMod.config.parseGroups();
            }
            catch (FileNotFoundException e)
            {
                Log.write("config file not found: " + e.Message);
            }
            catch (Exception e2)
            {
                Log.write("Could not parse the config file :(");
                Log.write(" =====>" + e2.Message);
            }

            PocColorMod.doLog = PocColorMod.config.log;
            Log.write("Build Cache -- start");

            // Guard: Skip campaign-specific initialization in Custom Battle
            if (Campaign.Current == null)
            {
                Log.write("==> Not a Campaign game mode (Custom Battle?), skipping campaign initialization");
                return;
            }

            try
            {
                for (int i = 0; i < Campaign.Current.Kingdoms.Count; i++)
                {
                    Kingdom kingdom = Campaign.Current.Kingdoms[i];
                    Log.write("====> kingdom [" + kingdom.Name.ToString() + "]");

                    Clan playerClan = Clan.PlayerClan;
                    string text;
                    if (playerClan == null)
                    {
                        text = null;
                    }
                    else
                    {
                        Kingdom kingdom4 = playerClan.Kingdom;
                        if (kingdom4 == null)
                        {
                            text = null;
                        }
                        else
                        {
                            TextObject name = kingdom4.Name;
                            text = ((name != null) ? name.ToString() : null);
                        }
                    }
                    string playerKingdomName = text;

                    bool flag;
                    if (!string.IsNullOrEmpty(playerKingdomName))
                    {
                        TextObject name2 = kingdom.Name;
                        flag = (((name2 != null) ? name2.ToString() : null) == playerKingdomName);
                    }
                    else
                    {
                        flag = false;
                    }
                    bool isPlayerKingdom = flag;

                    PocColorModConfig config = PocColorMod.config;
                    TextObject name3 = kingdom.Name;
                    ValueTuple<string, string, string, string> kingdomConfig = config.GetKingdomConfig((name3 != null) ? name3.ToString() : null, isPlayerKingdom);
                    string kingdomBannerStr = kingdomConfig.Item1;
                    string kingdomShieldStr = kingdomConfig.Item2;
                    string primary = kingdomConfig.Item3;
                    string secondary = kingdomConfig.Item4;

                    string bannerStr = PocColorMod.SerializeBanner(kingdom.Banner);
                    bannerKingdomCacheTmp.TryAdd(bannerStr, kingdom.Name.ToString());
                    ConcurrentDictionary<string, string> concurrentDictionary = bannerClanCacheTmp;
                    string key = bannerStr;
                    Clan rulingClan = kingdom.RulingClan;
                    concurrentDictionary.TryAdd(key, (rulingClan != null) ? rulingClan.Name.ToString() : null);

                    bool flag2 = PocColorMod.doLog;
                    if (flag2)
                    {
                        Log.write("> Initial Kingdom Banner loaded from game: [" + bannerStr + "]");
                    }

                    bool flag3 = PocColorMod.doLog;
                    if (flag3)
                    {
                        Log.write("> Initial Kingdom primary color loaded from game: [" + BannerManager.GetColorId(kingdom.PrimaryBannerColor).ToString() + "]");
                    }

                    bool flag4 = PocColorMod.doLog;
                    if (flag4)
                    {
                        Log.write("> Initial Kingdom secondary color loaded from game: [" + BannerManager.GetColorId(kingdom.SecondaryBannerColor).ToString() + "]");
                    }

                    bool flag5 = primary != null;
                    if (flag5)
                    {
                        uint color = PocColorModSetColors.parseColor(primary);
                        FieldInfo fi = typeof(Kingdom).GetField("<PrimaryBannerColor>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic);
                        TypedReference reference = __makeref(kingdom);
                        fi.SetValueDirect(reference, color);
                        FieldInfo fi2 = typeof(Kingdom).GetField("<Color>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic);
                        TypedReference reference2 = __makeref(kingdom);
                        fi2.SetValueDirect(reference2, color);

                        bool flag6 = PocColorMod.doLog;
                        if (flag6)
                        {
                            Log.write("> updating primaryColor for kingdom: [" + PocColorMod.SerializeBanner(kingdom.Banner) + "]");
                        }

                        Banner banner3 = kingdom.Banner;
                        if (banner3 != null)
                        {
                            banner3.ChangePrimaryColor(color);
                        }

                        bool flag7 = PocColorMod.doLog;
                        if (flag7)
                        {
                            Log.write("> updated Banner: [" + PocColorMod.SerializeBanner(kingdom.Banner) + "]");
                        }
                    }

                    bool flag8 = secondary != null;
                    if (flag8)
                    {
                        uint color2 = PocColorModSetColors.parseColor(secondary);
                        FieldInfo fi3 = typeof(Kingdom).GetField("<SecondaryBannerColor>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic);
                        TypedReference reference3 = __makeref(kingdom);
                        fi3.SetValueDirect(reference3, color2);
                        FieldInfo fi4 = typeof(Kingdom).GetField("<Color2>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic);
                        TypedReference reference4 = __makeref(kingdom);
                        fi4.SetValueDirect(reference4, color2);

                        bool flag9 = PocColorMod.doLog;
                        if (flag9)
                        {
                            Log.write("> updating SecondaryColor for kingdom: [" + PocColorMod.SerializeBanner(kingdom.Banner) + "]");
                        }

                        Banner banner4 = kingdom.Banner;
                        if (banner4 != null)
                        {
                            banner4.ChangeIconColors(color2);
                        }

                        bool flag10 = PocColorMod.doLog;
                        if (flag10)
                        {
                            Log.write("> updated Banner: [" + PocColorMod.SerializeBanner(kingdom.Banner) + "]");
                        }
                    }

                    bool flag11 = kingdomBannerStr != null;
                    if (flag11)
                    {
                        bool flag12 = PocColorMod.doLog;
                        if (flag12)
                        {
                            Log.write(string.Concat(new string[]
                            {
                        "> updating banner for kingdom: [",
                        kingdom.Name.ToString(),
                        ", ",
                        kingdomBannerStr,
                        "]"
                            }));
                        }

                        Banner kingdomBanner = new Banner(kingdomBannerStr);
                        FieldInfo fi5 = typeof(Kingdom).GetField("<Banner>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic);
                        TypedReference reference5 = __makeref(kingdom);
                        fi5.SetValueDirect(reference5, kingdomBanner);

                        bool flag13 = PocColorMod.doLog;
                        if (flag13)
                        {
                            Log.write("> updated KingdomBanner from Config: [" + PocColorMod.SerializeBanner(kingdom.Banner) + "]");
                        }
                    }

                    kingdomBannerStr = PocColorMod.SerializeBanner(kingdom.Banner);
                    Log.write("> adding new cache for banner: [" + kingdomBannerStr + "]");
                    bannerKingdomCacheTmp.TryAdd(kingdomBannerStr, kingdom.Name.ToString());
                    ConcurrentDictionary<string, string> concurrentDictionary2 = bannerClanCacheTmp;
                    string key2 = kingdomBannerStr;
                    Clan rulingClan2 = kingdom.RulingClan;
                    concurrentDictionary2.TryAdd(key2, (rulingClan2 != null) ? rulingClan2.Name.ToString() : null);

                    foreach (Clan clan in kingdom.Clans)
                    {
                        try
                        {
                            Log.write(string.Concat(new string[]
                            {
                        "====> kingdom [",
                        kingdom.Name.ToString(),
                        "], clan [",
                        clan.Name.ToString(),
                        "]"
                            }));
                            // CRITICAL: Skip player clan immediately
                            string playerClanNameCheck = Clan.PlayerClan?.Name?.ToString();
                            bool isPlayerClanCheck = !string.IsNullOrEmpty(playerClanNameCheck) && clan.Name?.ToString() == playerClanNameCheck;

                            if (isPlayerClanCheck)
                            {
                                Log.write($"> ✓✓✓ PLAYER CLAN DETECTED - SKIPPING ALL MODIFICATIONS ✓✓✓");
                                continue;
                            }

                            Clan playerClan2 = Clan.PlayerClan;
                            string text2;
                            if (playerClan2 == null)
                            {
                                text2 = null;
                            }
                            else
                            {
                                TextObject name4 = playerClan2.Name;
                                text2 = ((name4 != null) ? name4.ToString() : null);
                            }
                            string playerClanName = text2;

                            Clan playerClan3 = Clan.PlayerClan;
                            string text3;
                            if (playerClan3 == null)
                            {
                                text3 = null;
                            }
                            else
                            {
                                Kingdom kingdom5 = playerClan3.Kingdom;
                                if (kingdom5 == null)
                                {
                                    text3 = null;
                                }
                                else
                                {
                                    TextObject name5 = kingdom5.Name;
                                    text3 = ((name5 != null) ? name5.ToString() : null);
                                }
                            }
                            playerKingdomName = text3;

                            bool flag14;
                            if (!string.IsNullOrEmpty(playerClanName))
                            {
                                TextObject name6 = clan.Name;
                                flag14 = (((name6 != null) ? name6.ToString() : null) == playerClanName);
                            }
                            else
                            {
                                flag14 = false;
                            }
                            bool isPlayerClan = flag14;

                            bool flag15;
                            if (!string.IsNullOrEmpty(playerKingdomName))
                            {
                                TextObject name7 = kingdom.Name;
                                flag15 = (((name7 != null) ? name7.ToString() : null) == playerKingdomName);
                            }
                            else
                            {
                                flag15 = false;
                            }
                            isPlayerKingdom = flag15;

                            string[] array = new string[8];
                            array[0] = "> clan: [";
                            int num = 1;
                            TextObject name8 = clan.Name;
                            array[num] = ((name8 != null) ? name8.ToString() : null);
                            array[2] = "] kingdom: ";
                            int num2 = 3;
                            TextObject name9 = kingdom.Name;
                            array[num2] = ((name9 != null) ? name9.ToString() : null);
                            array[4] = ", isplayerKingdom: ";
                            array[5] = isPlayerKingdom.ToString();
                            array[6] = ", isPlayerClan:";
                            array[7] = isPlayerClan.ToString();
                            Log.write(string.Concat(array));

                            PocColorModConfig config2 = PocColorMod.config;
                            TextObject name10 = kingdom.Name;
                            string kingdom6 = (name10 != null) ? name10.ToString() : null;
                            TextObject name11 = clan.Name;
                            ValueTuple<bool, bool, string, string, string, string> clanConfig = config2.GetClanConfig(kingdom6, (name11 != null) ? name11.ToString() : null, isPlayerKingdom, isPlayerClan);
                            bool follow = clanConfig.Item1;
                            bool followBG = clanConfig.Item2;
                            string clanBanner = clanConfig.Item3;
                            string clanShield = clanConfig.Item4;
                            primary = clanConfig.Item5;
                            secondary = clanConfig.Item6;

                            string[] array2 = new string[8];
                            array2[0] = "> clan: [";
                            int num3 = 1;
                            TextObject name12 = clan.Name;
                            array2[num3] = ((name12 != null) ? name12.ToString() : null);
                            array2[2] = "] follow colors: ";
                            array2[3] = follow.ToString();
                            array2[4] = ", primary: ";
                            array2[5] = primary;
                            array2[6] = ", secondary:";
                            array2[7] = secondary;
                            Log.write(string.Concat(array2));

                            bool flag16 = PocColorMod.doLog;
                            if (flag16)
                            {
                                Log.write("> Initial clan Banner loaded from game: [" + PocColorMod.SerializeBanner(clan.Banner) + "]");
                            }

                            bool flag17 = PocColorMod.doLog;
                            if (flag17)
                            {
                                Log.write("> Initial clan primary loaded from game: [" + BannerManager.GetColorId(clan.Color).ToString() + "]");
                            }

                            bool flag18 = PocColorMod.doLog;
                            if (flag18)
                            {
                                Log.write("> Initial clan secondary loaded from game: [" + BannerManager.GetColorId(clan.Color2).ToString() + "]");
                            }

                            bool flag19 = clanBanner != null && !isPlayerClan;
                            if (flag19)
                            {
                                Log.write(string.Concat(new string[]
                                {
                            "> updating banner for clan: [",
                            clan.Name.ToString(),
                            ", ",
                            clanBanner,
                            "]"
                                }));

                                Banner banner = new Banner(clanBanner);
                                Traverse.Create(clan).Field("_banner").SetValue(banner);
                            }

                            bool flag20 = primary != null;
                            if (flag20)
                            {
                                Log.write("> updating primary color for clan");
                                uint color3 = PocColorModSetColors.parseColor(primary);
                                Traverse.Create(clan).Property("Color").SetValue(color3);
                                Banner banner5 = clan.Banner;
                                if (banner5 != null)
                                {
                                    banner5.ChangePrimaryColor(color3);
                                }
                            }
                            else if (clan.Banner != null && clan.Banner.BannerDataList != null && clan.Banner.BannerDataList.Count > 0)
                            {
                                // FALLBACK: Extract primary color from banner
                                int colorId = clan.Banner.BannerDataList[0].ColorId;
                                uint color1 = BannerManager.GetColor(colorId);
                                Traverse.Create(clan).Property("Color").SetValue(color1);
                                Log.write($"> Extracted primary color from banner: {color1} (colorId:{colorId})");
                            }

                            bool flag21 = secondary != null;
                            if (flag21)
                            {
                                Log.write("> updating secondary color for clan");
                                uint color4 = PocColorModSetColors.parseColor(secondary);
                                Traverse.Create(clan).Property("Color2").SetValue(color4);
                                Banner banner6 = clan.Banner;
                                if (banner6 != null)
                                {
                                    banner6.ChangeIconColors(color4);
                                }
                            }
                            else if (clan.Banner != null && clan.Banner.BannerDataList != null && clan.Banner.BannerDataList.Count > 0)
                            {
                                // FALLBACK: Extract secondary color from banner
                                int colorId = clan.Banner.BannerDataList[0].ColorId;
                                int colorId2 = clan.Banner.BannerDataList[0].ColorId2;

                                // If both colors are the same, try to get a different one from next layer
                                if (colorId2 == colorId && clan.Banner.BannerDataList.Count > 1)
                                {
                                    int layer1ColorId = clan.Banner.BannerDataList[1].ColorId;
                                    int layer1ColorId2 = clan.Banner.BannerDataList[1].ColorId2;

                                    if (layer1ColorId != colorId)
                                        colorId2 = layer1ColorId;
                                    else if (layer1ColorId2 != colorId)
                                        colorId2 = layer1ColorId2;
                                }

                                uint color2 = BannerManager.GetColor(colorId2);
                                Traverse.Create(clan).Property("Color2").SetValue(color2);
                                Log.write($"> Extracted secondary color from banner: {color2} (colorId:{colorId2})");
                            }

                            bool flag22 = string.IsNullOrEmpty(primary) && string.IsNullOrEmpty(secondary) && follow;
                            if (flag22)
                            {
                                Log.write("> updating colors of clan from Kingdom");
                                Traverse.Create(clan).Method("UpdateBannerColorsAccordingToKingdom", Array.Empty<object>()).GetValue();
                            }

                            // CRITICAL FIX: If this is player clan and no banner in config, keep vanilla banner
                            bool isThisPlayerClan = !string.IsNullOrEmpty(playerClanName) && clan.Name?.ToString() == playerClanName;
                            if (isThisPlayerClan)
                            {
                                Log.write($"> PLAYER clan detected in OnGameInitializationFinished");

                                // NEVER modify player clan banner during game initialization
                                // The player designed their banner during character creation
                                // We only load saved banners when loading existing games

                                Log.write($"> Skipping ALL modifications to player clan (preserving character creation banner)");

                                // Don't add to cache yet - let OnApplicationTick save the real banner
                                continue; // Skip to next clan
                            }

                            bannerStr = PocColorMod.SerializeBanner(clan.Banner);
                            Log.write("> adding new cache for banner: [" + bannerStr + "]");
                            bannerClanCacheTmp.TryAdd(bannerStr, clan.Name.ToString());
                            ConcurrentDictionary<string, string> concurrentDictionary3 = bannerKingdomCacheTmp;
                            string key3 = bannerStr;
                            string value;
                            if (kingdom == null)
                            {
                                value = null;
                            }
                            else
                            {
                                TextObject name13 = kingdom.Name;
                                value = ((name13 != null) ? name13.ToString() : null);
                            }
                            concurrentDictionary3.TryAdd(key3, value);
                        }
                        catch (Exception e3)
                        {
                            Log.write(e3.Message);
                        }
                    }
                }

                foreach (Clan clan2 in Campaign.Current.Clans)
                {
                    Kingdom kingdom2 = clan2.Kingdom;
                    Log.write("====> clan [" + clan2.Name.ToString() + "]");

                    // CRITICAL: Skip player clan immediately
                    string playerClanNameCheck2 = Clan.PlayerClan?.Name?.ToString();
                    bool isPlayerClanCheck2 = !string.IsNullOrEmpty(playerClanNameCheck2) && clan2.Name?.ToString() == playerClanNameCheck2;

                    if (isPlayerClanCheck2)
                    {
                        Log.write($"> ✓✓✓ PLAYER CLAN IN SECOND LOOP - SKIPPING ✓✓✓");
                        continue;
                    }

                    Clan playerClan4 = Clan.PlayerClan;
                    string text4;
                    if (playerClan4 == null)
                    {
                        text4 = null;
                    }
                    else
                    {
                        TextObject name14 = playerClan4.Name;
                        text4 = ((name14 != null) ? name14.ToString() : null);
                    }
                    string playerClanName2 = text4;

                    bool flag23;
                    if (!string.IsNullOrEmpty(playerClanName2))
                    {
                        TextObject name15 = clan2.Name;
                        flag23 = (((name15 != null) ? name15.ToString() : null) == playerClanName2);
                    }
                    else
                    {
                        flag23 = false;
                    }
                    bool isPlayerClan2 = flag23;

                    Clan playerClan5 = Clan.PlayerClan;
                    string text5;
                    if (playerClan5 == null)
                    {
                        text5 = null;
                    }
                    else
                    {
                        Kingdom kingdom7 = playerClan5.Kingdom;
                        if (kingdom7 == null)
                        {
                            text5 = null;
                        }
                        else
                        {
                            TextObject name16 = kingdom7.Name;
                            text5 = ((name16 != null) ? name16.ToString() : null);
                        }
                    }
                    string playerKingdomName2 = text5;

                    bool flag24;
                    if (!string.IsNullOrEmpty(playerKingdomName2))
                    {
                        string a;
                        if (kingdom2 == null)
                        {
                            a = null;
                        }
                        else
                        {
                            TextObject name17 = kingdom2.Name;
                            a = ((name17 != null) ? name17.ToString() : null);
                        }
                        flag24 = (a == playerKingdomName2);
                    }
                    else
                    {
                        flag24 = false;
                    }
                    bool isPlayerKingdom2 = flag24;

                    string[] array3 = new string[8];
                    array3[0] = "> clan: [";
                    int num4 = 1;
                    TextObject name18 = clan2.Name;
                    array3[num4] = ((name18 != null) ? name18.ToString() : null);
                    array3[2] = "] kingdom: ";
                    int num5 = 3;
                    string text6;
                    if (kingdom2 == null)
                    {
                        text6 = null;
                    }
                    else
                    {
                        TextObject name19 = kingdom2.Name;
                        text6 = ((name19 != null) ? name19.ToString() : null);
                    }
                    array3[num5] = text6;
                    array3[4] = ", isplayerKingdom: ";
                    array3[5] = isPlayerKingdom2.ToString();
                    array3[6] = ", isPlayerClan:";
                    array3[7] = isPlayerClan2.ToString();
                    Log.write(string.Concat(array3));

                    PocColorModConfig config3 = PocColorMod.config;
                    string kingdom8;
                    if (kingdom2 == null)
                    {
                        kingdom8 = null;
                    }
                    else
                    {
                        TextObject name20 = kingdom2.Name;
                        kingdom8 = ((name20 != null) ? name20.ToString() : null);
                    }
                    TextObject name21 = clan2.Name;
                    ValueTuple<bool, bool, string, string, string, string> clanConfig2 = config3.GetClanConfig(kingdom8, (name21 != null) ? name21.ToString() : null, isPlayerKingdom2, isPlayerClan2);
                    bool follow = clanConfig2.Item1;
                    bool followBG = clanConfig2.Item2;
                    string clanBanner = clanConfig2.Item3;
                    string clanShield = clanConfig2.Item4;
                    string primary = clanConfig2.Item5;
                    string secondary = clanConfig2.Item6;

                    string[] array4 = new string[10];
                    array4[0] = "> clan: [";
                    int num6 = 1;
                    TextObject name22 = clan2.Name;
                    array4[num6] = ((name22 != null) ? name22.ToString() : null);
                    array4[2] = "] follow: ";
                    array4[3] = follow.ToString();
                    array4[4] = ", follow Background: ";
                    array4[5] = followBG.ToString();
                    array4[6] = ", primary: ";
                    array4[7] = primary;
                    array4[8] = ", secondary:";
                    array4[9] = secondary;
                    Log.write(string.Concat(array4));

                    bool flag25 = PocColorMod.doLog;
                    if (flag25)
                    {
                        Log.write("> Initial clan Banner loaded from game: [" + PocColorMod.SerializeBanner(clan2.Banner) + "]");
                    }

                    bool flag26 = PocColorMod.doLog;
                    if (flag26)
                    {
                        Log.write("> Initial clan primary loaded from game: [" + BannerManager.GetColorId(clan2.Color).ToString() + "]");
                    }

                    bool flag27 = PocColorMod.doLog;
                    if (flag27)
                    {
                        Log.write("> Initial clan secondary loaded from game: [" + BannerManager.GetColorId(clan2.Color2).ToString() + "]");
                    }

                    bool flag28 = clanBanner != null && !isPlayerClan2;
                    if (flag28)
                    {
                        Log.write(string.Concat(new string[]
                        {
                    "> updating banner for clan: [",
                    clan2.Name.ToString(),
                    ", ",
                    clanBanner,
                    "]"
                        }));

                        Banner banner2 = new Banner(clanBanner);
                        Traverse.Create(clan2).Field("_banner").SetValue(banner2);
                    }

                    bool flag29 = primary != null;
                    if (flag29)
                    {
                        Log.write("> updating primary color for clan");
                        uint color5 = PocColorModSetColors.parseColor(primary);
                        Traverse.Create(clan2).Property("Color").SetValue(color5);
                        Banner banner7 = clan2.Banner;
                        if (banner7 != null)
                        {
                            banner7.ChangePrimaryColor(color5);
                        }
                    }
                    else if (clan2.Banner != null && clan2.Banner.BannerDataList != null && clan2.Banner.BannerDataList.Count > 0)
                    {
                        // FALLBACK: Extract primary color from banner
                        int colorId = clan2.Banner.BannerDataList[0].ColorId;
                        uint color1 = BannerManager.GetColor(colorId);
                        Traverse.Create(clan2).Property("Color").SetValue(color1);
                        Log.write($"> Extracted primary color from banner: {color1} (colorId:{colorId})");
                    }

                    bool flag30 = secondary != null;
                    if (flag30)
                    {
                        Log.write("> updating secondary color for clan");
                        uint color6 = PocColorModSetColors.parseColor(secondary);
                        Traverse.Create(clan2).Property("Color2").SetValue(color6);
                        Banner banner8 = clan2.Banner;
                        if (banner8 != null)
                        {
                            banner8.ChangeIconColors(color6);
                        }
                    }
                    else if (clan2.Banner != null && clan2.Banner.BannerDataList != null && clan2.Banner.BannerDataList.Count > 0)
                    {
                        // FALLBACK: Extract secondary color from banner
                        int colorId = clan2.Banner.BannerDataList[0].ColorId;
                        int colorId2 = clan2.Banner.BannerDataList[0].ColorId2;

                        // If both colors are the same, try to get a different one from next layer
                        if (colorId2 == colorId && clan2.Banner.BannerDataList.Count > 1)
                        {
                            int layer1ColorId = clan2.Banner.BannerDataList[1].ColorId;
                            int layer1ColorId2 = clan2.Banner.BannerDataList[1].ColorId2;

                            if (layer1ColorId != colorId)
                                colorId2 = layer1ColorId;
                            else if (layer1ColorId2 != colorId)
                                colorId2 = layer1ColorId2;
                        }

                        uint color2 = BannerManager.GetColor(colorId2);
                        Traverse.Create(clan2).Property("Color2").SetValue(color2);
                        Log.write($"> Extracted secondary color from banner: {color2} (colorId:{colorId2})");
                    }

                    bool flag31 = string.IsNullOrEmpty(primary) && string.IsNullOrEmpty(secondary) && (follow || followBG);
                    if (flag31)
                    {
                        Log.write("> updating colors of clan from Kingdom");
                        Traverse.Create(clan2).Method("UpdateBannerColorsAccordingToKingdom", Array.Empty<object>()).GetValue();
                    }



                    string bannerStr2 = PocColorMod.SerializeBanner(clan2.Banner);
                    Log.write("> adding new cache for banner: [" + bannerStr2 + "]");
                    bannerClanCacheTmp.TryAdd(bannerStr2, clan2.Name.ToString());
                    ConcurrentDictionary<string, string> concurrentDictionary4 = bannerKingdomCacheTmp;
                    string key4 = bannerStr2;
                    string value2;
                    if (kingdom2 == null)
                    {
                        value2 = null;
                    }
                    else
                    {
                        TextObject name23 = kingdom2.Name;
                        value2 = ((name23 != null) ? name23.ToString() : null);
                    }
                    concurrentDictionary4.TryAdd(key4, value2);
                }

                PocColorMod.bannerClanCache = bannerClanCacheTmp;
                PocColorMod.bannerKingdomCache = bannerKingdomCacheTmp;
                Log.write("Build Cache -- done");

                _needsKingdomColorReapply = true;
                _reapplyTickCounter = 0;

                // CRITICAL FIX: Apply colors even for loaded games
                if (isLoadedGame)
                {
                    Log.write("==> LOADED GAME: Applying clan colors from config");

                    foreach (Clan clan in Campaign.Current.Clans)
                    {
                        try
                        {
                            var kingdom = clan.Kingdom;
                            string playerClanName = Clan.PlayerClan?.Name?.ToString();
                            string playerKingdomName = Clan.PlayerClan?.Kingdom?.Name?.ToString();

                            bool isPlayerClan = !string.IsNullOrEmpty(playerClanName) && clan.Name?.ToString() == playerClanName;
                            bool isPlayerKingdom = !string.IsNullOrEmpty(playerKingdomName) && kingdom?.Name?.ToString() == playerKingdomName;

                            string clanName = clan.Name?.ToString();
                            string kingdomName = kingdom?.Name?.ToString();

                            var clanConfig = PocColorMod.config.GetClanConfig(kingdomName, clanName, isPlayerKingdom, isPlayerClan);
                            string primary = clanConfig.Item5;
                            string secondary = clanConfig.Item6;

                            if (primary != null)
                            {
                                uint color = parseColor(primary);
                                Traverse.Create(clan).Property("Color").SetValue(color);
                                if (clan.Banner != null)
                                    clan.Banner.ChangePrimaryColor(color);
                                Log.write($">>> LOADED: Applied primary color to [{clanName}]: {color}");
                            }

                            if (secondary != null)
                            {
                                uint color2 = parseColor(secondary);
                                Traverse.Create(clan).Property("Color2").SetValue(color2);
                                if (clan.Banner != null)
                                    clan.Banner.ChangeIconColors(color2);
                                Log.write($">>> LOADED: Applied secondary color to [{clanName}]: {color2}");
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.write($"Error applying colors to clan: {ex.Message}");
                        }
                    }

                    Log.write("==> LOADED GAME: Clan colors applied successfully");
                }

                SavePlayerBannerToConfig();


                bool generateTemplate = PocColorMod.config.generateTemplate;
                if (generateTemplate)
                {
                    // ... código de generación de template (sin cambios) ...
                }

                // Check player banner AFTER all processing
                if (Clan.PlayerClan != null && Clan.PlayerClan.Banner != null)
                {
                    Log.write($"==> AFTER processing - Player banner BannerDataList count: {Clan.PlayerClan.Banner.BannerDataList?.Count ?? -1}");

                    try
                    {
                        string bannerTest = Clan.PlayerClan.Banner.Serialize();
                        Log.write($"==> AFTER processing - Player banner serialized: [{bannerTest}]");
                    }
                    catch (Exception ex)
                    {
                        Log.write($"==> AFTER processing - Serialize error: {ex.Message}");
                    }
                }

                Log.write("==> OnGameInitializationFinished END");

            }
            catch (Exception e4)
            {
                Log.write(e4.Message);
            }
        }

        public override void OnAfterGameInitializationFinished(Game game, object starterObject)
        {
            base.OnAfterGameInitializationFinished(game, starterObject);

            if (game.GameType is Campaign && Campaign.Current != null)
            {
                // Guardar la hora actual del juego
                _campaignStartTime = CampaignTime.Now;
                _bannerSavedForCurrentGame = false;

                Log.write($">>> Campaign started at: {_campaignStartTime.ToHours}");
                Log.write(">>> Will save banner after 1 in-game hour");
            }
        }

        // DIAGNOSTIC VERSION - Patches one by one to find the problematic patch
        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
            base.OnGameStart(game, gameStarterObject);

            // RESETEAR FLAG al iniciar cualquier partida (nueva o cargada)

            // Reset banner save flag (will be saved in OnCampaignStart)
            _bannerSavedForCurrentGame = false;
            _campaignStartTime = CampaignTime.Zero;

            // APPLY HARMONY HERE - After character creation is complete
            if (!_harmonyPatched && Campaign.Current != null)
            {
                try
                {
                    Log.write("==> Applying Harmony patches INDIVIDUALLY (diagnostic mode)...");

                    var harmony = new Harmony("com.prpl.poccolormod");

                    // Get all types from the assembly
                    var assembly = Assembly.GetExecutingAssembly();
                    var types = assembly.GetTypes();

                    int successCount = 0;
                    int failCount = 0;

                    foreach (var type in types)
                    {
                        // Check if this is a Harmony patch class
                        var harmonyAttributes = type.GetCustomAttributes(typeof(HarmonyPatch), false);

                        if (harmonyAttributes.Length > 0)
                        {
                            try
                            {
                                // Try to patch this specific class
                                harmony.CreateClassProcessor(type).Patch();
                                successCount++;
                                Log.write($"  ✓ Successfully patched: {type.Name}");
                            }
                            catch (Exception patchEx)
                            {
                                failCount++;
                                Log.write($"  ✗ FAILED to patch: {type.Name}");
                                Log.write($"    Error: {patchEx.Message}");
                            }
                        }
                    }

                    _harmonyPatched = true;

                    // MANUAL PATCH for EquipWeaponWithNewEntity (fails with automatic patching)
                    try
                    {
                        Log.write("==> Attempting MANUAL patch for Agent.EquipWeaponWithNewEntity...");

                        var agentType = typeof(Agent);
                        var originalMethod = agentType.GetMethod("EquipWeaponWithNewEntity",
                            BindingFlags.Public | BindingFlags.Instance,
                            null,
                            new Type[] { typeof(EquipmentIndex), typeof(MissionWeapon).MakeByRefType() },
                            null);

                        if (originalMethod != null)
                        {
                            var postfixMethod = typeof(PocColor.PocColorModSetColors.PocColorModAgentEquipNewEntity)
                                .GetMethod("Postfix", BindingFlags.Public | BindingFlags.Static);

                            if (postfixMethod != null)
                            {
                                harmony.Patch(originalMethod, postfix: new HarmonyMethod(postfixMethod));
                                successCount++;
                                Log.write("  ✓ MANUAL patch SUCCESS: Agent.EquipWeaponWithNewEntity");
                            }
                            else
                            {
                                Log.write("  ✗ Could not find Postfix method for manual patching");
                                failCount++;
                            }
                        }
                        else
                        {
                            Log.write("  ✗ Could not find Agent.EquipWeaponWithNewEntity method");
                            failCount++;
                        }
                    }
                    catch (Exception manualEx)
                    {
                        Log.write($"  ✗ MANUAL patch FAILED: {manualEx.Message}");
                        failCount++;
                    }

                   

                    // MANUAL PATCH for AgentVisuals.Refresh (PRIVATE method)
                    try
                    {
                        Log.write("==> Attempting MANUAL patch for AgentVisuals.Refresh...");

                        var agentVisualsType = typeof(AgentVisuals);
                        var refreshMethod = agentVisualsType.GetMethod("Refresh",
                            BindingFlags.NonPublic | BindingFlags.Instance,
                            null,
                            new Type[] { typeof(bool), typeof(bool), typeof(Equipment), typeof(bool), typeof(bool) },
                            null);

                        if (refreshMethod != null)
                        {
                            var prefixMethod = typeof(PocColor.PocColorModSetColors.PocColorModAgentVisualsRefresh)
                                .GetMethod("Prefix", BindingFlags.Public | BindingFlags.Static);

                            if (prefixMethod != null)
                            {
                                harmony.Patch(refreshMethod, prefix: new HarmonyMethod(prefixMethod));
                                successCount++;
                                Log.write("  ✓ MANUAL patch SUCCESS: AgentVisuals.Refresh");
                            }
                            else
                            {
                                Log.write("  ✗ Could not find Prefix method");
                                failCount++;
                            }
                        }
                        else
                        {
                            Log.write("  ✗ Could not find AgentVisuals.Refresh method");
                            failCount++;
                        }
                    }
                    catch (Exception patchEx)
                    {
                        Log.write($"  ✗ MANUAL patch FAILED: {patchEx.Message}");
                        failCount++;
                    }


                    // MANUAL PATCH for AddSkinArmorWeaponMultiMeshesToEntity (PRIVATE method)
                    try
                    {
                        Log.write("==> Attempting MANUAL patch for AgentVisuals.AddSkinArmorWeaponMultiMeshesToEntity...");

                        var agentVisualsType = typeof(AgentVisuals);
                        var originalMethod = agentVisualsType.GetMethod("AddSkinArmorWeaponMultiMeshesToEntity",
                            BindingFlags.NonPublic | BindingFlags.Instance,
                            null,
                            new Type[] { typeof(uint), typeof(uint), typeof(bool), typeof(bool) },
                            null);

                        if (originalMethod != null)
                        {
                            var prefixMethod = typeof(PocColor.PocColorModSetColors.PocColorModAgentVisualsAddMeshes)  // ← NOMBRE CORRECTO
                                .GetMethod("Prefix", BindingFlags.Public | BindingFlags.Static);

                            if (prefixMethod != null)
                            {
                                harmony.Patch(originalMethod, prefix: new HarmonyMethod(prefixMethod));
                                successCount++;
                                Log.write("  ✓ MANUAL patch SUCCESS: AgentVisuals.AddSkinArmorWeaponMultiMeshesToEntity");
                            }
                            else
                            {
                                Log.write("  ✗ Could not find Prefix method");
                                failCount++;
                            }
                        }
                        else
                        {
                            Log.write("  ✗ Could not find AddSkinArmorWeaponMultiMeshesToEntity method");
                            failCount++;
                        }
                    }
                    catch (Exception manualEx)
                    {
                        Log.write($"  ✗ MANUAL patch FAILED: {manualEx.Message}");
                        failCount++;
                    }

                    // MANUAL PATCH for AgentVisuals.Refresh (PRIVATE method)
                    try
                    {
                        Log.write("==> Attempting MANUAL patch for AgentVisuals.Refresh...");

                        var agentVisualsType = typeof(AgentVisuals);
                        var refreshMethod = agentVisualsType.GetMethod("Refresh",
                            BindingFlags.NonPublic | BindingFlags.Instance,
                            null,
                            new Type[] { typeof(bool), typeof(bool), typeof(Equipment), typeof(bool), typeof(bool) },
                            null);

                        if (refreshMethod != null)
                        {
                            var prefixMethod = typeof(PocColor.PocColorModSetColors.PocColorModAgentVisualsRefresh)
                                .GetMethod("Prefix", BindingFlags.Public | BindingFlags.Static);

                            if (prefixMethod != null)
                            {
                                harmony.Patch(refreshMethod, prefix: new HarmonyMethod(prefixMethod));
                                successCount++;
                                Log.write("  ✓ MANUAL patch SUCCESS: AgentVisuals.Refresh");
                            }
                            else
                            {
                                Log.write("  ✗ Could not find Prefix method");
                                failCount++;
                            }
                        }
                        else
                        {
                            Log.write("  ✗ Could not find AgentVisuals.Refresh method");
                            failCount++;
                        }
                    }
                    catch (Exception patchEx)
                    {
                        Log.write($"  ✗ MANUAL patch FAILED: {patchEx.Message}");
                        failCount++;
                    }

                    // MANUAL PATCH (FINALIZER) for MissionScreen.ApplyBannerTextureToMesh
                    // Guards against NRE when armor banner tableaus (from other mods)
                    // render with a null mesh/texture on async thumbnail completion.
                    try
                    {
                        Log.write("==> Attempting MANUAL patch for MissionScreen.ApplyBannerTextureToMesh...");

                        var applyBannerMethod = AccessTools.Method("TaleWorlds.MountAndBlade.View.Screens.MissionScreen:ApplyBannerTextureToMesh");

                        if (applyBannerMethod != null)
                        {
                            var finalizerMethod = typeof(PocColor.PocColorModSetColors.PocColorModApplyBannerTextureGuard)
                                .GetMethod("Finalizer", BindingFlags.Public | BindingFlags.Static);

                            if (finalizerMethod != null)
                            {
                                harmony.Patch(applyBannerMethod, finalizer: new HarmonyMethod(finalizerMethod));
                                successCount++;
                                Log.write("  ✓ MANUAL patch SUCCESS: MissionScreen.ApplyBannerTextureToMesh (Finalizer guard)");
                            }
                            else
                            {
                                Log.write("  ✗ Could not find Finalizer method");
                                failCount++;
                            }
                        }
                        else
                        {
                            Log.write("  ✗ Could not find MissionScreen.ApplyBannerTextureToMesh method");
                            failCount++;
                        }
                    }
                    catch (Exception manualEx)
                    {
                        Log.write($"  ✗ MANUAL patch FAILED: {manualEx.Message}");
                        failCount++;
                    }

                    Log.write($"==> Harmony patching completed: {successCount} success, {failCount} failed");

                    if (failCount == 0)
                    {
                        InformationManager.DisplayMessage(new InformationMessage(
                            $"POC Color Mod - ACTIVE!",
                            Colors.Green));
                    }
                    else
                    {
                        InformationManager.DisplayMessage(new InformationMessage(
                            $"POC Color Mod - PARTIAL! ({successCount} OK, {failCount} failed)",
                            Colors.Yellow));
                    }
                }
                catch (Exception ex)
                {
                    Log.write("==> Harmony patching CRITICAL ERROR: " + ex.Message);
                    Log.write("==> Stack trace: " + ex.StackTrace);

                    InformationManager.DisplayMessage(new InformationMessage(
                        "POC Color Mod - ERROR! Check log file.",
                        Colors.Red));
                }
            }

            // Load banner from file if it exists
            if (Campaign.Current != null && Clan.PlayerClan != null)
            {
                Log.write("==> OnGameStart: Checking for saved banner...");

                string saveId = Campaign.Current?.UniqueGameId;
                string bannerPath = saveId != null ? $"..\\..\\modules\\PocColor\\PlayerBanners\\{saveId}.json" : null;

                if (bannerPath != null && File.Exists(bannerPath))
                {
                    Log.write("==> OnGameStart: Banner file EXISTS - force loading...");

                    // FORZAR carga del banner
                    LoadPlayerBannerFromSaveGame();

                    // Reconstruir cache
                    RebuildCacheOnLoad();
                }
                else
                {
                    Log.write("==> OnGameStart: No banner file - new game, rebuilding cache only...");
                    RebuildCacheOnLoad();
                }
            }
        }





        public override void OnGameLoaded(Game game, object initializerObject)
        {
            base.OnGameLoaded(game, initializerObject);

            try
            {
                if (game.GameType is Campaign && Campaign.Current != null && Clan.PlayerClan != null)
                {
                    Log.write(">>> ==========================================");
                    Log.write(">>> OnGameLoaded CALLED");
                    Log.write(">>> ==========================================");

                    // FORZAR carga del banner SIEMPRE
                    LoadPlayerBannerFromSaveGame();

                    // Reconstruir cache DESPUÉS de cargar banner
                    RebuildCacheOnLoad();

                    // FORZAR actualización del cache con el banner recién cargado
                    if (Clan.PlayerClan.Banner != null)
                    {
                        string playerBannerStr = PocColorMod.SerializeBanner(Clan.PlayerClan.Banner);
                        if (!string.IsNullOrEmpty(playerBannerStr))
                        {
                            PocColorMod.bannerClanCache[playerBannerStr] = Clan.PlayerClan.Name.ToString();

                            if (Clan.PlayerClan.Kingdom != null)
                            {
                                PocColorMod.bannerKingdomCache[playerBannerStr] = Clan.PlayerClan.Kingdom.Name.ToString();
                            }

                            Log.write($">>> FORCED player banner into cache: [{playerBannerStr}]");
                        }
                    }

                    Log.write(">>> ==========================================");
                }
            }
            catch (Exception e)
            {
                Log.write($"Error in OnGameLoaded: {e.Message}");
            }
        }


        // NEW METHOD: Rebuild cache when loading a saved game
        private void RebuildCacheOnLoad()
        {
            try
            {
                Log.write("==> Rebuilding banner cache on game load...");

                Map<string, string> bannerKingdomCacheTmp = new Map<string, string>();
                Map<string, string> bannerClanCacheTmp = new Map<string, string>();

                // Rebuild cache from all current kingdoms and clans
                if (Campaign.Current != null)
                {
                    foreach (Kingdom kingdom in Campaign.Current.Kingdoms)
                    {
                        if (kingdom.Banner != null)
                        {
                            string bannerStr = PocColorMod.SerializeBanner(kingdom.Banner);
                            bannerKingdomCacheTmp.TryAdd(bannerStr, kingdom.Name.ToString());

                            if (kingdom.RulingClan != null)
                            {
                                bannerClanCacheTmp.TryAdd(bannerStr, kingdom.RulingClan.Name.ToString());
                            }
                        }

                        foreach (Clan clan in kingdom.Clans)
                        {
                            if (clan.Banner != null)
                            {
                                string clanBannerStr = PocColorMod.SerializeBanner(clan.Banner);
                                bannerClanCacheTmp.TryAdd(clanBannerStr, clan.Name.ToString());
                                bannerKingdomCacheTmp.TryAdd(clanBannerStr, kingdom.Name.ToString());
                            }
                        }
                    }

                    // Add all independent clans
                    foreach (Clan clan in Campaign.Current.Clans)
                    {
                        if (clan.Banner != null && clan.Kingdom == null)
                        {
                            string clanBannerStr = PocColorMod.SerializeBanner(clan.Banner);
                            bannerClanCacheTmp.TryAdd(clanBannerStr, clan.Name.ToString());
                        }
                    }
                }

                // Update global cache
                PocColorMod.bannerClanCache = bannerClanCacheTmp;
                PocColorMod.bannerKingdomCache = bannerKingdomCacheTmp;

                Log.write($"==> Cache rebuilt: {bannerClanCacheTmp.Count} clan entries, {bannerKingdomCacheTmp.Count} kingdom entries");

                // CRITICAL: Log player's banner specifically
                if (Clan.PlayerClan?.Banner != null)
                {
                    string playerBannerStr = PocColorMod.SerializeBanner(Clan.PlayerClan.Banner);
                    Log.write($"==> Player banner in cache: [{playerBannerStr}] -> Clan: [{PocColorMod.bannerClanCache[playerBannerStr]}]");
                }
            }
            catch (Exception e)
            {
                Log.write($"Error rebuilding cache: {e.Message}");
            }
        }

        private void SavePlayerBannerToSaveGameFile()
        {
            Log.write(">>> ========================================");
            Log.write(">>> SavePlayerBannerToSaveGameFile CALLED");
            Log.write(">>> ========================================");

            try
            {
                if (Clan.PlayerClan == null)
                {
                    Log.write(">>> ERROR: PlayerClan is NULL");
                    return;
                }

                if (Clan.PlayerClan.Banner == null)
                {
                    Log.write(">>> ERROR: PlayerClan.Banner is NULL");
                    return;
                }

                Log.write($">>> PlayerClan: [{Clan.PlayerClan.Name?.ToString()}]");

                // CRITICAL: Log banner BEFORE serializing
                if (Clan.PlayerClan.Banner.BannerDataList != null)
                {
                    Log.write($">>> Banner.BannerDataList.Count: [{Clan.PlayerClan.Banner.BannerDataList.Count}]");

                    if (Clan.PlayerClan.Banner.BannerDataList.Count >= 2)
                    {
                        int bg = Clan.PlayerClan.Banner.BannerDataList[0].ColorId;
                        int icon = Clan.PlayerClan.Banner.BannerDataList[1].ColorId;
                        Log.write($">>> Banner colors BEFORE serialize: Background={bg}, Icon={icon}");
                    }
                }

                string bannerPath = GetSaveGameBannerPath();
                Log.write($">>> Banner path: [{bannerPath}]");

                if (bannerPath == null)
                {
                    Log.write(">>> ERROR: bannerPath is NULL");
                    return;
                }

                string bannerStr = "";
                try
                {
                    bannerStr = Clan.PlayerClan.Banner.Serialize();
                    Log.write($">>> Banner serialized: [{bannerStr}]");
                }
                catch (Exception serEx)
                {
                    Log.write($">>> ERROR serializing banner: {serEx.Message}");
                    return;
                }

                if (string.IsNullOrEmpty(bannerStr))
                {
                    Log.write(">>> ERROR: Banner string is EMPTY after serialization");
                    return;
                }

                uint color1 = Clan.PlayerClan.Color;
                uint color2 = Clan.PlayerClan.Color2;

                int colorId1 = BannerManager.GetColorId(color1);
                int colorId2 = BannerManager.GetColorId(color2);

                Log.write($">>> Clan.Color={color1} (id:{colorId1}), Clan.Color2={color2} (id:{colorId2})");

                var bannerData = new
                {
                    clanBanner = bannerStr,
                    primaryColor = colorId1.ToString(),
                    secondaryColor = colorId2.ToString()
                };

                string json = JsonConvert.SerializeObject(bannerData, Formatting.Indented);
                Log.write($">>> JSON to save: [{json}]");

                // Create directory if doesn't exist
                string directory = "..\\..\\modules\\PocColor\\PlayerBanners";
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                    Log.write(">>> Directory created");
                }

                File.WriteAllText(bannerPath, json);

                Log.write($">>> ✓✓✓ File saved successfully! ✓✓✓");
                Log.write(">>> ========================================");
            }
            catch (Exception e)
            {
                Log.write($">>> ✗✗✗ CRITICAL ERROR: {e.Message}");
                Log.write($">>> Stack trace: {e.StackTrace}");
                Log.write(">>> ========================================");
            }
        }
        private void SavePlayerBannerToConfig()
        {
            SavePlayerBannerToSaveGameFile();
        }


        protected override void OnApplicationTick(float dt)
        {
            base.OnApplicationTick(dt);

            // Re-apply kingdom/clan colors after save load
            if (_needsKingdomColorReapply)
            {
                _reapplyTickCounter++;
                if (_reapplyTickCounter >= 3 && Campaign.Current != null && config != null)
                {
                    _needsKingdomColorReapply = false;
                    try
                    {
                        Log.write("==> ReapplyColors: Re-applying kingdom/clan colors post-load");
                        string playerKingdomName = Clan.PlayerClan?.Kingdom?.Name?.ToString();

                        for (int ki = 0; ki < Campaign.Current.Kingdoms.Count; ki++)
                        {
                            Kingdom kingdom = Campaign.Current.Kingdoms[ki];
                            string kingdomName = kingdom.Name?.ToString();
                            bool isPlayerKingdom = !string.IsNullOrEmpty(playerKingdomName) && kingdomName == playerKingdomName;

                            var kConfig = config.GetKingdomConfig(kingdomName, isPlayerKingdom);

                            if (kConfig.Item3 != null)
                            {
                                uint c = PocColorModSetColors.parseColor(kConfig.Item3);
                                typeof(Kingdom).GetField("<PrimaryBannerColor>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic)
                                    .SetValueDirect(__makeref(kingdom), c);
                                typeof(Kingdom).GetField("<Color>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic)
                                    .SetValueDirect(__makeref(kingdom), c);
                                if (kingdom.Banner != null) kingdom.Banner.ChangePrimaryColor(c);
                            }

                            if (kConfig.Item4 != null)
                            {
                                uint c2 = PocColorModSetColors.parseColor(kConfig.Item4);
                                typeof(Kingdom).GetField("<SecondaryBannerColor>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic)
                                    .SetValueDirect(__makeref(kingdom), c2);
                                typeof(Kingdom).GetField("<Color2>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic)
                                    .SetValueDirect(__makeref(kingdom), c2);
                                if (kingdom.Banner != null) kingdom.Banner.ChangeIconColors(c2);
                            }

                            if (kConfig.Item1 != null)
                            {
                                Banner kb = new Banner(kConfig.Item1);
                                typeof(Kingdom).GetField("<Banner>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic)
                                    .SetValueDirect(__makeref(kingdom), kb);
                                Log.write($"> Reapplied kingdomBanner for [{kingdomName}]");
                            }

                            foreach (Clan clan in kingdom.Clans)
                            {
                                string pClanName = Clan.PlayerClan?.Name?.ToString();
                                if (!string.IsNullOrEmpty(pClanName) && clan.Name?.ToString() == pClanName) continue;

                                var cc = config.GetClanConfig(kingdomName, clan.Name?.ToString(), isPlayerKingdom, false);

                                if (cc.Item5 != null)
                                {
                                    uint c = PocColorModSetColors.parseColor(cc.Item5);
                                    Traverse.Create(clan).Property("Color").SetValue(c);
                                    if (clan.Banner != null) clan.Banner.ChangePrimaryColor(c);
                                }
                                if (cc.Item6 != null)
                                {
                                    uint c2 = PocColorModSetColors.parseColor(cc.Item6);
                                    Traverse.Create(clan).Property("Color2").SetValue(c2);
                                    if (clan.Banner != null) clan.Banner.ChangeIconColors(c2);
                                }
                                if (string.IsNullOrEmpty(cc.Item5) && string.IsNullOrEmpty(cc.Item6) && cc.Item1)
                                {
                                    Traverse.Create(clan).Method("UpdateBannerColorsAccordingToKingdom", Array.Empty<object>()).GetValue();
                                }
                            }
                        }
                        Log.write("==> ReapplyColors: Done");
                    }
                    catch (Exception reapplyEx)
                    {
                        Log.write($"==> ReapplyColors ERROR: {reapplyEx.Message}");
                    }
                }
            }

            try
            {
                // VALIDACIÓN: Si no hay campaña o clan, salir
                if (Campaign.Current == null || Clan.PlayerClan == null || Clan.PlayerClan.Banner == null)
                    return;

                // Si ya guardamos, salir
                if (_bannerSavedForCurrentGame)
                    return;

                // Si no hemos inicializado el tiempo de inicio, salir
                if (_campaignStartTime == CampaignTime.Zero)
                    return;

                // Verificar que tengamos UniqueGameId
                string saveId = Campaign.Current?.UniqueGameId;
                if (saveId == null)
                    return;

                string bannerPath = $"..\\..\\modules\\PocColor\\PlayerBanners\\{saveId}.json";

                // Si el archivo YA existe (partida cargada), marcar como guardado y salir
                if (File.Exists(bannerPath))
                {
                    _bannerSavedForCurrentGame = true;
                    _campaignStartTime = CampaignTime.Zero;
                    return;
                }

                // CRÍTICO: Esperar a que pase 1 hora de juego
                double hoursElapsed = CampaignTime.Now.ToHours - _campaignStartTime.ToHours;

                if (hoursElapsed < 1.0)
                {
                    // Todavía no ha pasado 1 hora
                    return;
                }

                Log.write($">>> 1 hour passed ({hoursElapsed:F2} hours), saving banner now...");

                // Intentar serializar el banner
                string bannerTest = "";
                try
                {
                    bannerTest = Clan.PlayerClan.Banner.Serialize();
                }
                catch
                {
                    return;
                }

                if (string.IsNullOrEmpty(bannerTest))
                    return;

                Log.write($">>> Banner to save: [{bannerTest}]");

                // Crear directorio si no existe
                string directory = "..\\..\\modules\\PocColor\\PlayerBanners";
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // Guardar el banner
                uint color1 = Clan.PlayerClan.Color;
                uint color2 = Clan.PlayerClan.Color2;

                var bannerData = new
                {
                    clanBanner = bannerTest,
                    primaryColor = BannerManager.GetColorId(color1).ToString(),
                    secondaryColor = BannerManager.GetColorId(color2).ToString()
                };

                string json = JsonConvert.SerializeObject(bannerData, Formatting.Indented);
                File.WriteAllText(bannerPath, json);

                _bannerSavedForCurrentGame = true;
                _campaignStartTime = CampaignTime.Zero;

                Log.write(">>> ✓✓✓ Banner saved after 1 in-game hour ✓✓✓");
            }
            catch
            {
                // Silenciar errores
            }
        }

        private string GetSaveGameBannerPath()
        {
            try
            {
                // Get current save game unique ID
                string saveId = Campaign.Current?.UniqueGameId ?? "default";
                string bannerConfigPath = $"..\\..\\modules\\PocColor\\PlayerBanners\\{saveId}.json";

                // Create directory if doesn't exist
                string directory = "..\\..\\modules\\PocColor\\PlayerBanners";
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                Log.write($">>> Banner file path for this save: [{bannerConfigPath}]");
                return bannerConfigPath;
            }
            catch (Exception e)
            {
                Log.write($">>> Error getting banner path: {e.Message}");
                return null;
            }
        }

        private static uint parseColor(string colorStr)
        {
            if (colorStr.StartsWith("#"))
            {
                string hex = "FF" + colorStr.Substring(1, 6);
                return Convert.ToUInt32(hex, 16);
            }

            try
            {
                int i = int.Parse(colorStr);
                return BannerManager.GetColor(i);
            }
            catch
            {
                return BannerManager.GetColor(40);
            }
        }
        private void LoadPlayerBannerFromSaveGame()
        {
            try
            {
                Log.write(">>> ============================================");
                Log.write(">>> === LoadPlayerBannerFromSaveGame START ===");
                Log.write(">>> ============================================");

                if (Clan.PlayerClan == null)
                {
                    Log.write(">>> ERROR: PlayerClan is NULL - aborting");
                    Log.write(">>> ============================================");
                    return;
                }

                Log.write($">>> PlayerClan: [{Clan.PlayerClan.Name?.ToString()}]");

                // PRIORITY 0: Check if user wants to use advanced config.json settings
                bool useAdvancedConfig = false;
                if (config?.clans != null && config.clans.ContainsKey("PlayerClan"))
                {
                    var playerConfig = config.clans["PlayerClan"];

                    // Check for useAdvancedConfig flag
                    var useAdvancedProp = playerConfig.GetType().GetProperty("useAdvancedConfig");
                    if (useAdvancedProp != null)
                    {
                        var value = useAdvancedProp.GetValue(playerConfig);
                        if (value is bool)
                        {
                            useAdvancedConfig = (bool)value;
                        }
                        else if (value is string)
                        {
                            bool.TryParse(value.ToString(), out useAdvancedConfig);
                        }
                    }

                    Log.write($">>> useAdvancedConfig from config.json: [{useAdvancedConfig}]");
                }

                // If advanced config is enabled, skip save file and go straight to config.json
                if (useAdvancedConfig)
                {
                    Log.write(">>> ⚙️ ADVANCED CONFIG MODE - Using config.json (skipping PlayerBanners file)");

                    var playerConfig = config.clans["PlayerClan"];
                    if (!string.IsNullOrEmpty(playerConfig.clanBanner))
                    {
                        Log.write($">>> Loading player banner from config.json: [{playerConfig.clanBanner}]");
                        var banner = new Banner(playerConfig.clanBanner);
                        Traverse.Create(Clan.PlayerClan).Field("_banner").SetValue(banner);

                        // NO aplicar colores - el banner YA tiene los colores correctos
                        // Solo actualizar los campos Color y Color2 del clan
                        if (!string.IsNullOrEmpty(playerConfig.primaryColor))
                        {
                            uint color = parseColor(playerConfig.primaryColor);
                            Clan.PlayerClan.Color = color;
                            Log.write($">>> Clan.Color updated from config: [{color}]");
                        }

                        if (!string.IsNullOrEmpty(playerConfig.secondaryColor))
                        {
                            uint color2 = parseColor(playerConfig.secondaryColor);
                            Clan.PlayerClan.Color2 = color2;
                            Log.write($">>> Clan.Color2 updated from config: [{color2}]");
                        }

                        // Update cache
                        string bannerStr = PocColorMod.SerializeBanner(banner);
                        if (!string.IsNullOrEmpty(bannerStr))
                        {
                            PocColorMod.bannerClanCache[bannerStr] = Clan.PlayerClan.Name?.ToString();
                            if (Clan.PlayerClan.Kingdom != null)
                            {
                                PocColorMod.bannerKingdomCache[bannerStr] = Clan.PlayerClan.Kingdom.Name?.ToString();
                            }
                        }

                        Log.write(">>> ✓✓✓ Player banner loaded from config.json (ADVANCED MODE) ✓✓✓");
                        Log.write(">>> ============================================");
                        return;
                    }
                }

                // PRIORITY 1: Check save game file (for casual users)
                string bannerPath = GetSaveGameBannerPath();
                Log.write($">>> Banner path from GetSaveGameBannerPath: [{bannerPath}]");

                if (bannerPath == null)
                {
                    Log.write(">>> ERROR: bannerPath is NULL!");
                    Log.write(">>> ============================================");
                    return;
                }

                bool fileExists = File.Exists(bannerPath);
                Log.write($">>> File.Exists({bannerPath}): [{fileExists}]");

                if (fileExists)
                {
                    Log.write($">>> ✓ File EXISTS! Reading...");

                    string json = File.ReadAllText(bannerPath);
                    Log.write($">>> JSON file content: [{json}]");

                    if (string.IsNullOrEmpty(json))
                    {
                        Log.write(">>> ERROR: JSON content is EMPTY!");
                        Log.write(">>> ============================================");
                        return;
                    }

                    var bannerData = JsonConvert.DeserializeObject<dynamic>(json);

                    if (bannerData == null)
                    {
                        Log.write(">>> ERROR: Failed to deserialize JSON!");
                        Log.write(">>> ============================================");
                        return;
                    }

                    string bannerStr = bannerData.clanBanner;
                    Log.write($">>> Banner string from file: [{bannerStr}]");

                    if (string.IsNullOrEmpty(bannerStr))
                    {
                        Log.write(">>> ERROR: Banner string is EMPTY in file");
                        Log.write(">>> ============================================");
                        return;
                    }

                    Log.write($">>> ✓ Valid banner string found: [{bannerStr}]");
                    Log.write($">>> Creating Banner object...");

                    var banner = new Banner(bannerStr);
                    Log.write($">>> ✓ Banner object created");
                    Log.write($">>> Setting banner to PlayerClan using Traverse...");

                    Traverse.Create(Clan.PlayerClan).Field("_banner").SetValue(banner);
                    Log.write($">>> ✓ Banner set to PlayerClan");

                    // NO aplicar colores - el banner YA tiene los colores correctos
                    // ChangePrimaryColor() y ChangeIconColors() CORROMPEN el banner
                    Log.write($">>> ⚠️ Skipping color application - banner already has correct colors from string");

                    // Solo actualizar los campos Color y Color2 del clan SIN modificar el banner
                    if (bannerData.primaryColor != null)
                    {
                        string primaryColorStr = bannerData.primaryColor.ToString();
                        uint color = parseColor(primaryColorStr);
                        Clan.PlayerClan.Color = color;
                        Log.write($">>> ✓ Clan.Color updated: [{color}]");
                    }

                    if (bannerData.secondaryColor != null)
                    {
                        string secondaryColorStr = bannerData.secondaryColor.ToString();
                        uint color2 = parseColor(secondaryColorStr);
                        Clan.PlayerClan.Color2 = color2;
                        Log.write($">>> ✓ Clan.Color2 updated: [{color2}]");
                    }

                    // Update cache
                    PocColorMod.bannerClanCache[bannerStr] = Clan.PlayerClan.Name?.ToString();
                    if (Clan.PlayerClan.Kingdom != null)
                    {
                        PocColorMod.bannerKingdomCache[bannerStr] = Clan.PlayerClan.Kingdom.Name?.ToString();
                    }

                    Log.write($">>> ✓ Cache updated");

                    Log.write(">>> ✓✓✓ Player banner loaded from save game file (CASUAL MODE) ✓✓✓");
                    Log.write(">>> ============================================");
                    return;
                }
                else
                {
                    Log.write($">>> File does NOT exist at path: [{bannerPath}]");
                }

                // PRIORITY 2: Check config.json for PlayerClan (fallback)
                Log.write(">>> Checking config.json for PlayerClan...");

                if (config == null)
                {
                    Log.write(">>> ERROR: config is NULL!");
                    Log.write(">>> ============================================");
                    return;
                }

                if (config.clans == null)
                {
                    Log.write(">>> config.clans is NULL");
                    Log.write(">>> Skipping to PRIORITY 3 (vanilla)");
                    Log.write(">>> ============================================");
                    return;
                }

                bool hasPlayerClan = config.clans.ContainsKey("PlayerClan");
                Log.write($">>> config.clans.ContainsKey('PlayerClan'): [{hasPlayerClan}]");

                if (hasPlayerClan)
                {
                    var playerConfig = config.clans["PlayerClan"];

                    if (playerConfig == null)
                    {
                        Log.write(">>> ERROR: playerConfig is NULL!");
                        Log.write(">>> ============================================");
                        return;
                    }

                    string configBanner = playerConfig.clanBanner;
                    Log.write($">>> playerConfig.clanBanner: [{configBanner ?? "NULL"}]");

                    if (!string.IsNullOrEmpty(configBanner))
                    {
                        Log.write($">>> Loading player banner from config.json: [{configBanner}]");
                        var banner = new Banner(configBanner);
                        Traverse.Create(Clan.PlayerClan).Field("_banner").SetValue(banner);

                        // NO aplicar colores - el banner YA tiene los colores correctos
                        if (!string.IsNullOrEmpty(playerConfig.primaryColor))
                        {
                            uint color = parseColor(playerConfig.primaryColor);
                            Clan.PlayerClan.Color = color;
                            Log.write($">>> Clan.Color updated from fallback: [{color}]");
                        }

                        if (!string.IsNullOrEmpty(playerConfig.secondaryColor))
                        {
                            uint color2 = parseColor(playerConfig.secondaryColor);
                            Clan.PlayerClan.Color2 = color2;
                            Log.write($">>> Clan.Color2 updated from fallback: [{color2}]");
                        }

                        // Update cache
                        string bannerStr = PocColorMod.SerializeBanner(banner);
                        if (!string.IsNullOrEmpty(bannerStr))
                        {
                            PocColorMod.bannerClanCache[bannerStr] = Clan.PlayerClan.Name?.ToString();
                            if (Clan.PlayerClan.Kingdom != null)
                            {
                                PocColorMod.bannerKingdomCache[bannerStr] = Clan.PlayerClan.Kingdom.Name?.ToString();
                            }
                        }

                        Log.write(">>> Player banner loaded from config.json (FALLBACK)");
                        Log.write(">>> ============================================");
                        return;
                    }
                    else
                    {
                        Log.write(">>> playerConfig.clanBanner is NULL or EMPTY");
                    }
                }

                // PRIORITY 3: Use vanilla game banner
                Log.write(">>> No saved banner found, using vanilla game banner");
                Log.write(">>> ============================================");
            }
            catch (Exception e)
            {
                Log.write($">>> ✗✗✗ ERROR loading player banner: {e.Message}");
                Log.write($">>> Stack trace: {e.StackTrace}");
                Log.write(">>> ============================================");
            }
        }

    


    }
}