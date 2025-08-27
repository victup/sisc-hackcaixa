# SISC - Sistema de Simulações Caixa

## Sobre o SISC

O SISC (Sistema de Simulação de Crédito Caixa) é uma API desenvolvida para gerenciar simulações de produtos financeiros, com foco em organização, clareza e boas práticas de arquitetura.
O projeto foi construído seguindo princípios de SOLID, Clean Code e com documentação detalhada via Swagger e README estruturado, garantindo fácil manutenção e entendimento.

Além das funcionalidades básicas de criação, listagem e consulta de simulações, o sistema traz alguns diferenciais importantes:

- Documentação completa e organizada, descrevendo endpoints, exemplos de requisição e resposta.

- Boas práticas de desenvolvimento, utilizando separação de responsabilidades, injeção de dependência e camadas bem definidas.

- Serviço de Relatório com IA, capaz de gerar insights estratégicos a partir dos dados das simulações, enriquecendo a análise dos produtos.

- Telemetria com cache inteligente, permitindo monitoramento de desempenho dos endpoints e reduzindo sobrecarga de consultas repetitivas.

- Paginação e filtros otimizados, garantindo performance em consultas de grandes volumes de dados.


## Pré-requisitos

- [Docker](https://www.docker.com/get-started) instalado e em execução  
- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) (opcional, caso queira rodar fora do container)  
- [SQL Server](https://www.microsoft.com/pt-br/sql-server/sql-server-downloads) em execução (local ou remoto)  

## Configuração do Banco de Dados

O projeto utiliza **dois bancos**:  
- **SQLite** para armazenar simulações  
- **SQL Server** para armazenar produtos  

As strings de conexão são definidas via variáveis de ambiente no `docker run`.

---

## Passo a passo para rodar com Docker

### 1. Limpar ambiente antigo (opcional)

```powershell
docker rm -f sisc-api 2>$null
docker rmi -f sisc-api 2>$null
docker volume rm -f sisc-sqlite
```
### 2. Criar volume do SQLite

```powershell
docker volume create sisc-sqlite
```

### 3. Buildar a imagem

```powershell
docker build --no-cache -t sisc-api .
```

### 4. Executar o container

```powershell
docker run -d --name sisc-api -p 5000:8080 `
  -e ASPNETCORE_ENVIRONMENT=Development `
  -e ConnectionStrings__SimulacoesDb="Data Source=/app/data/simulacoes.db" `
  -e ConnectionStrings__ProdutosDb="Server=host.docker.internal,1433;Database=SISC;User Id=sa;Password=Sa@2024!;TrustServerCertificate=True;" `
  --add-host=host.docker.internal:host-gateway `
  -v sisc-sqlite:/app/data `
  sisc-api
```

## Endpoints principais
### Relatório com IA
#### GET /api/v1/relatorio
```json
{
  "dataReferencia": "2025-08-27T11:17:05.6691347Z",
  "parecerIA": "Com base nos dados fornecidos sobre as simulações dos quatro produtos, podemos extrair os seguintes insights estratégicos:\n\n1. **Produto 1**: Este produto mostrou um interesse significativo, já que foi simulado duas vezes. O valor médio simulado é de ¤160,000.00, com um prazo mais frequente de 123 meses. Isso indica que os clientes ou usuários possivelmente buscam um investimento de longo prazo e estão confortáveis com um alto valor médio. Estratégias para este produto podem focar em destacar os benefícios de longo prazo e potencial retornos sobre o investimento. Além disso, verificar complementos que possam aumentar o valor percebido seria interessante.\n\n2. **Produto 2**: Teve apenas uma simulação, o que pode indicar um interesse inicial ainda em teste ou uma baixa atratividade em relação ao Produto 1 para os clientes. O valor médio também é de ¤160,000.00, mas com um prazo mais curto, de 77 meses. Isso sugere que poderíamos posicionar o Produto 2 para aqueles que buscam retornos no médio prazo. Analisar o feedback de potenciais consumidores ou ajustar a comunicação para enfatizar vantagens únicas poderia aumentar seu apelo.\n\n3. **Produto 3** e **Produto 4**: Ambos ainda não tiveram nenhuma simulação, indicando desinteresse ou falta de visibilidade no mercado. É crucial rever o pacote de benefícios oferecido, estudar o posicionamento de mercado e talvez realizar uma análise competitiva para entender por que esses produtos não estão atraindo simulações. Pode ser necessário reformular a estratégia de marketing ou realizar campanhas de conscientização para aumentar a visibilidade desses produtos.\n\n**Considerações gerais**:\n\n- **Marketing e Comunicação**: É essencial fortalecer as campanhas de marketing para os Produtos 3 e 4 enquanto se mantém e se potencializa o interesse no Produto 1.\n  \n- **Pesquisa de Mercado**: Realizar estudos para identificar barreiras à entrada ou causas de desinteresse nos produtos menos simulados e ajustar estratégias conforme necessário.\n\n- **Insights do Cliente**: Coletar feedback direto de consumidores potenciais e atuais para entender melhor suas necessidades e ajustar as ofertas de produtos.\n\n- **Diversificação de Estratégia**: Considerar a diversificação do portfólio ou ajustar condições de venda, como prazos e valores médios, para atingir diferentes segmentos de mercado.\n\nAtravés dessas abordagens, pode-se aumentar a atratividade dos produtos simulados em menor quantidade e otimizar a performance dos produtos já em demanda.",
  "insights": [
    {
      "codigoProduto": "1",
      "nomeProduto": "Produto 1",
      "totalSimulacoes": 2,
      "valorMedioDesejado": 160000,
      "prazoMaisFrequente": 123,
      "tendencia": "Alta procura por longo prazo"
    },
    {
      "codigoProduto": "2",
      "nomeProduto": "Produto 2",
      "totalSimulacoes": 1,
      "valorMedioDesejado": 160000,
      "prazoMaisFrequente": 77,
      "tendencia": "Preferência por curto/médio prazo"
    },
    {
      "codigoProduto": "3",
      "nomeProduto": "Produto 3",
      "totalSimulacoes": 0,
      "valorMedioDesejado": 0,
      "prazoMaisFrequente": 0,
      "tendencia": "Sem simulações ainda"
    },
    {
      "codigoProduto": "4",
      "nomeProduto": "Produto 4",
      "totalSimulacoes": 0,
      "valorMedioDesejado": 0,
      "prazoMaisFrequente": 0,
      "tendencia": "Sem simulações ainda"
    }
  ]
}
```

### Criar simulação
#### POST /api/v1/simulacoes/simular
```json
{
  "valorDesejado": 10000,
  "prazo": 12
}
```
#### Exemplo de response
```json
{
  "idSimulacao": 1,
  "codigoProduto": "P001",
  "descricaoProduto": "Crédito Consignado",
  "taxaJuros": 0.02,
  "resultadoSimulacao": [
    {
      "tipo": "SAC",
      "parcelas": [
        {
          "numero": 1,
          "valorAmortizacao": 833.33,
          "valorJuros": 200.00,
          "valorPrestacao": 1033.33
        }
      ]
    },
    {
      "tipo": "PRICE",
      "parcelas": [
        {
          "numero": 1,
          "valorAmortizacao": 816.67,
          "valorJuros": 200.00,
          "valorPrestacao": 1016.67
        }
      ]
    }
  ]
}
```

### Listar simulações (com paginação)
#### GET /api/v1/simulacoes?pagina=1&qtdPorPagina=10
#### Exemplo de response
```json
{
  "pagina": 1,
  "qtdRegistros": 5,
  "qtdRegistrosPagina": 5,
  "registros": [
    {
      "idSimulacao": 1,
      "valorDesejado": 10000,
      "prazo": 12,
      "valorTotalParcelas": 12000
    }
  ]
}
```

### Buscar simulações por data
#### GET /api/v1/simulacoes/por-data/2025-08-26'
#### Exemplo de response
```json
{
  "dataReferencia": "2025-08-21",
  "simulacoes": [
    {
      "codigoProduto": "P001",
      "descricaoProduto": "Crédito Consignado",
      "taxaMediaJuro": 0.02,
      "valorMedioPrestacao": 1000,
      "valorTotalDesejado": 10000,
      "valorTotalCredito": 12000
    }
  ]
}
```

### Telemetria
#### GET /api/v1/telemetria
#### Exemplo de response
```json
{
  "dataReferencia": "2025-08-26T01:57:50.705Z",
  "listaEndpoints": [
    {
      "nomeApi": "Simulação",
      "qtdRequisicoes": 128,
      "tempoMedio": 152,
      "tempoMinimo": 45,
      "tempoMaximo": 430,
      "percentualSucesso": 97.5
    },
    {
      "nomeApi": "Produtos",
      "qtdRequisicoes": 87,
      "tempoMedio": 98,
      "tempoMinimo": 30,
      "tempoMaximo": 220,
      "percentualSucesso": 100
    }
  ]
}
```
