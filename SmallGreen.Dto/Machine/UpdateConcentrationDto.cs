namespace SmallGreen.Dto.Machine
{
    public class UpdateConcentrationDto
    {
        public long BucketId { get; set; }
        /// <summary>
        /// 助剂浓度，单位：克/升
        /// </summary>
        public float Concentration { get; set; }
    }
}
