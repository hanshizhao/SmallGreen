using Mapster;
using SmallGreen.Dto.Base;
using SmallGreen.Entity.Basic;

namespace SmallGreen.API.Configuration
{
    public static class MapsterConfiguration
    {
        public static void Configure()
        {
            TypeAdapterConfig<LevelGaugeModule, LevelGaugeModuleDto>.NewConfig();
            //TypeAdapterConfig<LevelGaugeModuleDto, LevelGaugeModule>.NewConfig();


            TypeAdapterConfig<TemperatureModule, TemperatureModuleDto>.NewConfig();
            TypeAdapterConfig<TemperatureModuleDto, TemperatureModule>.NewConfig();

            TypeAdapterConfig<ClearModule, ClearModuleDto>
               .NewConfig()
               .Map(dest => dest.Id, src => src.Id)
               .Map(dest => dest.BlendPlanMinute, src => src.DomBlendPlanMinute.GetCurrentValue())
               .Map(dest => dest.DelayPlanMinute, src => src.DomBlendPlanMinute.GetCurrentValue())
               .Map(dest => dest.DelayRealMinute, src => src.DomBlendPlanMinute.GetCurrentValue())
               .Map(dest => dest.LoopPlanMinute, src => src.DomBlendPlanMinute.GetCurrentValue())
               .Map(dest => dest.WaterPlanLitre, src => src.DomBlendPlanMinute.GetCurrentValue())
               .Map(dest => dest.EmptyLitre, src => src.DomBlendPlanMinute.GetCurrentValue())
               ;

            TypeAdapterConfig<PumpModule, PumpModuleDto>
                .NewConfig()
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.IsLoopping, src => src.DomStartLoop.GetCurrentValue())
                .Map(dest => dest.SendPower, src => src.DomSendPower == null ? 0f : src.DomSendPower.GetCurrentValue())
                .Map(dest => dest.LoopPower, src => src.DomLoopPower == null ? 0f : src.DomLoopPower.GetCurrentValue())
                .Map(dest => dest.DelayPlanMinute, src => src.DomDelayPlanMinute == null ? 0f : src.DomDelayPlanMinute.GetCurrentValue())
                .Map(dest => dest.DelayRealMinute, src => src.DomDelayRealMinute == null ? 0f : src.DomDelayRealMinute.GetCurrentValue())
                .Map(dest => dest.LoopPlanMinute, src => src.DomLoopPlanMinute == null ? 0f : src.DomLoopPlanMinute.GetCurrentValue())
                .Map(dest => dest.LoopRealMinute, src => src.DomLoopRealMinute == null ? 0f : src.DomLoopRealMinute.GetCurrentValue())
                .Map(dest => dest.IfEmptyLitre, src => src.DomIfEmptyLitre == null ? 0f : src.DomIfEmptyLitre.GetCurrentValue())
                ;

        }
    }
}
