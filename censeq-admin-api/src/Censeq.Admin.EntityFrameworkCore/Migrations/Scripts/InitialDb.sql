CREATE TABLE IF NOT EXISTS ef_migrations_history (
    migration_id character varying(150) NOT NULL,
    product_version character varying(32) NOT NULL,
    CONSTRAINT pk_ef_migrations_history PRIMARY KEY (migration_id)
);

START TRANSACTION;

CREATE TABLE censeq_audit_log (
    id uuid NOT NULL,
    application_name character varying(96),
    user_id uuid,
    user_name character varying(256),
    tenant_id uuid,
    tenant_name character varying(64),
    impersonator_user_id uuid,
    impersonator_user_name character varying(256),
    impersonator_tenant_id uuid,
    impersonator_tenant_name character varying(64),
    execution_time timestamp without time zone NOT NULL,
    execution_duration integer NOT NULL,
    client_ip_address character varying(64),
    client_name character varying(128),
    client_id character varying(64),
    correlation_id character varying(64),
    browser_info character varying(512),
    http_method character varying(16),
    url character varying(256),
    exceptions text,
    comments character varying(256),
    http_status_code integer,
    extra_properties text NOT NULL,
    concurrency_stamp character varying(40) NOT NULL,
    CONSTRAINT pk_censeq_audit_log PRIMARY KEY (id)
);

CREATE TABLE censeq_feature_definition_record (
    id uuid NOT NULL,
    group_name character varying(128) NOT NULL,
    name character varying(128) NOT NULL,
    parent_name character varying(128) NOT NULL,
    display_name character varying(256) NOT NULL,
    description character varying(256) NOT NULL,
    localization_key character varying(512),
    description_localization_key character varying(512),
    default_value character varying(256) NOT NULL,
    is_visible_to_clients boolean NOT NULL,
    is_available_to_host boolean NOT NULL,
    allowed_providers character varying(256) NOT NULL,
    value_type character varying(2048) NOT NULL,
    extra_properties text NOT NULL,
    CONSTRAINT pk_censeq_feature_definition_record PRIMARY KEY (id)
);

CREATE TABLE censeq_feature_group_definition_record (
    id uuid NOT NULL,
    name character varying(128) NOT NULL,
    display_name character varying(256) NOT NULL,
    localization_key character varying(512),
    extra_properties text NOT NULL,
    CONSTRAINT pk_censeq_feature_group_definition_record PRIMARY KEY (id)
);

CREATE TABLE censeq_feature_value (
    id uuid NOT NULL,
    name character varying(128) NOT NULL,
    value character varying(128) NOT NULL,
    provider_name character varying(64) NOT NULL,
    provider_key character varying(64) NOT NULL,
    CONSTRAINT pk_censeq_feature_value PRIMARY KEY (id)
);

CREATE TABLE censeq_identity_claim_type (
    id uuid NOT NULL,
    name character varying(256) NOT NULL,
    required boolean NOT NULL,
    is_static boolean NOT NULL,
    regex character varying(512),
    regex_description character varying(128),
    description character varying(256),
    value_type integer NOT NULL,
    extra_properties text NOT NULL,
    concurrency_stamp character varying(40) NOT NULL,
    CONSTRAINT pk_censeq_identity_claim_type PRIMARY KEY (id)
);

CREATE TABLE censeq_identity_link_user (
    id uuid NOT NULL,
    source_user_id uuid NOT NULL,
    source_tenant_id uuid,
    target_user_id uuid NOT NULL,
    target_tenant_id uuid,
    CONSTRAINT pk_censeq_identity_link_user PRIMARY KEY (id)
);

CREATE TABLE censeq_identity_role (
    id uuid NOT NULL,
    tenant_id uuid,
    name character varying(256) NOT NULL,
    normalized_name character varying(256) NOT NULL,
    code character varying(64),
    is_default boolean NOT NULL,
    is_static boolean NOT NULL,
    is_public boolean NOT NULL,
    entity_version integer NOT NULL,
    extra_properties text NOT NULL,
    concurrency_stamp character varying(40) NOT NULL,
    CONSTRAINT pk_censeq_identity_role PRIMARY KEY (id)
);

