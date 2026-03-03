namespace SmallGreen.Dto.Data
{
    public class PageInfo<T>
    {
        /// <summary>
        /// 当前页码号
        /// </summary>
        private int pageNumber;

        public int PageNumber
        {
            get { return pageNumber; }
            set { pageNumber = value; }
        }

        /// <summary>
        /// 总条目数量
        /// </summary>
        private int totalItemCount;

        public int TotalItemCount
        {
            get { return totalItemCount; }
            set { totalItemCount = value; }
        }


        /// <summary>
        /// 每页显示条目数
        /// </summary>
        private int pageSize;

        public int PageSize
        {
            get { return pageSize; }
            set { pageSize = value; }
        }


        /// <summary>
        /// 总页数
        /// </summary>

        public int TotalPageCount
        {
            get
            {
                if (pageSize == 0) return 0;
                if (totalItemCount == 0) return 0;

                if (totalItemCount % pageSize == 0)
                {
                    return totalItemCount / pageSize;
                }
                else
                {
                    return totalItemCount / pageSize + 1;
                }
            }
        }

        public List<T> Items { get; set; }

        public bool HasPreviousPage => pageNumber - 1 > 0;

        public bool HasNextPage => pageNumber < TotalPageCount;

        public PageInfo()
        {
            pageNumber = 1;
            Items = new List<T>();
            TotalItemCount = 0;
        }



    }
}
