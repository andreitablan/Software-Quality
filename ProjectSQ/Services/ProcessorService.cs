using Microsoft.AspNetCore.SignalR;
using ProjectSQ.Interfaces.Memory;
using ProjectSQ.Interfaces.Processor;
using ProjectSQ.Models;
using System.Diagnostics;
using System.Text.RegularExpressions;
using ProjectSQ.Models.Assertions;
using System;

namespace ProjectSQ.Services
{
    public partial class ProcessorService : IProcessorService
    {
        private readonly IHubContext<RealTimeHub> hubContext;
        private readonly IMemoryService memoryService;

        public ProcessorService(IHubContext<RealTimeHub> hubContext, IMemoryService memoryService)
        {
            this.hubContext = hubContext;
            this.memoryService = memoryService;
        }

        public void ExecuteFile()
        {
            //preconditions
            //processor
            CustomAssert.IsTrue(Processor.registerDictionary != null, "Precondition failed: Processor.registerDictionary is not initialized");
            string[] expectedRegisters = { "reg1", "reg2", "reg3", "reg4", "reg5", "reg6", "reg7", "reg8" };
            foreach (string registerName in expectedRegisters)
            {
                if (!Processor.registerDictionary.ContainsKey(registerName))
                {
                    CustomAssert.IsTrue(false, $"Precondition failed: Processor.registerDictionary does not contain the key {registerName}");
                }
            }
            CustomAssert.IsTrue(Processor.StackPointer == Memory.startStack, "Precondition failed: Processor.StackPointer must be equal to Memory.startStack");
            CustomAssert.IsTrue(Processor.Greater == false, "Precondition failed: Greater flag is not set on false");
            CustomAssert.IsTrue(Processor.Less == false, "Precondition failed: Greater flag is not set on false");
            CustomAssert.IsTrue(Processor.NotEqual == false, "Precondition failed: Greater flag is not set on false");
            CustomAssert.IsTrue(Processor.LessEqual == false, "Precondition failed: Greater flag is not set on false");
            CustomAssert.IsTrue(Processor.GreaterEqual == false, "Precondition failed: Greater flag is not set on false");
            CustomAssert.IsTrue(Processor.Equal == false, "Precondition failed: Greater flag is not set on false");

            //memory
            CustomAssert.IsTrue(Memory.internalMemory != null, "Precondition failed: Memory.internalMemory is not initialized");
            CustomAssert.IsTrue(Memory.programData != null, "Precondition failed: Memory.programData is not initialized");
            CustomAssert.IsTrue(Memory.instructionsNumber > 0, "Precondition failed: Memory.instructionsNumber must be greater than zero");
            CustomAssert.IsTrue(Memory.currentInstruction >= 0 && Memory.currentInstruction < Memory.instructionsNumber, "Precondition failed: Memory.currentInstruction is out of bounds");
            
           
            bool isInputFileGood = true;
            while (Memory.currentInstruction < Memory.instructionsNumber && isInputFileGood)
            {
                //invariants
                CustomAssert.IsTrue(Processor.StackPointer >= Memory.startStack,
                    "Invariant failed: Processor.StackPointer must be greater than or equal to Memory.startStack");
                CustomAssert.IsTrue(Processor.StackPointer < Memory.endStack,
                    "Invariant failed: Processor.StackPointer must be lower than Memory.endStack");
                CustomAssert.IsTrue(
                    Memory.currentInstruction >= 0 && Memory.currentInstruction < Memory.instructionsNumber,
                    "Invariant failed: Memory.currentInstruction is out of bounds");

                string instruction = Memory.internalMemory[Memory.currentInstruction];
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
                        isInputFileGood = ShiftRight(operands[0], operands[1]);
                        break;
                    case "shl":
                        operands = words[1].Split(",");
                        isInputFileGood = ShiftLeft(operands[0], operands[1]);
                        break;
                    case "cmp":
                        operands = words[1].Split(",");
                        Compare(operands[0], operands[1]);
                        break;
                    case "jmp":
                        isInputFileGood = Jump(words[1]);
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
                }
                Memory.currentInstruction++;
            }
            var executionResult = new ExecutionResult();
            executionResult.Registers = LoadResultRegisters();
            executionResult.Memory = memoryService.LoadMemoryData();
            this.hubContext.Clients.All.SendAsync("ReceiveExecutionResult", executionResult);

            //postconditions
            CustomAssert.IsTrue(Memory.currentInstruction <= Memory.instructionsNumber + 1, "Postcondition failed: Execution did not complete successfully");
            CustomAssert.IsTrue(Processor.StackPointer >= Memory.startStack, "Postcondition failed: Processor.StackPointer must be greater than or equal to Memory.startStack");
            CustomAssert.IsTrue(Processor.StackPointer < Memory.endStack, "Postcondition failed: Processor.StackPointer must be lower than to Memory.startStack");

        }