CREATE TABLE censeq_identity_security_log (
    id uuid NOT NULL,
    tenant_id uuid,
    application_name character varying(96),
    identity character varying(96),
    action character varying(96) NOT NULL,
    user_id uuid,
    user_name character varying(256),
    tenant_name character varying(64),
    client_id character varying(64),
    correlation_id character varying(64),
    client_ip_address character varying(64),
    browser_info character varying(512),
    creation_time timestamp without time zone NOT NULL,
    extra_properties text NOT NULL,
    concurrency_stamp character varying(40) NOT NULL,
    CONSTRAINT pk_censeq_identity_security_log PRIMARY KEY (id)
);

CREATE TABLE censeq_identity_session (
    id uuid NOT NULL,
    session_id character varying(128) NOT NULL,
    device character varying(64) NOT NULL,
    device_info character varying(64) NOT NULL,
    tenant_id uuid,
    user_id uuid NOT NULL,
    client_id character varying(64) NOT NULL,
    ip_addresses character varying(256) NOT NULL,
    signed_in timestamp without time zone NOT NULL,
    last_accessed timestamp without time zone,
    CONSTRAINT pk_censeq_identity_session PRIMARY KEY (id)
);

CREATE TABLE censeq_identity_user (
    id uuid NOT NULL,
    tenant_id uuid,
    user_name character varying(256) NOT NULL,
    normalized_user_name character varying(256) NOT NULL,
    name character varying(64),
    surname character varying(64),
    email character varying(256) NOT NULL,
    normalized_email character varying(256) NOT NULL,
    email_confirmed boolean NOT NULL DEFAULT FALSE,
    password_hash character varying(256) NOT NULL,
    security_stamp character varying(256) NOT NULL,
    is_external boolean NOT NULL DEFAULT FALSE,
    phone_number character varying(16),
    phone_number_confirmed boolean NOT NULL DEFAULT FALSE,
    is_active boolean NOT NULL,
    two_factor_enabled boolean NOT NULL DEFAULT FALSE,
    lockout_end timestamp with time zone,
    lockout_enabled boolean NOT NULL DEFAULT FALSE,
    access_failed_count integer NOT NULL DEFAULT 0,
    should_change_password_on_next_login boolean NOT NULL,
    entity_version integer NOT NULL,
    last_password_change_time timestamp with time zone,
    extra_properties text NOT NULL,
    concurrency_stamp character varying(40) NOT NULL,
    creation_time timestamp without time zone NOT NULL,
    creator_id uuid,
    last_modification_time timestamp without time zone,
    last_modifier_id uuid,
    is_deleted boolean NOT NULL DEFAULT FALSE,
    deleter_id uuid,
    deletion_time timestamp without time zone,
    CONSTRAINT pk_censeq_identity_user PRIMARY KEY (id)
);

CREATE TABLE censeq_identity_user_delegation (
    id uuid NOT NULL,
    tenant_id uuid,
    source_user_id uuid NOT NULL,
    target_user_id uuid NOT NULL,
    start_time timestamp without time zone NOT NULL,
    end_time timestamp without time zone NOT NULL,
    CONSTRAINT pk_censeq_identity_user_delegation PRIMARY KEY (id)
);

