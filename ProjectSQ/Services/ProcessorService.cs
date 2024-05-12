﻿using ProjectSQ.Interfaces.Processor;
using ProjectSQ.Models;
using System.Reflection.Emit;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ProjectSQ.Services
{
    public partial class ProcessorService : IProcessorService
    {

        public void ExecuteFile()
        {
            bool isInputFileGood = true;
            while (Memory.currentInstruction < Memory.instructionsNumber && isInputFileGood)
            {
                string instruction = Memory.internalMemory[Memory.currentInstruction++];
                string[] words = instruction.Split(' ');
                string operation = words[0];
                switch (operation)
                {
                    case "main":
                        break;
                    case "mov":
                        string[] operands = words[1].Split(",");
                        isInputFileGood = Assignment(operands[0], operands[1]);
                        break;
                    case "add":
                        operands = words[1].Split(",");
                        isInputFileGood = Addition(operands[0], operands[1]);
                        break;
                    case "sub":
                        operands = words[1].Split(",");
                        isInputFileGood = Subtraction(operands[0], operands[1]);
                        break;
                    case "mul":
                        operands = words[1].Split(",");
                        isInputFileGood = Multiplication(operands[0], operands[1]);
                        break;
                    case "div":
                        operands = words[1].Split(",");
                        isInputFileGood = Division(operands[0], operands[1]);
                        break;
                    case "not":
                        isInputFileGood = Not(words[1]);
                        break;
                    case "and":
                        operands = words[1].Split(",");
                        isInputFileGood = And(operands[0], operands[1]);
                        break;
                    case "or":
                        operands = words[1].Split(",");
                        isInputFileGood = Or(operands[0], operands[1]);
                        break;
                    case "xor":
                        operands = words[1].Split(",");
                        isInputFileGood = Xor(operands[0], operands[1]);
                        break;
                    case "shr":
                        operands = words[1].Split(",");
                        isInputFileGood = ShiftLeft(operands[0], operands[1]);
                        break;
                    case "shl":
                        operands = words[1].Split(",");
                        isInputFileGood = ShiftRight(operands[0], operands[1]);
                        break;
                    case "cmp":
                        operands = words[1].Split(",");
                        Compare(operands[0], operands[1]);
                        break;
                    case "jmp":
                        Jump(words[1]);
                        break;
                    case "je":
                        JumpIfEqual(words[1]);
                        break;
                    case "jle":
                        JumpIfLessThanOrEqual(words[1]);
                        break;
                    case "jge":
                        JumpIfGreaterThanOrEqual(words[1]);
                        break;
                    case "jne":
                        JumpIfNotEqual(words[1]);
                        break;
                    case "jl":
                        JumpIfLessThan(words[1]);
                        break;
                    case "jg":
                        JumpIfGreaterThan(words[1]);
                        break;
                    case "label":
                        break;
                    case "push":
                        Push(words[1]);
                        break;
                    case "pop":
                        Pop(words[1]);
                        break;
                    case "call":
                        Call(words[1]);
                        break;
                    case "function":
                        break;
                    case "return":
                        Return();
                        break;
                    case "read":
                        Read(words[1]);
                        break;
                    default:
                        break;
                }
            }
        }

        public bool Assignment(string operandOne, string operandTwo)
        {
            //first operand is data register
            if (Processor.registerDictionary.ContainsKey(operandOne))
            {
                ushort value = GetValue(operandTwo);
                Processor.registerDictionary[operandOne] = value;
                return true;
            }
            //operandOne is a constant value -> error (mov 2, orice)
            if (DigitsOnlyRegex().IsMatch(operandOne))
            {
                return false;
            }
            //operandOne is a memory location
            if (operandOne.Contains("mem["))
            {
                ushort indexOperandOne = GetMemoryIndex(operandOne);

                if (indexOperandOne >= Memory.keyboardBufferIndex)
                {
                    return false;
                }

                ushort valueOperandTwo = GetValue(operandTwo);
                WriteValueToMemory(indexOperandOne, valueOperandTwo);
                return true;
            }
            //here is an error, not good
            return false;
        }
        public bool Addition(string operandOne, string operandTwo)
        {
            //first operand is data register
            if (Processor.registerDictionary.ContainsKey(operandOne))
            {
                ushort value = GetValue(operandTwo);
                Processor.registerDictionary[operandOne] += value;
                return true;
            }
            //operandOne is a constant value -> error (add 2, orice)
            if (DigitsOnlyRegex().IsMatch(operandOne))
            {
                return false;
            }
            //operandOne is a memory location
            if (operandOne.Contains("mem["))
            {
                ushort indexOperandOne = GetMemoryIndex(operandOne);
                ushort valueOperandOne = ReadValueFromMemory(indexOperandOne);
                ushort valueOperandTwo = GetValue(operandTwo);
                ushort result = (ushort)(valueOperandOne + valueOperandTwo);

                WriteValueToMemory(indexOperandOne, result);
                return true;
            }
            //here is an error, not good
            return false;
        }
        public bool Subtraction(string operandOne, string operandTwo)
        {
            //first operand is data register
            if (Processor.registerDictionary.ContainsKey(operandOne))
            {
                ushort value = GetValue(operandTwo);
                Processor.registerDictionary[operandOne] -= value;
                return true;
            }
            //operandOne is a constant value -> error (sub 2, orice)
            if (DigitsOnlyRegex().IsMatch(operandOne))
            {
                return false;
            }
            //operandOne is a memory location
            if (operandOne.Contains("mem["))
            {
                ushort indexOperandOne = GetMemoryIndex(operandOne);

                ushort valueOperandOne = ReadValueFromMemory(indexOperandOne);
                ushort valueOperandTwo = GetValue(operandTwo);
                ushort result = (ushort)(valueOperandOne - valueOperandTwo);

                WriteValueToMemory(indexOperandOne, result);
                return true;
            }
            //here is an error, not good
            return false;
        }
        public bool Multiplication(string operandOne, string operandTwo)
        {
            //first operand is data register
            if (Processor.registerDictionary.ContainsKey(operandOne))
            {
                ushort value = GetValue(operandTwo);
                Processor.registerDictionary[operandOne] *= value;
                return true;
            }
            //operandOne is a constant value -> error (mul 2, orice)
            if (DigitsOnlyRegex().IsMatch(operandOne))
            {
                return false;
            }
            //operandOne is a memory location
            if (operandOne.Contains("mem["))
            {
                ushort indexOperandOne = GetMemoryIndex(operandOne);
                ushort valueOperandOne = ReadValueFromMemory(indexOperandOne);
                ushort valueOperandTwo = GetValue(operandTwo);
                ushort result = (ushort)(valueOperandOne * valueOperandTwo);

                WriteValueToMemory(indexOperandOne, result);
                return true;
            }
            //here is an error, not good
            return false;
        }
        public bool Division(string operandOne, string operandTwo)
        {
            //first operand is data register
            if (Processor.registerDictionary.ContainsKey(operandOne))
            {
                ushort value = GetValue(operandTwo);
                Processor.registerDictionary[operandOne] /= value;
                return true;
            }
            //operandOne is a constant value -> error (div 2, orice)
            if (DigitsOnlyRegex().IsMatch(operandOne))
            {
                return false;
            }
            //operandOne is a memory location
            if (operandOne.Contains("mem["))
            {
                ushort indexOperandOne = GetMemoryIndex(operandOne);
                ushort valueOperandOne = ReadValueFromMemory(indexOperandOne);
                ushort valueOperandTwo = GetValue(operandTwo);
                ushort result = (ushort)(valueOperandOne / valueOperandTwo);

                WriteValueToMemory(indexOperandOne, result);
                return true;
            }
            //here is an error, not good
            return false;
        }
        public bool Not(string operandOne)
        {
            //register
            if (Processor.registerDictionary.ContainsKey(operandOne))
            {
                Processor.registerDictionary[operandOne] = (ushort)~Processor.registerDictionary[operandOne];
                return true;
            }
            //constant value -> you can not do that
            if (DigitsOnlyRegex().IsMatch(operandOne))
            {
                return false;
            }
            if (operandOne.Contains("mem["))
            {
                ushort index = GetMemoryIndex(operandOne);
                Memory.programData[index] = (byte)~Memory.programData[index];
                Memory.programData[index + 1] = (byte)~Memory.programData[index + 1];
                return true;
            }
            return false;
        }
        public bool And(string operandOne, string operandTwo)
        {
            //first operand is data register
            if (Processor.registerDictionary.ContainsKey(operandOne))
            {
                ushort value = GetValue(operandTwo);
                Processor.registerDictionary[operandOne] &= value;
                return true;
            }
            //operandOne is a constant value -> error (and 2, orice)
            if (DigitsOnlyRegex().IsMatch(operandOne))
            {
                return false;
            }
            //operandOne is a memory location
            if (operandOne.Contains("mem["))
            {
                ushort indexOperandOne = GetMemoryIndex(operandOne);
                ushort valueOperandOne = ReadValueFromMemory(indexOperandOne);
                ushort valueOperandTwo = GetValue(operandTwo);
                ushort result = (ushort)(valueOperandOne & valueOperandTwo);

                WriteValueToMemory(indexOperandOne, result);
                return true;
            }
            //here is an error, not good
            return false;
        }
        public bool Or(string operandOne, string operandTwo)
        {
            //first operand is data register
            if (Processor.registerDictionary.ContainsKey(operandOne))
            {
                ushort value = GetValue(operandTwo);
                Processor.registerDictionary[operandOne] |= value;
                return true;
            }
            //operandOne is a constant value -> error (or 2, orice)
            if (DigitsOnlyRegex().IsMatch(operandOne))
            {
                return false;
            }
            //operandOne is a memory location
            if (operandOne.Contains("mem["))
            {
                ushort indexOperandOne = GetMemoryIndex(operandOne);
                ushort valueOperandOne = ReadValueFromMemory(indexOperandOne);
                ushort valueOperandTwo = GetValue(operandTwo);
                ushort result = (ushort)(valueOperandOne | valueOperandTwo);

                WriteValueToMemory(indexOperandOne, result);
                return true;
            }
            //here is an error, not good
            return false;
        }
        public bool Xor(string operandOne, string operandTwo)
        {
            //first operand is data register
            if (Processor.registerDictionary.ContainsKey(operandOne))
            {
                ushort value = GetValue(operandTwo);
                Processor.registerDictionary[operandOne] ^= value;
                return true;
            }
            //operandOne is a constant value -> error (xor 2, orice)
            if (DigitsOnlyRegex().IsMatch(operandOne))
            {
                return false;
            }
            //operandOne is a memory location
            if (operandOne.Contains("mem["))
            {
                ushort indexOperandOne = GetMemoryIndex(operandOne);
                ushort valueOperandOne = ReadValueFromMemory(indexOperandOne);
                ushort valueOperandTwo = GetValue(operandTwo);
                ushort result = (ushort)(valueOperandOne ^ valueOperandTwo);

                WriteValueToMemory(indexOperandOne, result);
                return true;
            }
            //here is an error, not good
            return false;
        }
        public bool ShiftLeft(string operandOne, string operandTwo)
        {
            //first operand is data register
            if (Processor.registerDictionary.ContainsKey(operandOne))
            {
                ushort value = GetValue(operandTwo);
                Processor.registerDictionary[operandOne] <<= value;
                return true;
            }
            //operandOne is a constant value -> error (shl 2, orice)
            if (DigitsOnlyRegex().IsMatch(operandOne))
            {
                return false;
            }
            //operandOne is a memory location
            if (operandOne.Contains("mem["))
            {
                ushort indexOperandOne = GetMemoryIndex(operandOne);
                ushort valueOperandOne = ReadValueFromMemory(indexOperandOne);
                ushort valueOperandTwo = GetValue(operandTwo);
                ushort result = (ushort)(valueOperandOne << valueOperandTwo);

                WriteValueToMemory(indexOperandOne, result);
                return true;
            }
            //here is an error, not good
            return false;
        }
        public bool ShiftRight(string operandOne, string operandTwo)
        {
            //first operand is data register
            if (Processor.registerDictionary.ContainsKey(operandOne))
            {
                ushort value = GetValue(operandTwo);
                Processor.registerDictionary[operandOne] >>= value;
                return true;
            }
            //operandOne is a constant value -> error (shr 2, orice)
            if (DigitsOnlyRegex().IsMatch(operandOne))
            {
                return false;
            }
            //operandOne is a memory location
            if (operandOne.Contains("mem["))
            {
                ushort indexOperandOne = GetMemoryIndex(operandOne);
                ushort valueOperandOne = ReadValueFromMemory(indexOperandOne);
                ushort valueOperandTwo = GetValue(operandTwo);
                ushort result = (ushort)(valueOperandOne >> valueOperandTwo);

                WriteValueToMemory(indexOperandOne, result);
                return true;
            }
            //here is an error, not good
            return false;
        }
        public bool Compare(string operandOne, string operandTwo)
        {
            //first operand is data register
            if (Processor.registerDictionary.ContainsKey(operandOne))
            {
                ushort valueOperandTwo = GetValue(operandTwo);
                SetCompareFlags(Processor.registerDictionary[operandOne], valueOperandTwo);
                return true;
            }
            //operandOne is a constant value -> (cmp 2, orice)
            if (DigitsOnlyRegex().IsMatch(operandOne))
            {
                ushort valueOperandOne = ushort.Parse(operandOne);
                ushort valueOperandTwo = GetValue(operandTwo);
                SetCompareFlags(valueOperandOne, valueOperandTwo);
                return true;
            }
            //operandOne is a memory location
            //TODO: add checks if the memory location is valid
            if (operandOne.Contains("mem["))
            {
                int indexOperandOne = GetMemoryIndex(operandOne);
                ushort valueOperandOne = ReadValueFromMemory(indexOperandOne);
                ushort valueOperandTwo = GetValue(operandTwo);
                SetCompareFlags(valueOperandOne, valueOperandTwo);
                return true;
            }
            //here is an error, not good
            return false;
        }
        public void Jump(string label)
        {
            for (ushort index = 0; index < Memory.internalMemory.Length; index++)
                if (Memory.internalMemory[index].Split(' ')[1] == label && Memory.internalMemory[index].Split(' ')[0] == "label")
                {
                    Memory.currentInstruction = index;
                    break;
                }
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

        public void Pop(string operand)
        {
            Processor.StackPointer -= 2;
            ushort valueStack = ReadValueFromMemory(Processor.StackPointer);
            Memory.programData[Processor.StackPointer] = 0;
            Memory.programData[Processor.StackPointer + 1] = 0;
            
            Processor.registerDictionary[operand] = valueStack;
        }

        public void Push(string operand)
        {
            ushort val = GetValue(operand);
            WriteValueToMemory(Processor.StackPointer, val);
            Processor.StackPointer += 2;
        }
        public void Call(string functionName)
        {
            WriteValueToMemory(Processor.StackPointer, Memory.currentInstruction);
            Processor.StackPointer += 2;
            for (ushort index = 0; index < Memory.internalMemory.Length; index++)
                if (Memory.internalMemory[index].Split(' ')[1] == functionName && Memory.internalMemory[index].Split(' ')[0] == "function")
                {
                    Memory.currentInstruction = index;
                    break;
                }

        }
        public void Return()
        {
            Processor.StackPointer -= 2;
            Memory.currentInstruction = ReadValueFromMemory(Processor.StackPointer);
            Memory.programData[Processor.StackPointer] = 0;
            Memory.programData[Processor.StackPointer + 1] = 0;
            if (Memory.currentInstruction < Memory.instructionsNumber)
                Memory.currentInstruction++;
        }
        public void Read(string operand)
        {
            ushort result = 0;
            while ((char)Memory.programData[Memory.currentIndexMemoryVideo] != ' ')
            {
                result = (ushort)(result * 10 + (ushort)((char)(Memory.programData[Memory.currentIndexMemoryVideo])));
                Memory.currentIndexMemoryVideo++;
            }
            while ((char)Memory.programData[Memory.currentIndexMemoryVideo] == ' ')
            {
                Memory.currentIndexMemoryVideo++;
            }
            Processor.registerDictionary[operand] = result;
        }
        public ResultRegisters LoadResultRegisters()
        {
            ResultRegisters resultRegisters = new ResultRegisters();
            resultRegisters.Reg1 = Processor.registerDictionary["reg1"];
            resultRegisters.Reg2 = Processor.registerDictionary["reg2"];
            resultRegisters.Reg3 = Processor.registerDictionary["reg3"];
            resultRegisters.Reg4 = Processor.registerDictionary["reg4"];
            resultRegisters.Reg5 = Processor.registerDictionary["reg5"];
            resultRegisters.Reg6 = Processor.registerDictionary["reg6"];
            resultRegisters.Reg7 = Processor.registerDictionary["reg7"];
            resultRegisters.Reg8 = Processor.registerDictionary["reg8"];
            return resultRegisters;
        }
        public void ResetData()
        {
            foreach (var kvp in Processor.registerDictionary)
                Processor.registerDictionary[kvp.Key] = 0;
            Memory.currentInstruction = 0;
            for (int i = 0; i < Memory.programData.Length; i++)
                Memory.programData[i] = 0;
            Processor.StackPointer = Memory.startStack;
        }

        public void WriteValueToKeyboardBuffer(ushort value)
        {
            byte lowByte = (byte)(value & 0xFF);
            Memory.programData[Memory.keyboardBufferIndex] = lowByte;
            Memory.isKeyboardBufferChanged = true;
        }

        public void WriteToVideoMemory()
        {
            while (true)
            {
                if (Memory.isKeyboardBufferChanged)
                {
                    Memory.programData[Memory.lastIndexOfMemoryVideo] = Memory.programData[Memory.keyboardBufferIndex];

                    if (Memory.lastIndexOfMemoryVideo < Memory.maxIndexOfMemoryVideo)
                    {
                        Memory.lastIndexOfMemoryVideo++;
                    }

                    Memory.isKeyboardBufferChanged = false;
                }
            }
        }

        public string ReadFromVideoMemory()
        {
            var result = "";

            for (int i = Memory.firstVideoMemoryIndex; i < Memory.lastIndexOfMemoryVideo; i++)
            {
                result += ((char)Memory.programData[i]).ToString();
            }

            return result;
        }

        public void RemoveFromVideoMemory()
        {
            Memory.lastIndexOfMemoryVideo--;

            Memory.programData[Memory.lastIndexOfMemoryVideo] = 0;
        }

        private static void WriteValueToMemory(int indexOperandOne, ushort valueOperandTwo)
        {
            byte highByte = (byte)(valueOperandTwo >> 8);
            byte lowByte = (byte)(valueOperandTwo & 0xFF);

            // write to two consecutive addresses
            Memory.programData[indexOperandOne] = lowByte;
            Memory.programData[indexOperandOne + 1] = highByte;
        }
        private static ushort ReadValueFromMemory(int index)
        {
            byte lowByte = Memory.programData[index];
            byte highByte = Memory.programData[index + 1];
            ushort result = (ushort)((highByte << 8) | lowByte);
            return result;
        }


        private static ushort GetValue(string operand)
        {
            //second operand is a data register
            if (Processor.registerDictionary.TryGetValue(operand, out ushort value))
            {
                return value;
            }
            //second operand is a constant value
            if (DigitsOnlyRegex().IsMatch(operand))
            {
                return ushort.Parse(operand);
            }

            //second operand is a memory location -> need to read two consecutive addresses
            int index = GetMemoryIndex(operand);
            ushort result = ReadValueFromMemory(index);
            return result;

            //NOTE: case if the memory location is invalid
            //now, we consider the input file does not have errors
        }

        private static ushort GetMemoryIndex(string operand)
        {
            int startIndex = operand.IndexOf('[') + 1;
            int endIndex = operand.IndexOf(']');
            string indexStr = operand[startIndex..endIndex];

            //address specified by a constant value
            if (ushort.TryParse(indexStr, out ushort index))
            {
                return index;
            }

            //address specified by a data register
            Processor.registerDictionary.TryGetValue(indexStr, out index);
            return index;

            //NOTE: case if the memory location is invalid
            //now, we consider the input file does not have errors
        }

        private static void SetCompareFlags(ushort operandValueOne, ushort operandValueTwo)
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
