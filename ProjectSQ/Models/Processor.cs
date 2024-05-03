using System.Numerics;

namespace ProjectSQ.Models
{
    public static class Processor
    {
        #region prepare processor
        public static int startStack = 60000, endStack = 65535, currentStackPointer = 60000;

        public static readonly Dictionary<string, int> registerDictionary = new Dictionary<string, int> {
            {"reg1" , 0 }, {"reg2" , 0 }, {"reg3" , 0 },{"reg4" , 0 }, {"reg5" , 0 }, {"reg6" , 0 }, {"reg7" , 0 }, {"reg8" , 0 }
        };
        public static bool Equal { get; set; }
        public static bool NotEqual { get; set; }
        public static bool Less { get; set; }
        public static bool Greater { get; set; }
        public static bool LessEqual { get; set; }
        public static bool GreaterEqual { get; set; }
        #endregion prepare processor
    }
}
