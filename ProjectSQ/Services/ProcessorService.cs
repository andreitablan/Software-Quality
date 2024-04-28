using ProjectSQ.Interfaces;
using ProjectSQ.Models;

namespace ProjectSQ.Services
{
    public class ProcessorService : IProcessorService
    {
        public void Addition(string reg1, string reg2)
        {
            Processor.registerDictionary[reg1] += Processor.registerDictionary[reg2];
            throw new NotImplementedException();
        }

        public void Addition(string reg1, int val)
        {
            Processor.registerDictionary[reg1] += val;
            throw new NotImplementedException();
        }

        public void And(string reg1, string reg2)
        {
            Processor.registerDictionary[reg1] &= Processor.registerDictionary[reg2];
            throw new NotImplementedException();
        }

        public void Assignment(string reg1, string reg2)
        {
            Processor.registerDictionary[reg1] = Processor.registerDictionary[reg2];
            throw new NotImplementedException();
        }

        public void Assignment(string reg1, int val)
        {
            Processor.registerDictionary[reg1] = val;
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
            }
            else
            {
                Processor.Equal = false;
            }
            throw new NotImplementedException();
        }

        public void Compare(string reg1, int val)
        {
            throw new NotImplementedException();
        }

        public void Division(string reg1, string reg2)
        {
            Processor.registerDictionary[reg1] /= Processor.registerDictionary[reg2];
            throw new NotImplementedException();
        }

        public void Division(string reg1, int val)
        {
            Processor.registerDictionary[reg1] /= val;
            throw new NotImplementedException();
        }

        public void Jump(string label)
        {
            throw new NotImplementedException();
        }

        public void Multiplication(string reg1, string reg2)
        {
            Processor.registerDictionary[reg1] *= Processor.registerDictionary[reg2];
            throw new NotImplementedException();
        }

        public void Multiplication(string reg1, int val)
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
            throw new NotImplementedException();
        }

        public void Return()
        {
            throw new NotImplementedException();
        }

        public void Shift(string reg1, int val)
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

        public void Subtraction(string reg1, int val)
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
