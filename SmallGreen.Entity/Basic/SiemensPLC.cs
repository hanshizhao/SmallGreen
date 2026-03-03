using S7.Net;
using SqlSugar;
using System.Collections;
using System.Text;

namespace SmallGreen.Entity.Basic
{
    public class SiemensPLC
    {
        /// <summary>
        /// PLC的ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public long Id { get; set; }

        /// <summary>
        /// PLC名称
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// PLC类型
        /// </summary>
        public CpuType CpuType { get; set; }

        /// <summary>
        /// IP地址
        /// </summary>
        public string IPAddress { get; set; } = null!;

        /// <summary>
        /// Rack
        /// </summary>
        public short Rack { get; set; }

        /// <summary>
        /// Slot
        /// </summary>
        public short Slot { get; set; }

        /// <summary>
        /// 数据池
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public List<DataPool> ListDataPool { get; set; } = null!;

        private Plc Plc = null!;

        /// <summary>
        /// 初始化并连接S7PLC
        /// </summary>
        /// <returns></returns>
        public void ConnectionS7Plc(List<DataPool> list_data_pool)
        {
            Plc = new Plc(CpuType, IPAddress, Rack, Slot);
            ListDataPool = list_data_pool;
        }

        /// <summary>
        /// 检查PLC是否连接正常
        /// </summary>
        /// <returns></returns>
        public async Task<OperateResult> IsConnected()
        {
            try
            {
                if (!Plc.IsConnected) await Plc.OpenAsync();
                return new OperateResult { IsSuccess = true };
            }
            catch (Exception ex)
            {
                return new OperateResult($"连接 [{Name}] 失败," + ex.Message);
            }
        }

        /// <summary>
        /// 写入bool值到PLC
        /// </summary>
        /// <param name="dom"></param>
        /// <returns></returns>
        public async Task<OperateResult> WriteBit(Dom<bool> dom)
        {
            var resultConnceted = await IsConnected();
            if (!resultConnceted.IsSuccess) return resultConnceted;

            if (dom == null) return new OperateResult { IsSuccess = false, Message = $"写入变量时，传入的变量为空" };

            try
            {
                await Plc.WriteBitAsync(dom.DataItem.DataType, dom.DataItem.DB, dom.DataItem.StartByteAdr, dom.DataItem.BitAdr, dom.NewValue);
                return new OperateResult { IsSuccess = true };
            }
            catch (Exception ex)
            {
                return new OperateResult { IsSuccess = false, Message = $"写入变量 时出现异常：{ex.Message}" };
            }
        }

        /// <summary>
        /// 写入值到PLC
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dom"></param>
        /// <returns></returns>
        public async Task<OperateResult> Write<T>(Dom<T> dom) where T : struct
        {
            var resultConnceted = await IsConnected();
            if (!resultConnceted.IsSuccess) return resultConnceted;

            if (dom == null) return new OperateResult { IsSuccess = false, Message = $"写入变量时，传入的变量为空" };
            var value = dom.GetNewValue();
            if (value == null) return new OperateResult { IsSuccess = false, Message = $"写入变量时，传入的值为空" };

            try
            {
                if (typeof(T) == typeof(bool))
                {
                    await Plc.WriteBitAsync(dom.DataItem.DataType, dom.DataItem.DB, dom.DataItem.StartByteAdr, dom.DataItem.BitAdr, (bool)value);
                }
                else
                {
                    await Plc.WriteAsync(dom.DataItem.DataType, dom.DataItem.DB, dom.DataItem.StartByteAdr, value);
                }
            }
            catch (Exception ex)
            {
                return new OperateResult { IsSuccess = false, Message = $"写入变量 时出现异常：{ex.Message}" };
            }
            return new OperateResult { IsSuccess = true };
        }

