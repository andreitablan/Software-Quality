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
                    NonZeroValue nonZeroValue = new NonZeroValue
                    {
                        Position = i,
                        LowBits = Convert.ToString(memory[i], 2).PadLeft(8, '0'),
                        HighBits = Convert.ToString(memory[i + 1], 2).PadLeft(8, '0')
                    };
                    NonZeroValues.Add(nonZeroValue);
                }
            }
        }
    }
}
