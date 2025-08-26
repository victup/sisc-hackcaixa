using SISC.DTOs.Responses.Simulacao;
using SISC.DTOs.Simulacao;
using SISC.Models.Simulacao;
using AutoMapper;

namespace SISC.Mappings
{
    public class SimulacaoProfile : Profile
    {
        public SimulacaoProfile()
        {
            CreateMap<Simulacao, SimulacaoCreateResponse>()
                .ForMember(dest => dest.IdSimulacao, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ResultadoSimulacao, opt => opt.MapFrom(src => src.Resultados));

            CreateMap<ResultadoSimulacao, ResultadoSimulacaoCreateResponse>();
            CreateMap<Parcela, ParcelaCreateResponse>();

            CreateMap<Simulacao, ResultadoSimulacaoGetAllResponse>()
                .ForMember(dest => dest.IdSimulacao, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.valorDesejado, opt => opt.MapFrom(src => src.ValorDesejado))
                .ForMember(dest => dest.Prazo, opt => opt.MapFrom(src => src.Prazo))
                .ForMember(dest => dest.valorTotalParcelas, opt =>
                    opt.MapFrom(src => src.Resultados.SelectMany(r => r.Parcelas).Sum(p => p.ValorPrestacao))
                );
        }
    }
}
