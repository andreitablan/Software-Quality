namespace ProjectSQ.Interfaces.Parser
{
    public interface IParseService
    {
        void LoadInstructions(string file);
        void Assignment(string operand1, string operand2);
    }
}
