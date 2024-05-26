using ProjectSQ.Interfaces.Parser;
using ProjectSQ.Models;
using ProjectSQ.Models.Assertions;
using System.Reflection;

namespace ProjectSQ.Services
{
    public class ParseService : IParseService
    {
        public void LoadInstructions(string file)
        {
            if (file == null)
            {
                throw new ArgumentNullException(nameof(file), "File name cannot be null.");
            }
            CustomAssert.IsTrue(!string.IsNullOrWhiteSpace(file), "Precondition failed: File name cannot be null or whitespace.");

            Assembly assembly = Assembly.GetExecutingAssembly();

            using (Stream stream = assembly.GetManifestResourceStream(file))
            {
                if (stream == null)
                {
                    Console.WriteLine("Resource not found.");
                    return;
                }
                // Precondition: Check if the resource stream is successfully obtained
                CustomAssert.IsTrue(stream != null, "Precondition failed: Resource not found.");
                using (StreamReader reader = new StreamReader(stream))
                {
                    string line;
                    ushort index = 0;
                    while ((line = reader.ReadLine()) != null)
                    {
                        Memory.internalMemory[index] = line;
                        CustomAssert.IsTrue(index < Memory.internalMemory.Length, "Invariant failed: Index exceeds the internal memory size.");

                        if (line.Contains("main"))
                            Memory.currentInstruction = index;
                        index++;

                        // Loop invariant: After each iteration, the index is incremented correctly
                        CustomAssert.IsTrue(index > 0, "Invariant failed: Index was not incremented.");
                    }
                    Memory.instructionsNumber = index;
                    Processor.StackPointer = Memory.startStack;
                    
                    // Postcondition: Ensure that instructions have been loaded and pointers are correctly set
                    CustomAssert.IsTrue(index > 0, "Postcondition failed: No instructions were loaded.");
                    CustomAssert.IsTrue(Memory.instructionsNumber == index, "Postcondition failed: Instructions number mismatch.");
                    CustomAssert.IsTrue(Memory.currentInstruction < Memory.instructionsNumber, "Postcondition failed: Current instruction pointer out of bounds.");
                    CustomAssert.IsTrue(Processor.StackPointer == Memory.startStack, "Postcondition failed: StackPointer was not set to startStack.");

                }
            }
        }
    }
}
