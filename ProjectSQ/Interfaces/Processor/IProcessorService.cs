namespace ProjectSQ.Interfaces.Processor
{
    public interface IProcessorService
    {
        void ExecuteFile();
        bool Assignment(string operandOne, string operandTwo);

        // Arithmetic Operations
        bool Addition(string operandOne, string operandTwo);
        void Subtraction(string reg1, string reg2);
        void Subtraction(string reg1, short val);
        void Multiplication(string reg1, string reg2);
        void Multiplication(string reg1, short val);
        void Division(string reg1, string reg2);
        void Division(string reg1, short val);

        // Boolean Operations
        void Not(string reg1);
        void And(string reg1, string reg2);
        void Or(string reg1, string reg2);
        void Xor(string reg1, string reg2);
        void Shift(string reg1, short val);
        void Shift(string reg1, string reg2);

        // Comparison
        public bool Compare(string operandOne, string operandTwo);

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

    }
}
