using ProjectSQ.Interfaces.Parser;
using ProjectSQ.Models;

namespace ProjectSQ.Services
{
    public class ParseService : IParseService
    {
        public void LoadInstructions(string file)
        {
            if (File.Exists(file))
            {
                try
                {
                    string[] lines = File.ReadAllLines(file);
                    for (int i = 0; i < lines.Length; i++)
                    {
                        Memory.internalMemory[i] = lines[i];
                    }
                }
                catch (IOException e)
                {
                    Console.WriteLine($"An error occurred while reading the file: {e.Message}");
                }
            }
            else
            {
                Console.WriteLine($"The file '{file}' does not exist.");
            }
        }
        public void Assignment(string operand1, string operand2)
        {
            //ambii operanzi registri
            if (Processor.registerDictionary.ContainsKey(operand1) && Processor.registerDictionary.ContainsKey(operand2))
            {
                new ProcessorService().Assignment(operand1, operand2);
                return;
            }    
            if (Processor.registerDictionary.ContainsKey(operand1))
            {

            }
            if (Pro.ContainsKey(operand1))
        }
    }
}