        public bool Assignment(string operandOne, string operandTwo)
        {
            //preconditions
            //processor
            CustomAssert.IsTrue(Processor.registerDictionary != null, "Precondition failed: Processor.registerDictionary is not initialized");
            string[] expectedRegisters = { "reg1", "reg2", "reg3", "reg4", "reg5", "reg6", "reg7", "reg8" };
            foreach (string registerName in expectedRegisters)
            {
                if (!Processor.registerDictionary.ContainsKey(registerName))
                {
                    CustomAssert.IsTrue(false, $"Precondition failed: Processor.registerDictionary does not contain the key {registerName}");
                }
            }
            //memory
            CustomAssert.IsTrue(Memory.programData != null, "Precondition failed: Memory.programData is not initialized");

            //first operand is data register
            if (Processor.registerDictionary.ContainsKey(operandOne))
            {
                ushort value = GetValue(operandTwo);
                Processor.registerDictionary[operandOne] = value;

                CustomAssert.IsTrue(Processor.registerDictionary[operandOne] == value, "Postcondition failed: The register was not set");
                return true;
            }
            //operandOne is a constant value -> error (mov 2, orice)
            if (DigitsOnlyRegex().IsMatch(operandOne))
            {
                CustomAssert.IsTrue(true, "No post condition here");
                return false;
            }
            //operandOne is a memory location
            if (operandOne.Contains("mem["))
            {
                ushort indexOperandOne = GetMemoryIndex(operandOne);

                if (indexOperandOne >= Memory.keyboardBufferIndex)
                {
                    CustomAssert.IsTrue(true, "No post condition here");
                    return false;
                }

                ushort valueOperandTwo = GetValue(operandTwo);
                WriteValueToMemory(indexOperandOne, valueOperandTwo);

                CustomAssert.IsTrue(ReadValueFromMemory(indexOperandOne) == valueOperandTwo, "Postcondition failed: The memory location was not set");
                return true;
            }

            CustomAssert.IsTrue(true, "No post condition here");
            return false;
        }
        public bool Addition(string operandOne, string operandTwo)
        {
            //preconditions
            //processor
            CustomAssert.IsTrue(Processor.registerDictionary != null, "Precondition failed: Processor.registerDictionary is not initialized");
            string[] expectedRegisters = { "reg1", "reg2", "reg3", "reg4", "reg5", "reg6", "reg7", "reg8" };
            foreach (string registerName in expectedRegisters)
            {
                if (!Processor.registerDictionary.ContainsKey(registerName))
                {
                    CustomAssert.IsTrue(false, $"Precondition failed: Processor.registerDictionary does not contain the key {registerName}");
                }
            }
            //memory
            CustomAssert.IsTrue(Memory.programData != null, "Precondition failed: Memory.programData is not initialized");


            //first operand is data register
            if (Processor.registerDictionary.ContainsKey(operandOne))
            {
                ushort value = GetValue(operandTwo);
                ushort oldValue = Processor.registerDictionary[operandOne];
                Processor.registerDictionary[operandOne] += value;

                CustomAssert.IsTrue(Processor.registerDictionary[operandOne] - value == oldValue, "Postcondition failed: The register was not added");
                return true;
            }
            //operandOne is a constant value -> error (add 2, orice)
            if (DigitsOnlyRegex().IsMatch(operandOne))
            {
                CustomAssert.IsTrue(true, "No post condition here");
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

                CustomAssert.IsTrue(ReadValueFromMemory(indexOperandOne) == result, "Postcondition failed: The memory location was not added");
                return true;
            }

            CustomAssert.IsTrue(true, "No post condition here");
            return false;
        }
        public bool Subtraction(string operandOne, string operandTwo)
        {
            //preconditions
            //processor
            CustomAssert.IsTrue(Processor.registerDictionary != null, "Precondition failed: Processor.registerDictionary is not initialized");
            string[] expectedRegisters = { "reg1", "reg2", "reg3", "reg4", "reg5", "reg6", "reg7", "reg8" };
            foreach (string registerName in expectedRegisters)
            {
                if (!Processor.registerDictionary.ContainsKey(registerName))
                {
                    CustomAssert.IsTrue(false, $"Precondition failed: Processor.registerDictionary does not contain the key {registerName}");
                }
            }
            //memory
            CustomAssert.IsTrue(Memory.programData != null, "Precondition failed: Memory.programData is not initialized");

            //first operand is data register
            if (Processor.registerDictionary.ContainsKey(operandOne))
            {
                ushort value = GetValue(operandTwo);
                ushort oldValue = Processor.registerDictionary[operandOne];
                Processor.registerDictionary[operandOne] -= value;

                CustomAssert.IsTrue(Processor.registerDictionary[operandOne] == oldValue - value, "Postcondition failed: The register was not subtracted");
                return true;
            }
            //operandOne is a constant value -> error (sub 2, orice)
            if (DigitsOnlyRegex().IsMatch(operandOne))
            {
                CustomAssert.IsTrue(true, "No post condition here");
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
                
                CustomAssert.IsTrue(ReadValueFromMemory(indexOperandOne) == result, "Postcondition failed: The memory location was not subtracted");
                return true;
            }

            CustomAssert.IsTrue(true, "No post condition here");
            return false;
        }
        public bool Multiplication(string operandOne, string operandTwo)
        {
            //preconditions
            //processor
            CustomAssert.IsTrue(Processor.registerDictionary != null, "Precondition failed: Processor.registerDictionary is not initialized");
            string[] expectedRegisters = { "reg1", "reg2", "reg3", "reg4", "reg5", "reg6", "reg7", "reg8" };
            foreach (string registerName in expectedRegisters)
            {
                if (!Processor.registerDictionary.ContainsKey(registerName))
                {
                    CustomAssert.IsTrue(false, $"Precondition failed: Processor.registerDictionary does not contain the key {registerName}");
                }
            }
            //memory
            CustomAssert.IsTrue(Memory.programData != null, "Precondition failed: Memory.programData is not initialized");


            //first operand is data register
            if (Processor.registerDictionary.ContainsKey(operandOne))
            {
                ushort value = GetValue(operandTwo);
                ushort oldValue = Processor.registerDictionary[operandOne];
                Processor.registerDictionary[operandOne] *= value;

                CustomAssert.IsTrue(Processor.registerDictionary[operandOne] == oldValue*value, "Postcondition failed: The register was not multiplied");
                return true;
            }
            //operandOne is a constant value -> error (mul 2, orice)
            if (DigitsOnlyRegex().IsMatch(operandOne))
            {
                CustomAssert.IsTrue(true, "No post condition here");
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
                
                CustomAssert.IsTrue(ReadValueFromMemory(indexOperandOne) == result, "Postcondition failed: The memory location was not multiplied");
                return true;
            }

            CustomAssert.IsTrue(true, "No post condition here");
            return false;
        }
        public bool Division(string operandOne, string operandTwo)
        {
            //preconditions
            //processor
            CustomAssert.IsTrue(Processor.registerDictionary != null, "Precondition failed: Processor.registerDictionary is not initialized");
            string[] expectedRegisters = { "reg1", "reg2", "reg3", "reg4", "reg5", "reg6", "reg7", "reg8" };
            foreach (string registerName in expectedRegisters)
            {
                if (!Processor.registerDictionary.ContainsKey(registerName))
                {
                    CustomAssert.IsTrue(false, $"Precondition failed: Processor.registerDictionary does not contain the key {registerName}");
                }
            }
            //memory
            CustomAssert.IsTrue(Memory.programData != null, "Precondition failed: Memory.programData is not initialized");

            //first operand is data register
            if (Processor.registerDictionary.ContainsKey(operandOne))
            {
                ushort value = GetValue(operandTwo);
                ushort oldValue = Processor.registerDictionary[operandOne];
                Processor.registerDictionary[operandOne] /= value;

                CustomAssert.IsTrue(Processor.registerDictionary[operandOne] == oldValue/value, "Postcondition failed: The register was not divided");
                return true;
            }
            //operandOne is a constant value -> error (div 2, orice)
            if (DigitsOnlyRegex().IsMatch(operandOne))
            {
                CustomAssert.IsTrue(true, "No post condition here");
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
                
                CustomAssert.IsTrue(ReadValueFromMemory(indexOperandOne) == result, "Postcondition failed: The memory location was not divided");
                return true;
            }

            CustomAssert.IsTrue(true, "No post condition here");
            return false;
        }
        public bool Not(string operandOne)
        {
            //preconditions
            //processor
            CustomAssert.IsTrue(Processor.registerDictionary != null, "Precondition failed: Processor.registerDictionary is not initialized");
            string[] expectedRegisters = { "reg1", "reg2", "reg3", "reg4", "reg5", "reg6", "reg7", "reg8" };
            foreach (string registerName in expectedRegisters)
            {
                if (!Processor.registerDictionary.ContainsKey(registerName))
                {
                    CustomAssert.IsTrue(false, $"Precondition failed: Processor.registerDictionary does not contain the key {registerName}");
                }
            }
            //memory
            CustomAssert.IsTrue(Memory.programData != null, "Precondition failed: Memory.programData is not initialized");

            //register
            if (Processor.registerDictionary.ContainsKey(operandOne))
            {
                ushort oldValue = Processor.registerDictionary[operandOne];
                Processor.registerDictionary[operandOne] = (ushort)~Processor.registerDictionary[operandOne];

                CustomAssert.IsTrue(oldValue == (ushort)~Processor.registerDictionary[operandOne], "Postcondition failed: The register was not negated");
                return true;
            }
            //constant value -> you can not do that
            if (DigitsOnlyRegex().IsMatch(operandOne))
            {
                CustomAssert.IsTrue(true, "No post condition here");
                return false;
            }
            if (operandOne.Contains("mem["))
            {
                ushort index = GetMemoryIndex(operandOne);
                byte oldValue = Memory.programData[index];
                Memory.programData[index] = (byte)~Memory.programData[index];
                Memory.programData[index + 1] = (byte)~Memory.programData[index + 1];

                CustomAssert.IsTrue(oldValue == (byte)~Memory.programData[index], "Postcondition failed: The memory location was not negated");
                return true;
            }

            CustomAssert.IsTrue(true, "No post condition here");
            return false;
        }
        public bool And(string operandOne, string operandTwo)
        {
            //preconditions
            //processor
            CustomAssert.IsTrue(Processor.registerDictionary != null, "Precondition failed: Processor.registerDictionary is not initialized");
            string[] expectedRegisters = { "reg1", "reg2", "reg3", "reg4", "reg5", "reg6", "reg7", "reg8" };
            foreach (string registerName in expectedRegisters)
            {
                if (!Processor.registerDictionary.ContainsKey(registerName))
                {
                    CustomAssert.IsTrue(false, $"Precondition failed: Processor.registerDictionary does not contain the key {registerName}");
                }
            }
            //memory
            CustomAssert.IsTrue(Memory.programData != null, "Precondition failed: Memory.programData is not initialized");

            //first operand is data register
            if (Processor.registerDictionary.ContainsKey(operandOne))
            {
                ushort value = GetValue(operandTwo);
                ushort oldValue = Processor.registerDictionary[operandOne];
                Processor.registerDictionary[operandOne] &= value;

                CustomAssert.IsTrue(Processor.registerDictionary[operandOne] == (ushort)(oldValue & value), "Postcondition failed: The register was not anded");
                return true;
            }
            //operandOne is a constant value -> error (and 2, orice)
            if (DigitsOnlyRegex().IsMatch(operandOne))
            {
                CustomAssert.IsTrue(true, "No post condition here");
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

                CustomAssert.IsTrue(ReadValueFromMemory(indexOperandOne) == result, "Postcondition failed: The memory location was not anded");
                return true;
            }
            return false;
        }
        public bool Or(string operandOne, string operandTwo)
        {
            //preconditions
            //processor
            CustomAssert.IsTrue(Processor.registerDictionary != null, "Precondition failed: Processor.registerDictionary is not initialized");
            string[] expectedRegisters = { "reg1", "reg2", "reg3", "reg4", "reg5", "reg6", "reg7", "reg8" };
            foreach (string registerName in expectedRegisters)
            {
                if (!Processor.registerDictionary.ContainsKey(registerName))
                {
                    CustomAssert.IsTrue(false, $"Precondition failed: Processor.registerDictionary does not contain the key {registerName}");
                }
            }
            //memory
            CustomAssert.IsTrue(Memory.programData != null, "Precondition failed: Memory.programData is not initialized");

            //first operand is data register
            if (Processor.registerDictionary.ContainsKey(operandOne))
            {
                ushort value = GetValue(operandTwo);
                ushort oldValue = Processor.registerDictionary[operandOne];
                Processor.registerDictionary[operandOne] |= value;

                CustomAssert.IsTrue(Processor.registerDictionary[operandOne] == (ushort)(oldValue | value), "Postcondition failed: The register was not ored");
                return true;
            }
            //operandOne is a constant value -> error (or 2, orice)
            if (DigitsOnlyRegex().IsMatch(operandOne))
            {
                CustomAssert.IsTrue(true, "No post condition here");
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

                CustomAssert.IsTrue(ReadValueFromMemory(indexOperandOne) == result, "Postcondition failed: The memory location was not ored");
                return true;
            }

            CustomAssert.IsTrue(true, "No post condition here");
            return false;
        }
        public bool Xor(string operandOne, string operandTwo)
        {
            //preconditions
            //processor
            CustomAssert.IsTrue(Processor.registerDictionary != null, "Precondition failed: Processor.registerDictionary is not initialized");
            string[] expectedRegisters = { "reg1", "reg2", "reg3", "reg4", "reg5", "reg6", "reg7", "reg8" };
            foreach (string registerName in expectedRegisters)
            {
                if (!Processor.registerDictionary.ContainsKey(registerName))
                {
                    CustomAssert.IsTrue(false, $"Precondition failed: Processor.registerDictionary does not contain the key {registerName}");
                }
            }
            //memory
            CustomAssert.IsTrue(Memory.programData != null, "Precondition failed: Memory.programData is not initialized");

            //first operand is data register
            if (Processor.registerDictionary.ContainsKey(operandOne))
            {
                ushort value = GetValue(operandTwo);
                ushort oldValue = Processor.registerDictionary[operandOne];
                Processor.registerDictionary[operandOne] ^= value;

                CustomAssert.IsTrue(Processor.registerDictionary[operandOne] == (ushort)(oldValue ^ value), "Postcondition failed: The register was not xored");
                return true;
            }
            //operandOne is a constant value -> error (xor 2, orice)
            if (DigitsOnlyRegex().IsMatch(operandOne))
            {
                CustomAssert.IsTrue(true, "No post condition here");
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

                CustomAssert.IsTrue(ReadValueFromMemory(indexOperandOne) == result, "Postcondition failed: The memory location was not xored");
                return true;
            }
            return false;
        }
        public bool ShiftLeft(string operandOne, string operandTwo)
        {
            //preconditions
            //processor
            CustomAssert.IsTrue(Processor.registerDictionary != null, "Precondition failed: Processor.registerDictionary is not initialized");
            string[] expectedRegisters = { "reg1", "reg2", "reg3", "reg4", "reg5", "reg6", "reg7", "reg8" };
            foreach (string registerName in expectedRegisters)
            {
                if (!Processor.registerDictionary.ContainsKey(registerName))
                {
                    CustomAssert.IsTrue(false, $"Precondition failed: Processor.registerDictionary does not contain the key {registerName}");
                }
            }
            //memory
            CustomAssert.IsTrue(Memory.programData != null, "Precondition failed: Memory.programData is not initialized");

            //first operand is data register
            if (Processor.registerDictionary.ContainsKey(operandOne))
            {
                ushort value = GetValue(operandTwo);
                ushort oldValue = Processor.registerDictionary[operandOne];
                Processor.registerDictionary[operandOne] <<= value;

                CustomAssert.IsTrue(Processor.registerDictionary[operandOne] == (ushort)(oldValue << value), "Postcondition failed: The register was not shifted left");
                return true;
            }
            //operandOne is a constant value -> error (shl 2, orice)
            if (DigitsOnlyRegex().IsMatch(operandOne))
            {
                CustomAssert.IsTrue(true, "No post condition here");
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

                CustomAssert.IsTrue(ReadValueFromMemory(indexOperandOne) == result, "Postcondition failed: The memory location was not shifted left");
                return true;
            }

            CustomAssert.IsTrue(true, "No post condition here");
            return false;
        }
        public bool ShiftRight(string operandOne, string operandTwo)
        {
            //preconditions
            //processor
            CustomAssert.IsTrue(Processor.registerDictionary != null, "Precondition failed: Processor.registerDictionary is not initialized");
            string[] expectedRegisters = { "reg1", "reg2", "reg3", "reg4", "reg5", "reg6", "reg7", "reg8" };
            foreach (string registerName in expectedRegisters)
            {
                if (!Processor.registerDictionary.ContainsKey(registerName))
                {
                    CustomAssert.IsTrue(false, $"Precondition failed: Processor.registerDictionary does not contain the key {registerName}");
                }
            }
            //memory
            CustomAssert.IsTrue(Memory.programData != null, "Precondition failed: Memory.programData is not initialized");

            //first operand is data register
            if (Processor.registerDictionary.ContainsKey(operandOne))
            {
                ushort value = GetValue(operandTwo);
                ushort oldValue = Processor.registerDictionary[operandOne];
                Processor.registerDictionary[operandOne] >>= value;

                CustomAssert.IsTrue(Processor.registerDictionary[operandOne] == (ushort)(oldValue >> value), "Postcondition failed: The register was not shifted right");
                return true;
            }
            //operandOne is a constant value -> error (shr 2, orice)
            if (DigitsOnlyRegex().IsMatch(operandOne))
            {
                CustomAssert.IsTrue(true, "No post condition here");
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

                CustomAssert.IsTrue(ReadValueFromMemory(indexOperandOne) == result, "Postcondition failed: The memory location was not shifted right");
                return true;
            }

            CustomAssert.IsTrue(true, "No post condition here");
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
            if (operandOne.Contains("mem["))
            {
                int indexOperandOne = GetMemoryIndex(operandOne);
                ushort valueOperandOne = ReadValueFromMemory(indexOperandOne);
                ushort valueOperandTwo = GetValue(operandTwo);
                SetCompareFlags(valueOperandOne, valueOperandTwo);
                return true;
            }
            return false;
        }
        public bool Jump(string label)
        {
            for (ushort index = 0; index < Memory.instructionsNumber; index++)
                if (Memory.internalMemory[index].Split(' ')[0] == "label" && Memory.internalMemory[index].Split(' ')[1] == label)
                {
                    Memory.currentInstruction = index;
                    return true;
                }
            return false;
        }

