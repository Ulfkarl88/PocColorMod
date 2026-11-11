using System;

namespace PocColor.Config
{
    // Token: 0x02000007 RID: 7
    public class ConfigSectionTier : ConfigSection
    {
        // Token: 0x1700000A RID: 10
        // (get) Token: 0x0600002E RID: 46 RVA: 0x0000600A File Offset: 0x0000420A
        // (set) Token: 0x0600002F RID: 47 RVA: 0x00006012 File Offset: 0x00004212
        public Map<string, ConfigSection> types { get; set; }
    }
}
