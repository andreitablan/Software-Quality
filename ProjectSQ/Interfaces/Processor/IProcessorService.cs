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
        void Jump(string label);
        void JumpIfEqual(string label);
        void JumpIfNotEqual(string label);
        void JumpIfLessThan(string label);
        void JumpIfGreaterThan(string label);
        void JumpIfLessThanOrEqual(string label);
        void JumpIfGreaterThanOrEqual(string label);

        // Stack Operations
        void Push(string reg);
        void Pop(string reg);

        // Function Call/Return
        void Call(string functionName);
        void Return();

        ResultRegisters LoadResultRegisters();
        void ResetData();

        //keyboard memory
        void WriteValueToKeyboardBuffer(ushort value);
    }
}
