create table  `aix_distribution_lock`
(
       `lock_name`       VARCHAR(50) not null comment '����'
) comment '�ֲ�ʽ��';
alter  table `aix_distribution_lock`
       add constraint `PK_aix_disock_lock_nameE2BC` primary key (`lock_name`);


       insert into aix_distribution_lock(lock_name) values('AixScheduleTaskLock');


create table  `aix_schedule_task_info`
(
       `id`              INT auto_increment primary key not null comment '����',
       `task_group`      VARCHAR(50) comment '������ ������Ҫ������չ',
       `status`          TINYINT default 0 not null comment '״̬ 0=���� 1=����',
       `task_name`       VARCHAR(50) not null comment '��������',
       `task_desc`       VARCHAR(200) comment '��������',
       `cron`            VARCHAR(50) not null comment '��ʱ���ʽ',
       `task_content`    VARCHAR(500) not null comment '����',
       `last_execute_time` BIGINT default 0 not null comment '�ϴ�ִ��ʱ��',
       `next_execute_time` BIGINT default 0 not null comment '�´�ִ��ʱ��',
       `max_retry_count` INT default 0 not null comment '������Դ��� 0=������',
       `creator_id`      VARCHAR(50) not null comment '�����˱��',
       `create_time`     DATETIME default now() not null comment '��������',
       `modifier_id`     VARCHAR(50) not null comment '�޸��˱��',
       `modify_time`     DATETIME default now() not null comment '�޸�����'
) comment '��ʱ����';