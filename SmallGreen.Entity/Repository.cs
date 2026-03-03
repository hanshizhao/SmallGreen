using SqlSugar;

namespace SmallGreen.Entity
{
    public class Repository<T> : SimpleClient<T> where T : class, new()
    {
        public Repository(ISqlSugarClient context = null) : base(context)//注意这里要有默认值等于null
        {

            if (context == null)
            {
                base.Context = new SqlSugarClient(new ConnectionConfig()
                {
                    DbType = DbType.SqlServer,
                    ConnectionString = "database=SmallGreenDB;uid=sa;pwd=123;Encrypt=True;TrustServerCertificate=True",
                    IsAutoCloseConnection = true
                }
                );
            }
        }
    }
}
