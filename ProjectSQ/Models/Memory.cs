using System.Reflection;

namespace ProjectSQ.Models
{
    public static class Memory
    {
        public static int startStack = 60000, endStack = 65535, currentStackPointer = 60000;
        public static int currentInstruction = 0;
        public static string[] internalMemory = new string[1024];
        public static byte[] programData = new byte[65536];//0->60.000 mem, restul stack
        public static int instructionsNumber = 0;
        public static int keyboardBufferIndex = 50000;
        public static bool isKeyboardBufferChanged = false;
        public static int lastIndexOfMemoryVideo = 50001;
        public static int maxIndexOfMemoryVideo = 60000;


        public static void InitMemory()
        {
            string configFilePath = "ProjectSQ.Utils.config.txt";

            Assembly assembly = Assembly.GetExecutingAssembly();

            using (Stream stream = assembly.GetManifestResourceStream(configFilePath))
            {
                if (stream == null)
                {
                    Console.WriteLine("Resource not found.");
                    return;
                }

                using (StreamReader reader = new StreamReader(stream))
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
                            }

                            else if (key == "programData")
                            {
                                if (int.TryParse(value, out int arraySize))
                                {
                                    programData = new byte[arraySize];
                                    var memoryPart = arraySize / 8;
                                    keyboardBufferIndex = 5 * memoryPart;
                                    lastIndexOfMemoryVideo = keyboardBufferIndex + 1;
                                    maxIndexOfMemoryVideo = 7 * memoryPart;
                                    startStack = maxIndexOfMemoryVideo + 1;
                                    endStack = arraySize;

                                }
                            }
                        }
                    }
                }
            }
        }
    }


}
