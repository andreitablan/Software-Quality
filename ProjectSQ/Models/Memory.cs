﻿using ProjectSQ.Interfaces.Memory;

namespace ProjectSQ.Models
{
    public class Memory : IMemoryService
    {
        public static int currentInstruction = 0;
        public static string[] internalMemory = new string[1024];
        public static byte[] programData = new byte[65536];//0->60.000 mem, restul stack
        public static int instructionNumber = 0;

        //program's configuration file
        //each size must be a multiple of 1 KB ( = 1024 bytes)
        //each size may not exceed 65536 ( = 216) bytes
        public void ConfigureMemorySize(string file)
        {
            throw new NotImplementedException();
        }

    }
}
