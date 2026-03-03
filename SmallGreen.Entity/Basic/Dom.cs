using S7.Net;
using S7.Net.Types;

namespace SmallGreen.Entity.Basic
{
    public interface IDom
    {
        DataItem DataItem { get; set; }
        object? GetNewValue();

        void RefreshValue(object value);
    }

    public class Dom<T> : IDom
    {
        public T? NewValue { get; set; } = default;

        public DataItem DataItem { get; set; } = null!;

        public Dom(DataType dataType, VarType varType, int db, int startByteAdr, byte bitAdr, int count)
        {
            if (db < 0)
            {
                throw new ArgumentException("DB cannot be negative.", nameof(db));
            }

            if (startByteAdr < 0)
            {
                throw new ArgumentException("StartByteAdr cannot be negative.", nameof(startByteAdr));
            }

            if (count < 0)
            {
                throw new ArgumentException("Count cannot be negative.", nameof(count));
            }

            DataItem = new DataItem
            {
                DataType = dataType,
                VarType = varType,
                DB = db,
                StartByteAdr = startByteAdr,
                BitAdr = bitAdr,
                Count = count,
            };
        }

        public T GetCurrentValue()
        {
            try
            {
                if (DataItem.Value == null) return default!;
                return (T)DataItem.Value;
            }
            catch (InvalidCastException ex)
            {
                throw new InvalidCastException($"PLC数据类型转化异常({DataItem.Value}):{ex.Message}", ex);
            }
        }


        public object? GetNewValue()
        {
            return NewValue;
        }

        public void RefreshValue(object value)
        {
            DataItem.Value = value;
        }
    }
}
