using System;

namespace PocColor.Config
{
    // Token: 0x02000009 RID: 9
    internal interface UnitsContainerSection
    {
        // Token: 0x1700000C RID: 12
        // (get) Token: 0x06000034 RID: 52
        // (set) Token: 0x06000035 RID: 53
        Map<string, PocColorModConfigUnit> units { get; set; }
    }
}