CREATE TABLE censeq_menu (
    id uuid NOT NULL,
    tenant_id uuid,
    parent_id uuid,
    name character varying(64) NOT NULL,
    title character varying(128) NOT NULL,
    route_name character varying(128),
    path character varying(256),
    component character varying(256),
    redirect character varying(256),
    icon character varying(128),
    type smallint NOT NULL,
    sort integer NOT NULL,
    visible boolean NOT NULL,
    keep_alive boolean NOT NULL,
    affix boolean NOT NULL,
    is_external boolean NOT NULL,
    external_url character varying(512),
    is_iframe boolean NOT NULL,
    status boolean NOT NULL,
    authorization_mode smallint NOT NULL,
    scope smallint NOT NULL,
    remark character varying(512),
    button_code character varying(128),
    permission_groups character varying(512),
    extra_properties text NOT NULL,
    concurrency_stamp character varying(40) NOT NULL,
    creation_time timestamp without time zone NOT NULL,
    creator_id uuid,
    last_modification_time timestamp without time zone,
    last_modifier_id uuid,
    is_deleted boolean NOT NULL DEFAULT FALSE,
    deleter_id uuid,
    deletion_time timestamp without time zone,
    CONSTRAINT pk_censeq_menu PRIMARY KEY (id),
    CONSTRAINT fk_censeq_menu_censeq_menu_parent_id FOREIGN KEY (parent_id) REFERENCES censeq_menu (id) ON DELETE RESTRICT
);

CREATE TABLE censeq_openiddict_application (
    id uuid NOT NULL,
    application_type character varying(50),
    client_id character varying(100),
    client_secret text,
    client_type character varying(50),
    consent_type character varying(50),
    display_name text,
    display_names text,
    json_web_key_set text,
    permissions text,
    post_logout_redirect_uris text,
    properties text,
    redirect_uris text,
    requirements text,
    settings text,
    client_uri text,
    logo_uri text,
    extra_properties text NOT NULL,
    concurrency_stamp character varying(40) NOT NULL,
    creation_time timestamp without time zone NOT NULL,
    creator_id uuid,
    last_modification_time timestamp without time zone,
    last_modifier_id uuid,
    is_deleted boolean NOT NULL DEFAULT FALSE,
    deleter_id uuid,
    deletion_time timestamp without time zone,
    CONSTRAINT pk_censeq_openiddict_application PRIMARY KEY (id)
);

CREATE TABLE censeq_openiddict_authorization (
    id uuid NOT NULL,
    application_id uuid,
    creation_date timestamp without time zone,
    properties text,
    scopes text,
    status character varying(50),
    subject character varying(400),
    type character varying(50),
    extra_properties text NOT NULL,
    concurrency_stamp character varying(40) NOT NULL,
    creation_time timestamp without time zone NOT NULL,
    creator_id uuid,
    last_modification_time timestamp without time zone,
    last_modifier_id uuid,
    is_deleted boolean NOT NULL DEFAULT FALSE,
    deleter_id uuid,
    deletion_time timestamp without time zone,
    CONSTRAINT pk_censeq_openiddict_authorization PRIMARY KEY (id)
);

CREATE TABLE censeq_openiddict_scope (
    id uuid NOT NULL,
    description text,
    descriptions text,
    display_name text,
    display_names text,
    name character varying(200),
    properties text,
    resources text,
    extra_properties text NOT NULL,
    concurrency_stamp character varying(40) NOT NULL,
    creation_time timestamp without time zone NOT NULL,
    creator_id uuid,
    last_modification_time timestamp without time zone,
    last_modifier_id uuid,
    is_deleted boolean NOT NULL DEFAULT FALSE,
    deleter_id uuid,
    deletion_time timestamp without time zone,
    CONSTRAINT pk_censeq_openiddict_scope PRIMARY KEY (id)
);

CREATE TABLE censeq_openiddict_token (
    id uuid NOT NULL,
    application_id uuid,
    authorization_id uuid,
    creation_date timestamp without time zone,
    expiration_date timestamp without time zone,
    payload text,
    properties text,
    redemption_date timestamp without time zone,
    reference_id character varying(100),
    status character varying(50),
    subject character varying(400),
    type character varying(50),
    extra_properties text NOT NULL,
    concurrency_stamp character varying(40) NOT NULL,
    creation_time timestamp without time zone NOT NULL,
    creator_id uuid,
    last_modification_time timestamp without time zone,
    last_modifier_id uuid,
    is_deleted boolean NOT NULL DEFAULT FALSE,
    deleter_id uuid,
    deletion_time timestamp without time zone,
    CONSTRAINT pk_censeq_openiddict_token PRIMARY KEY (id)
);

