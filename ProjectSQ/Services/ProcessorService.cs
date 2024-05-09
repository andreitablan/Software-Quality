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
            while (Memory.currentInstruction < Memory.instructionNumber && isInputFileGood)
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
                        return true;
                    }

                    //memory location and data register
                    if (Processor.registerDictionary.ContainsKey(operandTwo))
                    {
                        Memory.programData[index] = Processor.registerDictionary[operandTwo];
                        return true;
                    }
                    //memory location and memory location
                    if (operandTwo.Contains("mem["))
                    {
                        int indexTwo = GetMemoryIndex(operandTwo);
                        if(indexTwo != -1)
                        {
                            Memory.programData[index] = Memory.programData[indexTwo];
                            return true;
                        }
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
                        return true;
                    }

                    //memory location and data register
                    if (Processor.registerDictionary.ContainsKey(operandTwo))
                    {
                        Memory.programData[index] += Processor.registerDictionary[operandTwo];
                        return true;
                    }
                    //memory location and memory location
                    if (operandTwo.Contains("mem["))
                    {
                        int indexTwo = GetMemoryIndex(operandTwo);
                        if (indexTwo != -1)
                        {
                            Memory.programData[index] += Memory.programData[indexTwo];
                            return true;
                        }
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

        public bool Compare(string operandOne, string operandTwo)
        {
            //first operand is data register
            if (Processor.registerDictionary.ContainsKey(operandOne))
            {
                //both operands are data register (cmp reg1,reg2)
                if (Processor.registerDictionary.ContainsKey(operandTwo))
                {
                    SetCompareFlags(Processor.registerDictionary[operandOne], Processor.registerDictionary[operandTwo]);
                    return true;
                }
                //data register and constant value (cmp reg1,2)
                if (DigitsOnlyRegex().IsMatch(operandTwo))
                {
                    short value = short.Parse(operandTwo);
                    SetCompareFlags(Processor.registerDictionary[operandOne], value);
                    return true;
                }
                //data register and memory location (cmp reg1, mem[index])
                if (operandTwo.Contains("mem["))
                {
                    int index = GetMemoryIndex(operandTwo);
                    if (index != -1)
                    {
                        SetCompareFlags(Processor.registerDictionary[operandOne], Memory.programData[index]);
                        return true;
                    }
                    return false;
                }
            }
            //operandOne is a constant value -> (cmp 2, orice)
            if (DigitsOnlyRegex().IsMatch(operandOne))
            {
                var value = short.Parse(operandOne);
                //data register and constant value (cmp 2, reg1)
                if (Processor.registerDictionary.ContainsKey(operandTwo))
                {
                    SetCompareFlags(value, Processor.registerDictionary[operandTwo]);
                    return true;
                }

                //data register and memory location (cmp 2, mem[index])
                if (operandTwo.Contains("mem["))
                {
                    int index = GetMemoryIndex(operandTwo);
                    if (index != -1)
                    {
                        SetCompareFlags(value, Memory.programData[index]);
                        return true;
                    }
                    return false;
                }
                //data register and constant value (cmp 2, reg1)

                if (DigitsOnlyRegex().IsMatch(operandTwo))
                {
                    short operandTwoValue = short.Parse(operandTwo);
                    SetCompareFlags(value, operandTwoValue);
                    return true;
                }
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
                        SetCompareFlags(Memory.programData[index], short.Parse(operandTwo));
                        return true;
                    }

                    //memory location and data register
                    if (Processor.registerDictionary.ContainsKey(operandTwo))
                    {
                        SetCompareFlags(Memory.programData[index], Processor.registerDictionary[operandTwo]);
                        return true;
                    }

                    //memory location and memory location
                    if (operandTwo.Contains("mem["))
                    {
                        int indexOperandTwo = GetMemoryIndex(operandTwo);
                        if (indexOperandTwo != -1)
                        {
                            SetCompareFlags(Memory.programData[index], Memory.programData[indexOperandTwo]);
                            return true;
                        }
                        return false;
                    }
                }
            }
            //here is an error, not good
            return false;


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
            for(int index = 0; index < Memory.internalMemory.Length; index++)
                if (Memory.internalMemory[index].Equals(label))
                    Memory.currentInstruction = index;
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
            //Processor.registerDictionary[reg1] = ~Processor.registerDictionary[reg1];
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
            if (Processor.currentStackPointer < Memory.programData.Length)
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

        private void SetCompareFlags(short operandValueOne, short operandValueTwo)
        {
            Processor.Equal = false;
            Processor.LessEqual = false;
            Processor.GreaterEqual = false;

            Processor.NotEqual = false;
            Processor.Less = false;
            Processor.Greater = false;

            if (operandValueOne == operandValueTwo)
            {
                Processor.Equal = true;
                Processor.LessEqual = true;
                Processor.GreaterEqual = true;

                Processor.NotEqual = false;
                Processor.Less = operandValueOne <= operandValueTwo;
                Processor.Greater = operandValueOne >= operandValueTwo;

                return;
            }
            if (operandValueOne < operandValueTwo)
            {
                Processor.Equal = false;
                Processor.LessEqual = true;
                Processor.GreaterEqual = false;

                Processor.NotEqual = true;
                Processor.Less = true;
                Processor.Greater = false;

                return;
            }
            if (operandValueOne > operandValueTwo)
            {
                Processor.Equal = false;
                Processor.LessEqual = false;
                Processor.GreaterEqual = true;

                Processor.NotEqual = true;
                Processor.Less = false;
                Processor.Greater = true;

                return;
            }
            Console.Write("Error: Other case in Compare function");
        }

        [GeneratedRegex(@"^\d+$")]
        private static partial Regex DigitsOnlyRegex();

    }
}
