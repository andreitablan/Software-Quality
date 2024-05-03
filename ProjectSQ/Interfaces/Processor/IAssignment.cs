using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Reflection.Metadata;

namespace ProjectSQ.Interfaces.Processor
{
    public interface IAssignment
    {
        // mov reg1, reg2
        // mov reg2, reg1
        void Assignment(string reg1, string reg2);

        // constant values
        void Assignment(string reg1, short val);
    }
}
