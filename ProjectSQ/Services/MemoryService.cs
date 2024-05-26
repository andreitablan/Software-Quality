using ProjectSQ.Interfaces.Memory;
using ProjectSQ.Models;
using ProjectSQ.Models.Assertions;

namespace ProjectSQ.Services
{
    public class MemoryService : IMemoryService
    {
        public ResultMemory LoadMemoryData()
        {
            // Precondition: Validate the state of Memory before loading memory data
            CustomAssert.IsTrue(Memory.programData != null, "Precondition failed: Memory.programData is not initialized");
            CustomAssert.IsTrue(Memory.programData.Length > 0, "Precondition failed: Memory.programData length must be greater than zero");

            ResultMemory resultMemory = new ResultMemory(Memory.programData);

            // Postcondition: Validate the result of the LoadMemoryData operation
            CustomAssert.IsTrue(resultMemory != null, "Postcondition failed: resultMemory is null");
            CustomAssert.IsTrue(resultMemory.Data.Count() == Memory.programData.Length, "Postcondition failed: resultMemory.Data length does not match Memory.programData length");

            for (int i = 0; i < Memory.programData.Length; i++)
            {
                CustomAssert.IsTrue(resultMemory.Data.ElementAt(i) == Memory.programData[i], $"Postcondition failed: resultMemory.Data[{i}] does not match Memory.programData[{i}]");
            }

            return resultMemory;
        }

    }
}