        public bool JumpIfEqual(string label)
        {
            if (Processor.Equal)
            {
                bool res = Jump(label);
                return res;
            }
            return false;
        }

        public bool JumpIfNotEqual(string label)
        {
            if (Processor.NotEqual)
            {
                bool res = Jump(label);
                return res;
            }
            return false;
        }

        public bool JumpIfLessThan(string label)
        {
            if (Processor.Less)
            {
                bool res = Jump(label);
                return res;
            }
            return false;
        }

        public bool JumpIfGreaterThan(string label)
        {
            if (Processor.Greater)
            {
                bool res = Jump(label);
                return res;
            }
            return false;
        }

        public bool JumpIfLessThanOrEqual(string label)
        {
            if (Processor.LessEqual)
            {
                bool res = Jump(label);
                return res;
            }
            return false;
        }

        public bool JumpIfGreaterThanOrEqual(string label)
        {
            if (Processor.GreaterEqual)
            {
                bool res = Jump(label);
                return res;
            }
            return false;
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
            // Precondition: Validate Processor and Memory states before Call operation
            CustomAssert.IsTrue(Processor.StackPointer < Memory.endStack, "Precondition failed: Processor.StackPointer must be lower than Memory.endStack");
            CustomAssert.IsTrue(Memory.internalMemory != null, "Precondition failed: Memory.internalMemory is not initialized");
            CustomAssert.IsTrue(Memory.instructionsNumber >= 0, "Precondition failed: Memory.instructionsNumber must be greater than zero");
            CustomAssert.IsTrue(!string.IsNullOrEmpty(functionName), "Precondition failed: functionName must not be null or empty");

            WriteValueToMemory(Processor.StackPointer, Memory.currentInstruction);
            Processor.StackPointer += 2;

            bool functionFound = false;
            for (ushort index = 0; index < Memory.instructionsNumber; index++)
                if (Memory.internalMemory[index].Split(' ')[0] == "function" && Memory.internalMemory[index].Split(' ')[1] == functionName)
                {
                    Memory.currentInstruction = index;
                    functionFound = true;
                    break;
                }
            CustomAssert.IsTrue(functionFound, $"Precondition failed: Function {functionName} not found");

            // Postcondition: Validate Processor and Memory states after Call operation
            CustomAssert.IsTrue(Processor.StackPointer <= Memory.endStack, "Postcondition failed: Processor.StackPointer must be lower than or equal to Memory.endStack");
            CustomAssert.IsTrue(Memory.currentInstruction >= 0 && Memory.currentInstruction < Memory.instructionsNumber, "Postcondition failed: Memory.currentInstruction is out of bounds after Call operation");


        }
        public void Return()
        {
            // Precondition: Validate Processor and Memory states before Return operation
            CustomAssert.IsTrue(Processor.StackPointer <= Memory.endStack, "Precondition failed: Processor.StackPointer must be lower than or equal to Memory.endStack");
            CustomAssert.IsTrue(Memory.programData != null, "Precondition failed: Memory.programData is not initialized");

            Processor.StackPointer -= 2;

            ushort returnAddress = ReadValueFromMemory(Processor.StackPointer);
            CustomAssert.IsTrue(returnAddress >= 0 && returnAddress < Memory.instructionsNumber, "Precondition failed: Return address is out of bounds");

            Memory.currentInstruction = ReadValueFromMemory(Processor.StackPointer);
            Memory.programData[Processor.StackPointer] = 0;
            Memory.programData[Processor.StackPointer + 1] = 0;

            // Postcondition: Validate Processor and Memory states after Return operation
            CustomAssert.IsTrue(Memory.currentInstruction >= 0 && Memory.currentInstruction < Memory.instructionsNumber, "Postcondition failed: Memory.currentInstruction is out of bounds after Return operation");
            CustomAssert.IsTrue(Memory.programData[Processor.StackPointer] == 0 && Memory.programData[Processor.StackPointer + 1] == 0, "Postcondition failed: Memory.programData at stack pointer was not cleared");
            CustomAssert.IsTrue(Processor.StackPointer < Memory.endStack, "Postcondition failed: Processor.StackPointer must be lower than Memory.endStack");
        }
        public void Read(string operand)
        {
            // Precondition: Validate Processor and Memory states before Read operation
            CustomAssert.IsTrue(Processor.registerDictionary != null, "Precondition failed: Processor.registerDictionary is not initialized");
            CustomAssert.IsTrue(Memory.programData != null, "Precondition failed: Memory.programData is not initialized");
            CustomAssert.IsTrue(Memory.currentIndexMemoryVideo >= 0 && Memory.currentIndexMemoryVideo < Memory.programData.Length, "Precondition failed: Memory.currentIndexMemoryVideo is out of bounds");
            CustomAssert.IsTrue(!string.IsNullOrEmpty(operand), "Precondition failed: operand must not be null or empty");

            this.hubContext.Clients.All.SendAsync("ReadOpearion");

            ushort result = 0;
            while ((char)Memory.programData[Memory.currentIndexMemoryVideo] != ' ')
            {
                result = (ushort)(result * 10 + ((char)(Memory.programData[Memory.currentIndexMemoryVideo]) - '0'));
                Memory.currentIndexMemoryVideo++;
            }
            while ((char)Memory.programData[Memory.currentIndexMemoryVideo] == ' ')
            {
                Memory.currentIndexMemoryVideo++;
            }
            // Postcondition: Validate the result of the Read operation
            CustomAssert.IsTrue(Processor.registerDictionary.ContainsKey(operand), $"Postcondition failed: Processor.registerDictionary does not contain the key {operand}");
            Processor.registerDictionary[operand] = result;
            CustomAssert.IsTrue(Processor.registerDictionary[operand] == result, "Postcondition failed: The register was not correctly set");
        }
        public ResultRegisters LoadResultRegisters()
        {
            // Precondition: Validate Processor state before loading result registers
            CustomAssert.IsTrue(Processor.registerDictionary != null, "Precondition failed: Processor.registerDictionary is not initialized");
            string[] expectedRegisters = { "reg1", "reg2", "reg3", "reg4", "reg5", "reg6", "reg7", "reg8" };
            foreach (string registerName in expectedRegisters)
            {
                CustomAssert.IsTrue(Processor.registerDictionary.ContainsKey(registerName), $"Precondition failed: Processor.registerDictionary does not contain the key {registerName}");
            }

            ResultRegisters resultRegisters = new ResultRegisters();
            resultRegisters.Reg1 = Processor.registerDictionary["reg1"];
            resultRegisters.Reg2 = Processor.registerDictionary["reg2"];
            resultRegisters.Reg3 = Processor.registerDictionary["reg3"];
            resultRegisters.Reg4 = Processor.registerDictionary["reg4"];
            resultRegisters.Reg5 = Processor.registerDictionary["reg5"];
            resultRegisters.Reg6 = Processor.registerDictionary["reg6"];
            resultRegisters.Reg7 = Processor.registerDictionary["reg7"];
            resultRegisters.Reg8 = Processor.registerDictionary["reg8"];

            // Postcondition: Validate the state of the loaded result registers
            CustomAssert.IsTrue(resultRegisters.Reg1 == Processor.registerDictionary["reg1"], "Postcondition failed: resultRegisters.Reg1 was not correctly set");
            CustomAssert.IsTrue(resultRegisters.Reg2 == Processor.registerDictionary["reg2"], "Postcondition failed: resultRegisters.Reg2 was not correctly set");
            CustomAssert.IsTrue(resultRegisters.Reg3 == Processor.registerDictionary["reg3"], "Postcondition failed: resultRegisters.Reg3 was not correctly set");
            CustomAssert.IsTrue(resultRegisters.Reg4 == Processor.registerDictionary["reg4"], "Postcondition failed: resultRegisters.Reg4 was not correctly set");
            CustomAssert.IsTrue(resultRegisters.Reg5 == Processor.registerDictionary["reg5"], "Postcondition failed: resultRegisters.Reg5 was not correctly set");
            CustomAssert.IsTrue(resultRegisters.Reg6 == Processor.registerDictionary["reg6"], "Postcondition failed: resultRegisters.Reg6 was not correctly set");
            CustomAssert.IsTrue(resultRegisters.Reg7 == Processor.registerDictionary["reg7"], "Postcondition failed: resultRegisters.Reg7 was not correctly set");
            CustomAssert.IsTrue(resultRegisters.Reg8 == Processor.registerDictionary["reg8"], "Postcondition failed: resultRegisters.Reg8 was not correctly set");

            return resultRegisters;
        }

