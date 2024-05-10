using ProjectSQ.Interfaces.Memory;
using ProjectSQ.Models;

namespace ProjectSQ.Services
{
    public class MemoryService : IMemoryService
    {
        //program's configuration file
        //each size must be a multiple of 1 KB ( = 1024 bytes)
        //each size may not exceed 65536 ( = 216) bytes
        public void ConfigureMemorySize(string file)
        {
            throw new NotImplementedException();
        }

        public ResultMemory LoadMemoryData()
        {
            ResultMemory resultMemory = new ResultMemory(Memory.programData);
            return resultMemory;
        }
    }
}
