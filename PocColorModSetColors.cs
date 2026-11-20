using HarmonyLib;
using Helpers;
using Newtonsoft.Json;
using PocColor.Config;
using SandBox.GauntletUI.BannerEditor;
using SandBox.View.Map;
using SandBox.ViewModelCollection.Nameplate;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.SceneInformationPopupTypes;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.CampaignSystem.ViewModelCollection.ClanManagement;
using TaleWorlds.CampaignSystem.ViewModelCollection.GameMenu.Overlay;
using TaleWorlds.CampaignSystem.ViewModelCollection.GameMenu.TroopSelection;
using TaleWorlds.CampaignSystem.ViewModelCollection.Inventory;
using TaleWorlds.CampaignSystem.ViewModelCollection.Party;
using TaleWorlds.Core;
using TaleWorlds.Core.ImageIdentifiers;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Core.ViewModelCollection.ImageIdentifiers;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.Scripts;
using TaleWorlds.MountAndBlade.View.Tableaus;
using TaleWorlds.MountAndBlade.ViewModelCollection.OrderOfBattle;
using TaleWorlds.ObjectSystem;

namespace PocColor
{
    internal class PocColorModSetColors
    {
        #region Constants
        public const int DEFAULT_MODE = 0;
        public const int RANDOMIZED = 1;
        public const int RANDOMIZED_LINKED = 11;
        public const int SHIELD_LINKED = 2;
        public const int SHIELD_LINKED_RANDOM = 21;
        public const int SHIELD_LINKED_RANDOM_FULL = 22;
        public const int BANNER_LINKED = 4;
        public const int BANNER_LINKED_RANDOM = 41;
        public const int BANNER_LINKED_RANDOM_FULL = 42;
        public const int OVERRIDE = 3;
        public const int COLOR_COUNT = 158;

        public static string DUMMY_BANNER1 = "30.82.88.3734.3734.768.768.1.0.0.522.40.40.924.924.762.775.0.0.0";
        public static string DUMMY_BANNER2 = "30.116.83.3734.3734.768.768.1.0.0.514.40.40.924.924.762.775.0.0.0";
        public static string DUMMY_BANNER3 = "4.82.88.1638.1638.768.768.1.0.0.517.116.40.924.924.762.775.0.0.0";
        #endregion

        #region Static Fields
        public static Random rnd = new Random();
        public static Map<string, Integer> extraColorMap = new Map<string, Integer>();
        public static Map<string, ColorData> colorDataCache = new Map<string, ColorData>();
        public static ColorData savedColorData;
        public static Map<string, ColorData> savedColorDataMap = new Map<string, ColorData>();
        #endregion

        #region Color Parsing and Random Generation
        public static uint parseColor(string colorStr)
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

        public static uint getRandomColor()
        {
            int r = rnd.Next(0, COLOR_COUNT);
            return BannerManager.GetColor(r);
        }

        public static uint getRandomColorFromColors(string[] colors)
        {
            if (colors == null || colors.Length == 0) return getRandomColor();
            int r = rnd.Next(0, colors.Length);
            return parseColor(colors[r]);
        }

        // Colores naturales para civiles - tonos medievales realistas
        private static readonly uint[] civilianColors = new uint[]
        {
            0xFF8B7355, // marrón claro
            0xFF6B4226, // marrón oscuro
            0xFF8B6914, // marrón dorado
            0xFFA0522D, // sienna
            0xFF856343, // marrón medio
            0xFF5C4033, // marrón café
            0xFF483C32, // marrón gris
            0xFF3B3C36, // gris oscuro
            0xFF696969, // gris medio
            0xFF808080, // gris
            0xFF9E9E9E, // gris claro
            0xFFA9A9A9, // gris plata
            0xFF556B2F, // verde oliva oscuro
            0xFF4A5D23, // verde bosque
            0xFF6B7B3A, // verde musgo
            0xFF2E3B2E, // verde muy oscuro
            0xFF8B0000, // rojo oscuro
            0xFF722F37, // vino
            0xFF800020, // borgoña
            0xFF36454F, // gris azulado
            0xFF1C1C1C, // casi negro
            0xFF2F2F2F, // carbón
            0xFFC4A882, // beige arena
            0xFFD2B48C, // tan
            0xFFDEB887, // burlywood
            0xFFF5DEB3, // trigo
            0xFFE8D5B7, // crema
            0xFFBDB76B, // khaki oscuro
            0xFF918151, // oliva dorado
            0xFF4B3621, // café oscuro
        };

        public static uint getRandomCivilianColor()
        {
            return civilianColors[rnd.Next(0, civilianColors.Length)];
        }

        public static string getRandomBannerFromBanners(string[] banners)
        {
            if (banners == null || banners.Length == 0) return null;
            int r = rnd.Next(0, banners.Length);
            return banners[r];
        }

        public static (uint?, uint?) getRandomPairColorsFromColors(string[] colors, string[] colors2)
        {
            int min = Math.Min(colors?.Length ?? 0, colors2?.Length ?? 0);
            if (min == 0) return (null, null);

            int r = rnd.Next(0, min);
            return (parseColor(colors[r]), parseColor(colors2[r]));
        }
        #endregion

        #region Core Config Retrieval (Public API)
        public static (int, uint?, uint?, string, string) getColorFromConfig(
            string clan,
            ref Banner banner,
            string unitName,
            bool isHero,
            bool isMounted,
            bool isRanged,
            int tier,
            string culture)
        {
            string bannerStr = banner != null ? PocColorMod.SerializeBanner(banner) : null;
            return getColorFromConfigInternal(clan, ref banner, ref bannerStr, unitName, isHero, isMounted, isRanged, tier, culture, false);
        }

        public static (int, uint?, uint?, string, string) getBattleColorFromConfig(
            string clan,
            ref Banner banner,
            string unitName,
            bool isHero,
            bool isMounted,
            bool isRanged,
            int tier,
            string culture)
        {
            string bannerStr = banner != null ? PocColorMod.SerializeBanner(banner) : null;
            return getColorFromConfigInternal(clan, ref banner, ref bannerStr, unitName, isHero, isMounted, isRanged, tier, culture, true);
        }
        #endregion