        public void ResetData()
        {
            Processor.InitProcessor();
            Memory.InitMemory();
        }

        public void WriteValueToKeyboardBuffer(ushort value)
        {
            byte lowByte = (byte)(value & 0xFF);
            Memory.programData[Memory.keyboardBufferIndex] = lowByte;
            Memory.isKeyboardBufferChanged = true;
        }

        public static void WriteToVideoMemory()
        {
            while (Memory.StopWriteToVideoMemory)
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
            //preconditions
            CustomAssert.IsTrue(Memory.programData != null, "Precondition failed: Memory.programData is not initialized");

            byte highByte = (byte)(valueOperandTwo >> 8);
            byte lowByte = (byte)(valueOperandTwo & 0xFF);

            // write to two consecutive addresses
            Memory.programData[indexOperandOne] = lowByte;
            Memory.programData[indexOperandOne + 1] = highByte;

            //postconditions
            CustomAssert.IsTrue(ReadValueFromMemory(indexOperandOne) == valueOperandTwo, "Postcondition failed: The memory location was not set");
        }
        private static ushort ReadValueFromMemory(int index)
        {
            //preconditions
            CustomAssert.IsTrue(Memory.programData != null, "Precondition failed: Memory.programData is not initialized");

            byte lowByte = Memory.programData[index];
            byte highByte = Memory.programData[index + 1];
            ushort result = (ushort)((highByte << 8) | lowByte);
            
            //postconditions
            CustomAssert.IsTrue(result == (ushort)((Memory.programData[index + 1] << 8) | Memory.programData[index]), "Postcondition failed: The memory location was not read");
            return result;
        }


