namespace FluentMigrator.Generator.Core;

public class MigrationsBuilder
{
    private static string _prefix = "Migrations";
    private static Lazy<MigrationsBuilder> Lazy => new Lazy<MigrationsBuilder>(new MigrationsBuilder());

    public static MigrationsBuilder Instance => Lazy.Value;

    public string Type { get; set; }


    static MigrationsBuilder()
    {
    }

    private MigrationsBuilder()
    {
    }

    public MigrationsBuilder Init(string[] arguments)
    {
        Arguments = arguments;

        return this;
    }
    private string[] Arguments { get; set; }

    public void Build()
    {
        
    }
}