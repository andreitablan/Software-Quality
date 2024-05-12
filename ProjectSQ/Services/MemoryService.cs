using ProjectSQ.Interfaces.Memory;
using ProjectSQ.Models;

namespace ProjectSQ.Services
{
    public class MemoryService : IMemoryService
    {
        public ResultMemory LoadMemoryData()
        {
            ResultMemory resultMemory = new ResultMemory(Memory.programData);
            return resultMemory;
        }
    }
}
