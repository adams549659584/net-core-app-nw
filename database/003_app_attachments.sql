-- Table: public.app_attachments

-- DROP TABLE public.app_attachments;

CREATE TABLE public.app_attachments
(
    id bigint NOT NULL DEFAULT snow_flake_id(),
    content_type character varying(64) COLLATE pg_catalog."default" NOT NULL,
    file_name character varying(256) COLLATE pg_catalog."default" NOT NULL,
    length bigint NOT NULL DEFAULT 0,
    content bytea,
    business_id bigint NOT NULL,
    created_at timestamp without time zone NOT NULL DEFAULT now(),
    creator_id character varying(32) COLLATE pg_catalog."default" NOT NULL,
    CONSTRAINT app_attachments_pkey PRIMARY KEY (id),
    CONSTRAINT fk_app_attachments_owner FOREIGN KEY (creator_id)
        REFERENCES public.app_users (id) MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE CASCADE
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

ALTER TABLE public.app_attachments
    OWNER to postgres;
COMMENT ON TABLE public.app_attachments
    IS '附件表';

COMMENT ON COLUMN public.app_attachments.id
    IS '附件ID';

COMMENT ON COLUMN public.app_attachments.content_type
    IS '内容类型（HTTP Content Type）';

COMMENT ON COLUMN public.app_attachments.content
    IS '附件内容';

COMMENT ON COLUMN public.app_attachments.created_at
    IS '创建时间';

COMMENT ON COLUMN public.app_attachments.creator_id
    IS '创建附件的用户ID';

COMMENT ON COLUMN public.app_attachments.file_name
    IS '文件名';

COMMENT ON COLUMN public.app_attachments.length
    IS '附件大小';

COMMENT ON COLUMN public.app_attachments.business_id
    IS '附件所属的业务ID，可以是任意表的ID，如果业务表有附件， 则需要来这里删除。';
