CREATE TABLE IF NOT EXISTS ef_migrations_history (
    migration_id character varying(150) NOT NULL,
    product_version character varying(32) NOT NULL,
    CONSTRAINT pk_ef_migrations_history PRIMARY KEY (migration_id)
);

START TRANSACTION;

CREATE TABLE abp_application (
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
    CONSTRAINT pk_abp_application PRIMARY KEY (id)
);

CREATE TABLE abp_audit_log (
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
    CONSTRAINT pk_abp_audit_log PRIMARY KEY (id)
);

CREATE TABLE abp_authorization (
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
    CONSTRAINT pk_abp_authorization PRIMARY KEY (id)
);

CREATE TABLE abp_background_job_record (
    id uuid NOT NULL,
    job_name character varying(128) NOT NULL,
    job_args character varying(1048576) NOT NULL,
    try_count smallint NOT NULL DEFAULT 0,
    creation_time timestamp without time zone NOT NULL,
    next_try_time timestamp without time zone NOT NULL,
    last_try_time timestamp without time zone,
    is_abandoned boolean NOT NULL DEFAULT FALSE,
    priority smallint NOT NULL DEFAULT 15,
    extra_properties text NOT NULL,
    concurrency_stamp character varying(40) NOT NULL,
    CONSTRAINT pk_abp_background_job_record PRIMARY KEY (id)
);

CREATE TABLE abp_claim_type (
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
    CONSTRAINT pk_abp_claim_type PRIMARY KEY (id)
);

CREATE TABLE abp_feature_definition_record (
    id uuid NOT NULL,
    group_name character varying(128) NOT NULL,
    name character varying(128) NOT NULL,
    parent_name character varying(128),
    display_name character varying(256) NOT NULL,
    description character varying(256),
    default_value character varying(256),
    is_visible_to_clients boolean NOT NULL,
    is_available_to_host boolean NOT NULL,
    allowed_providers character varying(256),
    value_type character varying(2048),
    extra_properties text,
    CONSTRAINT pk_abp_feature_definition_record PRIMARY KEY (id)
);

CREATE TABLE abp_feature_group_definition_record (
    id uuid NOT NULL,
    name character varying(128) NOT NULL,
    display_name character varying(256) NOT NULL,
    extra_properties text,
    CONSTRAINT pk_abp_feature_group_definition_record PRIMARY KEY (id)
);

CREATE TABLE abp_feature_value (
    id uuid NOT NULL,
    name character varying(128) NOT NULL,
    value character varying(128) NOT NULL,
    provider_name character varying(64),
    provider_key character varying(64),
    CONSTRAINT pk_abp_feature_value PRIMARY KEY (id)
);

CREATE TABLE abp_link_user (
    id uuid NOT NULL,
    source_user_id uuid NOT NULL,
    source_tenant_id uuid,
    target_user_id uuid NOT NULL,
    target_tenant_id uuid,
    CONSTRAINT pk_abp_link_user PRIMARY KEY (id)
);

CREATE TABLE abp_organization_unit (
    id uuid NOT NULL,
    tenant_id uuid,
    parent_id uuid,
    code character varying(95) NOT NULL,
    display_name character varying(128) NOT NULL,
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
    CONSTRAINT pk_abp_organization_unit PRIMARY KEY (id)
);

CREATE TABLE abp_permission_definition_record (
    id uuid NOT NULL,
    group_name character varying(128) NOT NULL,
    name character varying(128) NOT NULL,
    parent_name character varying(128),
    display_name character varying(256) NOT NULL,
    is_enabled boolean NOT NULL,
    multi_tenancy_side smallint NOT NULL,
    providers character varying(128),
    state_checkers character varying(256),
    extra_properties text,
    CONSTRAINT pk_abp_permission_definition_record PRIMARY KEY (id)
);

CREATE TABLE abp_permission_grant (
    id uuid NOT NULL,
    tenant_id uuid,
    name character varying(128) NOT NULL,
    provider_name character varying(64) NOT NULL,
    provider_key character varying(64) NOT NULL,
    CONSTRAINT pk_abp_permission_grant PRIMARY KEY (id)
);

