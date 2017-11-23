namespace WebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
            "dbo.UserActivityLogs",
                    c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 64),
                        UserName = c.String(nullable: false, maxLength: 64),
                        Timestamp = c.DateTime(nullable: false),
                        FunctionPerfomed = c.String(maxLength: 256),
                        IPAddress = c.String(maxLength: 256),
                        CreatedUtc = c.DateTime(nullable: false),
                        UpdatedUtc = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);

            CreateTable(
            "dbo.ProtoContents",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        ContentTypeId = c.String(nullable: false, maxLength: 64),
                        CreatedByUserId = c.String(maxLength: 128),
                        CreatedByUserName = c.String(maxLength: 256),
                        CreatedByUserDisplayName = c.String(maxLength: 256),
                        CreatedUtc = c.DateTime(nullable: false),
                        UpdatedUtc = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ProtoFields",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        ContentId = c.String(nullable: false, maxLength: 128),
                        FieldName = c.String(nullable: false, maxLength: 64),
                        StringValue = c.String(),
                        NumberValue = c.Decimal(precision: 21, scale: 6),
                        BooleanValue = c.Boolean(),
                        DateTimeValue = c.DateTime(),
                        CreatedUtc = c.DateTime(nullable: false),
                        UpdatedUtc = c.DateTime(nullable: false),
                        FieldClassTypeName = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ProtoContents", t => t.ContentId, cascadeDelete: true)
                .Index(t => t.ContentId);
            
            CreateTable(
                "dbo.ProtoOAuthClients",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        SecretHash = c.String(nullable: false, maxLength: 256),
                        Name = c.String(nullable: false, maxLength: 128),
                        ApplicationType = c.Int(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        RefreshTokenLifetimeSeconds = c.Int(nullable: false),
                        AllowedOriginsCsv = c.String(nullable: false, maxLength: 512),
                        CreatedUtc = c.DateTime(nullable: false),
                        UpdatedUtc = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ProtoOAuthRefreshTokens",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Subject = c.String(nullable: false, maxLength: 128),
                        ClientId = c.String(nullable: false, maxLength: 128),
                        IssuedUtc = c.DateTime(nullable: false),
                        ExpiresUtc = c.DateTime(nullable: false),
                        ProtectedTicket = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ProtoOAuthClients", t => t.ClientId, cascadeDelete: true)
                .Index(t => new { t.Subject, t.ClientId }, unique: true, name: "UQ_Subject_ClientId");
            
            CreateTable(
                "dbo.ProtoPermissionsMaps",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        RoleName = c.String(nullable: false, maxLength: 128),
                        PermissionId = c.String(nullable: false, maxLength: 128),
                        HasPermission = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => new { t.RoleName, t.PermissionId }, unique: true, name: "UQ_PermissionsMap");
            
            CreateTable(
                "dbo.ProtoSettingFields",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        SettingId = c.String(nullable: false, maxLength: 128),
                        FieldName = c.String(nullable: false),
                        StringValue = c.String(),
                        NumberValue = c.Decimal(precision: 21, scale: 6),
                        BooleanValue = c.Boolean(),
                        DateTimeValue = c.DateTime(),
                        CreatedUtc = c.DateTime(nullable: false),
                        UpdatedUtc = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.SettingId, name: "IX_ProtoSettingField_SettingId");
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        CreatedUtc = c.DateTime(nullable: false),
                        UpdatedUtc = c.DateTime(nullable: false),
                        Description = c.String(maxLength: 512),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        CreatedUtc = c.DateTime(nullable: false),
                        UpdatedUtc = c.DateTime(nullable: false),
                        DisplayName = c.String(nullable: false, maxLength: 256),
                        IsActivated = c.Boolean(nullable: false),
                        PhotoUrl = c.String(maxLength: 256),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.ProtoOAuthRefreshTokens", "ClientId", "dbo.ProtoOAuthClients");
            DropForeignKey("dbo.ProtoFields", "ContentId", "dbo.ProtoContents");
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.ProtoSettingFields", "IX_ProtoSettingField_SettingId");
            DropIndex("dbo.ProtoPermissionsMaps", "UQ_PermissionsMap");
            DropIndex("dbo.ProtoOAuthRefreshTokens", "UQ_Subject_ClientId");
            DropIndex("dbo.ProtoFields", new[] { "ContentId" });
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.ProtoSettingFields");
            DropTable("dbo.ProtoPermissionsMaps");
            DropTable("dbo.ProtoOAuthRefreshTokens");
            DropTable("dbo.ProtoOAuthClients");
            DropTable("dbo.ProtoFields");
            DropTable("dbo.ProtoContents");
        }
    }
}
