using System.Data.Entity;
using System.Data.Entity.Migrations;

namespace JrzAsp.Lib.EntityFrameworkUtilities {
    /// <summary>
    ///     Migrate DB to another version just like when publishing to Azure, but with this
    ///     we can do it via code, preferably in Global.asax Application_Start
    /// </summary>
    /// <typeparam name="TDbContext">DbContext type</typeparam>
    /// <typeparam name="TMigrationConfig">
    ///     MigrationConfig class type, by default inside ~/Migrations/Configuration.cs of a
    ///     standard scaffolded web project.
    /// </typeparam>
    public class DatabaseMigrator<TDbContext, TMigrationConfig>
        where TDbContext : DbContext
        where TMigrationConfig : DbMigrationsConfiguration<TDbContext>, new() {
        /// <summary>
        ///     Do the DB migration
        /// </summary>
        /// <param name="allowDataLoss">If true, then will make db changes even though it can result to data loss</param>
        /// <param name="targetMigration">The target migration, if null, then migrate to latest.</param>
        public void MigrateToLatestVersion(bool allowDataLoss, string targetMigration = null) {
            var cfg = new TMigrationConfig {AutomaticMigrationDataLossAllowed = allowDataLoss};
            var migrator = new DbMigrator(cfg);
            if (string.IsNullOrWhiteSpace(targetMigration)) {
                migrator.Update();
            } else {
                migrator.Update(targetMigration);
            }
        }
    }
}