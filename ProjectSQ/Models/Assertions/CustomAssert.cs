namespace ProjectSQ.Models.Assertions
{
    public static class CustomAssert
    {
        public static void IsTrue(bool condition, string message)
        {
            if (!condition)
            {
                throw new AssertionFailedException(message);
            }
        }
    }
}
