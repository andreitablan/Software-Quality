namespace ProjectSQ.Models
{
    public static class Processor
    {
        public static Dictionary<string, ushort> registerDictionary = new Dictionary<string, ushort> {
            {"reg1" , 0 }, {"reg2" , 0 }, {"reg3" , 0 },{"reg4" , 0 }, {"reg5" , 0 }, {"reg6" , 0 }, {"reg7" , 0 }, {"reg8" , 0 }
        };
        public static ushort StackPointer { get; set; }
        public static bool Equal { get; set; }
        public static bool NotEqual { get; set; }
        public static bool Less { get; set; }
        public static bool Greater { get; set; }
        public static bool LessEqual { get; set; }
        public static bool GreaterEqual { get; set; }

        public static void InitProcessor()
        {
            registerDictionary = new Dictionary<string, ushort> {
            {"reg1" , 0 }, {"reg2" , 0 }, {"reg3" , 0 },{"reg4" , 0 }, {"reg5" , 0 }, {"reg6" , 0 }, {"reg7" , 0 }, {"reg8" , 0 }
        };

            StackPointer = 60000;
            Greater = false;
            Less = false;
            NotEqual = false;
            LessEqual = false;
            GreaterEqual = false;
            Equal = false;
        }
    }
}
