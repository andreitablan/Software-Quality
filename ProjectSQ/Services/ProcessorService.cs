using ProjectSQ.Interfaces.Processor;
using ProjectSQ.Models;
using System.Text.RegularExpressions;

namespace ProjectSQ.Services
{
    public partial class ProcessorService : IProcessorService
    {
        public void ExecuteFile()
        {
            bool isInputFileGood = true;
            while (Memory.currentInstruction < Memory.internalMemory.Length && isInputFileGood)
            {
                string instruction = Memory.internalMemory[Memory.currentInstruction++];
                string[] words = instruction.Split(' ');
                string operation = words[0];
                switch (operation)
                {
                    case "mov":
                        string[] operands = words[1].Split(",");
                        isInputFileGood = Assignment(operands[0], operands[1]);
                        break;

                    case "add":
                        operands = words[1].Split(",");
                        isInputFileGood = Addition(operands[0], operands[1]);
                        break;
                    case "sub":
                        break;
                    case "mul":
                        break;
                    case "div":
                        break;

                    case "and":
                        break;
                    case "or":
                        break;
                    case "xor":
                        break;
                    case "not":
                        break;
                    case "shr":
                        break;
                    case "shl":
                        break;

                    case "jmp":
                        break;

                }
            }
        }

        public bool Assignment(string operandOne, string operandTwo)
        {
            //both operands are data register (mov reg1,reg2)
            if (Processor.registerDictionary.ContainsKey(operandOne) && Processor.registerDictionary.ContainsKey(operandTwo))
            {
                Processor.registerDictionary[operandOne] = Processor.registerDictionary[operandTwo];
                return true;
            }
            //first operand is data register
            if (Processor.registerDictionary.ContainsKey(operandOne))
            {
                //data register and constant value (mov reg1,2)
                if (DigitsOnlyRegex().IsMatch(operandTwo))
                {
                    short value = short.Parse(operandTwo);
                    Processor.registerDictionary[operandOne] = value;
                    return true;
                }
                //data register and memory location (mov reg1, mem[index])
                if (operandTwo.Contains("mem["))
                {
                    int index = GetMemoryIndex(operandTwo);
                    if (index != -1)
                    {
                        Processor.registerDictionary[operandOne] = Memory.programData[index];
                        return true;
                    }
                    return false;
                }
            }
            //operandOne is a constant value -> error (mov 2, orice)
            if (DigitsOnlyRegex().IsMatch(operandOne))
            {
                return false;
            }
            //operandOne is a memory location
            //TODO: add checks if the memory location is valid
            if (operandOne.Contains("mem["))
            {
                int index = GetMemoryIndex(operandOne);
                if (index != -1)
                {
                    //memory location and constant value
                    if (DigitsOnlyRegex().IsMatch(operandTwo))
                    {
                        Memory.programData[index] = short.Parse(operandTwo);
                        return false;
                    }

                    //memory location and data register
                    if (Processor.registerDictionary.ContainsKey(operandTwo))
                    {
                        Memory.programData[index] = Processor.registerDictionary[operandTwo];
                        return false;
                    }
                }
            }
            //here is an error, not good
            return false;
        }

        public bool Addition(string operandOne, string operandTwo)
        {
            //both operands are data register (add reg1,reg2)
            if (Processor.registerDictionary.ContainsKey(operandOne) && Processor.registerDictionary.ContainsKey(operandTwo))
            {
                Processor.registerDictionary[operandOne] += Processor.registerDictionary[operandTwo];
                return true;
            }
            //first operand is data register
            if (Processor.registerDictionary.ContainsKey(operandOne))
            {
                //data register and constant value (add reg1,2)
                if (DigitsOnlyRegex().IsMatch(operandTwo))
                {
                    short value = short.Parse(operandTwo);
                    Processor.registerDictionary[operandOne] += value;
                    return true;
                }
                //data register and memory location (add reg1, mem[index])
                if (operandTwo.Contains("mem["))
                {
                    int index = GetMemoryIndex(operandTwo);
                    if (index != -1)
                    {
                        Processor.registerDictionary[operandOne] += Memory.programData[index];
                        return true;
                    }
                    return false;
                }
            }
            //operandOne is a constant value -> error (add 2, orice)
            if (DigitsOnlyRegex().IsMatch(operandOne))
            {
                return false;
            }
            //operandOne is a memory location
            //TODO: add checks if the memory location is valid
            if (operandOne.Contains("mem["))
            {
                int index = GetMemoryIndex(operandOne);
                if (index != -1)
                {
                    //memory location and constant value
                    if (DigitsOnlyRegex().IsMatch(operandTwo))
                    {
                        Memory.programData[index] += short.Parse(operandTwo);
                        return false;
                    }

                    //memory location and data register
                    if (Processor.registerDictionary.ContainsKey(operandTwo))
                    {
                        Memory.programData[index] += Processor.registerDictionary[operandTwo];
                        return false;
                    }
                }
            }
            //here is an error, not good
            return false;
        }

        public void And(string reg1, string reg2)
        {
            Processor.registerDictionary[reg1] &= Processor.registerDictionary[reg2];
            throw new NotImplementedException();
        }

        public void Call(string functionName)
        {
            throw new NotImplementedException();
        }