CREATE TABLE censeq_organization_unit (
    id uuid NOT NULL,
    tenant_id uuid,
    parent_id uuid,
    code character varying(95) NOT NULL,
    display_name character varying(128) NOT NULL,
    status integer NOT NULL DEFAULT 1,
    remark character varying(512),
    entity_version integer NOT NULL,
    extra_properties text NOT NULL,
    concurrency_stamp character varying(40) NOT NULL,
    creation_time timestamp without time zone NOT NULL,
    creator_id uuid,
    last_modification_time timestamp without time zone,
    last_modifier_id uuid,
    is_deleted boolean NOT NULL DEFAULT FALSE,
    deleter_id uuid,
    deletion_time timestamp without time zone,
    CONSTRAINT pk_censeq_organization_unit PRIMARY KEY (id)
);

CREATE TABLE censeq_permission_definition_record (
    id uuid NOT NULL,
    group_name character varying(128) NOT NULL,
    name character varying(128) NOT NULL,
    parent_name character varying(128),
    display_name character varying(256) NOT NULL,
    localization_key character varying(512),
    is_enabled boolean NOT NULL,
    multi_tenancy_side smallint NOT NULL,
    providers character varying(128),
    state_checkers character varying(256),
    extra_properties text NOT NULL,
    CONSTRAINT pk_censeq_permission_definition_record PRIMARY KEY (id)
);

CREATE TABLE censeq_permission_grant (
    id uuid NOT NULL,
    tenant_id uuid,
    name character varying(128) NOT NULL,
    provider_name character varying(64) NOT NULL,
    provider_key character varying(64) NOT NULL,
    CONSTRAINT pk_censeq_permission_grant PRIMARY KEY (id)
);

CREATE TABLE censeq_permission_group (
    id uuid NOT NULL,
    name character varying(128) NOT NULL,
    display_name character varying(256) NOT NULL,
    localization_key character varying(512),
    extra_properties text NOT NULL,
    CONSTRAINT pk_censeq_permission_group PRIMARY KEY (id)
);

CREATE TABLE censeq_setting (
    id uuid NOT NULL,
    name character varying(128) NOT NULL,
    value character varying(2048) NOT NULL,
    provider_name character varying(64),
    provider_key character varying(64),
    CONSTRAINT pk_censeq_setting PRIMARY KEY (id)
);

CREATE TABLE censeq_setting_definition_record (
    id uuid NOT NULL,
    name character varying(128) NOT NULL,
    display_name character varying(256) NOT NULL,
    localization_key character varying(512),
    description character varying(512),
    description_localization_key character varying(512),
    default_value character varying(2048),
    is_visible_to_clients boolean NOT NULL,
    providers character varying(1024),
    is_inherited boolean NOT NULL,
    is_encrypted boolean NOT NULL,
    extra_properties text NOT NULL,
    CONSTRAINT pk_censeq_setting_definition_record PRIMARY KEY (id)
);

CREATE TABLE censeq_tenant (
    id uuid NOT NULL,
    name character varying(64) NOT NULL,
    normalized_name character varying(64) NOT NULL,
    code character varying(64),
    domain character varying(256),
    icon character varying(512),
    copyright character varying(256),
    icp_no character varying(64),
    icp_address character varying(512),
    remark character varying(1024),
    max_user_count integer NOT NULL DEFAULT 0,
    is_active boolean NOT NULL DEFAULT TRUE,
    entity_version integer NOT NULL,
    extra_properties text NOT NULL,
    concurrency_stamp character varying(40) NOT NULL,
    creation_time timestamp without time zone NOT NULL,
    creator_id uuid,
    last_modification_time timestamp without time zone,
    last_modifier_id uuid,
    is_deleted boolean NOT NULL DEFAULT FALSE,
    deleter_id uuid,
    deletion_time timestamp without time zone,
    CONSTRAINT pk_censeq_tenant PRIMARY KEY (id)
);