CREATE TABLE abp_permission_group_definition_record (
    id uuid NOT NULL,
    name character varying(128) NOT NULL,
    display_name character varying(256) NOT NULL,
    extra_properties text,
    CONSTRAINT pk_abp_permission_group_definition_record PRIMARY KEY (id)
);

CREATE TABLE abp_role (
    id uuid NOT NULL,
    tenant_id uuid,
    name character varying(256) NOT NULL,
    normalized_name character varying(256) NOT NULL,
    is_default boolean NOT NULL,
    is_static boolean NOT NULL,
    is_public boolean NOT NULL,
    entity_version integer NOT NULL,
    extra_properties text NOT NULL,
    concurrency_stamp character varying(40) NOT NULL,
    CONSTRAINT pk_abp_role PRIMARY KEY (id)
);

CREATE TABLE abp_scope (
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
    CONSTRAINT pk_abp_scope PRIMARY KEY (id)
);

CREATE TABLE abp_security_log (
    id uuid NOT NULL,
    tenant_id uuid,
    application_name character varying(96),
    identity character varying(96),
    action character varying(96),
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
    CONSTRAINT pk_abp_security_log PRIMARY KEY (id)
);

CREATE TABLE abp_session (
    id uuid NOT NULL,
    session_id character varying(128) NOT NULL,
    device character varying(64) NOT NULL,
    device_info character varying(64),
    tenant_id uuid,
    user_id uuid NOT NULL,
    client_id character varying(64),
    ip_addresses character varying(256),
    signed_in timestamp without time zone NOT NULL,
    last_accessed timestamp without time zone,
    CONSTRAINT pk_abp_session PRIMARY KEY (id)
);

CREATE TABLE abp_setting (
    id uuid NOT NULL,
    name character varying(128) NOT NULL,
    value character varying(2048) NOT NULL,
    provider_name character varying(64),
    provider_key character varying(64),
    CONSTRAINT pk_abp_setting PRIMARY KEY (id)
);

CREATE TABLE abp_setting_definition_record (
    id uuid NOT NULL,
    name character varying(128) NOT NULL,
    display_name character varying(256) NOT NULL,
    description character varying(512),
    default_value character varying(2048),
    is_visible_to_clients boolean NOT NULL,
    providers character varying(1024),
    is_inherited boolean NOT NULL,
    is_encrypted boolean NOT NULL,
    extra_properties text,
    CONSTRAINT pk_abp_setting_definition_record PRIMARY KEY (id)
);

CREATE TABLE abp_tenant (
    id uuid NOT NULL,
    name character varying(64) NOT NULL,
    normalized_name character varying(64) NOT NULL,
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
    CONSTRAINT pk_abp_tenant PRIMARY KEY (id)
);

CREATE TABLE abp_token (
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
    CONSTRAINT pk_abp_token PRIMARY KEY (id)
);

CREATE TABLE abp_user (
    id uuid NOT NULL,
    tenant_id uuid,
    user_name character varying(256) NOT NULL,
    normalized_user_name character varying(256) NOT NULL,
    name character varying(64),
    surname character varying(64),
    email character varying(256) NOT NULL,
    normalized_email character varying(256) NOT NULL,
    email_confirmed boolean NOT NULL DEFAULT FALSE,
    password_hash character varying(256),
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
    CONSTRAINT pk_abp_user PRIMARY KEY (id)
);

CREATE TABLE abp_user_delegation (
    id uuid NOT NULL,
    tenant_id uuid,
    source_user_id uuid NOT NULL,
    target_user_id uuid NOT NULL,
    start_time timestamp without time zone NOT NULL,
    end_time timestamp without time zone NOT NULL,
    CONSTRAINT pk_abp_user_delegation PRIMARY KEY (id)
);

