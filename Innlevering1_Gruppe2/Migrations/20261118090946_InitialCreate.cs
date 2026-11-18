using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Innlevering1_Gruppe2.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Account",
                columns: table => new
                {
                    account_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    security_stamp = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    failed_attempts = table.Column<int>(type: "int", nullable: false),
                    locked_until = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    access_revoked = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    email_confirmed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    account_type = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    email = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    contact_name = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false, defaultValue: "")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    phone = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, defaultValue: "")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    password_hash = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Account", x => x.account_id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AppWriteLock",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppWriteLock", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AuditEntry",
                columns: table => new
                {
                    audit_entry_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    account_id = table.Column<int>(type: "int", nullable: true),
                    action = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    target = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    occurred_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditEntry", x => x.audit_entry_id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Location",
                columns: table => new
                {
                    location_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    address_or_area = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    latitude = table.Column<decimal>(type: "decimal(9,6)", precision: 9, scale: 6, nullable: false),
                    longitude = table.Column<decimal>(type: "decimal(10,7)", precision: 10, scale: 7, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Location", x => x.location_id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PublicOrganization",
                columns: table => new
                {
                    public_organization_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    organization_name = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    organization_type = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    organization_number = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    email_domain = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    contact_email = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    contact_name = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    phone = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    approval_status = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    verification = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PublicOrganization", x => x.public_organization_id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ResourceType",
                columns: table => new
                {
                    resource_type_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    category_name = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResourceType", x => x.resource_type_id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Skill",
                columns: table => new
                {
                    skill_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    description = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Skill", x => x.skill_id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AccountToken",
                columns: table => new
                {
                    account_token_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    security_stamp = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    account_id = table.Column<int>(type: "int", nullable: false),
                    token_hash = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    purpose = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    expires_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    used_at = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountToken", x => x.account_token_id);
                    table.ForeignKey(
                        name: "FK_AccountToken_Account_account_id",
                        column: x => x.account_id,
                        principalTable: "Account",
                        principalColumn: "account_id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ProviderAccount",
                columns: table => new
                {
                    account_id = table.Column<int>(type: "int", nullable: false),
                    provider_type = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    contact_name = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    phone = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    address = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    organization_name = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    organization_number = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProviderAccount", x => x.account_id);
                    table.ForeignKey(
                        name: "FK_ProviderAccount_Account_account_id",
                        column: x => x.account_id,
                        principalTable: "Account",
                        principalColumn: "account_id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "OrganizationInvitation",
                columns: table => new
                {
                    invitation_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    public_organization_id = table.Column<int>(type: "int", nullable: false),
                    email = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    token_hash = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    permission = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    expires_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    used_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    revoked = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationInvitation", x => x.invitation_id);
                    table.ForeignKey(
                        name: "FK_OrganizationInvitation_PublicOrganization_public_organizatio~",
                        column: x => x.public_organization_id,
                        principalTable: "PublicOrganization",
                        principalColumn: "public_organization_id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PublicActorAccount",
                columns: table => new
                {
                    account_id = table.Column<int>(type: "int", nullable: false),
                    public_organization_id = table.Column<int>(type: "int", nullable: false),
                    permission = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PublicActorAccount", x => x.account_id);
                    table.ForeignKey(
                        name: "FK_PublicActorAccount_Account_account_id",
                        column: x => x.account_id,
                        principalTable: "Account",
                        principalColumn: "account_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PublicActorAccount_PublicOrganization_public_organization_id",
                        column: x => x.public_organization_id,
                        principalTable: "PublicOrganization",
                        principalColumn: "public_organization_id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Resource",
                columns: table => new
                {
                    resource_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    account_id = table.Column<int>(type: "int", nullable: false),
                    resource_type_id = table.Column<int>(type: "int", nullable: false),
                    location_id = table.Column<int>(type: "int", nullable: false),
                    resource_name = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    description = table.Column<string>(type: "varchar(4000)", maxLength: 4000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    contact_details = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    verification_status = table.Column<string>(type: "varchar(75)", maxLength: 75, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    availability_status = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    archived = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    image_data = table.Column<byte[]>(type: "mediumblob", nullable: true),
                    image_content_type = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Resource", x => x.resource_id);
                    table.ForeignKey(
                        name: "FK_Resource_Location_location_id",
                        column: x => x.location_id,
                        principalTable: "Location",
                        principalColumn: "location_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Resource_ProviderAccount_account_id",
                        column: x => x.account_id,
                        principalTable: "ProviderAccount",
                        principalColumn: "account_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Resource_ResourceType_resource_type_id",
                        column: x => x.resource_type_id,
                        principalTable: "ResourceType",
                        principalColumn: "resource_type_id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "VolunteerProfile",
                columns: table => new
                {
                    volunteer_profile_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    account_id = table.Column<int>(type: "int", nullable: false),
                    service_area = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    volunteer_role = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    additional_information = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    archived = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VolunteerProfile", x => x.volunteer_profile_id);
                    table.ForeignKey(
                        name: "FK_VolunteerProfile_ProviderAccount_account_id",
                        column: x => x.account_id,
                        principalTable: "ProviderAccount",
                        principalColumn: "account_id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ResourceNeed",
                columns: table => new
                {
                    resource_need_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    public_actor_account_id = table.Column<int>(type: "int", nullable: false),
                    resource_type_id = table.Column<int>(type: "int", nullable: false),
                    location_id = table.Column<int>(type: "int", nullable: false),
                    title = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    description = table.Column<string>(type: "varchar(4000)", maxLength: 4000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    priority = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    status = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    required_start = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    required_end = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    deadline = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    archived = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResourceNeed", x => x.resource_need_id);
                    table.UniqueConstraint("AK_ResourceNeed_resource_need_id_public_actor_account_id", x => new { x.resource_need_id, x.public_actor_account_id });
                    table.ForeignKey(
                        name: "FK_ResourceNeed_Location_location_id",
                        column: x => x.location_id,
                        principalTable: "Location",
                        principalColumn: "location_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ResourceNeed_PublicActorAccount_public_actor_account_id",
                        column: x => x.public_actor_account_id,
                        principalTable: "PublicActorAccount",
                        principalColumn: "account_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ResourceNeed_ResourceType_resource_type_id",
                        column: x => x.resource_type_id,
                        principalTable: "ResourceType",
                        principalColumn: "resource_type_id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Availability",
                columns: table => new
                {
                    availability_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    resource_id = table.Column<int>(type: "int", nullable: true),
                    volunteer_profile_id = table.Column<int>(type: "int", nullable: true),
                    available_from = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    available_to = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Availability", x => x.availability_id);
                    table.CheckConstraint("CK_Availability_Owner", "(resource_id IS NOT NULL) <> (volunteer_profile_id IS NOT NULL)");
                    table.CheckConstraint("CK_Availability_Period", "available_to > available_from");
                    table.ForeignKey(
                        name: "FK_Availability_Resource_resource_id",
                        column: x => x.resource_id,
                        principalTable: "Resource",
                        principalColumn: "resource_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Availability_VolunteerProfile_volunteer_profile_id",
                        column: x => x.volunteer_profile_id,
                        principalTable: "VolunteerProfile",
                        principalColumn: "volunteer_profile_id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "VolunteerProfileSkill",
                columns: table => new
                {
                    volunteer_profile_id = table.Column<int>(type: "int", nullable: false),
                    skill_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VolunteerProfileSkill", x => new { x.volunteer_profile_id, x.skill_id });
                    table.ForeignKey(
                        name: "FK_VolunteerProfileSkill_Skill_skill_id",
                        column: x => x.skill_id,
                        principalTable: "Skill",
                        principalColumn: "skill_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VolunteerProfileSkill_VolunteerProfile_volunteer_profile_id",
                        column: x => x.volunteer_profile_id,
                        principalTable: "VolunteerProfile",
                        principalColumn: "volunteer_profile_id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ResourceMatch",
                columns: table => new
                {
                    resource_match_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    resource_id = table.Column<int>(type: "int", nullable: false),
                    resource_need_id = table.Column<int>(type: "int", nullable: false),
                    public_actor_account_id = table.Column<int>(type: "int", nullable: false),
                    match_status = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    assessed_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResourceMatch", x => x.resource_match_id);
                    table.UniqueConstraint("AK_ResourceMatch_resource_match_id_public_actor_account_id", x => new { x.resource_match_id, x.public_actor_account_id });
                    table.ForeignKey(
                        name: "FK_ResourceMatch_PublicActorAccount_public_actor_account_id",
                        column: x => x.public_actor_account_id,
                        principalTable: "PublicActorAccount",
                        principalColumn: "account_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ResourceMatch_ResourceNeed_resource_need_id_public_actor_acc~",
                        columns: x => new { x.resource_need_id, x.public_actor_account_id },
                        principalTable: "ResourceNeed",
                        principalColumns: new[] { "resource_need_id", "public_actor_account_id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ResourceMatch_Resource_resource_id",
                        column: x => x.resource_id,
                        principalTable: "Resource",
                        principalColumn: "resource_id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "VolunteerInterest",
                columns: table => new
                {
                    interest_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    volunteer_profile_id = table.Column<int>(type: "int", nullable: false),
                    resource_need_id = table.Column<int>(type: "int", nullable: false),
                    message = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    interest_status = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    submitted_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VolunteerInterest", x => x.interest_id);
                    table.ForeignKey(
                        name: "FK_VolunteerInterest_ResourceNeed_resource_need_id",
                        column: x => x.resource_need_id,
                        principalTable: "ResourceNeed",
                        principalColumn: "resource_need_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VolunteerInterest_VolunteerProfile_volunteer_profile_id",
                        column: x => x.volunteer_profile_id,
                        principalTable: "VolunteerProfile",
                        principalColumn: "volunteer_profile_id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Reservation",
                columns: table => new
                {
                    reservation_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    resource_match_id = table.Column<int>(type: "int", nullable: false),
                    public_actor_account_id = table.Column<int>(type: "int", nullable: false),
                    reservation_status = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    requested_start = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    requested_end = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    message = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reservation", x => x.reservation_id);
                    table.ForeignKey(
                        name: "FK_Reservation_ResourceMatch_resource_match_id_public_actor_acc~",
                        columns: x => new { x.resource_match_id, x.public_actor_account_id },
                        principalTable: "ResourceMatch",
                        principalColumns: new[] { "resource_match_id", "public_actor_account_id" },
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "AppWriteLock",
                column: "Id",
                value: 1);

            migrationBuilder.InsertData(
                table: "ResourceType",
                columns: new[] { "resource_type_id", "category_name" },
                values: new object[,]
                {
                    { 1, "Kjøretøy" },
                    { 2, "Heisekran" },
                    { 3, "Frivillige" },
                    { 4, "Tilhenger" },
                    { 5, "Utstyr" },
                    { 6, "Lokaler" },
                    { 7, "Strøm" },
                    { 8, "Vann" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Account_email",
                table: "Account",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AccountToken_account_id",
                table: "AccountToken",
                column: "account_id");

            migrationBuilder.CreateIndex(
                name: "IX_AccountToken_token_hash",
                table: "AccountToken",
                column: "token_hash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Availability_resource_id",
                table: "Availability",
                column: "resource_id");

            migrationBuilder.CreateIndex(
                name: "IX_Availability_volunteer_profile_id",
                table: "Availability",
                column: "volunteer_profile_id");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationInvitation_public_organization_id",
                table: "OrganizationInvitation",
                column: "public_organization_id");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationInvitation_token_hash",
                table: "OrganizationInvitation",
                column: "token_hash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PublicActorAccount_public_organization_id",
                table: "PublicActorAccount",
                column: "public_organization_id");

            migrationBuilder.CreateIndex(
                name: "IX_Reservation_resource_match_id",
                table: "Reservation",
                column: "resource_match_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reservation_resource_match_id_public_actor_account_id",
                table: "Reservation",
                columns: new[] { "resource_match_id", "public_actor_account_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Resource_account_id",
                table: "Resource",
                column: "account_id");

            migrationBuilder.CreateIndex(
                name: "IX_Resource_location_id",
                table: "Resource",
                column: "location_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Resource_resource_type_id",
                table: "Resource",
                column: "resource_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_ResourceMatch_public_actor_account_id",
                table: "ResourceMatch",
                column: "public_actor_account_id");

            migrationBuilder.CreateIndex(
                name: "IX_ResourceMatch_resource_id",
                table: "ResourceMatch",
                column: "resource_id");

            migrationBuilder.CreateIndex(
                name: "IX_ResourceMatch_resource_need_id",
                table: "ResourceMatch",
                column: "resource_need_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ResourceMatch_resource_need_id_public_actor_account_id",
                table: "ResourceMatch",
                columns: new[] { "resource_need_id", "public_actor_account_id" });

            migrationBuilder.CreateIndex(
                name: "IX_ResourceNeed_location_id",
                table: "ResourceNeed",
                column: "location_id");

            migrationBuilder.CreateIndex(
                name: "IX_ResourceNeed_public_actor_account_id",
                table: "ResourceNeed",
                column: "public_actor_account_id");

            migrationBuilder.CreateIndex(
                name: "IX_ResourceNeed_resource_type_id",
                table: "ResourceNeed",
                column: "resource_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_ResourceType_category_name",
                table: "ResourceType",
                column: "category_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Skill_name",
                table: "Skill",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VolunteerInterest_resource_need_id",
                table: "VolunteerInterest",
                column: "resource_need_id");

            migrationBuilder.CreateIndex(
                name: "IX_VolunteerInterest_volunteer_profile_id",
                table: "VolunteerInterest",
                column: "volunteer_profile_id");

            migrationBuilder.CreateIndex(
                name: "IX_VolunteerProfile_account_id",
                table: "VolunteerProfile",
                column: "account_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VolunteerProfileSkill_skill_id",
                table: "VolunteerProfileSkill",
                column: "skill_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountToken");

            migrationBuilder.DropTable(
                name: "AppWriteLock");

            migrationBuilder.DropTable(
                name: "AuditEntry");

            migrationBuilder.DropTable(
                name: "Availability");

            migrationBuilder.DropTable(
                name: "OrganizationInvitation");

            migrationBuilder.DropTable(
                name: "Reservation");

            migrationBuilder.DropTable(
                name: "VolunteerInterest");

            migrationBuilder.DropTable(
                name: "VolunteerProfileSkill");

            migrationBuilder.DropTable(
                name: "ResourceMatch");

            migrationBuilder.DropTable(
                name: "Skill");

            migrationBuilder.DropTable(
                name: "VolunteerProfile");

            migrationBuilder.DropTable(
                name: "ResourceNeed");

            migrationBuilder.DropTable(
                name: "Resource");

            migrationBuilder.DropTable(
                name: "PublicActorAccount");

            migrationBuilder.DropTable(
                name: "Location");

            migrationBuilder.DropTable(
                name: "ProviderAccount");

            migrationBuilder.DropTable(
                name: "ResourceType");

            migrationBuilder.DropTable(
                name: "PublicOrganization");

            migrationBuilder.DropTable(
                name: "Account");
        }
    }
}
