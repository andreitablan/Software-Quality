using ProjectSQ.Models;

namespace ProjectSQ.Interfaces.Processor
{
    public interface IProcessorService
    {
        void ExecuteFile();
        bool Assignment(string operandOne, string operandTwo);

        // Arithmetic Operations
        bool Addition(string operandOne, string operandTwo);
        bool Subtraction(string operandOne, string operandTwo);
        bool Multiplication(string operandOne, string operandTwo);
        bool Division(string operandOne, string operandTwo);

        // Boolean Operations
        bool Not(string operandOne);
        bool And(string operandOne, string operandTwo);
        bool Or(string operandOne, string operandTwo);
        bool Xor(string operandOne, string operandTwo);
        bool ShiftLeft(string operandOne, string operandTwo);
        bool ShiftRight(string operandOne, string operandTwo);

        // Comparison
        bool Compare(string operandOne, string operandTwo);

        // Jump Operations
        bool Jump(string label);
        bool JumpIfEqual(string label);
        bool JumpIfNotEqual(string label);
        bool JumpIfLessThan(string label);
        bool JumpIfGreaterThan(string label);
        bool JumpIfLessThanOrEqual(string label);
        bool JumpIfGreaterThanOrEqual(string label);

        // Stack Operations
        void Pop(string operand);
        void Push(string operand);

        // Function Call/Return
        void Call(string functionName);
        void Return();
        //read
        void Read(string operand);
        
        //helper functions
        ResultRegisters LoadResultRegisters();
        void ResetData();

        //keyboard memory
        void WriteValueToKeyboardBuffer(ushort value);

        //video memory
        string ReadFromVideoMemory();
        void RemoveFromVideoMemory();
    }
}
