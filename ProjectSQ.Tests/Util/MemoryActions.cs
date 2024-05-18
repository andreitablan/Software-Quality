using ProjectSQ.Models;

namespace ProjectSQ.Tests.util
{
    public static class MemoryActions
    {
        public static void WriteValueToMemory(int indexOperandOne, ushort valueOperandTwo)
        {
            var highByte = (byte)(valueOperandTwo >> 8);
            var lowByte = (byte)(valueOperandTwo & 0xFF);

            // write to two consecutive addresses
            Memory.programData[indexOperandOne] = lowByte;
            Memory.programData[indexOperandOne + 1] = highByte;
        }
    }
}