        /// <summary>
        /// 批量写入值到PLC
        /// </summary>
        /// <param name="listDom"></param>
        /// <returns></returns>
        public async Task<OperateResult> Write(List<IDom> listDom)
        {
            try
            {
                if (listDom == null) return new OperateResult("传入的Dom列表为空(null)");
                if (listDom.Count < 1) return new OperateResult("传入的Dom列表为空(count = 0)");

                var resultConnceted = await IsConnected();
                if (!resultConnceted.IsSuccess) return resultConnceted;

                foreach (var dom in listDom)
                {
                    var value = dom.GetNewValue();
                    if (value == null) return new OperateResult { IsSuccess = false, Message = $"写入变量时，传入的值为空" };

                    if (dom.DataItem.VarType == VarType.Bit)
                    {
                        await Plc.WriteBitAsync(dom.DataItem.DataType, dom.DataItem.DB, dom.DataItem.StartByteAdr, dom.DataItem.BitAdr, (bool)value);
                    }
                    else
                    {
                        await Plc.WriteAsync(dom.DataItem.DataType, dom.DataItem.DB, dom.DataItem.StartByteAdr, value);
                    }

                    //if (listDataItem.Count > 10)
                    //{
                    //    await Plc.WriteAsync(listDataItem.ToArray());
                    //    listDataItem.Clear();
                    //}

                    //// 这里需要和批量读取操作互斥，否者Value值会有被覆盖的风险
                    //dom.DataItem.Value = dom.GetNewValue();
                    //listDataItem.Add(dom.DataItem);
                }

                //if (listDataItem.Count > 0)
                //{
                //    await Plc.WriteAsync(listDataItem.ToArray());
                //}

                return new OperateResult { IsSuccess = true };
            }
            catch (Exception ex)
            {
                return new OperateResult { IsSuccess = false, Message = $"批量写入变量时出现异常：{ex.Message}" };
            }
        }

        /// <summary>
        /// 批量写入值到PLC
        /// </summary>
        /// <param name="listDom"></param>
        /// <returns></returns>
        public async Task<OperateResult> Write(IDom dom, byte[] bytes)
        {
            try
            {
                if (bytes == null) return new OperateResult("传入的bytes列表为空(null)");
                if (bytes.Length < 1) return new OperateResult("传入的bytes列表为空(count = 0)");

                var resultConnceted = await IsConnected();
                if (!resultConnceted.IsSuccess) return resultConnceted;

                await Plc.WriteBytesAsync(dom.DataItem.DataType, dom.DataItem.DB, dom.DataItem.StartByteAdr, bytes);

                return new OperateResult { IsSuccess = true };
            }
            catch (Exception ex)
            {
                return new OperateResult { IsSuccess = false, Message = $"批量写入变量时出现异常：{ex.Message}" };
            }
        }

        public async Task<OperateResult> RefreshDataPool()
        {
            var resultConnceted = await IsConnected();
            if (!resultConnceted.IsSuccess) return resultConnceted;
            foreach (var dp in ListDataPool)
            {
                try
                {
                    dp.Data = await Plc.ReadBytesAsync(DataType.DataBlock, dp.DB, dp.StartAddress, dp.Count);
                }
                catch (Exception ex)
                {
                    return new OperateResult<byte[]>($"刷新PLC{Name}数据池出现异常 DB-{dp.DB} StartAddress-{dp.StartAddress} Count-{dp.Count}：" + ex.Message);
                }
            }
            return new OperateResult { IsSuccess = true };
        }

        public async Task<OperateResult<byte[]>> ReadDBData(DataPool dataPool)
        {
            try
            {
                var data = await Plc.ReadBytesAsync(S7.Net.DataType.DataBlock, dataPool.DB, dataPool.StartAddress, dataPool.Count);
                return new OperateResult<byte[]> { IsSuccess = true, Content = data };
            }
            catch (Exception ex)
            {
                return new OperateResult<byte[]>($"读取DB块出现异常(db:{dataPool.DB}, startAddress:{dataPool.StartAddress}, count:{dataPool.Count})：{ex.Message}");
            }
        }

        public async Task<OperateResult> ReadMultipleVars(List<IDom> listAllDom)
        {
            try
            {
                foreach (var dom in listAllDom)
                {
                    var dataItem = dom.DataItem;
                    var bytes = await Plc.ReadBytesAsync(dataItem.DataType, dataItem.DB, dataItem.StartByteAdr, dataItem.Count);

                    var reversedBytes = bytes.Reverse().ToArray();

                    object value = dataItem.VarType switch
                    {
                        VarType.Real => BitConverter.ToSingle(reversedBytes.ToArray()),
                        VarType.Bit => new BitArray(bytes).Get(dataItem.BitAdr),
                        VarType.Int => BitConverter.ToInt16(reversedBytes.ToArray()),
                        VarType.Word => BitConverter.ToUInt16(reversedBytes.ToArray()),
                        VarType.Byte => bytes[dataItem.StartByteAdr],
                        VarType.String => Encoding.ASCII.GetString(bytes),
                        _ => throw new Exception($"未知的数据类型{dataItem.VarType}"),
                    };

                    dom.RefreshValue(value);
                }

                return new OperateResult { IsSuccess = true };

            }
            catch (Exception ex)
            {
                return new OperateResult($"批量读取变量出现异常：{ex.Message}");
            }
        }

