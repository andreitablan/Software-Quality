using ProjectSQ.Interfaces.Memory;

namespace ProjectSQ.Models
{
    public class Memory 
    {
        public static int startStack = 60000, endStack = 65535, currentStackPointer = 60000;
        public static int currentInstruction = 0;
        public static string[] internalMemory = new string[1024];
        public static byte[] programData = new byte[65536];//0->60.000 mem, restul stack
        public static int instructionsNumber = 0;

        public Memory()
        {
            string configFilePath = "configuration.txt";
            using (StreamReader reader = new StreamReader(configFilePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] parts = line.Split('=');
                    if (parts.Length == 2)
                    {
                        string key = parts[0].Trim();
                        string value = parts[1].Trim();

                        if (key == "internalMemory")
                        {
                            if (int.TryParse(value, out int arraySize))
                                internalMemory = new string[arraySize];
                            break;
                        }

                        else if (key == "programData")
                        {
                            if (int.TryParse(value, out int arraySize))
                                programData = new byte[arraySize];
                            break;
                        }
                    }
                }
            }
        }
    }

    
}