CREATE TABLE abp_audit_log_action (
    id uuid NOT NULL,
    tenant_id uuid,
    audit_log_id uuid NOT NULL,
    service_name character varying(256),
    method_name character varying(128),
    parameters character varying(2000),
    execution_time timestamp without time zone NOT NULL,
    execution_duration integer NOT NULL,
    extra_properties text,
    CONSTRAINT pk_abp_audit_log_action PRIMARY KEY (id),
    CONSTRAINT fk_abp_audit_log_action_abp_audit_log_audit_log_id FOREIGN KEY (audit_log_id) REFERENCES abp_audit_log (id) ON DELETE CASCADE
);

CREATE TABLE abp_entity_change (
    id uuid NOT NULL,
    audit_log_id uuid NOT NULL,
    tenant_id uuid,
    change_time timestamp without time zone NOT NULL,
    change_type smallint NOT NULL,
    entity_tenant_id uuid,
    entity_id character varying(128),
    entity_type_full_name character varying(128) NOT NULL,
    extra_properties text,
    CONSTRAINT pk_abp_entity_change PRIMARY KEY (id),
    CONSTRAINT fk_abp_entity_change_abp_audit_log_audit_log_id FOREIGN KEY (audit_log_id) REFERENCES abp_audit_log (id) ON DELETE CASCADE
);

CREATE TABLE abp_organization_unit_role (
    role_id uuid NOT NULL,
    organization_unit_id uuid NOT NULL,
    tenant_id uuid,
    creation_time timestamp without time zone NOT NULL,
    creator_id uuid,
    CONSTRAINT pk_abp_organization_unit_role PRIMARY KEY (organization_unit_id, role_id),
    CONSTRAINT fk_abp_organization_unit_role_abp_organization_unit_organizati FOREIGN KEY (organization_unit_id) REFERENCES abp_organization_unit (id) ON DELETE CASCADE
);

CREATE TABLE abp_role_claim (
    id uuid NOT NULL,
    role_id uuid NOT NULL,
    identity_role_id uuid,
    tenant_id uuid,
    claim_type character varying(256) NOT NULL,
    claim_value character varying(1024),
    CONSTRAINT pk_abp_role_claim PRIMARY KEY (id),
    CONSTRAINT fk_abp_role_claim_abp_role_identity_role_id FOREIGN KEY (identity_role_id) REFERENCES abp_role (id)
);

CREATE TABLE abp_tenant_connection_string (
    tenant_id uuid NOT NULL,
    name character varying(64) NOT NULL,
    value character varying(1024) NOT NULL,
    CONSTRAINT pk_abp_tenant_connection_string PRIMARY KEY (tenant_id, name),
    CONSTRAINT fk_abp_tenant_connection_string_abp_tenant_tenant_id FOREIGN KEY (tenant_id) REFERENCES abp_tenant (id) ON DELETE CASCADE
);
COMMENT ON COLUMN abp_tenant_connection_string.tenant_id IS '租户ID';

CREATE TABLE abp_user_claim (
    id uuid NOT NULL,
    user_id uuid NOT NULL,
    identity_user_id uuid,
    tenant_id uuid,
    claim_type character varying(256) NOT NULL,
    claim_value character varying(1024),
    CONSTRAINT pk_abp_user_claim PRIMARY KEY (id),
    CONSTRAINT fk_abp_user_claim_abp_user_identity_user_id FOREIGN KEY (identity_user_id) REFERENCES abp_user (id)
);

CREATE TABLE abp_user_login (
    user_id uuid NOT NULL,
    login_provider character varying(64) NOT NULL,
    tenant_id uuid,
    provider_key character varying(196) NOT NULL,
    provider_display_name character varying(128),
    identity_user_id uuid,
    CONSTRAINT pk_abp_user_login PRIMARY KEY (user_id, login_provider),
    CONSTRAINT fk_abp_user_login_abp_user_identity_user_id FOREIGN KEY (identity_user_id) REFERENCES abp_user (id)
);

