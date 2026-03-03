namespace SmallGreen.Entity.Basic
{
    public class DataPool
    {
        public int DB { get; set; }
        public int StartAddress { get; set; }
        public int Count { get; set; }
        public byte[]? Data { get; set; }

        public DataPool(int db, int startAddress, int count)
        {
            DB = db;
            StartAddress = startAddress;
            Count = count;
        }
    }
}