CREATE TABLE censeq_tenant_permission_grants (
    id uuid NOT NULL,
    tenant_id uuid NOT NULL,
    permission_name character varying(128) NOT NULL,
    "CreationTime" timestamp without time zone NOT NULL,
    "CreatorId" uuid,
    CONSTRAINT pk_censeq_tenant_permission_grants PRIMARY KEY (id)
);

CREATE TABLE censeq_audit_log_action (
    id uuid NOT NULL,
    tenant_id uuid,
    audit_log_id uuid NOT NULL,
    service_name character varying(256),
    method_name character varying(128),
    parameters character varying(2000),
    execution_time timestamp without time zone NOT NULL,
    execution_duration integer NOT NULL,
    extra_properties text NOT NULL,
    CONSTRAINT pk_censeq_audit_log_action PRIMARY KEY (id),
    CONSTRAINT fk_censeq_audit_log_action_censeq_audit_log_audit_log_id FOREIGN KEY (audit_log_id) REFERENCES censeq_audit_log (id) ON DELETE CASCADE
);

CREATE TABLE censeq_entity_change (
    id uuid NOT NULL,
    audit_log_id uuid NOT NULL,
    tenant_id uuid,
    change_time timestamp without time zone NOT NULL,
    change_type smallint NOT NULL,
    entity_tenant_id uuid,
    entity_id character varying(128),
    entity_type_full_name character varying(128) NOT NULL,
    extra_properties text NOT NULL,
    CONSTRAINT pk_censeq_entity_change PRIMARY KEY (id),
    CONSTRAINT fk_censeq_entity_change_censeq_audit_log_audit_log_id FOREIGN KEY (audit_log_id) REFERENCES censeq_audit_log (id) ON DELETE CASCADE
);

CREATE TABLE censeq_identity_role_claim (
    id uuid NOT NULL,
    role_id uuid NOT NULL,
    identity_role_id uuid,
    tenant_id uuid,
    claim_type character varying(256) NOT NULL,
    claim_value character varying(1024) NOT NULL,
    CONSTRAINT pk_censeq_identity_role_claim PRIMARY KEY (id),
    CONSTRAINT fk_censeq_identity_role_claim_censeq_identity_role_identity_ro FOREIGN KEY (identity_role_id) REFERENCES censeq_identity_role (id)
);

CREATE TABLE censeq_identity_user_claim (
    id uuid NOT NULL,
    user_id uuid NOT NULL,
    identity_user_id uuid,
    tenant_id uuid,
    claim_type character varying(256) NOT NULL,
    claim_value character varying(1024) NOT NULL,
    CONSTRAINT pk_censeq_identity_user_claim PRIMARY KEY (id),
    CONSTRAINT fk_censeq_identity_user_claim_censeq_identity_user_identity_us FOREIGN KEY (identity_user_id) REFERENCES censeq_identity_user (id)
);

CREATE TABLE censeq_identity_user_login (
    user_id uuid NOT NULL,
    login_provider character varying(64) NOT NULL,
    tenant_id uuid,
    provider_key character varying(196) NOT NULL,
    provider_display_name character varying(128) NOT NULL,
    identity_user_id uuid,
    CONSTRAINT pk_censeq_identity_user_login PRIMARY KEY (user_id, login_provider),
    CONSTRAINT fk_censeq_identity_user_login_censeq_identity_user_identity_us FOREIGN KEY (identity_user_id) REFERENCES censeq_identity_user (id)
);

