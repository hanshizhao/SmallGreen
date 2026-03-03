using Microsoft.AspNetCore.Mvc;
using SmallGreen.Common;
using SmallGreen.Dto.Base;
using SmallGreen.Dto.Machine;
using SmallGreen.Entity;
using SmallGreen.Entity.Machine;

namespace SmallGreen.API.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class ChemicalBucketController : ControllerBase
    {
        /// <summary>
        /// Query chemical buckets by subsystem names
        /// </summary>
        /// <param name="subSystemNames">Array of subsystem names (e.g., "QCL1", "QCL2", "GS1")</param>
        /// <returns>List of chemical bucket data</returns>
        [HttpGet]
        public async Task<ApiResponse<List<ChemicalBucketDto>>> Query([FromQuery] string[] subSystemNames)
        {
            try
            {
                var result = new List<ChemicalBucketDto>();
                var subSystemRepo = new Repository<SubSystem>();
                var equipmentRepo = new Repository<Equipment>();
                var bulkRepo = new Repository<Bulk>();

                foreach (var name in subSystemNames)
                {
                    // Parse subsystem name string to enum
                    if (!Enum.TryParse<SubSystemName>(name, out var subSystemName))
                        continue;

                    var subsystem = await subSystemRepo.AsQueryable()
                        .FirstAsync(s => s.SubSystemName == subSystemName);

                    if (subsystem == null) continue;

                    var equipments = await equipmentRepo.AsQueryable()
                        .Where(e => e.SubSystemId == subsystem.Id)
                        .ToListAsync();

                    foreach (var equipment in equipments)
                    {
                        var bulks = await bulkRepo.AsQueryable()
                            .Where(b => b.EquipmentId == equipment.Id)
                            .ToListAsync();

                        foreach (var bulk in bulks)
                        {
                            result.Add(new ChemicalBucketDto
                            {
                                Id = bulk.Id,
                                CodeNumber = bulk.CodeNumber,
                                Level = bulk.DataLevel?.GetCurrentValue() ?? 0,
                                MaxCapacity = 1000, // TODO: Get from config or equipment
                                Concentration = 85.0f, // 单位: g/L，TODO: 从实体获取
                                SubSystemId = subsystem.Id,
                                SubSystemName = subsystem.SubSystemName.ToString(),
                                LastCompleteTime = bulk.LastCompleteTime,
                                FormulaItems = ParseFormulaItems(bulk.DataFomulaArray?.GetCurrentValue())
                            });
                        }
                    }
                }

                return ApiResponse<List<ChemicalBucketDto>>.Success(result);
            }
            catch (Exception ex)
            {
                return ApiResponse<List<ChemicalBucketDto>>.Fail(ex.Message);
            }
        }

        /// <summary>
        /// Update concentration for a chemical bucket
        /// 单位：克/升，用于计算助剂消耗量
        /// 计算公式：消耗量 = 体积(L) × 浓度 ÷ 1000
        /// </summary>
        /// <param name="dto">Update concentration request</param>
        /// <returns>Success indicator</returns>
        [HttpPut]
        public async Task<ApiResponse<bool>> Concentration([FromBody] UpdateConcentrationDto dto)
        {
            try
            {
                // TODO: Implement concentration update logic
                // 1. Save to database
                // 2. Sync to PLC via Dom pattern

                return ApiResponse<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.Fail(ex.Message);
            }
        }

        /// <summary>
        /// Parse formula string to list of formula items
        /// </summary>
        private List<FormulaItemDto>? ParseFormulaItems(string? formulaString)
        {
            if (string.IsNullOrEmpty(formulaString))
                return null;

            var items = new List<FormulaItemDto>();
            // 使用 RemoveEmptyEntries 避免最后一位是 'A' 时产生空元素
            var parts = formulaString.Split(['A'], StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < parts.Length; i++)
            {
                if (float.TryParse(parts[i], out float value) && value > 0)
                {
                    items.Add(new FormulaItemDto
                    {
                        AssName = $"Pipeline {i + 1}",
                        PlanVolume = value,
                        RealVolume = 0 // TODO: Get from real litre array
                    });
                }
            }

            return items.Count > 0 ? items : null;
        }
    }
}
