using ProjectSQ.Interfaces.Parser;
using ProjectSQ.Models;
using System.Reflection;

namespace ProjectSQ.Services
{
    public class ParseService : IParseService
    {
        public void LoadInstructions(string file)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();

            using (Stream stream = assembly.GetManifestResourceStream(file))
            {
                if (stream == null)
                {
                    Console.WriteLine("Resource not found.");
                    return;
                }

                using (StreamReader reader = new StreamReader(stream))
                {
                    string line;
                    var index = 0;
                    while ((line = reader.ReadLine()) != null)
                    {
                        Memory.internalMemory[index] = line;
                        index++;
                    }
                    Memory.instructionNumber = index-1;

                }
            }
        }
    }
}
