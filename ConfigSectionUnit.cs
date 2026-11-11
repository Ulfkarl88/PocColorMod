using System;

namespace PocColor.Config
{
    // Token: 0x02000008 RID: 8
    public class ConfigSectionUnit : ConfigSectionCulture
    {
        // Token: 0x1700000B RID: 11
        // (get) Token: 0x06000031 RID: 49 RVA: 0x00006024 File Offset: 0x00004224
        // (set) Token: 0x06000032 RID: 50 RVA: 0x0000602C File Offset: 0x0000422C
        public Map<string, ConfigSectionCulture> cultures { get; set; }
    }
}