        private static ushort GetValue(string operand)
        {
            //preconditions
            //processor
            CustomAssert.IsTrue(Processor.registerDictionary != null, "Precondition failed: Processor.registerDictionary is not initialized");
            string[] expectedRegisters = { "reg1", "reg2", "reg3", "reg4", "reg5", "reg6", "reg7", "reg8" };
            foreach (string registerName in expectedRegisters)
            {
                if (!Processor.registerDictionary.ContainsKey(registerName))
                {
                    CustomAssert.IsTrue(false, $"Precondition failed: Processor.registerDictionary does not contain the key {registerName}");
                }
            }
            //memory
            CustomAssert.IsTrue(Memory.programData != null, "Precondition failed: Memory.programData is not initialized");

            //second operand is a data register
            if (Processor.registerDictionary.TryGetValue(operand, out ushort value))
            {
                CustomAssert.IsTrue(value >= 0 && value <= ushort.MaxValue, "Postcondition failed: The value is not in the correct range");
                return value;
            }
            //second operand is a constant value
            if (DigitsOnlyRegex().IsMatch(operand))
            {
                CustomAssert.IsTrue(value >= 0 && value <= ushort.MaxValue, "Postcondition failed: The value is not in the correct range");
                return ushort.Parse(operand);
            }

            //second operand is a memory location -> need to read two consecutive addresses
            int index = GetMemoryIndex(operand);
            ushort result = ReadValueFromMemory(index);
            CustomAssert.IsTrue(value >= 0 && value <= ushort.MaxValue, "Postcondition failed: The value is not in the correct range");
            return result;
        }

