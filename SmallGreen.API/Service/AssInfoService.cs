using Mapster;
using SmallGreen.API.IService;
using SmallGreen.Dto.Base;
using SmallGreen.Dto.Data;
using SmallGreen.Dto.Machine;
using SmallGreen.Entity;
using SmallGreen.Entity.Data;

namespace SmallGreen.API.Service
{
    public class AssInfoService : IAssInfoService
    {
        public Task<ApiResponse<List<MixedDetailDto>>> GetDetail(long prcsDataId)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<PageInfo<AssInfoDto>>> GetList()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 获取所有助剂桶列表（包含混合组分）
        /// </summary>
        public async Task<ApiResponse<List<AssBucketDto>>> GetBuckets()
        {
            try
            {
                var result = await AssInfo.GetAssInfo();
                if (!result.IsSuccess)
                {
                    return new ApiResponse<List<AssBucketDto>>
                    {
                        IsSuccess = false,
                        Message = result.Message ?? "查询助剂桶列表失败"
                    };
                }

                var list = result.Content ?? [];
                var dtoList = list.Select(ass => new AssBucketDto
                {
                    Id = ass.Id,
                    SubSystemName = GetSubSystemName(ass.Sequence),
                    Sequence = ass.Sequence,
                    CodeNumber = ass.CodeNumber,
                    Name = ass.Name,
                    Concentration = ass.Concentration,
                    MaxV = ass.MaxV,
                    IsMixed = ass.IsMixed,
                    MixedComponents = ass.ListMixedDetail?.Select(m => new MixedComponentDto
                    {
                        Id = m.Id,
                        ParentId = m.ParentId,
                        CodeNumber = m.CodeNumber,
                        Name = m.Name,
                        Concentration = m.Concentration,
                        Ratio = m.Ratio
                    }).ToList()
                }).ToList();

                return new ApiResponse<List<AssBucketDto>>
                {
                    IsSuccess = true,
                    Content = dtoList
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<AssBucketDto>>
                {
                    IsSuccess = false,
                    Message = $"查询助剂桶列表异常: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// 根据ID获取助剂桶详情
        /// </summary>
        public async Task<ApiResponse<AssBucketDto>> GetBucketById(long id)
        {
            try
            {
                var ass = await new Repository<AssInfo>().AsQueryable()
                    .Includes(it => it.ListMixedDetail)
                    .FirstAsync(it => it.Id == id);

                if (ass == null)
                {
                    return new ApiResponse<AssBucketDto>
                    {
                        IsSuccess = false,
                        Message = $"助剂桶不存在: Id={id}"
                    };
                }

                return new ApiResponse<AssBucketDto>
                {
                    IsSuccess = true,
                    Content = new AssBucketDto
                    {
                        Id = ass.Id,
                        SubSystemName = GetSubSystemName(ass.Sequence),
                        Sequence = ass.Sequence,
                        CodeNumber = ass.CodeNumber,
                        Name = ass.Name,
                        Concentration = ass.Concentration,
                        MaxV = ass.MaxV,
                        IsMixed = ass.IsMixed,
                        MixedComponents = ass.ListMixedDetail?.Select(m => new MixedComponentDto
                        {
                            Id = m.Id,
                            ParentId = m.ParentId,
                            CodeNumber = m.CodeNumber,
                            Name = m.Name,
                            Concentration = m.Concentration,
                            Ratio = m.Ratio
                        }).ToList()
                    }
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<AssBucketDto>
                {
                    IsSuccess = false,
                    Message = $"查询助剂桶详情异常: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// 更新助剂桶基本信息
        /// </summary>
        public async Task<ApiResponse<AssBucketDto>> UpdateBucket(UpdateAssBucketDto dto)
        {
            try
            {
                var repo = new Repository<AssInfo>();
                var ass = await repo.AsQueryable().FirstAsync(it => it.Id == dto.Id);
                if (ass == null)
                {
                    return new ApiResponse<AssBucketDto>
                    {
                        IsSuccess = false,
                        Message = $"助剂桶不存在: Id={dto.Id}"
                    };
                }

                // 更新字段
                ass.CodeNumber = dto.CodeNumber;
                ass.Name = dto.Name;
                ass.Concentration = dto.Concentration;
                ass.IsMixed = dto.IsMixed;

                var success = await repo.UpdateAsync(ass);
                if (!success)
                {
                    return new ApiResponse<AssBucketDto>
                    {
                        IsSuccess = false,
                        Message = "更新助剂桶失败"
                    };
                }

                return new ApiResponse<AssBucketDto>
                {
                    IsSuccess = true,
                    Content = new AssBucketDto
                    {
                        Id = ass.Id,
                        SubSystemName = GetSubSystemName(ass.Sequence),
                        Sequence = ass.Sequence,
                        CodeNumber = ass.CodeNumber,
                        Name = ass.Name,
                        Concentration = ass.Concentration,
                        MaxV = ass.MaxV,
                        IsMixed = ass.IsMixed
                    }
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<AssBucketDto>
                {
                    IsSuccess = false,
                    Message = $"更新助剂桶异常: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// 新增混合组分
        /// </summary>
        public async Task<ApiResponse<MixedComponentDto>> CreateMixedComponent(CreateMixedComponentDto dto)
        {
            try
            {
                // 验证父级助剂桶存在且为混合类型
                var assRepo = new Repository<AssInfo>();
                var parentAss = await assRepo.AsQueryable().FirstAsync(it => it.Id == dto.ParentId);
                if (parentAss == null)
                {
                    return new ApiResponse<MixedComponentDto>
                    {
                        IsSuccess = false,
                        Message = $"父级助剂桶不存在: Id={dto.ParentId}"
                    };
                }

                if (!parentAss.IsMixed)
                {
                    return new ApiResponse<MixedComponentDto>
                    {
                        IsSuccess = false,
                        Message = $"父级助剂桶不是混合类型，无法添加混合组分"
                    };
                }

                var detail = new MixedDetail
                {
                    ParentId = dto.ParentId,
                    CodeNumber = dto.CodeNumber,
                    Name = dto.Name,
                    Concentration = dto.Concentration,
                    Ratio = dto.Ratio
                };

                var repo = new Repository<MixedDetail>();
                // 使用 AsInsertable 显式忽略自增列
                var id = await repo.AsInsertable(detail).ExecuteReturnBigIdentityAsync();

                return new ApiResponse<MixedComponentDto>
                {
                    IsSuccess = true,
                    Content = new MixedComponentDto
                    {
                        Id = id,
                        ParentId = dto.ParentId,
                        CodeNumber = dto.CodeNumber,
                        Name = dto.Name,
                        Concentration = dto.Concentration,
                        Ratio = dto.Ratio
                    }
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<MixedComponentDto>
                {
                    IsSuccess = false,
                    Message = $"新增混合组分异常: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// 更新混合组分
        /// </summary>
        public async Task<ApiResponse<MixedComponentDto>> UpdateMixedComponent(UpdateMixedComponentDto dto)
        {
            try
            {
                var repo = new Repository<MixedDetail>();
                var detail = await repo.AsQueryable().FirstAsync(it => it.Id == dto.Id);
                if (detail == null)
                {
                    return new ApiResponse<MixedComponentDto>
                    {
                        IsSuccess = false,
                        Message = $"混合组分不存在: Id={dto.Id}"
                    };
                }

                detail.CodeNumber = dto.CodeNumber;
                detail.Name = dto.Name;
                detail.Concentration = dto.Concentration;
                detail.Ratio = dto.Ratio;

                var success = await repo.UpdateAsync(detail);
                if (!success)
                {
                    return new ApiResponse<MixedComponentDto>
                    {
                        IsSuccess = false,
                        Message = "更新混合组分失败"
                    };
                }

                return new ApiResponse<MixedComponentDto>
                {
                    IsSuccess = true,
                    Content = new MixedComponentDto
                    {
                        Id = detail.Id,
                        ParentId = detail.ParentId,
                        CodeNumber = detail.CodeNumber,
                        Name = detail.Name,
                        Concentration = detail.Concentration,
                        Ratio = detail.Ratio
                    }
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<MixedComponentDto>
                {
                    IsSuccess = false,
                    Message = $"更新混合组分异常: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// 删除混合组分
        /// </summary>
        public async Task<ApiResponse<bool>> DeleteMixedComponent(long id)
        {
            try
            {
                var repo = new Repository<MixedDetail>();
                var detail = await repo.AsQueryable().FirstAsync(it => it.Id == id);
                if (detail == null)
                {
                    return new ApiResponse<bool>
                    {
                        IsSuccess = false,
                        Message = $"混合组分不存在: Id={id}"
                    };
                }

                var success = await repo.DeleteAsync(detail);
                return new ApiResponse<bool>
                {
                    IsSuccess = success,
                    Content = success,
                    Message = success ? string.Empty : "删除混合组分失败"
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool>
                {
                    IsSuccess = false,
                    Message = $"删除混合组分异常: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// 根据 Sequence 获取子系统名称
        /// </summary>
        private static string GetSubSystemName(int sequence)
        {
            // Sequence 1-12: 前处理系统
            // Sequence 13+: 固色系统
            return sequence >= 13 ? "固色" : "前处理";
        }
    }
}
