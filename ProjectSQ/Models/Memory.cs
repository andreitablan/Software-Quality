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

            startStack = 60000;
            endStack = 65535;
            currentInstruction = 0;
            internalMemory = new string[1024];
            programData = new byte[65536];//0->60.000 mem, restul stack
            instructionsNumber = 0;

            keyboardBufferIndex = 50000;
            isKeyboardBufferChanged = false;
            firstVideoMemoryIndex = 50001;
            currentIndexMemoryVideo = 50001;
            lastIndexOfMemoryVideo = 50001;
            maxIndexOfMemoryVideo = 60000;
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

                            switch (key)
                            {
                                case "internalMemory":
                                    if (ushort.TryParse(value, out ushort internalMemorySize))
                                        internalMemory = new string[internalMemorySize];

                                    break;
                                case "programData":
                                    if (ushort.TryParse(value, out ushort programDataSize))
                                    {
                                        programData = new byte[programDataSize];
                                        var memoryPart = programDataSize / 8;
                                        startStack = (ushort)(7 * memoryPart + 1);
                                        endStack = programDataSize;
                                        Processor.StackPointer = startStack;
                                    }
                                    break;
                                case "keyboardBufferIndex":
                                    if (ushort.TryParse(value, out ushort keyboardBufferLocation))
                                    {
                                        keyboardBufferIndex = keyboardBufferLocation;
                                    }
                                    break;
                                case "videoMemoryStartIndex":
                                    if (ushort.TryParse(value, out ushort videoMemoryStartIndex))
                                    {
                                        currentIndexMemoryVideo = firstVideoMemoryIndex = lastIndexOfMemoryVideo = videoMemoryStartIndex;
                                    }
                                    break;
                                case "videoMemorySize":
                                    if (ushort.TryParse(value, out ushort videoMemorySize))
                                    {
                                        maxIndexOfMemoryVideo = videoMemorySize;
                                    }
                                    break;

                            }
                        }
                    }
                }
            }
        }

        public static void WipeVideoMemory()
        {
            for (int i = firstVideoMemoryIndex; i < lastIndexOfMemoryVideo; i++)
            {
                Memory.programData[i] = default;
            }

            currentIndexMemoryVideo = firstVideoMemoryIndex = lastIndexOfMemoryVideo = (ushort)(keyboardBufferIndex + 1);

        }
    }


}