        public async Task<OperateResult> ReadDom(IDom dom)
        {
            try
            {
                var dataItem = dom.DataItem;
                var bytes = await Plc.ReadBytesAsync(dataItem.DataType, dataItem.DB, dataItem.StartByteAdr, dataItem.Count);

                var reversedBytes = bytes.Reverse().ToArray();

                object value = dataItem.VarType switch
                {
                    VarType.Real => BitConverter.ToSingle(reversedBytes.ToArray()),
                    VarType.Bit => new BitArray(bytes).Get(dataItem.BitAdr),
                    VarType.Int => BitConverter.ToInt16(reversedBytes.ToArray()),
                    VarType.Word => BitConverter.ToUInt16(reversedBytes.ToArray()),
                    VarType.Byte => bytes[dataItem.StartByteAdr],
                    VarType.String => Encoding.ASCII.GetString(bytes),
                    _ => throw new Exception($"未知的数据类型{dataItem.VarType}"),
                };

                dom.RefreshValue(value);

                return new OperateResult { IsSuccess = true };

            }
            catch (Exception ex)
            {
                return new OperateResult($"批量读取变量出现异常：{ex.Message}");
            }
        }

        public void RefreshDomValue(List<IDom> listDom)
        {
            if (listDom.Count < 1) return;
            var star = -1;
            var count = -1;
            var bit = -1;
            //locker.EnterWriteLock();
            try
            {
                foreach (var dom in listDom)
                {
                    var data_pool = ListDataPool.Find(dp => dp.DB == dom.DataItem.DB);
                    if (data_pool == null) return;
                    if (data_pool.Data == null) return;

                    star = dom.DataItem.StartByteAdr;
                    count = dom.DataItem.Count;
                    bit = dom.DataItem.BitAdr;

                    switch (dom.DataItem.VarType)
                    {
                        case VarType.Real: ReadFloat(dom, data_pool.Data); break;
                        case VarType.Bit: ReadBool(dom, data_pool.Data); break;
                        case VarType.Int: ReadShort(dom, data_pool.Data); break;
                        case VarType.Word: ReadWord(dom, data_pool.Data); break;
                        case VarType.Byte: ReadByte(dom, data_pool.Data); break;
                        case VarType.String: ReadString(dom, data_pool.Data); break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"RefreshDomValue:{ex.Message}, star: {star}, count: {count}, bit: {bit}");
            }
            //finally
            //{
            //    if (locker.IsWriteLockHeld)
            //        locker.ExitWriteLock();
            //}

        }

        private void ReadByte(IDom dom, byte[] data)
        {
            var value = data[dom.DataItem.StartByteAdr];
            dom.RefreshValue(value);
        }

        private void ReadFloat(IDom dom, byte[] data)
        {
            var data_byte = new byte[4];
            Array.Copy(data, dom.DataItem.StartByteAdr, data_byte, 0, data_byte.Length);
            var value = BitConverter.ToSingle(data_byte.Reverse().ToArray());
            dom.RefreshValue(value);
        }

        private void ReadShort(IDom dom, byte[] data)
        {
            var data_byte = new byte[2];
            Array.Copy(data, dom.DataItem.StartByteAdr, data_byte, 0, data_byte.Length);
            var value = BitConverter.ToInt16(data_byte.Reverse().ToArray());
            dom.RefreshValue(value);
        }

        private void ReadWord(IDom dom, byte[] data)
        {
            var data_byte = new byte[2];
            Array.Copy(data, dom.DataItem.StartByteAdr, data_byte, 0, data_byte.Length);
            var value = BitConverter.ToUInt16(data_byte.Reverse().ToArray());
            dom.RefreshValue(value);
        }


        private void ReadBool(IDom dom, byte[] data)
        {
            var data_byte = new byte[1];
            Array.Copy(data, dom.DataItem.StartByteAdr, data_byte, 0, data_byte.Length);
            var value = new BitArray(data_byte).Get(dom.DataItem.BitAdr);
            dom.RefreshValue(value);
        }

        private void ReadString(IDom dom, byte[] data)
        {
            var data_byte = new byte[dom.DataItem.Count];
            Array.Copy(data, dom.DataItem.StartByteAdr, data_byte, 0, data_byte.Length);
            var value = Encoding.ASCII.GetString(data_byte);
            dom.RefreshValue(value);
        }
    }
}