CREATE TABLE censeq_identity_user_organization_unit (
    user_id uuid NOT NULL,
    organization_unit_id uuid NOT NULL,
    tenant_id uuid,
    identity_user_id uuid,
    creation_time timestamp without time zone NOT NULL,
    creator_id uuid,
    CONSTRAINT pk_censeq_identity_user_organization_unit PRIMARY KEY (organization_unit_id, user_id),
    CONSTRAINT fk_censeq_identity_user_organization_unit_censeq_identity_user FOREIGN KEY (identity_user_id) REFERENCES censeq_identity_user (id)
);

CREATE TABLE censeq_identity_user_role (
    user_id uuid NOT NULL,
    role_id uuid NOT NULL,
    tenant_id uuid,
    identity_user_id uuid,
    CONSTRAINT pk_censeq_identity_user_role PRIMARY KEY (user_id, role_id),
    CONSTRAINT fk_censeq_identity_user_role_censeq_identity_user_identity_use FOREIGN KEY (identity_user_id) REFERENCES censeq_identity_user (id)
);

CREATE TABLE censeq_identity_user_token (
    user_id uuid NOT NULL,
    login_provider character varying(64) NOT NULL,
    name character varying(128) NOT NULL,
    tenant_id uuid,
    value text NOT NULL,
    identity_user_id uuid,
    CONSTRAINT pk_censeq_identity_user_token PRIMARY KEY (user_id, login_provider, name),
    CONSTRAINT fk_censeq_identity_user_token_censeq_identity_user_identity_us FOREIGN KEY (identity_user_id) REFERENCES censeq_identity_user (id)
);

CREATE TABLE censeq_menu_permission (
    id uuid NOT NULL,
    menu_id uuid NOT NULL,
    permission_name character varying(64) NOT NULL,
    CONSTRAINT pk_censeq_menu_permission PRIMARY KEY (id),
    CONSTRAINT fk_censeq_menu_permission_censeq_menu_menu_id FOREIGN KEY (menu_id) REFERENCES censeq_menu (id) ON DELETE CASCADE
);

CREATE TABLE censeq_organization_unit_role (
    role_id uuid NOT NULL,
    organization_unit_id uuid NOT NULL,
    tenant_id uuid,
    creation_time timestamp without time zone NOT NULL,
    creator_id uuid,
    CONSTRAINT pk_censeq_organization_unit_role PRIMARY KEY (organization_unit_id, role_id),
    CONSTRAINT fk_censeq_organization_unit_role_censeq_organization_unit_orga FOREIGN KEY (organization_unit_id) REFERENCES censeq_organization_unit (id) ON DELETE CASCADE
);

CREATE TABLE censeq_tenant_connection_string (
    tenant_id uuid NOT NULL,
    name character varying(64) NOT NULL,
    value character varying(1024) NOT NULL,
    CONSTRAINT pk_censeq_tenant_connection_string PRIMARY KEY (tenant_id, name),
    CONSTRAINT fk_censeq_tenant_connection_string_censeq_tenant_tenant_id FOREIGN KEY (tenant_id) REFERENCES censeq_tenant (id) ON DELETE CASCADE
);
COMMENT ON COLUMN censeq_tenant_connection_string.tenant_id IS '租户ID';

CREATE TABLE censeq_entity_property_change (
    id uuid NOT NULL,
    tenant_id uuid,
    entity_change_id uuid NOT NULL,
    new_value character varying(512),
    original_value character varying(512),
    property_name character varying(128) NOT NULL,
    property_type_full_name character varying(64) NOT NULL,
    CONSTRAINT pk_censeq_entity_property_change PRIMARY KEY (id),
    CONSTRAINT fk_censeq_entity_property_change_censeq_entity_change_entity_c FOREIGN KEY (entity_change_id) REFERENCES censeq_entity_change (id) ON DELETE CASCADE
);

CREATE INDEX ix_censeq_audit_log_tenant_id_execution_time ON censeq_audit_log (tenant_id, execution_time);

