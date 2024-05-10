namespace ProjectSQ.Models
{
    public class ResultMemory
    {
        public List<NonZeroValue> NonZeroValues { get; set; }
        public ResultMemory(byte[] memory)
        {
            NonZeroValues = new List<NonZeroValue>();

            for (int i = 0; i < memory.Length; i++)
            {
                if (memory[i] != 0)
                {
                    NonZeroValues.Add(new NonZeroValue { Position = i, Value = memory[i] });
                }
            }
        }

    }
}
