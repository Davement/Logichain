using System.Diagnostics;
#pragma warning disable CS0162

namespace Migrations.Test;

public class CreateMigrationTests
{
    private const string MigrationName = "NAME";

    [Test]
    public void CreateMigration()
    {
        if (MigrationName != "NAME")
        {
            var migrationsCount = GetCountOfMigrations();
            var createMigration = Process.Start("powershell",
                $"cd {GetMigrationsPath()}; dotnet ef migrations add {MigrationName} --startup-project ../Logichain");
            createMigration.WaitForExit();
            
            Assert.That(GetCountOfMigrations(), Is.EqualTo(migrationsCount + 2), $"Failed to create migration for {MigrationName}");
            Assert.Pass($"Migration {MigrationName} successfully created");
        }
        Assert.Ignore();
    }

    [Test]
    public void WhenMigrationNameIsDefault_ThenAccepted()
    {
        Assert.That(MigrationName, Is.EqualTo("NAME"));
    }

    private static int GetCountOfMigrations()
    {
        return Directory.GetFiles(GetMigrationsPath() + "\\Migrations", "*", SearchOption.TopDirectoryOnly).Length;
    }

    private static string GetMigrationsPath()
    {
        var endPathIndex = Directory.GetCurrentDirectory().IndexOf("Migrations.Tests", StringComparison.Ordinal);
        return Directory.GetCurrentDirectory().Substring(0, endPathIndex) + "Migrations";
    }
}