CREATE INDEX ix_censeq_audit_log_tenant_id_user_id_execution_time ON censeq_audit_log (tenant_id, user_id, execution_time);

CREATE INDEX ix_censeq_audit_log_action_audit_log_id ON censeq_audit_log_action (audit_log_id);

CREATE INDEX ix_censeq_audit_log_action_tenant_id_service_name_method_name_ ON censeq_audit_log_action (tenant_id, service_name, method_name, execution_time);

CREATE INDEX ix_censeq_entity_change_audit_log_id ON censeq_entity_change (audit_log_id);

CREATE INDEX ix_censeq_entity_change_tenant_id_entity_type_full_name_entity ON censeq_entity_change (tenant_id, entity_type_full_name, entity_id);

CREATE INDEX ix_censeq_entity_property_change_entity_change_id ON censeq_entity_property_change (entity_change_id);

CREATE INDEX ix_censeq_feature_definition_record_group_name ON censeq_feature_definition_record (group_name);

CREATE UNIQUE INDEX ix_censeq_feature_definition_record_name ON censeq_feature_definition_record (name);

CREATE UNIQUE INDEX ix_censeq_feature_group_definition_record_name ON censeq_feature_group_definition_record (name);

CREATE UNIQUE INDEX ix_censeq_feature_value_name_provider_name_provider_key ON censeq_feature_value (name, provider_name, provider_key);

CREATE UNIQUE INDEX ix_censeq_identity_link_user_source_user_id_source_tenant_id_t ON censeq_identity_link_user (source_user_id, source_tenant_id, target_user_id, target_tenant_id);

CREATE UNIQUE INDEX ix_censeq_identity_role_code ON censeq_identity_role (code);

CREATE INDEX ix_censeq_identity_role_normalized_name ON censeq_identity_role (normalized_name);

CREATE INDEX ix_censeq_identity_role_claim_identity_role_id ON censeq_identity_role_claim (identity_role_id);

CREATE INDEX ix_censeq_identity_role_claim_role_id ON censeq_identity_role_claim (role_id);

CREATE INDEX ix_censeq_identity_security_log_tenant_id_action ON censeq_identity_security_log (tenant_id, action);

CREATE INDEX ix_censeq_identity_security_log_tenant_id_application_name ON censeq_identity_security_log (tenant_id, application_name);

CREATE INDEX ix_censeq_identity_security_log_tenant_id_identity ON censeq_identity_security_log (tenant_id, identity);

CREATE INDEX ix_censeq_identity_security_log_tenant_id_user_id ON censeq_identity_security_log (tenant_id, user_id);

CREATE INDEX ix_censeq_identity_session_device ON censeq_identity_session (device);

CREATE INDEX ix_censeq_identity_session_session_id ON censeq_identity_session (session_id);

CREATE INDEX ix_censeq_identity_session_tenant_id_user_id ON censeq_identity_session (tenant_id, user_id);

CREATE INDEX ix_censeq_identity_user_email ON censeq_identity_user (email);

CREATE INDEX ix_censeq_identity_user_normalized_email ON censeq_identity_user (normalized_email);

CREATE INDEX ix_censeq_identity_user_normalized_user_name ON censeq_identity_user (normalized_user_name);

CREATE INDEX ix_censeq_identity_user_user_name ON censeq_identity_user (user_name);

CREATE INDEX ix_censeq_identity_user_claim_identity_user_id ON censeq_identity_user_claim (identity_user_id);

CREATE INDEX ix_censeq_identity_user_claim_user_id ON censeq_identity_user_claim (user_id);

CREATE INDEX ix_censeq_identity_user_login_identity_user_id ON censeq_identity_user_login (identity_user_id);

CREATE INDEX ix_censeq_identity_user_login_login_provider_provider_key ON censeq_identity_user_login (login_provider, provider_key);