CREATE TABLE abp_user_organization_unit (
    user_id uuid NOT NULL,
    organization_unit_id uuid NOT NULL,
    tenant_id uuid,
    identity_user_id uuid,
    creation_time timestamp without time zone NOT NULL,
    creator_id uuid,
    CONSTRAINT pk_abp_user_organization_unit PRIMARY KEY (organization_unit_id, user_id),
    CONSTRAINT fk_abp_user_organization_unit_abp_user_identity_user_id FOREIGN KEY (identity_user_id) REFERENCES abp_user (id)
);

CREATE TABLE abp_user_role (
    user_id uuid NOT NULL,
    role_id uuid NOT NULL,
    tenant_id uuid,
    identity_user_id uuid,
    CONSTRAINT pk_abp_user_role PRIMARY KEY (user_id, role_id),
    CONSTRAINT fk_abp_user_role_abp_user_identity_user_id FOREIGN KEY (identity_user_id) REFERENCES abp_user (id)
);

CREATE TABLE abp_user_token (
    user_id uuid NOT NULL,
    login_provider character varying(64) NOT NULL,
    name character varying(128) NOT NULL,
    tenant_id uuid,
    value text,
    identity_user_id uuid,
    CONSTRAINT pk_abp_user_token PRIMARY KEY (user_id, login_provider, name),
    CONSTRAINT fk_abp_user_token_abp_user_identity_user_id FOREIGN KEY (identity_user_id) REFERENCES abp_user (id)
);

CREATE TABLE abp_entity_property_change (
    id uuid NOT NULL,
    tenant_id uuid,
    entity_change_id uuid NOT NULL,
    new_value character varying(512),
    original_value character varying(512),
    property_name character varying(128) NOT NULL,
    property_type_full_name character varying(64) NOT NULL,
    CONSTRAINT pk_abp_entity_property_change PRIMARY KEY (id),
    CONSTRAINT fk_abp_entity_property_change_abp_entity_change_entity_change_ FOREIGN KEY (entity_change_id) REFERENCES abp_entity_change (id) ON DELETE CASCADE
);

CREATE UNIQUE INDEX ix_abp_application_client_id ON abp_application (client_id);

CREATE INDEX ix_abp_audit_log_tenant_id_execution_time ON abp_audit_log (tenant_id, execution_time);

CREATE INDEX ix_abp_audit_log_tenant_id_user_id_execution_time ON abp_audit_log (tenant_id, user_id, execution_time);

CREATE INDEX ix_abp_audit_log_action_audit_log_id ON abp_audit_log_action (audit_log_id);

CREATE INDEX ix_abp_audit_log_action_tenant_id_service_name_method_name_exe ON abp_audit_log_action (tenant_id, service_name, method_name, execution_time);

CREATE INDEX ix_abp_authorization_application_id_status_subject_type ON abp_authorization (application_id, status, subject, type);

CREATE INDEX ix_abp_background_job_record_is_abandoned_next_try_time ON abp_background_job_record (is_abandoned, next_try_time);

CREATE INDEX ix_abp_entity_change_audit_log_id ON abp_entity_change (audit_log_id);

CREATE INDEX ix_abp_entity_change_tenant_id_entity_type_full_name_entity_id ON abp_entity_change (tenant_id, entity_type_full_name, entity_id);

CREATE INDEX ix_abp_entity_property_change_entity_change_id ON abp_entity_property_change (entity_change_id);

CREATE INDEX ix_abp_feature_definition_record_group_name ON abp_feature_definition_record (group_name);

CREATE UNIQUE INDEX ix_abp_feature_definition_record_name ON abp_feature_definition_record (name);

CREATE UNIQUE INDEX ix_abp_feature_group_definition_record_name ON abp_feature_group_definition_record (name);

CREATE UNIQUE INDEX ix_abp_feature_value_name_provider_name_provider_key ON abp_feature_value (name, provider_name, provider_key);

CREATE UNIQUE INDEX ix_abp_link_user_source_user_id_source_tenant_id_target_user_i ON abp_link_user (source_user_id, source_tenant_id, target_user_id, target_tenant_id);

