using SmallGreen.Desktop.Settings.IServices;
using SmallGreen.Dto.Base;
using SmallGreen.Dto.Machine;

namespace SmallGreen.Desktop.Settings.Services
{
    /// <summary>
    /// Chemical bucket service implementation for managing chemical bucket data
    /// </summary>
    public class ChemicalBucketService : IChemicalBucketService
    {
        private readonly HttpRestClient client;

        public ChemicalBucketService(HttpRestClient client)
        {
            this.client = client;
        }

        /// <summary>
        /// Get chemical buckets by subsystem names
        /// </summary>
        /// <param name="subSystemNames">Array of subsystem names</param>
        /// <returns>List of chemical buckets</returns>
        public async Task<ApiResponse<List<ChemicalBucketDto>>> GetBySubSystems(string[] subSystemNames)
        {
            BaseRequest request = new()
            {
                Method = RestSharp.Method.Get,
                Route = "ChemicalBucket/Query",
                Parameter = subSystemNames
            };
            return await client.ExecuteAsync<List<ChemicalBucketDto>>(request);
        }

        /// <summary>
        /// Update concentration ratio (save to DB and sync to PLC)
        /// </summary>
        /// <param name="dto">Update concentration request data</param>
        /// <returns>Success or failure response</returns>
        public async Task<ApiResponse<bool>> UpdateConcentration(UpdateConcentrationDto dto)
        {
            BaseRequest request = new()
            {
                Method = RestSharp.Method.Put,
                Route = "ChemicalBucket/Concentration",
                Parameter = dto
            };
            return await client.ExecuteAsync<bool>(request);
        }
    }
}