CREATE INDEX ix_censeq_identity_user_organization_unit_identity_user_id ON censeq_identity_user_organization_unit (identity_user_id);

CREATE INDEX ix_censeq_identity_user_organization_unit_user_id_organization ON censeq_identity_user_organization_unit (user_id, organization_unit_id);

CREATE INDEX ix_censeq_identity_user_role_identity_user_id ON censeq_identity_user_role (identity_user_id);

CREATE INDEX ix_censeq_identity_user_role_role_id_user_id ON censeq_identity_user_role (role_id, user_id);

CREATE INDEX ix_censeq_identity_user_token_identity_user_id ON censeq_identity_user_token (identity_user_id);

CREATE INDEX ix_censeq_menu_parent_id ON censeq_menu (parent_id);

CREATE UNIQUE INDEX ix_censeq_menu_tenant_id_parent_id_name ON censeq_menu (tenant_id, parent_id, name) WHERE is_deleted = false;

CREATE INDEX ix_censeq_menu_tenant_id_parent_id_sort ON censeq_menu (tenant_id, parent_id, sort);

CREATE UNIQUE INDEX ix_censeq_menu_tenant_id_path ON censeq_menu (tenant_id, path) WHERE is_deleted = false;

CREATE UNIQUE INDEX ix_censeq_menu_tenant_id_route_name ON censeq_menu (tenant_id, route_name) WHERE is_deleted = false;

CREATE UNIQUE INDEX ix_censeq_menu_permission_menu_id_permission_name ON censeq_menu_permission (menu_id, permission_name);

CREATE UNIQUE INDEX ix_censeq_openiddict_application_client_id ON censeq_openiddict_application (client_id);

CREATE INDEX ix_censeq_openiddict_authorization_application_id_status_subje ON censeq_openiddict_authorization (application_id, status, subject, type);

CREATE UNIQUE INDEX ix_censeq_openiddict_scope_name ON censeq_openiddict_scope (name);

CREATE INDEX ix_censeq_openiddict_token_application_id_status_subject_type ON censeq_openiddict_token (application_id, status, subject, type);

CREATE UNIQUE INDEX ix_censeq_openiddict_token_reference_id ON censeq_openiddict_token (reference_id);

CREATE INDEX ix_censeq_organization_unit_code ON censeq_organization_unit (code);

CREATE INDEX ix_censeq_organization_unit_role_role_id_organization_unit_id ON censeq_organization_unit_role (role_id, organization_unit_id);

CREATE INDEX ix_censeq_permission_definition_record_group_name ON censeq_permission_definition_record (group_name);

CREATE UNIQUE INDEX ix_censeq_permission_definition_record_name ON censeq_permission_definition_record (name);

CREATE UNIQUE INDEX ix_censeq_permission_grant_tenant_id_name_provider_name_provid ON censeq_permission_grant (tenant_id, name, provider_name, provider_key);

CREATE UNIQUE INDEX ix_censeq_permission_group_name ON censeq_permission_group (name);

CREATE UNIQUE INDEX ix_censeq_setting_name_provider_name_provider_key ON censeq_setting (name, provider_name, provider_key);

CREATE UNIQUE INDEX ix_censeq_setting_definition_record_name ON censeq_setting_definition_record (name);

CREATE UNIQUE INDEX ix_censeq_tenant_code ON censeq_tenant (code);

CREATE UNIQUE INDEX ix_censeq_tenant_domain ON censeq_tenant (domain);

CREATE INDEX ix_censeq_tenant_name ON censeq_tenant (name);

CREATE INDEX ix_censeq_tenant_normalized_name ON censeq_tenant (normalized_name);

CREATE UNIQUE INDEX ix_censeq_tenant_permission_grants_tenant_id_permission_name ON censeq_tenant_permission_grants (tenant_id, permission_name);

INSERT INTO ef_migrations_history (migration_id, product_version)
VALUES ('20260430151123_InitialDb', '8.0.16');

COMMIT;

