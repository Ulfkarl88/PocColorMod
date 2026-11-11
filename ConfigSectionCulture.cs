using System;

namespace PocColor.Config
{
    // Token: 0x02000006 RID: 6
    public class ConfigSectionCulture : ConfigSectionTier
    {
        // Token: 0x17000009 RID: 9
        // (get) Token: 0x0600002B RID: 43 RVA: 0x00005FF0 File Offset: 0x000041F0
        // (set) Token: 0x0600002C RID: 44 RVA: 0x00005FF8 File Offset: 0x000041F8
        public Map<int, ConfigSectionTier> tiers { get; set; }
    }
}
