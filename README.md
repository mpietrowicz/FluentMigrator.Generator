# FluentMigrator.Generator

## migration generation tool for FluentMigrations
- command  : fluentgenerate nameofmigration
- creates a new directory in the current execution path named `Migrations`

Example Result :

```c#
using FluentMigrator;

namespace Migrations
{
    [Migration("07022022161738")]
    public class nameofmigration : Migration
    {
        public override void Up()
        {
        }

        public override void Down()
        {
        }
    }
}
```
