﻿using ProjectSQ.Interfaces.Processor;
using ProjectSQ.Models;

namespace ProjectSQ.Services
{
    public class ProcessorService : IProcessorService
    {
        public void ExecuteFile()
        {
            while (Memory.currentInstruction < Memory.internalMemory.Length)
            {
                string instruction = Memory.internalMemory[Memory.currentInstruction++];
                string[] words = instruction.Split(' ');
                string operation = words[0];
                switch (operation)
                {
                    case "mov":
                        string[] operands = words[1].Split(",");
                        new ParseService().Assignment(operands[0], operands[1]);
                        break;

                    case "add":
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

        public void Assignment(string reg1, string reg2)
        {
            Processor.registerDictionary[reg1] = Processor.registerDictionary[reg2];
        }

        public void Assignment(string reg1, short val)
        {
            Processor.registerDictionary[reg1] = val;
        }

        public void Addition(string reg1, string reg2)
        {
            Processor.registerDictionary[reg1] += Processor.registerDictionary[reg2];
            throw new NotImplementedException();
        }

        public void Addition(string reg1, short val)
        {
            Processor.registerDictionary[reg1] += val;
            throw new NotImplementedException();
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
    }
}
