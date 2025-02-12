using EcoState.Context;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace EcoState.Extensions;

public static class Migration
{
    /// <summary>
    /// Запуск миграций для обновления БД
    /// </summary>
    /// <param name="app"></param>
    public static void RunMigration(this IHost app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        try
        {
            dbContext.Database.Migrate();
            Log.Information("Миграция успешно выполнена");
        }
        catch (Exception ex)
        {
            Log.Error($"Ошибка при миграции: {ex.Message}");
            Environment.Exit(1);
        }
    }
}