        public void Compare(string reg1, string reg2)
        {
            if (Processor.registerDictionary[reg1] == Processor.registerDictionary[reg2])
            {
                Processor.Equal = true;
                Processor.LessEqual = true;
                Processor.GreaterEqual = true;

                Processor.NotEqual = false;
                Processor.Less = false;
                Processor.Greater = false;
            }
            else if (Processor.registerDictionary[reg1] < Processor.registerDictionary[reg2])
            {
                Processor.Equal = false;
                Processor.LessEqual = false;
                Processor.GreaterEqual = false;

                Processor.NotEqual = true;
                Processor.Less = true;
                Processor.Greater = false;
            }
            else if (Processor.registerDictionary[reg1] > Processor.registerDictionary[reg2])
            {
                Processor.Equal = false;
                Processor.LessEqual = false;
                Processor.GreaterEqual = false;

                Processor.NotEqual = true;
                Processor.Less = false;
                Processor.Greater = true;
            }
            else
            {
                Console.Write("Error: Other case in Compare function");
            }
        }

        public void Compare(string reg1, short val)
        {
            if (Processor.registerDictionary[reg1] == val)
            {
                Processor.Equal = true;
                Processor.LessEqual = true;
                Processor.GreaterEqual = true;

                Processor.NotEqual = false;
                Processor.Less = false;
                Processor.Greater = false;
            }
            else if (Processor.registerDictionary[reg1] < val)
            {
                Processor.Equal = false;
                Processor.LessEqual = false;
                Processor.GreaterEqual = false;

                Processor.NotEqual = true;
                Processor.Less = true;
                Processor.Greater = false;
            }
            else if (Processor.registerDictionary[reg1] > val)
            {
                Processor.Equal = false;
                Processor.LessEqual = false;
                Processor.GreaterEqual = false;

                Processor.NotEqual = true;
                Processor.Less = false;
                Processor.Greater = true;
            }
            else
            {
                Console.Write("Error: Other case in Compare function");
            }
        }

        public void Division(string reg1, string reg2)
        {
            Processor.registerDictionary[reg1] /= Processor.registerDictionary[reg2];
            throw new NotImplementedException();
        }

        public void Division(string reg1, short val)
        {
            Processor.registerDictionary[reg1] /= val;
            throw new NotImplementedException();
        }


        public void Jump(string label)
        {
            for(int index = 0; index < Processor.internalMemory.Length; index++)
                if (Processor.internalMemory[index].Equals(label))
                    Processor.currentInstruction = index;
        }

        public void JumpIfEqual(string label)
        {
            if (Processor.Equal)
                Jump(label);
        }

        public void JumpIfNotEqual(string label)
        {
            if (Processor.NotEqual)
                Jump(label);
        }

        public void JumpIfLessThan(string label)
        {
            if (Processor.Less)
                Jump(label);
        }

        public void JumpIfGreaterThan(string label)
        {
            if (Processor.Greater)
                Jump(label);
        }

        public void JumpIfLessThanOrEqual(string label)
        {
            if (Processor.LessEqual)
                Jump(label);
        }

        public void JumpIfGreaterThanOrEqual(string label)
        {
            if (Processor.GreaterEqual)
                Jump(label);
        }


        public void Multiplication(string reg1, string reg2)
        {
            Processor.registerDictionary[reg1] *= Processor.registerDictionary[reg2];
            throw new NotImplementedException();
        }

        public void Multiplication(string reg1, short val)
        {
            Processor.registerDictionary[reg1] *= val;
            throw new NotImplementedException();
        }

        public void Not(string reg1)
        {
            Processor.registerDictionary[reg1] = ~Processor.registerDictionary[reg1];
            throw new NotImplementedException();
        }

        public void Or(string reg1, string reg2)
        {
            Processor.registerDictionary[reg1] |= Processor.registerDictionary[reg2];
            throw new NotImplementedException();
        }

        public void Pop(string reg)
        {
            throw new NotImplementedException();
        }

        public void Push(string reg)
        {
            if (Processor.currentStackPointer < Processor.maximumSize)
            {
                //Processor.programData[Processor.currentStackPointer] = reg;
                //Processor.currentStackPointer++;
            }
            throw new NotImplementedException();
        }

        public void Return()
        {
            throw new NotImplementedException();
        }

        public void Shift(string reg1, short val)
        {
            Processor.registerDictionary[reg1] <<= val;
            throw new NotImplementedException();
        }

        public void Shift(string reg1, string reg2)
        {
            Processor.registerDictionary[reg1] <<= Processor.registerDictionary[reg2];
            throw new NotImplementedException();
        }

        public void Subtraction(string reg1, string reg2)
        {
            Processor.registerDictionary[reg1] -= Processor.registerDictionary[reg2];
            throw new NotImplementedException();
        }

        public void Subtraction(string reg1, short val)
        {
            Processor.registerDictionary[reg1] -= val;
            throw new NotImplementedException();
        }

        public void Xor(string reg1, string reg2)
        {
            Processor.registerDictionary[reg1] ^= Processor.registerDictionary[reg2];
            throw new NotImplementedException();
        }
        private short GetValue(string operand)
        {
            if (DigitsOnlyRegex().IsMatch(operand))
            {
                return short.Parse(operand);
            }
            //TODO: add checks if the memory location is valid
            if (operand.Contains("mem["))
            {
                int index = GetMemoryIndex(operand);
                if (index != -1)
                {
                    return Memory.programData[index];
                }
            }
            //here is error value -> TODO: add logic here
            return 0;
        }

        private short GetMemoryIndex(string operand)
        {
            int startIndex = operand.IndexOf('[') + 1;
            int endIndex = operand.IndexOf(']');
            string indexStr = operand[startIndex..endIndex];

            if (short.TryParse(indexStr, out short index))
            {
                // TODO: Add checks for valid memory location if needed
                return index;
            }
            //invalid memory index -> TODO: add logic here
            return -1; 
        }

        [GeneratedRegex(@"^\d+$")]
        private static partial Regex DigitsOnlyRegex();
    }
}
