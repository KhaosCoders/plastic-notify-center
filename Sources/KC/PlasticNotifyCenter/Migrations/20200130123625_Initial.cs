using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PlasticNotifyCenter.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    Origin = table.Column<int>(nullable: false),
                    LdapGuid = table.Column<string>(nullable: true),
                    IsBuildIn = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    UserName = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(maxLength: 256, nullable: true),
                    Email = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    PasswordHash = table.Column<string>(nullable: true),
                    SecurityStamp = table.Column<string>(nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                    TwoFactorEnabled = table.Column<bool>(nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                    LockoutEnabled = table.Column<bool>(nullable: false),
                    AccessFailedCount = table.Column<int>(nullable: false),
                    Origin = table.Column<int>(nullable: false),
                    LdapGuid = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LdapSettings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LdapDcHost = table.Column<string>(nullable: true),
                    LdapDcPort = table.Column<int>(nullable: false),
                    LdapDcSSL = table.Column<bool>(nullable: false),
                    LdapBaseDN = table.Column<string>(nullable: true),
                    LdapUserDN = table.Column<string>(nullable: true),
                    LdapGroupDN = table.Column<string>(nullable: true),
                    LdapUserFilter = table.Column<string>(nullable: true),
                    LdapGroupFilter = table.Column<string>(nullable: true),
                    LdapUserGuidAttr = table.Column<string>(nullable: true),
                    LdapUserNameAttr = table.Column<string>(nullable: true),
                    LdapUserEmailAttr = table.Column<string>(nullable: true),
                    LdapGroupGuidAttr = table.Column<string>(nullable: true),
                    LdapGroupNameAttr = table.Column<string>(nullable: true),
                    LdapMember = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LdapSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NotificationHistory",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Timestamp = table.Column<DateTime>(nullable: false),
                    NotifierName = table.Column<string>(nullable: false),
                    SuccessCount = table.Column<int>(nullable: false),
                    FailedCount = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationHistory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Notifiers",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    DisplayName = table.Column<string>(nullable: false),
                    Version = table.Column<string>(nullable: true),
                    Discriminator = table.Column<string>(nullable: false),
                    Host = table.Column<string>(nullable: true),
                    EnableSSL = table.Column<bool>(nullable: true),
                    Port = table.Column<int>(nullable: true),
                    SenderMail = table.Column<string>(nullable: true),
                    SenderAlias = table.Column<string>(nullable: true),
                    Username = table.Column<string>(nullable: true),
                    Password = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifiers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TriggerHistory",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Trigger = table.Column<string>(nullable: false),
                    TimeStamp = table.Column<DateTime>(nullable: false),
                    Input = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TriggerHistory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TriggerVariables",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Trigger = table.Column<string>(nullable: false),
                    Variable = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TriggerVariables", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RoleId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(maxLength: 128, nullable: false),
                    ProviderKey = table.Column<string>(maxLength: 128, nullable: false),
                    ProviderDisplayName = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    RoleId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    LoginProvider = table.Column<string>(maxLength: 128, nullable: false),
                    Name = table.Column<string>(maxLength: 128, nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Rules",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    DisplayName = table.Column<string>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    OwnerId = table.Column<string>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    AdvancedFilter = table.Column<string>(nullable: true),
                    Trigger = table.Column<string>(nullable: false),
                    NotificationTitle = table.Column<string>(nullable: true),
                    NotificationBody = table.Column<string>(nullable: false),
                    NotificationBodyType = table.Column<int>(nullable: false),
                    UseGlobalMessageTemplate = table.Column<bool>(nullable: false),
                    NotificationTags = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rules_AspNetUsers_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AppSettings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BaseUrl = table.Column<string>(nullable: false),
                    AllowRegistration = table.Column<bool>(nullable: false),
                    HtmlMessageTemplate = table.Column<string>(nullable: true),
                    LdapSyncTimestamp = table.Column<DateTime>(nullable: false),
                    LdapConfigId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppSettings_LdapSettings_LdapConfigId",
                        column: x => x.LdapConfigId,
                        principalTable: "LdapSettings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NotificationRecipients",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<string>(nullable: true),
                    RoleId = table.Column<string>(nullable: true),
                    NotificationRuleId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationRecipients", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NotificationRecipients_Rules_NotificationRuleId",
                        column: x => x.NotificationRuleId,
                        principalTable: "Rules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NotificationRecipients_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NotificationRecipients_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RuleNotifiers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RuleId = table.Column<string>(nullable: true),
                    NotifierId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RuleNotifiers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RuleNotifiers_Notifiers_NotifierId",
                        column: x => x.NotifierId,
                        principalTable: "Notifiers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RuleNotifiers_Rules_RuleId",
                        column: x => x.RuleId,
                        principalTable: "Rules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppSettings_LdapConfigId",
                table: "AppSettings",
                column: "LdapConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NotificationRecipients_NotificationRuleId",
                table: "NotificationRecipients",
                column: "NotificationRuleId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationRecipients_RoleId",
                table: "NotificationRecipients",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationRecipients_UserId",
                table: "NotificationRecipients",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RuleNotifiers_NotifierId",
                table: "RuleNotifiers",
                column: "NotifierId");

            migrationBuilder.CreateIndex(
                name: "IX_RuleNotifiers_RuleId",
                table: "RuleNotifiers",
                column: "RuleId");

            migrationBuilder.CreateIndex(
                name: "IX_Rules_OwnerId",
                table: "Rules",
                column: "OwnerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppSettings");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "NotificationHistory");

            migrationBuilder.DropTable(
                name: "NotificationRecipients");

            migrationBuilder.DropTable(
                name: "RuleNotifiers");

            migrationBuilder.DropTable(
                name: "TriggerHistory");

            migrationBuilder.DropTable(
                name: "TriggerVariables");

            migrationBuilder.DropTable(
                name: "LdapSettings");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Notifiers");

            migrationBuilder.DropTable(
                name: "Rules");

            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }
    }
}
