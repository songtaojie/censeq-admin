using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Starshine.Admin.Migrations
{
    /// <inheritdoc />
    public partial class InitialDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "abp_application",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    application_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    client_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    client_secret = table.Column<string>(type: "text", nullable: true),
                    client_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    consent_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    display_name = table.Column<string>(type: "text", nullable: true),
                    display_names = table.Column<string>(type: "text", nullable: true),
                    json_web_key_set = table.Column<string>(type: "text", nullable: true),
                    permissions = table.Column<string>(type: "text", nullable: true),
                    post_logout_redirect_uris = table.Column<string>(type: "text", nullable: true),
                    properties = table.Column<string>(type: "text", nullable: true),
                    redirect_uris = table.Column<string>(type: "text", nullable: true),
                    requirements = table.Column<string>(type: "text", nullable: true),
                    settings = table.Column<string>(type: "text", nullable: true),
                    client_uri = table.Column<string>(type: "text", nullable: true),
                    logo_uri = table.Column<string>(type: "text", nullable: true),
                    extra_properties = table.Column<string>(type: "text", nullable: false),
                    concurrency_stamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    creation_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    creator_id = table.Column<Guid>(type: "uuid", nullable: true),
                    last_modification_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    last_modifier_id = table.Column<Guid>(type: "uuid", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    deleter_id = table.Column<Guid>(type: "uuid", nullable: true),
                    deletion_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_abp_application", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "abp_audit_log",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    application_name = table.Column<string>(type: "character varying(96)", maxLength: 96, nullable: true),
                    user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    user_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: true),
                    tenant_name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    impersonator_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    impersonator_user_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    impersonator_tenant_id = table.Column<Guid>(type: "uuid", nullable: true),
                    impersonator_tenant_name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    execution_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    execution_duration = table.Column<int>(type: "integer", nullable: false),
                    client_ip_address = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    client_name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    client_id = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    correlation_id = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    browser_info = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    http_method = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: true),
                    url = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    exceptions = table.Column<string>(type: "text", nullable: true),
                    comments = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    http_status_code = table.Column<int>(type: "integer", nullable: true),
                    extra_properties = table.Column<string>(type: "text", nullable: false),
                    concurrency_stamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_abp_audit_log", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "abp_authorization",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    application_id = table.Column<Guid>(type: "uuid", nullable: true),
                    creation_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    properties = table.Column<string>(type: "text", nullable: true),
                    scopes = table.Column<string>(type: "text", nullable: true),
                    status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    subject = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: true),
                    type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    extra_properties = table.Column<string>(type: "text", nullable: false),
                    concurrency_stamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    creation_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    creator_id = table.Column<Guid>(type: "uuid", nullable: true),
                    last_modification_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    last_modifier_id = table.Column<Guid>(type: "uuid", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    deleter_id = table.Column<Guid>(type: "uuid", nullable: true),
                    deletion_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_abp_authorization", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "abp_background_job_record",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    job_name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    job_args = table.Column<string>(type: "character varying(1048576)", maxLength: 1048576, nullable: false),
                    try_count = table.Column<short>(type: "smallint", nullable: false, defaultValue: (short)0),
                    creation_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    next_try_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    last_try_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    is_abandoned = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    priority = table.Column<byte>(type: "smallint", nullable: false, defaultValue: (byte)15),
                    extra_properties = table.Column<string>(type: "text", nullable: false),
                    concurrency_stamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_abp_background_job_record", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "abp_claim_type",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    required = table.Column<bool>(type: "boolean", nullable: false),
                    is_static = table.Column<bool>(type: "boolean", nullable: false),
                    regex = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    regex_description = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    description = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    value_type = table.Column<int>(type: "integer", nullable: false),
                    extra_properties = table.Column<string>(type: "text", nullable: false),
                    concurrency_stamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_abp_claim_type", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "abp_feature_definition_record",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    group_name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    parent_name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    display_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    description = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    default_value = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    is_visible_to_clients = table.Column<bool>(type: "boolean", nullable: false),
                    is_available_to_host = table.Column<bool>(type: "boolean", nullable: false),
                    allowed_providers = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    value_type = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    extra_properties = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_abp_feature_definition_record", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "abp_feature_group_definition_record",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    display_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    extra_properties = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_abp_feature_group_definition_record", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "abp_feature_value",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    value = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    provider_name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    provider_key = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_abp_feature_value", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "abp_link_user",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    source_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    source_tenant_id = table.Column<Guid>(type: "uuid", nullable: true),
                    target_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    target_tenant_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_abp_link_user", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "abp_organization_unit",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: true),
                    parent_id = table.Column<Guid>(type: "uuid", nullable: true),
                    code = table.Column<string>(type: "character varying(95)", maxLength: 95, nullable: false),
                    display_name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    entity_version = table.Column<int>(type: "integer", nullable: false),
                    extra_properties = table.Column<string>(type: "text", nullable: false),
                    concurrency_stamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    creation_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    creator_id = table.Column<Guid>(type: "uuid", nullable: true),
                    last_modification_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    last_modifier_id = table.Column<Guid>(type: "uuid", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    deleter_id = table.Column<Guid>(type: "uuid", nullable: true),
                    deletion_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_abp_organization_unit", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "abp_permission_definition_record",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    group_name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    parent_name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    display_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    multi_tenancy_side = table.Column<byte>(type: "smallint", nullable: false),
                    providers = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    state_checkers = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    extra_properties = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_abp_permission_definition_record", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "abp_permission_grant",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: true),
                    name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    provider_name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    provider_key = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_abp_permission_grant", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "abp_permission_group_definition_record",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    display_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    extra_properties = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_abp_permission_group_definition_record", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "abp_role",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: true),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    normalized_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    is_default = table.Column<bool>(type: "boolean", nullable: false),
                    is_static = table.Column<bool>(type: "boolean", nullable: false),
                    is_public = table.Column<bool>(type: "boolean", nullable: false),
                    entity_version = table.Column<int>(type: "integer", nullable: false),
                    extra_properties = table.Column<string>(type: "text", nullable: false),
                    concurrency_stamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_abp_role", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "abp_scope",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    descriptions = table.Column<string>(type: "text", nullable: true),
                    display_name = table.Column<string>(type: "text", nullable: true),
                    display_names = table.Column<string>(type: "text", nullable: true),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    properties = table.Column<string>(type: "text", nullable: true),
                    resources = table.Column<string>(type: "text", nullable: true),
                    extra_properties = table.Column<string>(type: "text", nullable: false),
                    concurrency_stamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    creation_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    creator_id = table.Column<Guid>(type: "uuid", nullable: true),
                    last_modification_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    last_modifier_id = table.Column<Guid>(type: "uuid", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    deleter_id = table.Column<Guid>(type: "uuid", nullable: true),
                    deletion_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_abp_scope", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "abp_security_log",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: true),
                    application_name = table.Column<string>(type: "character varying(96)", maxLength: 96, nullable: true),
                    identity = table.Column<string>(type: "character varying(96)", maxLength: 96, nullable: true),
                    action = table.Column<string>(type: "character varying(96)", maxLength: 96, nullable: true),
                    user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    user_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    tenant_name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    client_id = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    correlation_id = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    client_ip_address = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    browser_info = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    creation_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    extra_properties = table.Column<string>(type: "text", nullable: false),
                    concurrency_stamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_abp_security_log", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "abp_session",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    session_id = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    device = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    device_info = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: true),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    client_id = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    ip_addresses = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    signed_in = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    last_accessed = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_abp_session", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "abp_setting",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    value = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false),
                    provider_name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    provider_key = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_abp_setting", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "abp_setting_definition_record",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    display_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    description = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    default_value = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    is_visible_to_clients = table.Column<bool>(type: "boolean", nullable: false),
                    providers = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    is_inherited = table.Column<bool>(type: "boolean", nullable: false),
                    is_encrypted = table.Column<bool>(type: "boolean", nullable: false),
                    extra_properties = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_abp_setting_definition_record", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "abp_tenant",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    normalized_name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    entity_version = table.Column<int>(type: "integer", nullable: false),
                    extra_properties = table.Column<string>(type: "text", nullable: false),
                    concurrency_stamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    creation_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    creator_id = table.Column<Guid>(type: "uuid", nullable: true),
                    last_modification_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    last_modifier_id = table.Column<Guid>(type: "uuid", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    deleter_id = table.Column<Guid>(type: "uuid", nullable: true),
                    deletion_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_abp_tenant", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "abp_token",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    application_id = table.Column<Guid>(type: "uuid", nullable: true),
                    authorization_id = table.Column<Guid>(type: "uuid", nullable: true),
                    creation_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    expiration_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    payload = table.Column<string>(type: "text", nullable: true),
                    properties = table.Column<string>(type: "text", nullable: true),
                    redemption_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    reference_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    subject = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: true),
                    type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    extra_properties = table.Column<string>(type: "text", nullable: false),
                    concurrency_stamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    creation_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    creator_id = table.Column<Guid>(type: "uuid", nullable: true),
                    last_modification_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    last_modifier_id = table.Column<Guid>(type: "uuid", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    deleter_id = table.Column<Guid>(type: "uuid", nullable: true),
                    deletion_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_abp_token", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "abp_user",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: true),
                    user_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    normalized_user_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    surname = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    normalized_email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    email_confirmed = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    password_hash = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    security_stamp = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    is_external = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    phone_number = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: true),
                    phone_number_confirmed = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    two_factor_enabled = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    lockout_end = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    lockout_enabled = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    access_failed_count = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    should_change_password_on_next_login = table.Column<bool>(type: "boolean", nullable: false),
                    entity_version = table.Column<int>(type: "integer", nullable: false),
                    last_password_change_time = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    extra_properties = table.Column<string>(type: "text", nullable: false),
                    concurrency_stamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    creation_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    creator_id = table.Column<Guid>(type: "uuid", nullable: true),
                    last_modification_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    last_modifier_id = table.Column<Guid>(type: "uuid", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    deleter_id = table.Column<Guid>(type: "uuid", nullable: true),
                    deletion_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_abp_user", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "abp_user_delegation",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: true),
                    source_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    target_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    start_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    end_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_abp_user_delegation", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "abp_audit_log_action",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: true),
                    audit_log_id = table.Column<Guid>(type: "uuid", nullable: false),
                    service_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    method_name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    parameters = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    execution_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    execution_duration = table.Column<int>(type: "integer", nullable: false),
                    extra_properties = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_abp_audit_log_action", x => x.id);
                    table.ForeignKey(
                        name: "fk_abp_audit_log_action_abp_audit_log_audit_log_id",
                        column: x => x.audit_log_id,
                        principalTable: "abp_audit_log",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "abp_entity_change",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    audit_log_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: true),
                    change_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    change_type = table.Column<byte>(type: "smallint", nullable: false),
                    entity_tenant_id = table.Column<Guid>(type: "uuid", nullable: true),
                    entity_id = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    entity_type_full_name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    extra_properties = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_abp_entity_change", x => x.id);
                    table.ForeignKey(
                        name: "fk_abp_entity_change_abp_audit_log_audit_log_id",
                        column: x => x.audit_log_id,
                        principalTable: "abp_audit_log",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "abp_organization_unit_role",
                columns: table => new
                {
                    role_id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_unit_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: true),
                    creation_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    creator_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_abp_organization_unit_role", x => new { x.organization_unit_id, x.role_id });
                    table.ForeignKey(
                        name: "fk_abp_organization_unit_role_abp_organization_unit_organizati",
                        column: x => x.organization_unit_id,
                        principalTable: "abp_organization_unit",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "abp_role_claim",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    role_id = table.Column<Guid>(type: "uuid", nullable: false),
                    identity_role_id = table.Column<Guid>(type: "uuid", nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: true),
                    claim_type = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    claim_value = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_abp_role_claim", x => x.id);
                    table.ForeignKey(
                        name: "fk_abp_role_claim_abp_role_identity_role_id",
                        column: x => x.identity_role_id,
                        principalTable: "abp_role",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "abp_tenant_connection_string",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false, comment: "租户ID"),
                    name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    value = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_abp_tenant_connection_string", x => new { x.tenant_id, x.name });
                    table.ForeignKey(
                        name: "fk_abp_tenant_connection_string_abp_tenant_tenant_id",
                        column: x => x.tenant_id,
                        principalTable: "abp_tenant",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "abp_user_claim",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    identity_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: true),
                    claim_type = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    claim_value = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_abp_user_claim", x => x.id);
                    table.ForeignKey(
                        name: "fk_abp_user_claim_abp_user_identity_user_id",
                        column: x => x.identity_user_id,
                        principalTable: "abp_user",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "abp_user_login",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    login_provider = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: true),
                    provider_key = table.Column<string>(type: "character varying(196)", maxLength: 196, nullable: false),
                    provider_display_name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    identity_user_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_abp_user_login", x => new { x.user_id, x.login_provider });
                    table.ForeignKey(
                        name: "fk_abp_user_login_abp_user_identity_user_id",
                        column: x => x.identity_user_id,
                        principalTable: "abp_user",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "abp_user_organization_unit",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_unit_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: true),
                    identity_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    creation_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    creator_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_abp_user_organization_unit", x => new { x.organization_unit_id, x.user_id });
                    table.ForeignKey(
                        name: "fk_abp_user_organization_unit_abp_user_identity_user_id",
                        column: x => x.identity_user_id,
                        principalTable: "abp_user",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "abp_user_role",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    role_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: true),
                    identity_user_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_abp_user_role", x => new { x.user_id, x.role_id });
                    table.ForeignKey(
                        name: "fk_abp_user_role_abp_user_identity_user_id",
                        column: x => x.identity_user_id,
                        principalTable: "abp_user",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "abp_user_token",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    login_provider = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: true),
                    value = table.Column<string>(type: "text", nullable: true),
                    identity_user_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_abp_user_token", x => new { x.user_id, x.login_provider, x.name });
                    table.ForeignKey(
                        name: "fk_abp_user_token_abp_user_identity_user_id",
                        column: x => x.identity_user_id,
                        principalTable: "abp_user",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "abp_entity_property_change",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: true),
                    entity_change_id = table.Column<Guid>(type: "uuid", nullable: false),
                    new_value = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    original_value = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    property_name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    property_type_full_name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_abp_entity_property_change", x => x.id);
                    table.ForeignKey(
                        name: "fk_abp_entity_property_change_abp_entity_change_entity_change_",
                        column: x => x.entity_change_id,
                        principalTable: "abp_entity_change",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_abp_application_client_id",
                table: "abp_application",
                column: "client_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_abp_audit_log_tenant_id_execution_time",
                table: "abp_audit_log",
                columns: new[] { "tenant_id", "execution_time" });

            migrationBuilder.CreateIndex(
                name: "ix_abp_audit_log_tenant_id_user_id_execution_time",
                table: "abp_audit_log",
                columns: new[] { "tenant_id", "user_id", "execution_time" });

            migrationBuilder.CreateIndex(
                name: "ix_abp_audit_log_action_audit_log_id",
                table: "abp_audit_log_action",
                column: "audit_log_id");

            migrationBuilder.CreateIndex(
                name: "ix_abp_audit_log_action_tenant_id_service_name_method_name_exe",
                table: "abp_audit_log_action",
                columns: new[] { "tenant_id", "service_name", "method_name", "execution_time" });

            migrationBuilder.CreateIndex(
                name: "ix_abp_authorization_application_id_status_subject_type",
                table: "abp_authorization",
                columns: new[] { "application_id", "status", "subject", "type" });

            migrationBuilder.CreateIndex(
                name: "ix_abp_background_job_record_is_abandoned_next_try_time",
                table: "abp_background_job_record",
                columns: new[] { "is_abandoned", "next_try_time" });

            migrationBuilder.CreateIndex(
                name: "ix_abp_entity_change_audit_log_id",
                table: "abp_entity_change",
                column: "audit_log_id");

            migrationBuilder.CreateIndex(
                name: "ix_abp_entity_change_tenant_id_entity_type_full_name_entity_id",
                table: "abp_entity_change",
                columns: new[] { "tenant_id", "entity_type_full_name", "entity_id" });

            migrationBuilder.CreateIndex(
                name: "ix_abp_entity_property_change_entity_change_id",
                table: "abp_entity_property_change",
                column: "entity_change_id");

            migrationBuilder.CreateIndex(
                name: "ix_abp_feature_definition_record_group_name",
                table: "abp_feature_definition_record",
                column: "group_name");

            migrationBuilder.CreateIndex(
                name: "ix_abp_feature_definition_record_name",
                table: "abp_feature_definition_record",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_abp_feature_group_definition_record_name",
                table: "abp_feature_group_definition_record",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_abp_feature_value_name_provider_name_provider_key",
                table: "abp_feature_value",
                columns: new[] { "name", "provider_name", "provider_key" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_abp_link_user_source_user_id_source_tenant_id_target_user_i",
                table: "abp_link_user",
                columns: new[] { "source_user_id", "source_tenant_id", "target_user_id", "target_tenant_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_abp_organization_unit_code",
                table: "abp_organization_unit",
                column: "code");

            migrationBuilder.CreateIndex(
                name: "ix_abp_organization_unit_role_role_id_organization_unit_id",
                table: "abp_organization_unit_role",
                columns: new[] { "role_id", "organization_unit_id" });

            migrationBuilder.CreateIndex(
                name: "ix_abp_permission_definition_record_group_name",
                table: "abp_permission_definition_record",
                column: "group_name");

            migrationBuilder.CreateIndex(
                name: "ix_abp_permission_definition_record_name",
                table: "abp_permission_definition_record",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_abp_permission_grant_tenant_id_name_provider_name_provider_",
                table: "abp_permission_grant",
                columns: new[] { "tenant_id", "name", "provider_name", "provider_key" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_abp_permission_group_definition_record_name",
                table: "abp_permission_group_definition_record",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_abp_role_normalized_name",
                table: "abp_role",
                column: "normalized_name");

            migrationBuilder.CreateIndex(
                name: "ix_abp_role_claim_identity_role_id",
                table: "abp_role_claim",
                column: "identity_role_id");

            migrationBuilder.CreateIndex(
                name: "ix_abp_role_claim_role_id",
                table: "abp_role_claim",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "ix_abp_scope_name",
                table: "abp_scope",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_abp_security_log_tenant_id_action",
                table: "abp_security_log",
                columns: new[] { "tenant_id", "action" });

            migrationBuilder.CreateIndex(
                name: "ix_abp_security_log_tenant_id_application_name",
                table: "abp_security_log",
                columns: new[] { "tenant_id", "application_name" });

            migrationBuilder.CreateIndex(
                name: "ix_abp_security_log_tenant_id_identity",
                table: "abp_security_log",
                columns: new[] { "tenant_id", "identity" });

            migrationBuilder.CreateIndex(
                name: "ix_abp_security_log_tenant_id_user_id",
                table: "abp_security_log",
                columns: new[] { "tenant_id", "user_id" });

            migrationBuilder.CreateIndex(
                name: "ix_abp_session_device",
                table: "abp_session",
                column: "device");

            migrationBuilder.CreateIndex(
                name: "ix_abp_session_session_id",
                table: "abp_session",
                column: "session_id");

            migrationBuilder.CreateIndex(
                name: "ix_abp_session_tenant_id_user_id",
                table: "abp_session",
                columns: new[] { "tenant_id", "user_id" });

            migrationBuilder.CreateIndex(
                name: "ix_abp_setting_name_provider_name_provider_key",
                table: "abp_setting",
                columns: new[] { "name", "provider_name", "provider_key" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_abp_setting_definition_record_name",
                table: "abp_setting_definition_record",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_abp_tenant_name",
                table: "abp_tenant",
                column: "name");

            migrationBuilder.CreateIndex(
                name: "ix_abp_tenant_normalized_name",
                table: "abp_tenant",
                column: "normalized_name");

            migrationBuilder.CreateIndex(
                name: "ix_abp_token_application_id_status_subject_type",
                table: "abp_token",
                columns: new[] { "application_id", "status", "subject", "type" });

            migrationBuilder.CreateIndex(
                name: "ix_abp_token_reference_id",
                table: "abp_token",
                column: "reference_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_abp_user_email",
                table: "abp_user",
                column: "email");

            migrationBuilder.CreateIndex(
                name: "ix_abp_user_normalized_email",
                table: "abp_user",
                column: "normalized_email");

            migrationBuilder.CreateIndex(
                name: "ix_abp_user_normalized_user_name",
                table: "abp_user",
                column: "normalized_user_name");

            migrationBuilder.CreateIndex(
                name: "ix_abp_user_user_name",
                table: "abp_user",
                column: "user_name");

            migrationBuilder.CreateIndex(
                name: "ix_abp_user_claim_identity_user_id",
                table: "abp_user_claim",
                column: "identity_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_abp_user_claim_user_id",
                table: "abp_user_claim",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_abp_user_login_identity_user_id",
                table: "abp_user_login",
                column: "identity_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_abp_user_login_login_provider_provider_key",
                table: "abp_user_login",
                columns: new[] { "login_provider", "provider_key" });

            migrationBuilder.CreateIndex(
                name: "ix_abp_user_organization_unit_identity_user_id",
                table: "abp_user_organization_unit",
                column: "identity_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_abp_user_organization_unit_user_id_organization_unit_id",
                table: "abp_user_organization_unit",
                columns: new[] { "user_id", "organization_unit_id" });

            migrationBuilder.CreateIndex(
                name: "ix_abp_user_role_identity_user_id",
                table: "abp_user_role",
                column: "identity_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_abp_user_role_role_id_user_id",
                table: "abp_user_role",
                columns: new[] { "role_id", "user_id" });

            migrationBuilder.CreateIndex(
                name: "ix_abp_user_token_identity_user_id",
                table: "abp_user_token",
                column: "identity_user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "abp_application");

            migrationBuilder.DropTable(
                name: "abp_audit_log_action");

            migrationBuilder.DropTable(
                name: "abp_authorization");

            migrationBuilder.DropTable(
                name: "abp_background_job_record");

            migrationBuilder.DropTable(
                name: "abp_claim_type");

            migrationBuilder.DropTable(
                name: "abp_entity_property_change");

            migrationBuilder.DropTable(
                name: "abp_feature_definition_record");

            migrationBuilder.DropTable(
                name: "abp_feature_group_definition_record");

            migrationBuilder.DropTable(
                name: "abp_feature_value");

            migrationBuilder.DropTable(
                name: "abp_link_user");

            migrationBuilder.DropTable(
                name: "abp_organization_unit_role");

            migrationBuilder.DropTable(
                name: "abp_permission_definition_record");

            migrationBuilder.DropTable(
                name: "abp_permission_grant");

            migrationBuilder.DropTable(
                name: "abp_permission_group_definition_record");

            migrationBuilder.DropTable(
                name: "abp_role_claim");

            migrationBuilder.DropTable(
                name: "abp_scope");

            migrationBuilder.DropTable(
                name: "abp_security_log");

            migrationBuilder.DropTable(
                name: "abp_session");

            migrationBuilder.DropTable(
                name: "abp_setting");

            migrationBuilder.DropTable(
                name: "abp_setting_definition_record");

            migrationBuilder.DropTable(
                name: "abp_tenant_connection_string");

            migrationBuilder.DropTable(
                name: "abp_token");

            migrationBuilder.DropTable(
                name: "abp_user_claim");

            migrationBuilder.DropTable(
                name: "abp_user_delegation");

            migrationBuilder.DropTable(
                name: "abp_user_login");

            migrationBuilder.DropTable(
                name: "abp_user_organization_unit");

            migrationBuilder.DropTable(
                name: "abp_user_role");

            migrationBuilder.DropTable(
                name: "abp_user_token");

            migrationBuilder.DropTable(
                name: "abp_entity_change");

            migrationBuilder.DropTable(
                name: "abp_organization_unit");

            migrationBuilder.DropTable(
                name: "abp_role");

            migrationBuilder.DropTable(
                name: "abp_tenant");

            migrationBuilder.DropTable(
                name: "abp_user");

            migrationBuilder.DropTable(
                name: "abp_audit_log");
        }
    }
}