        private static ushort GetMemoryIndex(string operand)
        {
            //preconditions
            //processor
            CustomAssert.IsTrue(Processor.registerDictionary != null, "Precondition failed: Processor.registerDictionary is not initialized");
            string[] expectedRegisters = { "reg1", "reg2", "reg3", "reg4", "reg5", "reg6", "reg7", "reg8" };
            foreach (string registerName in expectedRegisters)
            {
                if (!Processor.registerDictionary.ContainsKey(registerName))
                {
                    CustomAssert.IsTrue(false, $"Precondition failed: Processor.registerDictionary does not contain the key {registerName}");
                }
            }
            //memory
            CustomAssert.IsTrue(operand.Contains("mem["), "Precondition failed: The operand is not a memory location");
            CustomAssert.IsTrue(Memory.programData != null, "Precondition failed: Memory.programData is not initialized");

            int startIndex = operand.IndexOf('[') + 1;
            int endIndex = operand.IndexOf(']');
            string indexStr = operand[startIndex..endIndex];

            //address specified by a constant value
            if (ushort.TryParse(indexStr, out ushort index))
            {
                CustomAssert.IsTrue(index >= 0 && index < Memory.programData.Length, "Postcondition failed: The index is not in the correct range");
                return index;
            }

            //address specified by a data register
            Processor.registerDictionary.TryGetValue(indexStr, out index);

            CustomAssert.IsTrue(index >= 0 && index < Memory.programData.Length, "Postcondition failed: The index is not in the correct range");
            return index;
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
                Processor.Less = operandValueOne < operandValueTwo;
                Processor.Greater = operandValueOne > operandValueTwo;

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
            }
        }

        [GeneratedRegex(@"^\d+$")]
        private static partial Regex DigitsOnlyRegex();
    }
}
