using System;
using System.IO;
using System.Reflection;

namespace PocColor
{
    // Token: 0x02000002 RID: 2
    internal class Log
    {
        // Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
        static Log()
        {
            try
            {
                Log.Writer = new StreamWriter(new FileStream(Path.Combine(Path.GetDirectoryName(Path.GetFullPath(Assembly.GetExecutingAssembly().Location)), "../../log.txt"), FileMode.Create, FileAccess.Write, FileShare.ReadWrite));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        // Token: 0x06000002 RID: 2 RVA: 0x000020B4 File Offset: 0x000002B4
        public static void write(string messageString)
        {
            try
            {
                bool flag = Log.Writer != null;
                if (flag)
                {
                    Log.Writer.WriteLine(messageString);
                    Log.Writer.Flush();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        // Token: 0x04000001 RID: 1
        private static StreamWriter Writer;
    }
}
