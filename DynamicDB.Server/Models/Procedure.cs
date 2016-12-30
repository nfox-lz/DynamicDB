namespace Compete.DynamicDB.Models
{
    public sealed class Procedure
    {
        public string Text { get; set; }

        public string Path { get; set; }

        public byte[] Binary { get; set; }
    }
}
