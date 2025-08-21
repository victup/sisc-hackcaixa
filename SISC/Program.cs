using Microsoft.EntityFrameworkCore;
using SISC.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --- ProdutosDb (SQL Server local / já existente, sem migrations automáticas) ---
builder.Services.AddDbContext<ProdutosDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("ProdutosDb"),
        sql => sql.EnableRetryOnFailure()
    )
);

// --- SimulacoesDb (SQLite dentro do container) ---
builder.Services.AddDbContext<SimulacoesDbContext>(options =>
    options.UseSqlite(
        builder.Configuration.GetConnectionString("SimulacoesDb")
    )
);

var app = builder.Build();

// aplica migrations automáticas SOMENTE no SimulacoesDb
using (var scope = app.Services.CreateScope())
{
    var simulacoesDb = scope.ServiceProvider.GetRequiredService<SimulacoesDbContext>();
    simulacoesDb.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();