CREATE INDEX ix_abp_organization_unit_code ON abp_organization_unit (code);

CREATE INDEX ix_abp_organization_unit_role_role_id_organization_unit_id ON abp_organization_unit_role (role_id, organization_unit_id);

CREATE INDEX ix_abp_permission_definition_record_group_name ON abp_permission_definition_record (group_name);

CREATE UNIQUE INDEX ix_abp_permission_definition_record_name ON abp_permission_definition_record (name);

CREATE UNIQUE INDEX ix_abp_permission_grant_tenant_id_name_provider_name_provider_ ON abp_permission_grant (tenant_id, name, provider_name, provider_key);

CREATE UNIQUE INDEX ix_abp_permission_group_definition_record_name ON abp_permission_group_definition_record (name);

CREATE INDEX ix_abp_role_normalized_name ON abp_role (normalized_name);

CREATE INDEX ix_abp_role_claim_identity_role_id ON abp_role_claim (identity_role_id);

CREATE INDEX ix_abp_role_claim_role_id ON abp_role_claim (role_id);

CREATE UNIQUE INDEX ix_abp_scope_name ON abp_scope (name);

CREATE INDEX ix_abp_security_log_tenant_id_action ON abp_security_log (tenant_id, action);

CREATE INDEX ix_abp_security_log_tenant_id_application_name ON abp_security_log (tenant_id, application_name);

CREATE INDEX ix_abp_security_log_tenant_id_identity ON abp_security_log (tenant_id, identity);

CREATE INDEX ix_abp_security_log_tenant_id_user_id ON abp_security_log (tenant_id, user_id);

CREATE INDEX ix_abp_session_device ON abp_session (device);

CREATE INDEX ix_abp_session_session_id ON abp_session (session_id);

CREATE INDEX ix_abp_session_tenant_id_user_id ON abp_session (tenant_id, user_id);

CREATE UNIQUE INDEX ix_abp_setting_name_provider_name_provider_key ON abp_setting (name, provider_name, provider_key);

CREATE UNIQUE INDEX ix_abp_setting_definition_record_name ON abp_setting_definition_record (name);

CREATE INDEX ix_abp_tenant_name ON abp_tenant (name);

CREATE INDEX ix_abp_tenant_normalized_name ON abp_tenant (normalized_name);

CREATE INDEX ix_abp_token_application_id_status_subject_type ON abp_token (application_id, status, subject, type);

CREATE UNIQUE INDEX ix_abp_token_reference_id ON abp_token (reference_id);

CREATE INDEX ix_abp_user_email ON abp_user (email);

CREATE INDEX ix_abp_user_normalized_email ON abp_user (normalized_email);

CREATE INDEX ix_abp_user_normalized_user_name ON abp_user (normalized_user_name);

CREATE INDEX ix_abp_user_user_name ON abp_user (user_name);

CREATE INDEX ix_abp_user_claim_identity_user_id ON abp_user_claim (identity_user_id);

CREATE INDEX ix_abp_user_claim_user_id ON abp_user_claim (user_id);

CREATE INDEX ix_abp_user_login_identity_user_id ON abp_user_login (identity_user_id);

CREATE INDEX ix_abp_user_login_login_provider_provider_key ON abp_user_login (login_provider, provider_key);

CREATE INDEX ix_abp_user_organization_unit_identity_user_id ON abp_user_organization_unit (identity_user_id);

CREATE INDEX ix_abp_user_organization_unit_user_id_organization_unit_id ON abp_user_organization_unit (user_id, organization_unit_id);

CREATE INDEX ix_abp_user_role_identity_user_id ON abp_user_role (identity_user_id);

CREATE INDEX ix_abp_user_role_role_id_user_id ON abp_user_role (role_id, user_id);

CREATE INDEX ix_abp_user_token_identity_user_id ON abp_user_token (identity_user_id);

INSERT INTO ef_migrations_history (migration_id, product_version)
VALUES ('20260203131533_InitialDb', '8.0.16');

COMMIT;

