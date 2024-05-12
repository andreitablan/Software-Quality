using System.Reflection;

namespace ProjectSQ.Models
{
    public static class Memory
    {
        public static ushort startStack = 60000, endStack = 65535;
        public static ushort currentInstruction = 0;
        public static string[] internalMemory = new string[1024];
        public static byte[] programData = new byte[65536];//0->60.000 mem, restul stack
        public static ushort instructionsNumber = 0;

        public static ushort keyboardBufferIndex = 50000;
        public static bool isKeyboardBufferChanged = false;
        public static ushort firstVideoMemoryIndex = 50001;
        public static ushort currentIndexMemoryVideo = 50001;
        public static ushort lastIndexOfMemoryVideo = 50001;
        public static ushort maxIndexOfMemoryVideo = 60000;


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
                                if (ushort.TryParse(value, out ushort arraySize))
                                    internalMemory = new string[arraySize];
                            }

                            else if (key == "programData")
                            {
                                if (ushort.TryParse(value, out ushort arraySize))
                                {
                                    programData = new byte[arraySize];
                                    ushort memoryPart = (ushort)(arraySize / 8);
                                    keyboardBufferIndex = (ushort)(5 * memoryPart);
                                    firstVideoMemoryIndex = lastIndexOfMemoryVideo = (ushort)(keyboardBufferIndex + 1);
                                    currentIndexMemoryVideo = firstVideoMemoryIndex;
                                    maxIndexOfMemoryVideo = (ushort)(7 * memoryPart);
                                    startStack = (ushort)(maxIndexOfMemoryVideo + 1);
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
