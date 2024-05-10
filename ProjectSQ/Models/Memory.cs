using ProjectSQ.Interfaces.Memory;

namespace ProjectSQ.Models
{
    public class Memory 
    {
        public static int currentInstruction = 0;
        public static string[] internalMemory = new string[1024];
        public static byte[] programData = new byte[65536];//0->60.000 mem, restul stack
        public static int instructionsNumber = 0;
    }
}
