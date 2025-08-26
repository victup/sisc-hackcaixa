# SISC - Sistema de Simulações Caixa

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
