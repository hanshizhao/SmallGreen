using SmallGreen.Dto.Base;
using SmallGreen.Dto.Machine;

namespace SmallGreen.Desktop.Settings.IServices
{
    /// <summary>
    /// Chemical bucket service interface for managing chemical bucket data
    /// </summary>
    public interface IChemicalBucketService
    {
        /// <summary>
        /// Get chemical buckets by subsystem names
        /// </summary>
        /// <param name="subSystemNames">Array of subsystem names</param>
        /// <returns>List of chemical buckets</returns>
        Task<ApiResponse<List<ChemicalBucketDto>>> GetBySubSystems(string[] subSystemNames);

        /// <summary>
        /// Update concentration ratio (save to DB and sync to PLC)
        /// </summary>
        /// <param name="dto">Update concentration request data</param>
        /// <returns>Success or failure response</returns>
        Task<ApiResponse<bool>> UpdateConcentration(UpdateConcentrationDto dto);
    }
}
