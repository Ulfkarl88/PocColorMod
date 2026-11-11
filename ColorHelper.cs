using HarmonyLib;
using Helpers;
using PocColor.Config;
using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Core.ImageIdentifiers;
using TaleWorlds.Core.ViewModelCollection.ImageIdentifiers;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace PocColor
{
    /// <summary>
    /// Helper class to reduce code duplication across patches
    /// Extracted common color/banner application logic
    /// </summary>
    internal static class ColorHelper
    {
        /// <summary>
        /// Gets colors for a character from config (simplified wrapper)
        /// </summary>
        public static (int mode, uint? color1, uint? color2, string newBanner, string newShield) GetCharacterColors(
            CharacterObject character,
            Banner banner,
            string clanName = null)
        {
            if (character == null) return (0, null, null, null, null);

            string charName = character.Name?.ToString() ?? "";
            if (clanName == null)
                clanName = character.HeroObject?.Clan?.Name?.ToString();

            return PocColorModSetColors.getColorFromConfig(
                clanName,
                ref banner,
                charName,
                character.IsHero,
                character.IsMounted,
                character.IsRanged,
                character.Tier,
                character.Culture?.Name?.ToString() ?? ""
            );
        }

        /// <summary>
        /// Applies colors to an ImageIdentifierVM by creating a new CharacterCode
        /// </summary>
        public static ImageIdentifierVM ApplyColorsToImageIdentifier(
            ImageIdentifierVM originalVm,
            uint? color1,
            uint? color2)
        {
            if (originalVm == null || (color1 == null && color2 == null))
                return originalVm;

            try
            {
                var imageIdentifier = (ImageIdentifier)Traverse.Create(originalVm)
                    .Field("_imageIdentifier")
                    .GetValue();

                if (imageIdentifier == null) return originalVm;

                var charCode = CharacterCode.CreateFrom(imageIdentifier.Id);

                if (color1.HasValue) charCode.Color1 = color1.Value;
                if (color2.HasValue) charCode.Color2 = color2.Value;

                return new CharacterImageIdentifierVM(charCode);
            }
            catch (Exception e)
            {
                Log.write($"Error applying colors to ImageIdentifier: {e.Message}");
                return originalVm;
            }
        }

        /// <summary>
        /// Updates banner for a ViewModel (common pattern across many patches)
        /// </summary>
        public static void UpdateViewModelBanner(
            object viewModel,
            string propertyName,
            Banner banner)
        {
            if (viewModel == null || banner == null) return;

            try
            {
                var bannerVm = new BannerImageIdentifierVM(banner, true);
                Traverse.Create(viewModel).Property(propertyName).SetValue(bannerVm);
            }
            catch (Exception e)
            {
                Log.write($"Error updating banner for {propertyName}: {e.Message}");
            }
        }

        /// <summary>
        /// Gets clan name safely from various sources
        /// </summary>
        public static string GetClanName(Hero hero = null, PartyBase party = null, string fallback = null)
        {
            if (hero?.Clan?.Name != null)
                return hero.Clan.Name.ToString();

            if (party?.MobileParty?.ActualClan?.Name != null)
                return party.MobileParty.ActualClan.Name.ToString();

            if (party?.Owner?.Clan?.Name != null)
                return party.Owner.Clan.Name.ToString();

            return fallback;
        }

        /// <summary>
        /// Creates new Banner from string, returns null if invalid
        /// </summary>
        public static Banner CreateBannerSafe(string bannerStr)
        {
            if (string.IsNullOrEmpty(bannerStr)) return null;

            try
            {
                return new Banner(bannerStr);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Updates CharacterViewModel armor colors using Traverse
        /// </summary>
        public static void UpdateCharacterViewModelColors(
            object characterViewModel,
            uint? color1,
            uint? color2)
        {
            if (characterViewModel == null) return;

            try
            {
                var traverse = Traverse.Create(characterViewModel);

                if (color1.HasValue)
                {
                    traverse.Field("_armorColor1").SetValue(color1.Value);
                    traverse.Method("OnPropertyChanged", "ArmorColor1").GetValue();
                }

                if (color2.HasValue)
                {
                    traverse.Field("_armorColor2").SetValue(color2.Value);
                    traverse.Method("OnPropertyChanged", "ArmorColor2").GetValue();
                }
            }
            catch (Exception e)
            {
                Log.write($"Error updating CharacterViewModel colors: {e.Message}");
            }
        }

        /// <summary>
        /// Common pattern: Get player clan and kingdom names
        /// </summary>
        public static (string clanName, string kingdomName) GetPlayerInfo()
        {
            var clan = Clan.PlayerClan;
            return (
                clan?.Name?.ToString(),
                clan?.Kingdom?.Name?.ToString()
            );
        }

        /// <summary>
        /// Determines if an entity belongs to player (clan or kingdom)
        /// </summary>
        public static (bool isPlayerClan, bool isPlayerKingdom) CheckPlayerAffiliation(
            string clanName,
            string kingdomName)
        {
            var (playerClan, playerKingdom) = GetPlayerInfo();

            bool isPlayerClan = !string.IsNullOrEmpty(playerClan) && clanName == playerClan;
            bool isPlayerKingdom = !string.IsNullOrEmpty(playerKingdom) && kingdomName == playerKingdom;

            return (isPlayerClan, isPlayerKingdom);
        }
    }
}