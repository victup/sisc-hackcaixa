using System.ComponentModel;
using System.Reflection;

namespace SISC.Models.Enums
{
    public enum FuncionalidadeEnum
    {
        // Simulações
        [Description("POST/api/v1/simulacoes/simular")]
        SM_001_SIMULAR,

        [Description("GET/api/v1/simulacoes")]
        SM_002_SIMULACOES,

        [Description("GET/api/v1/simulacoes/por-data/{data:datetime}")]
        SM_003_SIMULACOES_DATA,

        // Telemetria
        [Description("GET/api/v1/telemetria")]
        TL_001_TELEMETRIA,

        // Relatório
        [Description("GET/api/v1/relatorio")]
        RL_001_REL_IA
    }

}