        #region Internal Config Logic
        private static (int, uint?, uint?, string, string) getColorFromConfigInternal(
     string clan,
     ref Banner banner,
     ref string bannerStr,
     string unitName,
     bool isHero,
     bool isMounted,
     bool isRanged,
     int tier,
     string culture,
     bool isBattle)
        {
            // CRITICAL CHECK
            if (PocColorMod.config == null)
            {
                Log.write(">>> ERROR: PocColorMod.config is NULL!");
                return (0, null, null, null, null);
            }

            uint? color = null;
            uint? color2 = null;
            string newBanner = null;
            string newShield = null;
            int mode = SHIELD_LINKED;

            try
            {
                // CRITICAL FIX: Force player banner BEFORE resolving clan info
                bool isPlayer = unitName == Hero.MainHero?.Name?.ToString();

                if (isPlayer && Clan.PlayerClan != null)
                {
                    string playerClanName = Clan.PlayerClan.Name?.ToString();
                    string playerKingdomName = Clan.PlayerClan.Kingdom?.Name?.ToString();

                    clan = playerClanName;

                    if (Clan.PlayerClan.Banner != null)
                    {
                        try
                        {
                            string testSerialize = Clan.PlayerClan.Banner.Serialize();

                            if (!string.IsNullOrEmpty(testSerialize))
                            {
                                bannerStr = testSerialize;
                                banner = Clan.PlayerClan.Banner;

                                if (!string.IsNullOrEmpty(playerClanName))
                                {
                                    PocColorMod.bannerClanCache[bannerStr] = playerClanName;
                                    if (!string.IsNullOrEmpty(playerKingdomName))
                                    {
                                        PocColorMod.bannerKingdomCache[bannerStr] = playerKingdomName;
                                    }
                                }

                                Log.write($">>> PLAYER banner OK: [{testSerialize}]");
                            }
                            else
                            {
                                Log.write($">>> PLAYER banner corrupted - using clan defaults only");
                                banner = null;
                                bannerStr = null;
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.write($">>> PLAYER banner error: {ex.Message} - using defaults");
                            banner = null;
                            bannerStr = null;
                        }
                    }
                    else
                    {
                        Log.write($">>> PLAYER banner is NULL - using defaults");
                        banner = null;
                        bannerStr = null;
                    }
                }

                // Resolve clan/kingdom info
                var (resolvedClan, kingdom, isPlayerClan, isPlayerKingdom) =
                    ResolveClanKingdomInfo(clan, bannerStr, unitName);

                clan = resolvedClan;

                // ✅ NUEVO: Check si hay config para este clan/reino/unidad
                bool hasConfig = PocColorMod.config.HasAnyConfigFor(
                    kingdom, clan, unitName, isPlayerKingdom, isPlayerClan);

                if (!hasConfig)
                {
                    if (PocColorMod.doLog)
                        Log.write($"==> No config found for kingdom:[{kingdom}] clan:[{clan}] unit:[{unitName}] - using vanilla");

                    return (DEFAULT_MODE, null, null, null, null);
                }

                if (PocColorMod.doLog)
                {
                    LogCharacterInfo(unitName, clan, kingdom, isPlayerKingdom, isPlayerClan,
                        isHero, isMounted, isRanged, tier, culture);
                }

                // Get config based on battle or normal mode
                var config = isBattle
                    ? PocColorMod.config.GetBattleConfig(kingdom, clan, unitName, isPlayerKingdom,
                        isPlayerClan, unitName == Hero.MainHero?.Name?.ToString(), false, false,
                        isHero, isMounted, isRanged, tier, culture)
                    : PocColorMod.config.GetConfig(kingdom, clan, unitName, isPlayerKingdom,
                        isPlayerClan, unitName == Hero.MainHero?.Name?.ToString(), false, false,
                        isHero, isMounted, isRanged, tier, culture);

                mode = config.Item1;
                string[] colors = config.Item2;
                string[] colors2 = config.Item3;
                string[] banners = config.Item4;
                string[] shields = config.Item5;

                if (PocColorMod.doLog)
                {
                    LogConfigApplication(mode, colors, colors2, banners, shields);
                }

                // Apply banners (NEVER override player's banner)
                if (banners != null && banners.Length > 0 && !isPlayer)
                {
                    newBanner = getRandomBannerFromBanners(banners);
                    if (newBanner != null)
                    {
                        bannerStr = newBanner;
                        banner = new Banner(newBanner);
                    }
                }

                // Apply shields (match working version: mutate banner for the player too,
                // this is what makes the player's shield tableau render the custom shield)
                if (shields != null && shields.Length > 0)
                {
                    newShield = getRandomBannerFromBanners(shields);
                    if (newShield != null)
                    {
                        bannerStr = newShield;
                        banner = new Banner(newShield);
                    }
                }

                // Apply colors based on mode
                (color, color2) = ApplyColorMode(mode, banner, colors, colors2);
            }
            catch (Exception e)
            {
                Log.write($"Error in getColorFromConfig: {e.Message}");
            }

            return (mode, color, color2, newBanner, newShield);
        }

        private static (string clan, string kingdom, bool isPlayerClan, bool isPlayerKingdom)
     ResolveClanKingdomInfo(string clan, string bannerStr, string unitName)
        {
            bool isPlayer = unitName == Hero.MainHero?.Name?.ToString();
            string kingdom = "";

            // CRITICAL FIX: If this is the player, ALWAYS get fresh data from Clan.PlayerClan
            if (isPlayer && Clan.PlayerClan != null)
            {
                clan = Clan.PlayerClan.Name?.ToString();
                kingdom = Clan.PlayerClan.Kingdom?.Name?.ToString() ?? "";

                // Update cache with player's current banner
                if (Clan.PlayerClan.Banner != null && !string.IsNullOrEmpty(clan))
                {
                    string freshBannerStr = PocColorMod.SerializeBanner(Clan.PlayerClan.Banner);
                    PocColorMod.bannerClanCache[freshBannerStr] = clan;
                    if (!string.IsNullOrEmpty(kingdom))
                    {
                        PocColorMod.bannerKingdomCache[freshBannerStr] = kingdom;
                    }
                }

                return (clan, kingdom, true, !string.IsNullOrEmpty(kingdom));
            }

            // Try to get clan from cache if not provided
            if (clan == null && bannerStr != null)
            {
                clan = PocColorMod.bannerClanCache[bannerStr] ?? "";
            }

            // Try to get kingdom from cache or clan
            if (clan == null || clan == "")
            {
                kingdom = PocColorMod.bannerKingdomCache[bannerStr] ?? "";
            }
            else
            {
                Clan myclan = Clan.FindFirst(c => c.Name?.ToString() == clan);
                kingdom = myclan?.Kingdom?.Name?.ToString() ?? "";
            }

            // Get player info
            string playerClanName = Clan.PlayerClan?.Name?.ToString();
            string playerKingdomName = Clan.PlayerClan?.Kingdom?.Name?.ToString();

            // Update cache if player changed clan
            if (isPlayer && clan != playerClanName && !string.IsNullOrEmpty(playerClanName) && !string.IsNullOrEmpty(clan))
            {
                UpdateBannerCache(bannerStr, playerClanName, playerKingdomName);
                clan = playerClanName;
                kingdom = playerKingdomName ?? kingdom;
            }

            bool isPlayerClan = (!string.IsNullOrEmpty(playerClanName) && clan == playerClanName) ||
                                (isPlayer && !string.IsNullOrEmpty(clan));
            bool isPlayerKingdom = !string.IsNullOrEmpty(playerKingdomName) && kingdom == playerKingdomName;

            return (clan, kingdom, isPlayerClan, isPlayerKingdom);
        }

        // Returns ALL color IDs actually visible in the banner, deduplicated.
        // Layer 0 is the background: both ColorId (background) and ColorId2 (pattern) always render.
        // Sigil layers: ColorId2 is the stroke color and only renders when DrawStroke is enabled.
        // Used only by the "FULL" random modes (22 and 42).
        private static string[] GetAllBannerColorIds(Banner banner)
        {
            if (banner?.BannerDataList == null || banner.BannerDataList.Count == 0)
                return new string[0];

            var ids = new List<int>();

            for (int i = 0; i < banner.BannerDataList.Count; i++)
            {
                var bd = banner.BannerDataList[i];

                if (bd.ColorId >= 0 && !ids.Contains(bd.ColorId))
                    ids.Add(bd.ColorId);

                bool secondaryIsVisible = (i == 0) || bd.DrawStroke;

                if (secondaryIsVisible && bd.ColorId2 >= 0 && !ids.Contains(bd.ColorId2))
                    ids.Add(bd.ColorId2);
            }

            return ids.Select(id => id.ToString()).ToArray();
        }

        private static (uint?, uint?) ApplyColorMode(int mode, Banner banner, string[] colors, string[] colors2)
        {
            uint? color = null;
            uint? color2 = null;

            // Banner-linked modes (4, 41, 42)
            if ((mode == BANNER_LINKED || mode == BANNER_LINKED_RANDOM || mode == BANNER_LINKED_RANDOM_FULL)
                && banner?.BannerDataList != null && banner.BannerDataList.Count > 0)
            {
                int colorId = banner.BannerDataList[0].ColorId;
                int colorId2 = banner.BannerDataList[0].ColorId2;

                // FIX: Si ColorId2 == ColorId, buscar en otras capas
                if (colorId2 == colorId && banner.BannerDataList.Count > 1)
                {
                    int layer1ColorId = banner.BannerDataList[1].ColorId;
                    int layer1ColorId2 = banner.BannerDataList[1].ColorId2;

                    if (layer1ColorId != colorId)
                        colorId2 = layer1ColorId;
                    else if (layer1ColorId2 != colorId)
                        colorId2 = layer1ColorId2;
                }

                // Si todavía son iguales, usar GetSecondaryColor()
                if (colorId2 == colorId)
                {
                    uint secondaryColor = banner.GetSecondaryColor();
                    if (secondaryColor != uint.MaxValue)
                    {
                        colorId2 = BannerManager.GetColorId(secondaryColor);
                    }
                }

                if (mode == BANNER_LINKED)
                {
                    color = BannerManager.GetColor(colorId);
                    color2 = BannerManager.GetColor(colorId2);
                }
                else if (mode == BANNER_LINKED_RANDOM)
                {
                    var pair = getRandomPairColorsFromColors(
                        new[] { colorId.ToString(), colorId2.ToString() },
                        new[] { colorId2.ToString(), colorId.ToString() });
                    color = pair.Item1;
                    color2 = pair.Item2;
                }
                else if (mode == BANNER_LINKED_RANDOM_FULL && banner.BannerDataList.Count > 0)
                {
                    // Use every color of the banner, not just the first color of each layer
                    var bannerColors = GetAllBannerColorIds(banner);
                    color = getRandomColorFromColors(bannerColors);
                    color2 = getRandomColorFromColors(bannerColors);
                }

                return (color, color2);
            }

            // Shield-linked modes (2, 21, 22)
            if ((mode == SHIELD_LINKED || mode == SHIELD_LINKED_RANDOM || mode == SHIELD_LINKED_RANDOM_FULL)
                && banner?.BannerDataList != null && banner.BannerDataList.Count > 0)
            {
                int colorId = banner.BannerDataList[0].ColorId;
                int colorId2 = banner.BannerDataList[0].ColorId2;

                // FIX: Si ColorId2 == ColorId, buscar en otras capas
                if (colorId2 == colorId && banner.BannerDataList.Count > 1)
                {
                    int layer1ColorId = banner.BannerDataList[1].ColorId;
                    int layer1ColorId2 = banner.BannerDataList[1].ColorId2;

                    if (layer1ColorId != colorId)
                        colorId2 = layer1ColorId;
                    else if (layer1ColorId2 != colorId)
                        colorId2 = layer1ColorId2;
                }

                // Si todavía son iguales, usar GetSecondaryColor()
                if (colorId2 == colorId)
                {
                    uint secondaryColor = banner.GetSecondaryColor();
                    if (secondaryColor != uint.MaxValue)
                    {
                        colorId2 = BannerManager.GetColorId(secondaryColor);
                    }
                }

                if (mode == SHIELD_LINKED)
                {
                    color = BannerManager.GetColor(colorId);
                    color2 = BannerManager.GetColor(colorId2);
                }
                else if (mode == SHIELD_LINKED_RANDOM)
                {
                    var pair = getRandomPairColorsFromColors(
                        new[] { colorId.ToString(), colorId2.ToString() },
                        new[] { colorId2.ToString(), colorId.ToString() });
                    color = pair.Item1;
                    color2 = pair.Item2;
                }
                else if (mode == SHIELD_LINKED_RANDOM_FULL && banner.BannerDataList.Count > 0)
                {
                    // Use every color of the banner, not just the first color of each layer
                    var bannerColors = GetAllBannerColorIds(banner);
                    color = getRandomColorFromColors(bannerColors);
                    color2 = getRandomColorFromColors(bannerColors);
                }

                return (color, color2);
            }

            // Randomized modes (1, 11)
            if (mode == RANDOMIZED || mode == RANDOMIZED_LINKED)
            {
                bool hasColors = (colors != null && colors.Length > 0) || (colors2 != null && colors2.Length > 0);

                if (!hasColors)
                {
                    color = getRandomColor();
                    color2 = mode == RANDOMIZED_LINKED ? color : getRandomColor();
                }
                else
                {
                    if (colors == null) colors = colors2;
                    if (colors2 == null) colors2 = colors;

                    if (mode == RANDOMIZED)
                    {
                        color = getRandomColorFromColors(colors);
                        color2 = getRandomColorFromColors(colors2);
                    }
                    else // RANDOMIZED_LINKED
                    {
                        var pair = getRandomPairColorsFromColors(colors, colors2);
                        color = pair.Item1;
                        color2 = pair.Item2;
                    }
                }

                return (color, color2);
            }

            // Override mode (3)
            if (mode == OVERRIDE && colors != null && colors.Length >= 2)
            {
                color = parseColor(colors[0]);
                color2 = parseColor(colors[1]);
            }

            return (color, color2);
        }
        #endregion

        #region Banner and Icon Helpers
        public static (string, string) getBannerFromConfig(
     string clan,
     string bannerStr,
     string unitName,
     bool isHero,
     bool isMounted,
     bool isRanged,
     int tier,
     string culture)
        {
            string newBanner = null;
            string newShield = null;

            try
            {
                var (resolvedClan, kingdom, isPlayerClan, isPlayerKingdom) =
                    ResolveClanKingdomInfo(clan, bannerStr, unitName);

                // ✅ NUEVO: Check si hay config
                bool hasConfig = PocColorMod.config.HasAnyConfigFor(
                    kingdom, resolvedClan, unitName, isPlayerKingdom, isPlayerClan);

                if (!hasConfig)
                {
                    if (PocColorMod.doLog)
                        Log.write($"==> getBannerFromConfig: No config for [{resolvedClan}] - using vanilla");

                    return (null, null);
                }

                var config = PocColorMod.config.GetConfig(kingdom, resolvedClan, unitName,
                    isPlayerKingdom, isPlayerClan, unitName == Hero.MainHero?.Name?.ToString(),
                    false, false, isHero, isMounted, isRanged, tier, culture);

                string[] banners = config.Item4;
                string[] shields = config.Item5;

                if (banners != null && banners.Length > 0)
                    newBanner = getRandomBannerFromBanners(banners);

                if (shields != null && shields.Length > 0)
                    newShield = getRandomBannerFromBanners(shields);
            }
            catch (Exception e)
            {
                Log.write($"Error in getBannerFromConfig: {e.Message}");
            }

            return (newBanner, newShield);
        }

        public static string getIconFromConfig(
    string clan,
    Banner banner,
    string unitName,
    bool isHero,
    bool isMounted,
    bool isRanged,
    int tier,
    string culture)
        {
            string bannerStr = banner != null ? PocColorMod.SerializeBanner(banner) : null;

            try
            {
                var (resolvedClan, kingdom, isPlayerClan, isPlayerKingdom) =
                    ResolveClanKingdomInfo(clan, bannerStr, unitName);

                // ✅ NUEVO: Check si hay config
                bool hasConfig = PocColorMod.config.HasAnyConfigFor(
                    kingdom, resolvedClan, unitName, isPlayerKingdom, isPlayerClan);

                if (!hasConfig)
                {
                    if (PocColorMod.doLog)
                        Log.write($"==> getIconFromConfig: No config for [{resolvedClan}] - using vanilla");

                    return bannerStr;
                }

                // Try clan config first
                var clanConfig = PocColorMod.config.GetClanConfig(kingdom, resolvedClan, isPlayerKingdom, isPlayerClan);
                if (clanConfig.Item4 != null) // shield
                    return clanConfig.Item4;

                // Try kingdom config
                var kingdomConfig = PocColorMod.config.GetKingdomConfig(kingdom, isPlayerKingdom);
                if (kingdomConfig.Item2 != null) // shield
                    return kingdomConfig.Item2;

                // Fallback to unit config
                var colorConfig = getColorFromConfig(resolvedClan, ref banner, unitName,
                    isHero, isMounted, isRanged, tier, culture);

                return colorConfig.Item5 ?? bannerStr;
            }
            catch (Exception e)
            {
                Log.write($"Error in getIconFromConfig: {e.Message}");
                return bannerStr;
            }
        }
        #endregion

        #region Cache Management
        private static void UpdateBannerCache(string bannerStr, string clanName, string kingdomName)
        {
            if (string.IsNullOrEmpty(bannerStr)) return;

            try
            {
                PocColorMod.bannerClanCache.TryRemove(bannerStr, out _);
                PocColorMod.bannerKingdomCache.TryRemove(bannerStr, out _);

                if (!string.IsNullOrEmpty(clanName))
                    PocColorMod.bannerClanCache.TryAdd(bannerStr, clanName);

                if (!string.IsNullOrEmpty(kingdomName))
                    PocColorMod.bannerKingdomCache.TryAdd(bannerStr, kingdomName);
            }
            catch (Exception e)
            {
                Log.write($"Error updating banner cache: {e.Message}");
            }
        }

        public static void updateCache(string bannerStr, string unitName)
        {
            try
            {
                bool isPlayer = unitName == Hero.MainHero?.Name?.ToString();
                string clan = PocColorMod.bannerClanCache[bannerStr] ?? "";
                string playerClanName = Clan.PlayerClan?.Name?.ToString();
                string playerKingdomName = Clan.PlayerClan?.Kingdom?.Name?.ToString();

                if (isPlayer && clan != playerClanName && playerClanName != null)
                {
                    UpdateBannerCache(bannerStr, playerClanName, playerKingdomName);
                }
            }
            catch (Exception e)
            {
                Log.write($"Error in updateCache: {e.Message}");
            }
        }
        #endregion

        #region Logging
        private static void LogCharacterInfo(string unitName, string clan, string kingdom,
            bool isPlayerKingdom, bool isPlayerClan, bool isHero, bool isMounted, bool isRanged, int tier, string culture)
        {
            Log.write($"==> character is: [{unitName}] of clan [{clan}] of kingdom [{kingdom}] " +
                      $"isPlayerKingdom [{isPlayerKingdom}] isPlayerClan [{isPlayerClan}] " +
                      $"isHero [{isHero}] isMounted [{isMounted}] isRanged [{isRanged}] " +
                      $"tier [{tier}] culture [{culture}]");
        }

        private static void LogConfigApplication(int mode, string[] colors, string[] colors2, string[] banners, string[] shields)
        {
            string colorsStr = colors != null ? string.Join(",", colors) : "";
            string colors2Str = colors2 != null ? string.Join(",", colors2) : "";
            string bannersStr = banners != null ? string.Join(",", banners) : "";
            string shieldsStr = shields != null ? string.Join(",", shields) : "";

            Log.write($"> Applying colors according to: mode[{mode}] colors[{colorsStr}] " +
                      $"colors2[{colors2Str}] newBanner[{bannersStr}] shields[{shieldsStr}]");
        }
        #endregion

        #region AgentBuildData Patches
        [HarmonyPatch(typeof(AgentBuildData), "Banner")]
        internal class PocColorModColorsFromBanner
        {
            public static bool Prefix(ref AgentBuildData __instance, ref Banner banner)
            {
                try
                {
                    if (Campaign.Current == null || Clan.PlayerClan == null)
                        return true;

                    var character = __instance.AgentCharacter;
                    string charName = character?.GetName()?.ToString();

                    if (banner != null && character != null)
                    {
                        var chara = CharacterObject.FindFirst(c => c.Name?.ToString() == charName);
                        int tier = chara?.Tier ?? 0;

                        // FIX: Obtener clan del personaje en vez de pasar null
                        string clanName = chara?.HeroObject?.Clan?.Name?.ToString();

                        // Si no tiene clan directo, intentar del Origin
                        if (string.IsNullOrEmpty(clanName))
                        {
                            var origin = __instance.AgentOrigin;
                            if (origin?.BattleCombatant != null)
                            {
                                var party = origin.BattleCombatant as PartyBase;
                                clanName = party?.MobileParty?.ActualClan?.Name?.ToString()
                                        ?? party?.Owner?.Clan?.Name?.ToString();
                            }
                        }

                        var battleConfig = getBattleColorFromConfig(clanName, ref banner, charName,
                            character.IsHero, character.IsMounted, character.IsRanged, tier,
                            character.Culture?.Name?.ToString() ?? "");

                        // GUARDAR EN SAVEDCOLORDATA GLOBAL
                        savedColorData = new ColorData(battleConfig.Item1, battleConfig.Item2,
                            battleConfig.Item3, battleConfig.Item4, battleConfig.Item5);

                        Log.write($">>> BATTLE: [{charName}] colors saved: c1={battleConfig.Item2}, c2={battleConfig.Item3}");

                        if (battleConfig.Item2.HasValue && battleConfig.Item3.HasValue)
                        {
                            __instance.AgentData.ClothingColor1(battleConfig.Item2.Value);
                            __instance.AgentData.ClothingColor2(battleConfig.Item3.Value);
                        }

                        // FIX: Vassal hero color override
                        // When player is vassal, vanilla uses kingdom colors instead of clan colors
                        // If no config was found (colors are null), force clan banner colors for player hero
                        if (!battleConfig.Item2.HasValue && !battleConfig.Item3.HasValue)
                        {
                            bool isPlayerHero = charName == Hero.MainHero?.Name?.ToString();
                            if (isPlayerHero && Clan.PlayerClan?.Kingdom != null && Clan.PlayerClan?.Banner != null)
                            {
                                uint clanColor1 = Clan.PlayerClan.Banner.GetPrimaryColor();
                                uint clanColor2 = Clan.PlayerClan.Banner.GetFirstIconColor();
                                __instance.AgentData.ClothingColor1(clanColor1);
                                __instance.AgentData.ClothingColor2(clanColor2);
                                savedColorData = new ColorData(SHIELD_LINKED, clanColor1, clanColor2, null, null);
                                Log.write($">>> VASSAL FIX: Player hero [{charName}] forced clan colors: c1={clanColor1}, c2={clanColor2}");
                            }
                        }



                    }
                    else
                    {
                        // FIX: Si este agente no tiene banner o character,
                        // limpiar savedColorData para que no herede colores de otro agente
                        savedColorData = null;
                    }
                }
                catch (Exception e)
                {
                    Log.write($"Error in PocColorModColorsFromBanner: {e.Message}");
                }

                return true;
            }
        }

        [HarmonyPatch(typeof(AgentBuildData), "ClothingColor1")]
        internal class PocColorModColor1
        {
            public static bool Prefix(ref AgentBuildData __instance, ref uint color, ref AgentBuildData __result)
            {
                try
                {
                    // SAFETY: Skip during character creation
                    if (Campaign.Current == null || Clan.PlayerClan == null)
                        return true;

                    var character = __instance.AgentCharacter;
                    var banner = __instance.AgentBanner;

                    if (character != null && banner != null)
                    {
                        __result = __instance;
                        return false;
                    }

                    uint color2 = __instance.AgentClothingColor2;
                    updateAgentBuildDataColors(ref __instance, ref color, ref color2, true);
                }
                catch (Exception e)
                {
                    Log.write($"Error in PocColorModColor1: {e.Message}");
                }

                return true;
            }
        }

        [HarmonyPatch(typeof(AgentBuildData), "ClothingColor2")]
        internal class PocColorModColor2
        {
            public static bool Prefix(ref AgentBuildData __instance, ref uint color, ref AgentBuildData __result)
            {
                try
                {
                    // SAFETY: Skip during character creation
                    if (Campaign.Current == null || Clan.PlayerClan == null)
                        return true;

                    var character = __instance.AgentCharacter;
                    var banner = __instance.AgentBanner;

                    if (character != null && banner != null)
                    {
                        __result = __instance;
                        return false;
                    }

                    uint color2 = __instance.AgentClothingColor1;
                    updateAgentBuildDataColors(ref __instance, ref color2, ref color, false);
                }
                catch (Exception e)
                {
                    Log.write($"Error in PocColorModColor2: {e.Message}");
                }

                return true;
            }
        }

        public static bool updateAgentBuildDataColors(ref AgentBuildData __instance, ref uint color1, ref uint color2, bool first)
        {
            var character = __instance.AgentCharacter;
            var banner = __instance.AgentBanner;

            try
            {
                if (character == null) return true;

                if (banner != null)
                {
                    string charName = character.GetName()?.ToString();
                    var chara = CharacterObject.FindFirst(c => c.Name?.ToString() == charName);

                    var config = getColorFromConfig(null, ref banner, charName, character.IsHero,
                        character.IsMounted, character.IsRanged, chara?.Tier ?? 0,
                        character.Culture?.Name?.ToString() ?? "");

                    if (config.Item2.HasValue && config.Item3.HasValue)
                    {
                        color1 = config.Item2.Value;
                        color2 = config.Item3.Value;

                        if (first)
                            extraColorMap.TryAdd(character.StringId, new Integer(color2));
                        else
                            extraColorMap.TryAdd(character.StringId, new Integer(color1));

                        Traverse.Create(__instance).Property("AgentBanner").SetValue(banner);
                        __instance.AgentData.ClothingColor1(color1);
                        __instance.AgentData.ClothingColor2(color2);
                    }
                }
                else
                {
                    var origin = __instance.AgentOrigin;
                    if (origin?.Banner != null)
                    {
                        banner = origin.Banner;
                        string charName = character.GetName()?.ToString();

                        uint? c1, c2;
                        if (savedColorData != null)
                        {
                            c1 = savedColorData.c1;
                            c2 = savedColorData.c2;
                        }
                        else
                        {
                            var config = getColorFromConfig(null, ref banner, charName, character.IsHero,
                                character.IsMounted, character.IsRanged, 0,
                                character.Culture?.Name?.ToString() ?? "");
                            c1 = config.Item2;
                            c2 = config.Item3;
                        }

                        if (c1.HasValue && c2.HasValue)
                        {
                            color1 = c1.Value;
                            color2 = c2.Value;
                            Traverse.Create(__instance).Property("AgentBanner").SetValue(banner);
                            __instance.AgentData.ClothingColor1(color1);
                            __instance.AgentData.ClothingColor2(color2);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.write($"Error in updateAgentBuildDataColors: {e.Message}");
            }

            // FIX: Limpiar para no contaminar siguiente agente
            savedColorData = null;

            return true;
        }
        #endregion

        #region ViewModel Patches (Consolidated)

        // Common helper for ViewModel patches
        private static void ApplyColorsToViewModel(
            object viewModel,
            string visualFieldName,
            CharacterObject character,
            string clanName,
            Banner banner,
            out uint? color1,
            out uint? color2,
            out Banner updatedBanner)
        {
            color1 = null;
            color2 = null;
            updatedBanner = banner;

            try
            {
                string charName = character?.Name?.ToString();
                var config = getColorFromConfig(clanName, ref updatedBanner, charName,
                    character?.IsHero ?? false, character?.IsMounted ?? false,
                    character?.IsRanged ?? false, character?.Tier ?? 0,
                    character?.Culture?.Name?.ToString() ?? "");

                color1 = config.Item2;
                color2 = config.Item3;
                string newBanner = config.Item4;
                string newShield = config.Item5;

                // Update banner
                if (newShield != null)
                    updatedBanner = new Banner(newShield);
                else if (newBanner != null)
                    updatedBanner = new Banner(newBanner);

                // Apply colors to visual
                if (color1.HasValue && color2.HasValue && !string.IsNullOrEmpty(visualFieldName))
                {
                    var ivm = (ImageIdentifierVM)Traverse.Create(viewModel).Field(visualFieldName).GetValue();
                    if (ivm != null)
                    {
                        var imageIdentifier = (ImageIdentifier)Traverse.Create(ivm).Field("_imageIdentifier").GetValue();
                        var charCode = CharacterCode.CreateFrom(imageIdentifier.Id);
                        charCode.Color1 = color1.Value;
                        charCode.Color2 = color2.Value;

                        var newVm = new CharacterImageIdentifierVM(charCode);
                        Traverse.Create(viewModel).Field(visualFieldName).SetValue(newVm);
                    }
                }
            }
            catch (Exception e)
            {
                Log.write($"Error in ApplyColorsToViewModel: {e.Message}");
            }
        }

        [HarmonyPatch(typeof(ClanPartyItemVM), "RefreshValues")]
        internal class PocColorModClanPartyItemVM
        {
            public static bool Prefix(ClanPartyItemVM __instance, ref ColorData __state)
            {
                try
                {
                    var leader = (CharacterObject)Traverse.Create(__instance).Field("_leader").GetValue()
                                 ?? __instance.Leader?.CharacterObject;
                    string clan = __instance.Party?.LeaderHero?.Clan?.Name?.ToString();
                    string charName = __instance.Name;

                    Banner banner = null;
                    uint? c1, c2;
                    Banner updatedBanner;

                    ApplyColorsToViewModel(__instance, "_leaderVisual", leader, clan, banner,
                        out c1, out c2, out updatedBanner);

                    __state = new ColorData(2, c1, c2, null, null);

                    if (updatedBanner != null)
                        __instance.CharacterModel.BannerCodeText = updatedBanner.ToString();
                }
                catch (Exception e)
                {
                    Log.write($"Error in ClanPartyItemVM Prefix: {e.Message}");
                }
                return true;
            }

            public static void Postfix(ClanPartyItemVM __instance, ref ColorData __state)
            {
                try
                {
                    if (__state?.c1 != null)
                        Traverse.Create(__instance.CharacterModel).Field("_armorColor1").SetValue(__state.c1.Value);
                    if (__state?.c2 != null)
                        Traverse.Create(__instance.CharacterModel).Field("_armorColor2").SetValue(__state.c2.Value);
                }
                catch (Exception e)
                {
                    Log.write($"Error in ClanPartyItemVM Postfix: {e.Message}");
                }
            }
        }

        [HarmonyPatch(typeof(ClanPartyMemberItemVM), "RefreshValues")]
        internal class PocColorModClanPartyMemberItemVM
        {
            public static bool Prefix(ClanPartyMemberItemVM __instance)
            {
                try
                {
                    var hero = __instance.HeroObject;
                    if (hero == null) return true;

                    string clan = hero.Clan?.Name?.ToString();
                    var banner = hero.ClanBanner;
                    var character = hero.CharacterObject;

                    uint? c1, c2;
                    Banner updatedBanner;

                    ApplyColorsToViewModel(__instance, "_visual", character, clan, banner,
                        out c1, out c2, out updatedBanner);

                    if (updatedBanner != null)
                        __instance.Banner_9 = new BannerImageIdentifierVM(updatedBanner, true);
                }
                catch (Exception e)
                {
                    Log.write($"Error in ClanPartyMemberItemVM: {e.Message}");
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(PartyCharacterVM), "RefreshValues")]
        internal class PocColorModPartyCharacterVM
        {
            public static void Postfix(PartyCharacterVM __instance)
            {
                try
                {
                    var partyVm = (PartyVM)Traverse.Create(__instance).Field("_partyVm").GetValue();
                    var hero = partyVm?.SelectedCharacter != null
                        ? (Hero)Traverse.Create(partyVm.SelectedCharacter).Field("_hero").GetValue()
                        : null;

                    string clan = hero?.Clan?.Name?.ToString() ?? "";
                    var banner = hero?.ClanBanner;
                    var character = __instance.Character;

                    uint? c1, c2;
                    Banner updatedBanner;

                    ApplyColorsToViewModel(__instance, null, character, clan, banner,
                        out c1, out c2, out updatedBanner);

                    if (c1.HasValue && c2.HasValue)
                    {
                        var charCode = CharacterCode.CreateFrom(__instance.Code.Id);
                        charCode.Color1 = c1.Value;
                        charCode.Color2 = c2.Value;
                        __instance.Code = new CharacterImageIdentifierVM(charCode);
                    }
                }
                catch (Exception e)
                {
                    Log.write($"Error in PartyCharacterVM: {e.Message}");
                }
            }
        }

        [HarmonyPatch(typeof(PartyCharacterVM), "Character", MethodType.Setter)]
        internal class PocColorModPartyCharacterVMCharacter
        {
            public static void Postfix(PartyCharacterVM __instance)
            {
                // Same logic as RefreshValues
                PocColorModPartyCharacterVM.Postfix(__instance);
            }
        }

        [HarmonyPatch(typeof(GameMenuPartyItemVM), "RefreshValues")]
        internal class PocColorModGameMenuPartyItemVM
        {
            public static bool Prefix(GameMenuPartyItemVM __instance)
            {
                try
                {
                    var party = (PartyBase)Traverse.Create(__instance).Field("Party").GetValue();
                    var hero = party?.Owner;
                    string clan = hero?.Clan?.Name?.ToString() ?? "";
                    var banner = hero?.ClanBanner;

                    var character = (CharacterObject)Traverse.Create(__instance).Field("Character").GetValue()
                        ?? PartyBaseHelper.GetVisualPartyLeader(party);

                    uint? c1, c2;
                    Banner updatedBanner;

                    ApplyColorsToViewModel(__instance, "_visual", character, clan, banner,
                        out c1, out c2, out updatedBanner);

                    if (updatedBanner != null)
                        __instance.Banner_9 = new BannerImageIdentifierVM(updatedBanner, true);
                }
                catch (Exception e)
                {
                    Log.write($"Error in GameMenuPartyItemVM: {e.Message}");
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(OrderOfBattleHeroItemVM), "RefreshValues")]
        internal class PocColorModOrderOfBattleHeroItemVM
        {
            public static void Postfix(OrderOfBattleHeroItemVM __instance)
            {
                try
                {
                    var agent = (Agent)Traverse.Create(__instance).Field("Agent").GetValue();
                    string charName = agent?.Name;
                    if (charName == null) return;

                    var hero = Hero.FindFirst(h => h.Name?.ToString() == charName);
                    if (hero == null) return;

                    string clan = hero.Clan?.Name?.ToString();
                    var banner = hero.ClanBanner;
                    var character = hero.CharacterObject;

                    var config = getColorFromConfig(clan, ref banner, charName, true,
                        character.IsMounted, character.IsRanged, character.Tier,
                        character.Culture?.Name?.ToString() ?? "");

                    if (config.Item2.HasValue && config.Item3.HasValue)
                    {
                        var ivm = (ImageIdentifierVM)Traverse.Create(__instance).Field("_imageIdentifier").GetValue();
                        if (ivm != null)
                        {
                            var charCode = CharacterCode.CreateFrom(ivm.Id);
                            charCode.Color1 = config.Item2.Value;
                            charCode.Color2 = config.Item3.Value;
                            __instance.ImageIdentifier = new CharacterImageIdentifierVM(charCode);
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.write($"Error in OrderOfBattleHeroItemVM: {e.Message}");
                }
            }
        }

        [HarmonyPatch(typeof(OrderOfBattleHeroItemVM), "RefreshInformation")]
        internal class PocColorModOrderOfBattleHeroItemVMRefreshInformation
        {
            public static void Postfix(OrderOfBattleHeroItemVM __instance)
            {
                // Same logic as RefreshValues
                PocColorModOrderOfBattleHeroItemVM.Postfix(__instance);
            }
        }

        [HarmonyPatch(typeof(TroopSelectionItemVM))]
        [HarmonyPatch(MethodType.Constructor)]
        [HarmonyPatch(new Type[]
        {
            typeof(TroopRosterElement),
            typeof(Action<TroopSelectionItemVM>),
            typeof(Action<TroopSelectionItemVM>)
        })]
        internal class PocColorModTroopSelectionItemVM
        {
            public static void Postfix(TroopSelectionItemVM __instance)
            {
                try
                {
                    var troop = __instance.Troop;
                    var character = troop.Character;
                    if (character == null) return;

                    string charName = character.HeroObject?.Name?.ToString() ?? character.Name?.ToString();
                    string clan = character.HeroObject?.Clan?.Name?.ToString() ?? Clan.PlayerClan?.Name?.ToString();
                    var banner = character.HeroObject?.ClanBanner;

                    var config = getColorFromConfig(clan, ref banner, charName, character.IsHero,
                        character.IsMounted, character.IsRanged, character.Tier,
                        character.Culture?.Name?.ToString() ?? "");

                    if (config.Item2.HasValue && config.Item3.HasValue)
                    {
                        var ivm = __instance.Visual;
                        if (ivm != null)
                        {
                            var charCode = CharacterCode.CreateFrom(ivm.Id);
                            charCode.Color1 = config.Item2.Value;
                            charCode.Color2 = config.Item3.Value;
                            __instance.Visual = new CharacterImageIdentifierVM(charCode);
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.write($"Error in TroopSelectionItemVM: {e.Message}");
                }
            }
        }

        #endregion

        #region CharacterViewModel and HeroVM Patches

        [HarmonyPatch(typeof(CharacterViewModel), "ArmorColor1", MethodType.Setter)]
        internal class PocColorModVMColor1
        {
            public static bool Prefix(CharacterViewModel __instance, ref uint value)
            {
                try
                {
                    var character = __instance.CharStringId != null
                        ? CharacterObject.Find(__instance.CharStringId)
                        : null;

                    if (character != null && __instance.BannerCodeText != null)
                    {
                        string charName = character.Name?.ToString() ?? "";
                        string clanName = character.HeroObject?.Clan?.Name?.ToString();
                        string bannerStr = __instance.BannerCodeText;

                        Banner tempBanner = !string.IsNullOrEmpty(bannerStr) ? new Banner(bannerStr) : null;
                        var config = getColorFromConfig(clanName, ref tempBanner, charName,
                            character.IsHero, character.IsMounted, character.IsRanged,
                            character.Tier, character.Culture?.Name?.ToString() ?? "");

                        if (config.Item2.HasValue && config.Item3.HasValue)
                            return false; // Don't allow override
                    }
                }
                catch (Exception e)
                {
                    Log.write($"Error in CharacterViewModel ArmorColor1: {e.Message}");
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(CharacterViewModel), "ArmorColor2", MethodType.Setter)]
        internal class PocColorModVMColor2
        {
            public static bool Prefix(CharacterViewModel __instance, ref uint value)
            {
                try
                {
                    var character = __instance.CharStringId != null
                        ? CharacterObject.Find(__instance.CharStringId)
                        : null;

                    if (character != null && __instance.BannerCodeText != null)
                    {
                        string charName = character.Name?.ToString() ?? "";
                        string clanName = character.HeroObject?.Clan?.Name?.ToString();
                        string bannerStr = __instance.BannerCodeText;

                        Banner tempBanner = !string.IsNullOrEmpty(bannerStr) ? new Banner(bannerStr) : null;
                        var config = getColorFromConfig(clanName, ref tempBanner, charName,
                            character.IsHero, character.IsMounted, character.IsRanged,
                            character.Tier, character.Culture?.Name?.ToString() ?? "");

                        if (config.Item2.HasValue && config.Item3.HasValue)
                            return false; // Don't allow override
                    }
                }
                catch (Exception e)
                {
                    Log.write($"Error in CharacterViewModel ArmorColor2: {e.Message}");
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(PartyVM), "RefreshCurrentCharacterInformation")]
        internal class PocColorModPartyVM
        {
            public static void Postfix(ref PartyVM __instance)
            {
                try
                {
                    var hero = __instance.CurrentCharacter?.Character?.HeroObject;

                    var character = hero?.CharacterObject
                        ?? (__instance.SelectedCharacter?.CharStringId != null
                            ? CharacterObject.Find(__instance.SelectedCharacter.CharStringId)
                            : null);

                    if (character == null) return;

                    string clan = hero?.Clan?.Name?.ToString() ?? MobileParty.MainParty?.ActualClan?.Name?.ToString();
                    string charName = character.Name?.ToString();
                    string bannerStr = __instance.SelectedCharacter?.BannerCodeText;

                    if (bannerStr != null)
                    {
                        Banner banner = new Banner(bannerStr);
                        var config = getColorFromConfig(clan, ref banner, charName,
                            character.IsHero, character.IsMounted, character.IsRanged,
                            character.Tier, character.Culture?.Name?.ToString() ?? "");

                        if (config.Item2.HasValue && config.Item3.HasValue)
                        {
                            __instance.SelectedCharacter.BannerCodeText = bannerStr;
                            Traverse.Create(__instance.SelectedCharacter).Field("_armorColor1").SetValue(config.Item2);
                            Traverse.Create(__instance.SelectedCharacter).Field("_armorColor2").SetValue(config.Item3);
                            __instance.SelectedCharacter.OnPropertyChanged("ArmorColor1");
                            __instance.SelectedCharacter.OnPropertyChanged("ArmorColor2");

                            colorDataCache[charName] = new ColorData(config.Item1, config.Item2, config.Item3, config.Item4, config.Item5);
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.write($"Error in PartyVM: {e.Message}");
                }
            }
        }

        [HarmonyPatch(typeof(HeroViewModel), "FillFrom")]
        internal class PocColorModHeroFillFrom
        {
            public static void Postfix(ref HeroViewModel __instance, ref Hero hero)
            {
                try
                {
                    if (hero == null) return;

                    var character = hero.CharacterObject;
                    string charName = character.Name?.ToString();
                    string clan = hero.Clan?.Name?.ToString();
                    string bannerStr = __instance.BannerCodeText;

                    if (bannerStr != null && clan != null)
                    {
                        Banner banner = new Banner(bannerStr);
                        var config = getColorFromConfig(clan, ref banner, charName, true,
                            character.IsMounted, character.IsRanged, character.Tier,
                            character.Culture?.Name?.ToString() ?? "");

                        if (config.Item2.HasValue && config.Item3.HasValue)
                        {
                            __instance.BannerCodeText = bannerStr;
                            Traverse.Create(__instance).Field("_armorColor1").SetValue(config.Item2);
                            Traverse.Create(__instance).Field("_armorColor2").SetValue(config.Item3);
                            __instance.OnPropertyChanged("ArmorColor1");
                            __instance.OnPropertyChanged("ArmorColor2");
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.write($"Error in HeroViewModel FillFrom: {e.Message}");
                }
            }
        }

        [HarmonyPatch(typeof(HeroVM), "RefreshValues")]
        internal class PocColorModHeroVM
        {
            public static bool Prefix(ref HeroVM __instance)
            {
                try
                {
                    var hero = __instance.Hero;
                    if (hero == null || !hero.IsKnownToPlayer) return true;

                    var banner = hero.ClanBanner;
                    string charName = hero.Name?.ToString() ?? "";
                    string clanName = hero.Clan?.Name?.ToString();
                    var character = hero.CharacterObject;

                    if (banner != null || clanName != null)
                    {
                        var config = getColorFromConfig(clanName, ref banner, charName, true,
                            character.IsMounted, character.IsRanged, character.Tier,
                            character.Culture?.Name?.ToString() ?? "");

                        if (config.Item4 != null) // newBanner
                            banner = new Banner(config.Item4);
                        else if (config.Item5 != null) // newShield
                            banner = new Banner(config.Item5);

                        if (banner != null)
                        {
                            __instance.ClanBanner = new BannerImageIdentifierVM(banner, true);
                            __instance.ClanBanner_9 = new BannerImageIdentifierVM(banner, true);
                        }

                        if (config.Item2.HasValue && config.Item3.HasValue)
                        {
                            var charCode = CampaignUIHelper.GetCharacterCode(character, false);
                            charCode.Color1 = config.Item2.Value;
                            charCode.Color2 = config.Item3.Value;
                            __instance.ImageIdentifier = new CharacterImageIdentifierVM(charCode);
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.write($"Error in HeroVM: {e.Message}");
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(ClanLordItemVM), "UpdateProperties")]
        internal class PocColorModClanLordItemVM
        {
            public static void Postfix(ref ClanLordItemVM __instance)
            {
                try
                {
                    string bannerStr = __instance.HeroModel?.BannerCodeText;
                    if (bannerStr != null)
                        __instance.Banner_9 = new BannerImageIdentifierVM(new Banner(bannerStr), true);
                }
                catch (Exception e)
                {
                    Log.write($"Error in ClanLordItemVM UpdateProperties: {e.Message}");
                }
            }
        }

        [HarmonyPatch(typeof(ClanLordItemVM), "RefreshValues")]
        internal class PocColorModClanLordItemVMRefreshValues
        {
            public static bool Prefix(ref ClanLordItemVM __instance)
            {
                try
                {
                    var visuals = (CharacterImageIdentifierVM)Traverse.Create(__instance).Field("_visual").GetValue();
                    var hero = __instance.GetHero();
                    if (hero == null) return true;

                    var banner = hero.ClanBanner;
                    string charName = hero.Name?.ToString();
                    string clan = hero.Clan?.Name?.ToString();
                    var character = hero.CharacterObject;

                    var config = getColorFromConfig(clan, ref banner, charName, true,
                        character.IsMounted, character.IsRanged, character.Tier,
                        character.Culture?.Name?.ToString() ?? "");

                    if (config.Item4 != null) // newBanner
                        banner = new Banner(config.Item4);
                    else if (config.Item5 != null) // newShield
                        banner = new Banner(config.Item5);

                    if (config.Item2.HasValue && config.Item3.HasValue && visuals != null)
                    {
                        var imageIdentifier = (ImageIdentifier)Traverse.Create(visuals).Field("_imageIdentifier").GetValue();
                        var charCode = CharacterCode.CreateFrom(imageIdentifier.Id);
                        charCode.Color1 = config.Item2.Value;
                        charCode.Color2 = config.Item3.Value;

                        var newVm = new CharacterImageIdentifierVM(charCode);
                        Traverse.Create(__instance).Field("_visual").SetValue(newVm);
                    }

                    if (banner != null)
                        __instance.Banner_9 = new BannerImageIdentifierVM(banner, true);
                }
                catch (Exception e)
                {
                    Log.write($"Error in ClanLordItemVM RefreshValues: {e.Message}");
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(SPInventoryVM), "UpdateCurrentCharacterIfPossible")]
        internal class PocColorModSPInventoryVM
        {
            public static void Postfix(ref SPInventoryVM __instance)
            {
                try
                {
                    var current = (CharacterObject)Traverse.Create(__instance).Field("_currentCharacter").GetValue();
                    string charName = current?.Name?.ToString();
                    string clanName = current?.HeroObject?.Clan?.Name?.ToString();

                    if (__instance.MainCharacter?.CharStringId != null)
                    {
                        var character = CharacterObject.Find(__instance.MainCharacter.CharStringId);
                        charName = character?.Name?.ToString();
                        clanName = character?.HeroObject?.Clan?.Name?.ToString();
                    }

                    string bannerStr = __instance.MainCharacter?.BannerCodeText;
                    if (bannerStr != null)
                    {
                        Banner banner = new Banner(bannerStr);
                        Banner tempBanner = !string.IsNullOrEmpty(bannerStr) ? new Banner(bannerStr) : null;
                        var config = getColorFromConfig(clanName, ref tempBanner, charName, true, false, false, 0, "");

                        if (config.Item2.HasValue && config.Item3.HasValue)
                        {
                            __instance.MainCharacter.BannerCodeText = bannerStr;
                            Traverse.Create(__instance.MainCharacter).Field("_armorColor1").SetValue(config.Item2);
                            Traverse.Create(__instance.MainCharacter).Field("_armorColor2").SetValue(config.Item3);
                            __instance.MainCharacter.OnPropertyChanged("ArmorColor1");
                            __instance.MainCharacter.OnPropertyChanged("ArmorColor2");
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.write($"Error in SPInventoryVM: {e.Message}");
                }
            }
        }

        #endregion

        #region Agent Equipment and Weapon Patches - REACTIVATED FOR BANNERLORD 1.3

        /*
         * CONFIRMED BANNERLORD 1.3 SIGNATURES (from DnSpy):
         * - Agent.EquipItemsFromSpawnEquipment(bool neededBatchedItems, bool prepareImmediately, bool useFaceCache, int faceCacheID)
         * - Agent.EquipWeaponWithNewEntity(EquipmentIndex slotIndex, ref MissionWeapon weapon)
         */

        [HarmonyPatch(typeof(Agent), "EquipItemsFromSpawnEquipment")]
        [HarmonyPatch(new Type[] { typeof(bool), typeof(bool), typeof(bool), typeof(int) })]
        internal class PocColorModAgentEquipItemsFromSpawnEquipment
        {
            public static bool Prefix(ref Agent __instance, bool neededBatchedItems, bool prepareImmediately, bool useFaceCache, int faceCacheID)
            {
                try
                {
                    if (Campaign.Current == null || Clan.PlayerClan == null)
                        return true;

                    var equipment = __instance?.Equipment;
                    if (equipment == null) return true;

                    // CRITICAL FIX: Si NO hay savedColorData, calcularlo AHORA
                    if (savedColorData == null)
                    {
                        string charName = __instance.Name;
                        var chara = CharacterObject.FindFirst(c => c.Name?.ToString() == charName);

                        if (chara != null)
                        {
                            string clanName = chara.HeroObject?.Clan?.Name?.ToString();
                            Banner banner = chara.HeroObject?.ClanBanner;

                            // Si no tiene clan, intentar obtenerlo del party
                            if (string.IsNullOrEmpty(clanName) && __instance.Origin?.BattleCombatant != null)
                            {
                                var party = __instance.Origin.BattleCombatant as PartyBase;
                                clanName = party?.MobileParty?.ActualClan?.Name?.ToString()
                                        ?? party?.Owner?.Clan?.Name?.ToString();
                                banner = party?.Banner;
                            }

                            if (banner != null || !string.IsNullOrEmpty(clanName))
                            {
                                var config = getBattleColorFromConfig(clanName, ref banner, charName,
                                    chara.IsHero, chara.IsMounted, chara.IsRanged,
                                    chara.Tier, chara.Culture?.Name?.ToString() ?? "");

                                savedColorData = new ColorData(config.Item1, config.Item2, config.Item3,
                                                              config.Item4, config.Item5);

                                Log.write($">>> FALLBACK CALCULATED: [{charName}] c1={config.Item2}, c2={config.Item3}");
                            }
                            else
                            {
                                // Civilian/NPC sin clan ni banner - asignar colores random naturales
                                uint color1 = getRandomCivilianColor();
                                uint color2 = getRandomCivilianColor();

                                savedColorData = new ColorData(DEFAULT_MODE, color1, color2, null, null);

                                Log.write($">>> CIVILIAN RANDOM: [{charName}] c1={color1}, c2={color2}");
                            }
                        }
                        else
                        {
                            // Character no encontrado - colores random naturales
                            uint randomColor1 = getRandomCivilianColor();
                            uint randomColor2 = getRandomCivilianColor();

                            savedColorData = new ColorData(DEFAULT_MODE, randomColor1, randomColor2, null, null);

                            Log.write($">>> CIVILIAN RANDOM: [{charName}] c1={randomColor1}, c2={randomColor2}");
                        }
                    }

                    // CRITICAL FIX: Apply colors IMMEDIATELY if we have savedColorData
                    if (savedColorData != null && savedColorData.c1.HasValue && savedColorData.c2.HasValue)
                    {
                        Log.write($">>> PRE-EQUIPMENT: Applying saved colors c1={savedColorData.c1.Value}, c2={savedColorData.c2.Value} to [{__instance.Name}]");
                        __instance.SetClothingColor1(savedColorData.c1.Value);
                        __instance.SetClothingColor2(savedColorData.c2.Value);
                    }
                    else
                    {
                        Log.write($">>> PRE-EQUIPMENT WARNING: No colors available for [{__instance.Name}]");
                    }

                    MissionWeapon? weaponOpt = __instance.Equipment[EquipmentIndex.ExtraWeaponSlot];
                    bool isBanner = weaponOpt?.IsBanner() ?? false;

                    if (isBanner || weaponOpt?.Item?.ToString() == "mod_banner_1")
                    {
                        MissionWeapon weapon = weaponOpt.Value;
                        Banner banner = weapon.Banner;
                        string bannerStr = banner != null ? PocColorMod.SerializeBanner(banner) : null;

                        if (bannerStr != null)
                        {
                            string clan = PocColorMod.bannerClanCache[bannerStr] ?? "";
                            string charName = __instance.Name;
                            var chara = CharacterObject.FindFirst(c => c.Name?.ToString() == charName);

                            Banner newOverrideBanner = banner;
                            Banner newOverrideShield = banner;

                            uint? c1, c2;
                            string newbanner, newshield;

                            if (savedColorData != null)
                            {
                                c1 = savedColorData.c1;
                                c2 = savedColorData.c2;
                                newbanner = savedColorData.newbanner;
                                newshield = savedColorData.newshield;
                            }
                            else
                            {
                                var config = getBattleColorFromConfig(clan, ref banner, charName,
                                    __instance.IsHero, chara?.IsMounted ?? false, chara?.IsRanged ?? false,
                                    chara?.Tier ?? 0, chara?.Culture?.Name?.ToString() ?? "");
                                c1 = config.Item2;
                                c2 = config.Item3;
                                newbanner = config.Item4;
                                newshield = config.Item5;
                            }

                            if (newbanner != null)
                                newOverrideBanner = new Banner(newbanner);
                            if (newshield != null)
                                newOverrideShield = new Banner(newshield);

                            for (EquipmentIndex index = EquipmentIndex.WeaponItemBeginSlot;
                                 index < EquipmentIndex.NumAllWeaponSlots; index++)
                            {
                                if (__instance.Equipment[index].Item != null)
                                {
                                    __instance.Equipment[index] = new MissionWeapon(
                                        __instance.Equipment[index].Item,
                                        __instance.Equipment[index].ItemModifier,
                                        newOverrideShield);
                                }
                            }

                            FieldInfo fi = typeof(MissionWeapon).GetField("<Banner>k__BackingField",
                                BindingFlags.Instance | BindingFlags.NonPublic);
                            TypedReference reference = __makeref(weapon);
                            fi.SetValueDirect(reference, newOverrideBanner);

                            string item = weapon.Item?.ToString();
                            __instance.Equipment[EquipmentIndex.ExtraWeaponSlot] = new MissionWeapon(
                                MBObjectManager.Instance.GetObject<ItemObject>(item),
                                null,
                                newOverrideBanner);

                            if (c1.HasValue && c2.HasValue)
                            {
                                Log.write($">>> EQUIPMENT FINAL: Setting colors on agent [{__instance.Name}]: c1={c1.Value}, c2={c2.Value}");
                                __instance.SetClothingColor1(c1.Value);
                                __instance.SetClothingColor2(c2.Value);

                                Log.write($">>> EQUIPMENT VERIFY: Agent colors after set: c1={__instance.ClothingColor1}, c2={__instance.ClothingColor2}");
                            }
                            else
                            {
                                Log.write($">>> EQUIPMENT WARNING: Colors are NULL for [{__instance.Name}] - c1={c1}, c2={c2}");
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.write($"Error in AgentEquipItemsFromSpawnEquipment: {e.Message}");
                }

                // CRITICAL FIX: SIEMPRE limpiar savedColorData al final para que
                // NO contamine al siguiente agente (civiles, notables, etc.)
                savedColorData = null;

                return true;
            }
        }

        // MANUALLY PATCHED - See OnGameStart in PocColorMod.cs
        // HarmonyPatch attributes removed because they fail on some systems
        internal class PocColorModAgentEquipNewEntity
        {
            public static void Postfix(ref Agent __instance, EquipmentIndex slotIndex, ref MissionWeapon weapon)
            {
                try
                {
                    // SAFETY: Skip during character creation
                    if (Campaign.Current == null || Clan.PlayerClan == null)
                        return;

                    string name = weapon.GetModifiedItemName()?.ToString() ?? "";
                    Banner banner = weapon.Banner;
                    string bannerStr = banner != null ? PocColorMod.SerializeBanner(banner) : null;

                    if (bannerStr != null)
                    {
                        string clan = PocColorMod.bannerClanCache[bannerStr] ?? "";
                        string charName = __instance.Name;
                        var chara = CharacterObject.FindFirst(c => c.Name?.ToString() == charName);

                        Banner newOverrideBanner = banner;
                        Banner newOverrideShield = banner;

                        uint? c1, c2;
                        string newbanner, newshield;

                        if (savedColorData != null)
                        {
                            c1 = savedColorData.c1;
                            c2 = savedColorData.c2;
                            newbanner = savedColorData.newbanner;
                            newshield = savedColorData.newshield;
                        }
                        else
                        {
                            var config = getBattleColorFromConfig(clan, ref banner, charName,
                                __instance.IsHero, chara?.IsMounted ?? false, chara?.IsRanged ?? false,
                                chara?.Tier ?? 0, chara?.Culture?.Name?.ToString() ?? "");
                            c1 = config.Item2;
                            c2 = config.Item3;
                            newbanner = config.Item4;
                            newshield = config.Item5;
                        }

                        if (newbanner != null)
                            newOverrideBanner = new Banner(newbanner);
                        if (newshield != null)
                            newOverrideShield = new Banner(newshield);

                        bool isBannerWeapon = name.Contains("Campaign Banner") || name.Contains("Mod Banner") || weapon.IsBanner();

                        if (isBannerWeapon)
                        {
                            // Update all equipment with shield banner
                            for (EquipmentIndex index = EquipmentIndex.WeaponItemBeginSlot;
                                 index < EquipmentIndex.NumAllWeaponSlots; index++)
                            {
                                __instance.Equipment[index] = new MissionWeapon(
                                    __instance.Equipment[index].Item,
                                    __instance.Equipment[index].ItemModifier,
                                    newOverrideShield);
                            }

                            // Update the weapon that was just equipped
                            __instance.Equipment[slotIndex] = new MissionWeapon(
                                weapon.Item,
                                weapon.ItemModifier,
                                newOverrideBanner);

                            if (c1.HasValue && c2.HasValue)
                            {
                                __instance.SetClothingColor1(c1.Value);
                                __instance.SetClothingColor2(c2.Value);
                            }
                        }
                        else if (weapon.IsShield())
                        {
                            // Update shield with custom banner
                            __instance.Equipment[slotIndex] = new MissionWeapon(
                                weapon.Item,
                                weapon.ItemModifier,
                                newOverrideShield);

                            // Update banner weapons with new banner
                            for (EquipmentIndex index = EquipmentIndex.WeaponItemBeginSlot;
                                 index < EquipmentIndex.NumAllWeaponSlots; index++)
                            {
                                string itemName = __instance.Equipment[index].Item?.Name?.ToString();
                                if (itemName != null && (itemName.Contains("Campaign Banner") || itemName.Contains("Mod Banner")))
                                {
                                    string item = __instance.Equipment[index].Item?.ToString();
                                    __instance.Equipment[index] = new MissionWeapon(
                                        MBObjectManager.Instance.GetObject<ItemObject>(item),
                                        null,
                                        newOverrideBanner);
                                    break;
                                }
                            }

                            if (c1.HasValue && c2.HasValue)
                            {
                                __instance.SetClothingColor1(c1.Value);
                                __instance.SetClothingColor2(c2.Value);
                            }
                        }
                    }

                    savedColorData = null;
                }
                catch (Exception e)
                {
                    Log.write($"Error in AgentEquipWeaponWithNewEntity: {e.Message}");
                }
            }
        }


        // ===== POC Color Mod: crash guard for armor banner-tableau rendering =====
        // Prevents NRE in MissionScreen.ApplyBannerTextureToMesh when the armor mesh
        // or banner texture is null on async thumbnail render completion (armor
        // tableaus coming from other mods). Purely defensive: no game logic changed.
        public class PocColorModApplyBannerTextureGuard
        {
            public static Exception Finalizer(Exception __exception)
            {
                if (__exception is NullReferenceException)
                {
                    if (PocColorMod.doLog)
                    {
                        Log.write(">>> [Guard] Suppressed NRE in MissionScreen.ApplyBannerTextureToMesh: " + __exception.Message);
                    }
                    return null; // suppress only the null-material crash
                }
                return __exception; // let anything unexpected propagate
            }
        }



        #endregion

        #region AgentVisuals Patches - PARTIALLY DISABLED FOR BANNERLORD 1.3

        /*
         * CONFIRMED BANNERLORD 1.3 SIGNATURES (from DnSpy):
         * - AgentVisuals.AddSkinArmorWeaponMultiMeshesToEntity(uint, uint, bool, bool forceUseFaceCache = false) - PRIVATE METHOD, CANNOT PATCH
         * - AgentVisuals.Create(AgentVisualsData data, string name, bool isRandomProgress, bool needBatchedVersionForWeaponMeshes, bool forceUseFaceCache) - PUBLIC
         * 
         * AddSkinArmorWeaponMultiMeshesToEntity is PRIVATE, so we cannot patch it with Harmony.
         * AgentVisuals.Create is available if needed in the future.
         */

        // REACTIVATED FOR BANNERLORD 1.3 - Harmony CAN patch private methods
        [HarmonyPatch(typeof(AgentVisuals), "AddSkinArmorWeaponMultiMeshesToEntity")]
        [HarmonyPatch(new Type[] { typeof(uint), typeof(uint), typeof(bool), typeof(bool) })]
        internal class PocColorModAgentVisualsAddMeshes
        {
            public static bool Prefix(ref AgentVisuals __instance, ref uint teamColor1, ref uint teamColor2, bool needBatchedVersion, bool forceUseFaceCache)
            {
                try
                {
                    // USAR SAVEDCOLORDATA GLOBAL
                    if (savedColorData != null && savedColorData.c1.HasValue && savedColorData.c2.HasValue)
                    {
                        teamColor1 = savedColorData.c1.Value;
                        teamColor2 = savedColorData.c2.Value;

                        Log.write($">>> ADD_ARMOR_MESHES: ✓ REPLACED colors: c1={teamColor1}, c2={teamColor2}");
                    }
                }
                catch (Exception e)
                {
                    Log.write($">>> ADD_ARMOR_MESHES ERROR: {e.Message}");
                }

                // FIX: Limpiar para no contaminar siguiente agente
                savedColorData = null;

                return true;
            }
        }

        // REACTIVATED FOR BANNERLORD 1.3
        // REACTIVATED FOR BANNERLORD 1.3
        [HarmonyPatch(typeof(AgentVisuals), "Create")]
        [HarmonyPatch(new Type[] { typeof(AgentVisualsData), typeof(string), typeof(bool), typeof(bool), typeof(bool) })]
        internal class PocColorModAgentVisualsCreate
        {
            public static bool Prefix(ref AgentVisualsData data, ref string name, bool isRandomProgress, bool needBatchedVersionForWeaponMeshes, bool forceUseFaceCache)
            {
                try
                {
                    // SAFETY: Skip during character creation
                    if (Campaign.Current == null || Clan.PlayerClan == null)
                        return true;

                    if (name.StartsWith("PartyIcon"))
                    {
                        string charname = name.Substring(10);

                        // CharacterObjectStringId is a fluent METHOD (the property is CharacterObjectStringIdData)
                        data.CharacterObjectStringId(charname);

                        Log.write($">>> AGENT_VISUALS_CREATE: PartyIcon detected, charname=[{charname}]");

                        // CRITICAL FIX: Apply colors to PartyIcon (for campaign map display)
                        if (savedColorData != null && savedColorData.c1.HasValue && savedColorData.c2.HasValue)
                        {
                            data.ClothColor1(savedColorData.c1.Value);
                            data.ClothColor2(savedColorData.c2.Value);
                            Log.write($">>> AGENT_VISUALS_CREATE: Applied savedColorData c1:{savedColorData.c1.Value} c2:{savedColorData.c2.Value}");
                        }
                        else
                        {
                            // FALLBACK: Calculate colors from character/clan/banner
                            var character = CharacterObject.Find(charname);
                            if (character != null)
                            {
                                string clanName = character.HeroObject?.Clan?.Name?.ToString();
                                Banner banner = character.HeroObject?.ClanBanner ?? data.BannerData;

                                uint color1, color2;

                                if (banner != null && banner.BannerDataList != null && banner.BannerDataList.Count > 0)
                                {
                                    // Extract from banner
                                    int colorId = banner.BannerDataList[0].ColorId;
                                    int colorId2 = banner.BannerDataList[0].ColorId2;

                                    if (colorId2 == colorId && banner.BannerDataList.Count > 1)
                                    {
                                        int layer1ColorId = banner.BannerDataList[1].ColorId;
                                        int layer1ColorId2 = banner.BannerDataList[1].ColorId2;

                                        if (layer1ColorId != colorId)
                                            colorId2 = layer1ColorId;
                                        else if (layer1ColorId2 != colorId)
                                            colorId2 = layer1ColorId2;
                                    }

                                    color1 = BannerManager.GetColor(colorId);
                                    color2 = BannerManager.GetColor(colorId2);
                                    Log.write($">>> AGENT_VISUALS_CREATE: Extracted from banner c1:{color1} c2:{color2}");
                                }
                                else
                                {
                                    // Use clan colors as fallback
                                    var clan = character.HeroObject?.Clan;
                                    color1 = clan?.Color ?? BannerManager.GetColor(40);
                                    color2 = clan?.Color2 ?? BannerManager.GetColor(50);
                                    Log.write($">>> AGENT_VISUALS_CREATE: Using clan colors c1:{color1} c2:{color2}");
                                }

                                data.ClothColor1(color1);
                                data.ClothColor2(color2);
                            }
                            else
                            {
                                Log.write($">>> AGENT_VISUALS_CREATE: Character not found for [{charname}]");
                            }
                        }

                        if (data.BannerData == null && savedColorData?.newshield != null)
                        {
                            // FIXED: Banner is a method, not a property
                            data.Banner(new Banner(savedColorData.newshield));
                            Log.write($">>> AGENT_VISUALS_CREATE: Set banner from savedColorData");
                        }

                        // FIX: Limpiar para no contaminar siguiente PartyIcon
                        savedColorData = null;
                    }
                }
                catch (Exception e)
                {
                    Log.write($"Error in AgentVisualsCreate: {e.Message}");
                }
                return true;
            }
        }

        // CRITICAL PATCH for Bannerlord 1.3 - Modify colors in AgentVisualsData BEFORE Refresh
        [HarmonyPatch(typeof(AgentVisuals), "Refresh")]
        [HarmonyPatch(new Type[] { typeof(bool), typeof(bool), typeof(Equipment), typeof(bool), typeof(bool) })]
        internal class PocColorModAgentVisualsRefresh
        {
            public static void Prefix(ref AgentVisuals __instance, bool needBatchedVersionForWeaponMeshes, bool removeSkeleton, Equipment oldEquipment, bool isRandomProgress, bool forceUseFaceCache)
            {
                try
                {
                    var data = (AgentVisualsData)Traverse.Create(__instance).Field("_data").GetValue();
                    if (data == null)
                        return;

                    // USAR SAVEDCOLORDATA GLOBAL (es más simple y funciona)
                    if (savedColorData != null && savedColorData.c1.HasValue && savedColorData.c2.HasValue)
                    {
                        Log.write($">>> REFRESH: Applying savedColorData: c1={savedColorData.c1.Value}, c2={savedColorData.c2.Value}");

                        data.ClothColor1(savedColorData.c1.Value);
                        data.ClothColor2(savedColorData.c2.Value);

                        Log.write($">>> REFRESH: ✓ Colors updated in AgentVisualsData");
                    }

                    // FIX: Limpiar para no contaminar siguiente agente
                    savedColorData = null;
                }
                catch (Exception e)
                {
                    Log.write($">>> REFRESH ERROR: {e.Message}");
                }
            }
        }


        #endregion

        #region Tableau and Conversation Patches - PARTIALLY DISABLED

        /* THESE ARE DISABLED - CharacterTableau methods are PRIVATE in 1.3
        [HarmonyPatch(typeof(CharacterTableau), "RefreshCharacterTableau")]
        internal class PocColorModCharacterTableauRefresh
        {
            public static bool Prefix(ref CharacterTableau __instance)
            {
                try
                {
                    if (Campaign.Current == null || Clan.PlayerClan == null)
                        return true;

                    string stringId = (string)Traverse.Create(__instance).Field("_charStringId").GetValue();
                    var character = stringId != null ? CharacterObject.Find(stringId) : null;

                    if (character != null)
                    {
                        string charName = character.Name?.ToString();
                        string clanName = character.HeroObject?.Clan?.Name?.ToString();

                        if (string.IsNullOrEmpty(clanName))
                            return true;

                        Banner bannerObj = (Banner)Traverse.Create(__instance).Field("_banner").GetValue();

                        var colorData = colorDataCache[charName];

                        if (colorData != null)
                        {
                            savedColorData = colorData;
                            colorDataCache[charName] = null;
                        }
                        else
                        {
                            var config = getColorFromConfig(clanName, ref bannerObj, charName,
                                character.IsHero, character.IsMounted, character.IsRanged,
                                character.Tier, character.Culture?.Name?.ToString() ?? "");

                            savedColorData = new ColorData(config.Item1, config.Item2, config.Item3, config.Item4, config.Item5);
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.write($"Error in CharacterTableauRefresh: {e.Message}");
                    savedColorData = null;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(CharacterTableau), "UpdateBannerItem")]
        internal class PocColorModCharacterTableau
        {
            public static bool Prefix(ref CharacterTableau __instance, ref string __state)
            {
                try
                {
                    if (Campaign.Current == null || Clan.PlayerClan == null)
                        return true;

                    if (savedColorData != null)
                    {
                        __state = savedColorData.newshield;

                        if (!string.IsNullOrEmpty(savedColorData.newbanner))
                        {
                            __instance.SetBannerCode(savedColorData.newbanner);
                        }

                        if (savedColorData.c1.HasValue)
                            __instance.SetArmorColor1(savedColorData.c1.Value);
                        if (savedColorData.c2.HasValue)
                            __instance.SetArmorColor2(savedColorData.c2.Value);

                        savedColorData = null;
                    }
                }
                catch (Exception e)
                {
                    Log.write($"Error in CharacterTableau UpdateBannerItem: {e.Message}");
                    __state = null;
                }
                return true;
            }

            public static void Postfix(ref CharacterTableau __instance, ref string __state)
            {
                try
                {
                    if (!string.IsNullOrEmpty(__state))
                        __instance.SetBannerCode(__state);
                }
                catch (Exception e)
                {
                    Log.write($"Error in CharacterTableau Postfix: {e.Message}");
                }
            }
        }
        */

        [HarmonyPatch(typeof(MapConversationTableau), "SpawnOpponentLeader")]
        internal class PocColorModMapConversationTableauOppo
        {
            public static void Postfix(MapConversationTableau __instance)
            {
                try
                {
                    // SAFETY: Skip if Campaign not ready
                    if (Campaign.Current == null)
                        return;

                    var agentVisuals = (List<AgentVisuals>)Traverse.Create(__instance).Field("_agentVisuals").GetValue();
                    var data = (MapConversationTableauData)Traverse.Create(__instance).Field("_data").GetValue();

                    // SAFETY: Validate data exists
                    if (data?.ConversationPartnerData == null)
                        return;

                    var party = data.ConversationPartnerData.Party;
                    var character = data.ConversationPartnerData.Character;

                    // SAFETY: Validate party and character
                    if (party == null || character == null)
                        return;

                    string clanName = party?.MobileParty?.ActualClan?.Name?.ToString() ?? "";

                    if (agentVisuals.Count > 0)
                    {
                        var agent = agentVisuals[agentVisuals.Count - 1];
                        Banner banner = party.Banner;

                        // CRITICAL FIX: NEVER use savedColorData here - always calculate fresh
                        var config = getBattleColorFromConfig(clanName, ref banner, character.Name?.ToString(),
                            character.IsHero, character.IsMounted, character.IsRanged,
                            character.Tier, character.Culture?.Name?.ToString() ?? "");

                        // CRITICAL: ALWAYS apply colors, NEVER leave grey
                        uint color1, color2;

                        if (config.Item2.HasValue && config.Item3.HasValue)
                        {
                            // Colors from config
                            color1 = config.Item2.Value;
                            color2 = config.Item3.Value;
                            Log.write($">>> CONVERSATION [{character.Name}]: config colors c1:{color1} c2:{color2}");
                        }
                        else if (banner != null && banner.BannerDataList?.Count > 0)
                        {
                            // FALLBACK 1: Extract from banner
                            int colorId = banner.BannerDataList[0].ColorId;
                            int colorId2 = banner.BannerDataList[0].ColorId2;

                            if (colorId2 == colorId && banner.BannerDataList.Count > 1)
                            {
                                colorId2 = banner.BannerDataList[1].ColorId;
                            }

                            color1 = BannerManager.GetColor(colorId);
                            color2 = BannerManager.GetColor(colorId2);
                            Log.write($">>> CONVERSATION [{character.Name}]: banner colors c1:{color1} c2:{color2}");
                        }
                        else
                        {
                            // FALLBACK 2: Culture colors
                            color1 = character.Culture?.Color ?? BannerManager.GetColor(40);
                            color2 = character.Culture?.Color2 ?? BannerManager.GetColor(50);
                            Log.write($">>> CONVERSATION [{character.Name}]: culture colors c1:{color1} c2:{color2}");
                        }

                        // ALWAYS apply colors (guaranteed non-grey)
                        agent.SetClothingColors(color1, color2);

                        var avd = agent.GetCopyAgentVisualsData();
                        if (config.Item5 != null)
                            banner = new Banner(config.Item5);
                        else if (config.Item4 != null)
                            banner = new Banner(config.Item4);

                        avd.Banner(banner);
                        agent.Refresh(false, avd, false);
                    }
                }
                catch (Exception e)
                {
                    Log.write($"Error in MapConversationTableau: {e.Message}");
                }
            }
        }

        [HarmonyPatch(typeof(HeroHelper), "WillLordAttack")]
        internal class PocColorModWillLordAttackFix
        {
            public static bool Prefix(ref bool __result)
            {
                try
                {
                    if (PlayerEncounter.Current == null)
                        return true; // Let original run, it handles this

                    // Check if ConversationParty and EncounteredParty are both null
                    var convParty = Campaign.Current?.ConversationManager?.ConversationParty;
                    var encParty = PlayerEncounter.EncounteredParty;

                    if (convParty == null && encParty == null)
                    {
                        __result = false;
                        return false; // Skip original - would NullRef
                    }

                    return true; // Let original run normally
                }
                catch
                {
                    __result = false;
                    return false; // Skip original on any error
                }
            }
        }

        [HarmonyPatch(typeof(MapConversationTableau), "SpawnOpponentBodyguardCharacter")]
        internal class PocColorModMapConversationTableauOppoBodyGuards
        {
            public static void Postfix(MapConversationTableau __instance, CharacterObject character)
            {
                // Same logic as SpawnOpponentLeader
                PocColorModMapConversationTableauOppo.Postfix(__instance);
            }
        }

        [HarmonyPatch(typeof(CampaignSceneNotificationHelper), "CreateNotificationCharacterFromHero")]
        internal class PocColorModDefaultCreateNotificationCharacterFromHero
        {
            public static bool Prefix(
                            ref SceneNotificationData.SceneNotificationCharacter __result,
                            Hero hero,
                            Equipment overridenEquipment = null,
                            bool useCivilian = false,
                            BodyProperties overriddenBodyProperties = default(BodyProperties),
                            uint overriddenColor1 = uint.MaxValue,
                            uint overriddenColor2 = uint.MaxValue,
                            bool useHorse = false)
            {
                try
                {
                    // SAFETY: Skip if Campaign not ready or hero invalid
                    if (Campaign.Current == null || hero == null)
                        return true;

                    string clan = hero?.Clan?.Name?.ToString() ?? "";

                    // SAFETY: Skip if no clan
                    if (string.IsNullOrEmpty(clan))
                        return true;

                    Banner banner = hero.ClanBanner;
                    string charName = hero.Name?.ToString();

                    var config = getColorFromConfig(clan, ref banner, charName, true,
                        hero.CharacterObject.IsMounted, hero.CharacterObject.IsRanged,
                        hero.CharacterObject.Tier, hero.Culture?.Name?.ToString() ?? "");

                    if (config.Item5 != null)
                        banner = new Banner(config.Item5);
                    else if (config.Item4 != null)
                        banner = new Banner(config.Item4);

                    var mapFaction = hero.MapFaction;
                    if (overriddenColor1 == uint.MaxValue)
                        overriddenColor1 = mapFaction?.Color ?? hero.CharacterObject.Culture.Color;
                    if (overriddenColor2 == uint.MaxValue)
                        overriddenColor2 = mapFaction?.Color2 ?? hero.CharacterObject.Culture.Color2;

                    if (config.Item2.HasValue)
                        overriddenColor1 = config.Item2.Value;
                    if (config.Item3.HasValue)
                        overriddenColor2 = config.Item3.Value;

                    if (overridenEquipment == null)
                        overridenEquipment = useCivilian ? hero.CivilianEquipment : hero.BattleEquipment;

                    __result = new SceneNotificationData.SceneNotificationCharacter(
                        hero.CharacterObject,
                        overridenEquipment,
                        overriddenBodyProperties,
                        useCivilian,
                        overriddenColor1,
                        overriddenColor2,
                        useHorse);
                }
                catch (Exception e)
                {
                    Log.write($"Error in CreateNotificationCharacterFromHero: {e.Message}");
                    return true;
                }
                return false;
            }
        }

        #endregion

        #region PartyVisual and Nameplate Patches

        [HarmonyPatch(typeof(PartyNameplateVM), "PartyBanner", MethodType.Setter)]
        internal class PocColorModPartyBanner
        {
            public static bool Prefix(ref PartyNameplateVM __instance, ref BannerImageIdentifierVM value)
            {
                try
                {
                    // SAFETY CHECKS
                    if (__instance == null || __instance.Party == null)
                        return true;

                    string partyName = __instance.Party.Name?.ToString() ?? "";

                    // Check if it's a bandit/looter party
                    bool isMobilePartyBandit = __instance.Party?.IsBandit == true;
                    bool isBanditClan = __instance.Party.ActualClan?.IsBanditFaction == true;
                    bool isOutlawClan = __instance.Party.ActualClan?.IsOutlaw == true;
                    bool isLooter = partyName.Contains("Looter");
                    bool isDeserter = partyName.Contains("Deserter");
                    bool isBandit = partyName.Contains("Bandit") || partyName.Contains("Raider") || isMobilePartyBandit || isBanditClan || isOutlawClan;

                    string clanName = null;

                    // Si es bandido, intentar mapear a un "clan" ficticio
                    if (isLooter || isDeserter || isBandit)
                    {
                        if (partyName.Contains("Looter"))
                            clanName = "looters";
                        else if (partyName.Contains("Deserter"))
                            clanName = "deserters";
                        else if (partyName.Contains("Sea Raider"))
                            clanName = "sea_raiders";
                        else if (partyName.Contains("Forest Bandit"))
                            clanName = "forest_bandits";
                        else if (partyName.Contains("Mountain Bandit"))
                            clanName = "mountain_bandits";
                        else if (partyName.Contains("Steppe Bandit"))
                            clanName = "steppe_bandits";
                        else if (partyName.Contains("Desert Bandit"))
                            clanName = "desert_bandits";

                        // Verificar si existe config para este "clan"
                        bool hasConfig = false;
                        if (!string.IsNullOrEmpty(clanName) && PocColorMod.config?.clans != null)
                        {
                            hasConfig = PocColorMod.config.clans.ContainsKey(clanName);
                        }

                        if (!hasConfig)
                        {
                            // Sin config, dejar que el juego maneje el banner
                            return true;
                        }
                    }
                    else
                    {
                        // Party normal con clan
                        if (__instance.Party.ActualClan == null)
                        {
                            // Sin clan, dejar que el juego maneje el banner
                            return true;
                        }

                        clanName = __instance.Party.ActualClan.Name?.ToString();
                    }

                    // CRITICAL: Check si hay config ANTES de procesar
                    bool hasAnyConfig = false;

                    if (__instance.Party.LeaderHero != null)
                    {
                        var hero = __instance.Party.LeaderHero;
                        string kingdomName = hero.Clan?.Kingdom?.Name?.ToString();

                        hasAnyConfig = PocColorMod.config.HasAnyConfigFor(
                            kingdomName, clanName, null, false, false);
                    }
                    else if (__instance.Party.HomeSettlement?.OwnerClan != null)
                    {
                        string ownerClan = __instance.Party.HomeSettlement.OwnerClan.Name?.ToString();
                        string kingdomName = __instance.Party.HomeSettlement.OwnerClan.Kingdom?.Name?.ToString();

                        hasAnyConfig = PocColorMod.config.HasAnyConfigFor(
                            kingdomName, ownerClan ?? clanName, null, false, false);
                    }

                    // ✅ FIX: Si no hay config, dejar que el juego use el banner vanilla
                    if (!hasAnyConfig)
                    {
                        Log.write($">>> PartyBanner: No config for [{partyName}] - using vanilla banner");
                        return true; // Dejar que el juego maneje el banner completamente
                    }

                    // APLICAR BANNER (mismo código para todos)
                    if (__instance.Party.LeaderHero != null)
                    {
                        var hero = __instance.Party.LeaderHero;
                        string bannerKey = hero.ClanBanner != null ? PocColorMod.SerializeBanner(hero.ClanBanner) : null;
                        string charname = hero.Name?.ToString();

                        Banner bannerObj = !string.IsNullOrEmpty(bannerKey) ? new Banner(bannerKey) : null;
                        string newshield = getIconFromConfig(clanName, bannerObj, charname, true, false, false, 0, null);

                        if (!string.IsNullOrEmpty(newshield))
                        {
                            value = new BannerImageIdentifierVM(new Banner(newshield), true);
                        }
                    }
                    else if (__instance.Party.HomeSettlement != null && __instance.Party.HomeSettlement.OwnerClan != null)
                    {
                        string bannerStr = PocColorMod.SerializeBanner(__instance.Party.HomeSettlement.OwnerClan.Banner);
                        string unitName = __instance.Party.HomeSettlement.Owner?.Name?.ToString();
                        string ownerClan = __instance.Party.HomeSettlement.OwnerClan.Name?.ToString();

                        Banner tempBanner = !string.IsNullOrEmpty(bannerStr) ? new Banner(bannerStr) : null;
                        string icon = getIconFromConfig(ownerClan ?? clanName, tempBanner, unitName, false, false, false, 0, null);

                        if (!string.IsNullOrEmpty(icon))
                        {
                            value = new BannerImageIdentifierVM(new Banner(icon), true);
                        }
                    }
                    else
                    {
                        // Party sin líder ni settlement (ej: looters puros)
                        string icon = getIconFromConfig(clanName, null, partyName, false, false, false, 0, null);

                        if (!string.IsNullOrEmpty(icon))
                        {
                            value = new BannerImageIdentifierVM(new Banner(icon), true);
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.write($"Error in PartyBanner setter: {e.Message}");
                }

                return true;
            }
        }

        #endregion

        #region Clan and Banner Override Patches

        [HarmonyPatch(typeof(Clan), "UpdateBannerColorsAccordingToKingdom")]
        internal class PocColorModOverrideBanner
        {
            public static bool Prefix(ref Clan __instance, ref string __state)
            {
                try
                {
                    if (PocColorMod.config == null) return true;

                    // CRITICAL: NEVER modify player clan banner
                    if (__instance == Clan.PlayerClan)
                    {
                        Log.write(">>> UpdateBannerColorsAccordingToKingdom: SKIPPED for PlayerClan");
                        return false; // Don't execute the original method
                    }

                    __state = PocColorMod.SerializeBanner(__instance.Banner);
                }
                catch (Exception e)
                {
                    Log.write($"Error in Clan UpdateBanner Prefix: {e.Message}");
                }
                return true;
            }

            public static void Postfix(ref Clan __instance, string __state)
            {
                try
                {
                    if (PocColorMod.config == null) return;

                    // CRITICAL: NEVER modify player clan banner
                    if (__instance == Clan.PlayerClan)
                    {
                        Log.write(">>> UpdateBannerColorsAccordingToKingdom Postfix: SKIPPED for PlayerClan");
                        return;
                    }

                    var kingdom = __instance.Kingdom;
                    string playerClanName = Clan.PlayerClan?.Name?.ToString();
                    string playerKingdomName = Clan.PlayerClan?.Kingdom?.Name?.ToString();

                    bool isPlayerClan = !string.IsNullOrEmpty(playerClanName) && __instance.Name?.ToString() == playerClanName;
                    bool isPlayerKingdom = !string.IsNullOrEmpty(playerKingdomName) && kingdom?.Name?.ToString() == playerKingdomName;

                    string clanName = __instance.Name?.ToString();
                    string kingdomName = kingdom?.Name?.ToString();

                    // ✅ Check si hay config
                    bool hasConfig = false;
                    if (PocColorMod.config.clans != null && !string.IsNullOrEmpty(clanName))
                    {
                        hasConfig = PocColorMod.config.clans.ContainsKey(clanName);
                        if (!hasConfig && isPlayerClan)
                            hasConfig = PocColorMod.config.clans.ContainsKey("PlayerClan");
                    }

                    if (!hasConfig && PocColorMod.config.kingdoms != null && !string.IsNullOrEmpty(kingdomName))
                    {
                        hasConfig = PocColorMod.config.kingdoms.ContainsKey(kingdomName);
                        if (!hasConfig && isPlayerKingdom)
                            hasConfig = PocColorMod.config.kingdoms.ContainsKey("PlayerKingdom");
                    }

                    if (!hasConfig && PocColorMod.config.defaultConfig != null)
                    {
                        hasConfig = true;
                    }

                    // ✅ FIX: Si no hay config, RESTAURAR banner original y salir
                    if (!hasConfig)
                    {
                        Log.write($">>> UpdateBannerColorsAccordingToKingdom: No config for clan:[{clanName}] kingdom:[{kingdomName}] - restoring vanilla");

                        // SAFETY: Solo restaurar si __state no es NULL y tiene contenido válido
                        if (!string.IsNullOrEmpty(__state))
                        {
                            try
                            {
                                var restoredBanner = new Banner(__state);
                                Traverse.Create(__instance).Field("_banner").SetValue(restoredBanner);
                                Log.write($">>> Banner restored successfully for [{clanName}]");
                            }
                            catch (Exception ex)
                            {
                                Log.write($">>> ERROR restoring banner for [{clanName}]: {ex.Message}");
                            }
                        }
                        else
                        {
                            Log.write($">>> WARNING: __state is NULL/empty for [{clanName}], cannot restore banner");
                        }

                        return;
                    }

                    // Si llegamos aquí, HAY config, procesar normalmente
                    var clanConfig = PocColorMod.config.GetClanConfig(
                        kingdomName,
                        clanName,
                        isPlayerKingdom,
                        isPlayerClan);

                    bool follow = clanConfig.Item1;
                    bool followBG = clanConfig.Item2;
                    string clanBanner = clanConfig.Item3;
                    string primary = clanConfig.Item5;
                    string secondary = clanConfig.Item6;

                    uint color = __instance.Color;
                    uint color2 = __instance.Color2;

                    if (primary != null)
                        color = parseColor(primary);
                    if (secondary != null)
                        color2 = parseColor(secondary);

                    // Apply banner logic
                    if (!follow && !followBG && clanBanner == null)
                    {
                        // Restaurar banner original
                        if (!string.IsNullOrEmpty(__state))
                        {
                            Traverse.Create(__instance).Field("_banner").SetValue(new Banner(__state));
                        }
                    }
                    else if (!follow && !followBG && clanBanner != null)
                    {
                        var banner = new Banner(clanBanner);
                        Traverse.Create(__instance).Field("_banner").SetValue(banner);

                        if (primary != null)
                            banner.ChangePrimaryColor(color);
                        if (secondary != null)
                            banner.ChangeIconColors(color2);
                    }
                    else if ((follow || followBG) && clanBanner != null)
                    {
                        var banner = new Banner(clanBanner);
                        Traverse.Create(__instance).Field("_banner").SetValue(banner);

                        // SAFETY: Verificar que kingdom no sea NULL
                        if (kingdom != null)
                        {
                            banner.ChangePrimaryColor(kingdom.PrimaryBannerColor);
                            if (!followBG)
                                banner.ChangeIconColors(kingdom.SecondaryBannerColor);
                        }
                    }
                    else if ((follow || followBG) && kingdom != null)
                    {
                        // Si follow pero no hay clanBanner definido, usar el banner actual y solo cambiar colores
                        // SAFETY: No llamar UpdateBannerColorsAccordingToKingdom para evitar recursión infinita
                        // En su lugar, aplicar colores directamente

                        if (__instance.Banner != null)
                        {
                            __instance.Banner.ChangePrimaryColor(kingdom.PrimaryBannerColor);
                            if (!followBG)
                                __instance.Banner.ChangeIconColors(kingdom.SecondaryBannerColor);
                        }
                    }

                    // CRITICAL FIX: ALWAYS apply colors to Clan.Color and Clan.Color2 (for campaign map display)
                    if (primary != null)
                    {
                        Traverse.Create(__instance).Property("Color").SetValue(color);
                        Log.write($">>> Applied Clan.Color for [{clanName}]: {color}");
                    }

                    if (secondary != null)
                    {
                        Traverse.Create(__instance).Property("Color2").SetValue(color2);
                        Log.write($">>> Applied Clan.Color2 for [{clanName}]: {color2}");
                    }

                    // Update cache
                    if (__instance.Banner != null)
                    {
                        string bannerStr = PocColorMod.SerializeBanner(__instance.Banner);
                        if (!string.IsNullOrEmpty(bannerStr))
                        {
                            PocColorMod.bannerClanCache.TryAdd(bannerStr, clanName);
                            if (kingdom != null && !string.IsNullOrEmpty(kingdomName))
                                PocColorMod.bannerKingdomCache.TryAdd(bannerStr, kingdomName);
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.write($"Error in Clan UpdateBanner Postfix: {e.Message}");
                    Log.write($"Stack trace: {e.StackTrace}");
                }
            }
        }

        [HarmonyPatch(typeof(BannerEditorVM), "SetClanRelatedRules")]
        internal class BannerEditorVMSetClanRules
        {
            private static bool Prefix(BannerEditorVM __instance, ref bool canChangeBackgroundColor)
            {
                canChangeBackgroundColor = true;
                return true;
            }
        }

        [HarmonyPatch(typeof(GauntletBannerEditorScreen), "OnDone")]
        internal class BannerEditorGauntletScreen_OnDone
        {
            private static void Postfix(ref GauntletBannerEditorScreen __instance)
            {
                Log.write(">>> ============================================");
                Log.write(">>> BannerEditorGauntletScreen_OnDone CALLED!");
                Log.write(">>> ============================================");

                try
                {
                    Log.write(">>> Step 1: Getting clan field...");

                    // Get the clan from the private readonly field
                    var clanField = typeof(GauntletBannerEditorScreen).GetField("_clan",
                        BindingFlags.NonPublic | BindingFlags.Instance);

                    if (clanField == null)
                    {
                        Log.write(">>> ERROR: Could not find _clan field!");
                        return;
                    }

                    var playerClan = clanField.GetValue(__instance) as Clan;
                    Log.write($">>> Step 2: Got clan: [{playerClan?.Name?.ToString() ?? "NULL"}]");

                    // Only process if it's the player's clan
                    if (playerClan == null)
                    {
                        Log.write(">>> ERROR: playerClan is NULL!");
                        return;
                    }

                    if (playerClan != Clan.PlayerClan)
                    {
                        Log.write($">>> Not player clan, skipping. (Clan: {playerClan.Name}, PlayerClan: {Clan.PlayerClan?.Name})");
                        return;
                    }

                    // Avisar al mapa para redibujar el banner de la etiqueta del jugador
                    PocColorModPlayerNameplateRefresh.playerBannerChanged = true;

                    Log.write(">>> Step 3: Getting banner editor layer...");

                    // Get the banner editor layer
                    var bannerEditorLayerField = typeof(GauntletBannerEditorScreen).GetField("_bannerEditorLayer",
                        BindingFlags.NonPublic | BindingFlags.Instance);

                    if (bannerEditorLayerField == null)
                    {
                        Log.write(">>> ERROR: Could not find _bannerEditorLayer field!");
                        return;
                    }

                    var bannerEditorLayer = bannerEditorLayerField.GetValue(__instance);

                    if (bannerEditorLayer == null)
                    {
                        Log.write(">>> ERROR: bannerEditorLayer is NULL!");
                        return;
                    }

                    Log.write($">>> Step 4: Got banner editor layer type: [{bannerEditorLayer.GetType().Name}]");

                    // Get BannerEditorVM from DataSource property
                    var dataSourceProp = bannerEditorLayer.GetType().GetProperty("DataSource");

                    if (dataSourceProp == null)
                    {
                        Log.write(">>> ERROR: Could not find DataSource property!");
                        return;
                    }

                    var dataSource = dataSourceProp.GetValue(bannerEditorLayer);

                    if (dataSource == null)
                    {
                        Log.write(">>> ERROR: dataSource is NULL!");
                        return;
                    }

                    Log.write($">>> Step 5: Got dataSource type: [{dataSource.GetType().Name}]");

                    // Get BannerVM property
                    var bannerVMProp = dataSource.GetType().GetProperty("BannerVM");

                    if (bannerVMProp == null)
                    {
                        Log.write(">>> ERROR: Could not find BannerVM property!");
                        return;
                    }

                    var bannerVM = bannerVMProp.GetValue(dataSource);

                    if (bannerVM == null)
                    {
                        Log.write(">>> ERROR: bannerVM is NULL!");
                        return;
                    }

                    Log.write($">>> Step 6: Got bannerVM type: [{bannerVM.GetType().Name}]");

                    // Get Banner property from BannerVM
                    var bannerProp = bannerVM.GetType().GetProperty("Banner");

                    if (bannerProp == null)
                    {
                        Log.write(">>> ERROR: Could not find Banner property!");
                        return;
                    }

                    var editedBanner = bannerProp.GetValue(bannerVM) as Banner;

                    if (editedBanner == null)
                    {
                        Log.write(">>> ERROR: editedBanner is NULL!");
                        return;
                    }

                    Log.write(">>> Step 7: Got banner, serializing...");

                    // Serialize the new banner
                    string newBannerStr = PocColorMod.SerializeBanner(editedBanner);

                    if (string.IsNullOrEmpty(newBannerStr))
                    {
                        Log.write(">>> ERROR: Banner serialization returned empty string!");
                        return;
                    }

                    Log.write($">>> Step 8: Player edited banner in clan menu: [{newBannerStr}]");

                    // Get the new colors
                    uint primaryColor = editedBanner.GetPrimaryColor();
                    uint iconColor = editedBanner.GetFirstIconColor();

                    Log.write($">>> Step 9: Colors - Primary: {primaryColor}, Icon: {iconColor}");

                    // Update cache
                    PocColorMod.bannerClanCache[newBannerStr] = playerClan.Name?.ToString();
                    if (playerClan.Kingdom != null)
                    {
                        PocColorMod.bannerKingdomCache[newBannerStr] = playerClan.Kingdom.Name?.ToString();
                    }

                    Log.write(">>> Step 10: Cache updated, saving to file...");

                    // IMPORTANTE: Guardar inmediatamente a archivo
                    try
                    {
                        string saveId = Campaign.Current?.UniqueGameId ?? "default";
                        string directory = "..\\..\\modules\\PocColor\\PlayerBanners";

                        Log.write($">>> Directory: [{directory}]");

                        if (!Directory.Exists(directory))
                        {
                            Directory.CreateDirectory(directory);
                            Log.write(">>> Directory created");
                        }

                        string bannerPath = $"{directory}\\{saveId}.json";
                        Log.write($">>> Banner path: [{bannerPath}]");

                        var bannerData = new
                        {
                            clanBanner = newBannerStr,
                            primaryColor = BannerManager.GetColorId(primaryColor).ToString(),
                            secondaryColor = BannerManager.GetColorId(iconColor).ToString()
                        };

                        string json = JsonConvert.SerializeObject(bannerData, Formatting.Indented);
                        Log.write($">>> JSON content: [{json}]");

                        File.WriteAllText(bannerPath, json);

                        Log.write($">>> ✓✓✓ Banner SAVED to file successfully! ✓✓✓");
                        Log.write($">>> File path: [{bannerPath}]");
                        Log.write($">>> Banner content: [{newBannerStr}]");
                    }
                    catch (Exception saveEx)
                    {
                        Log.write($">>> ✗✗✗ ERROR saving banner to file: {saveEx.Message}");
                        Log.write($">>> Stack trace: {saveEx.StackTrace}");
                    }

                    Log.write(">>> Banner edit processing completed successfully");
                }
                catch (Exception ex)
                {
                    Log.write($">>> ✗✗✗ CRITICAL ERROR in BannerEditor OnDone: {ex.Message}");
                    Log.write($">>> Stack trace: {ex.StackTrace}");
                }

                Log.write(">>> ============================================");
                Log.write(">>> BannerEditorGauntletScreen_OnDone FINISHED");
                Log.write(">>> ============================================");
            }
        }

        // Vanilla no marca la etiqueta del jugador como "sucia" al editar el banner,
        // asi que el mapa sigue mostrando el banner viejo hasta recargar.
        // Forzamos un refresco una sola vez despues de pulsar "Done".
        [HarmonyPatch(typeof(PartyNameplatesVM), "Update")]
        internal class PocColorModPlayerNameplateRefresh
        {
            public static bool playerBannerChanged = false;

            public static void Prefix(PartyNameplatesVM __instance)
            {
                if (!playerBannerChanged)
                    return;

                try
                {
                    if (__instance.PlayerNameplate != null)
                    {
                        playerBannerChanged = false;
                        __instance.PlayerNameplate.RefreshDynamicProperties(true);
                        Log.write(">>> Player nameplate banner refreshed after banner edit");
                    }
                }
                catch (Exception e)
                {
                    playerBannerChanged = false;
                    Log.write($"Error in PlayerNameplateRefresh: {e.Message}");
                }
            }
        }



        [HarmonyPatch(typeof(Banner), "Serialize")]
        internal class PocColorModFixBannerSerialize
        {
            public static bool Prefix(ref Banner __instance, ref string __result)
            {
                try
                {
                    __result = PocColorMod.SerializeBanner(__instance);
                    return false;
                }
                catch (Exception ex)
                {
                    Log.write($"Error in Banner Serialize: {ex.Message}");
                }
                return true;
            }
        }

        #endregion
    }
}