namespace ProjectSQ.Models.Assertions
{
    public class AssertionFailedException : Exception
    {
        public AssertionFailedException(string message) : base(message) { }
    }
}
