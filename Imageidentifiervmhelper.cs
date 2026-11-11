using System;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Core.ViewModelCollection.ImageIdentifiers;

namespace PocColor
{
    /// <summary>
    /// Helper para crear ImageIdentifierVM en Bannerlord 1.3
    /// Usa las clases concretas: CharacterImageIdentifierVM, BannerImageIdentifierVM, ItemImageIdentifierVM
    /// </summary>
    public static class ImageIdentifierVMHelper
    {
        /// <summary>
        /// Crea ImageIdentifierVM para un character code
        /// </summary>
        public static ImageIdentifierVM CreateForCharacter(CharacterCode characterCode)
        {
            try
            {
                return new CharacterImageIdentifierVM(characterCode);
            }
            catch (Exception e)
            {
                Log.write($"Error creating CharacterImageIdentifierVM: {e.Message}");
                return null;
            }
        }

        /// <summary>
        /// Crea ImageIdentifierVM para un character code string
        /// </summary>
        public static ImageIdentifierVM CreateForCharacter(string characterCodeString)
        {
            try
            {
                var characterCode = CharacterCode.CreateFrom(characterCodeString);
                return new CharacterImageIdentifierVM(characterCode);
            }
            catch (Exception e)
            {
                Log.write($"Error creating CharacterImageIdentifierVM from string: {e.Message}");
                return null;
            }
        }

        /// <summary>
        /// Crea ImageIdentifierVM para un banner
        /// </summary>
        public static ImageIdentifierVM CreateForBanner(Banner banner, bool nineGrid = false)
        {
            try
            {
                return new BannerImageIdentifierVM(banner, nineGrid);
            }
            catch (Exception e)
            {
                Log.write($"Error creating BannerImageIdentifierVM: {e.Message}");
                return null;
            }
        }

        /// <summary>
        /// Crea ImageIdentifierVM para un item
        /// </summary>
        public static ImageIdentifierVM CreateForItem(ItemObject itemObject, string bannerCode = "")
        {
            try
            {
                return new ItemImageIdentifierVM(itemObject, bannerCode);
            }
            catch (Exception e)
            {
                Log.write($"Error creating ItemImageIdentifierVM: {e.Message}");
                return null;
            }
        }
    }
}