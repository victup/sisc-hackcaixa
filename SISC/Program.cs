using Microsoft.EntityFrameworkCore;
using SISC.Data;
using SISC.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ProdutosDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("ProdutosDb"),
        sql => sql.EnableRetryOnFailure()
    )
);

builder.Services.AddDbContext<SimulacoesDbContext>(options =>
    options.UseSqlite(
        builder.Configuration.GetConnectionString("SimulacoesDb")
    )
);

builder.Services.AddSiscServices();

var app = builder.Build();

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