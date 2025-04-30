using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;

namespace umfgcloud.apresentacao.identityRepositorio.Contexto;

public sealed class MySqlContexto : IdentityDbContext
{
    public MySqlContexto() => Migrar();
    public MySqlContexto(DbContextOptions<MySqlContexto> options)
        : base(options) => Migrar();

    private void Migrar()
    {
        if (!IsBancoCriado() || Database.GetPendingMigrations().Any())
            Database.Migrate();
    }

    private bool IsBancoCriado()
    {
        try
        {
            using var connection = new MySqlConnection(Database.GetConnectionString());
            connection.Open();
            return true;
        }
        catch (MySqlException ex)
        {
            if (ex.Number == 1049)
                return false;

            throw;
        }
    }
}