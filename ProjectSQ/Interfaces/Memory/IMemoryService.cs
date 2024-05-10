using ProjectSQ.Models;

namespace ProjectSQ.Interfaces.Memory
{
    public interface IMemoryService
    {
        void ConfigureMemorySize(string file);
        ResultMemory LoadMemoryData();
    }
}
