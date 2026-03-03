using SmallGreen.Dto.Data;

namespace SmallGreen.Desktop.Settings.Common
{
    public class GlobalParam
    {
        private readonly static object locker = new();
        private static GlobalParam globalParam = null!;
        public UserDto User { get; set; } = null!;


        private GlobalParam()
        {

        }

        public static GlobalParam GetInstance()
        {
            if (globalParam == null)
            {
                lock (locker)
                {
                    globalParam ??= new GlobalParam();
                }
            }
            return globalParam;
        }
    }
}
