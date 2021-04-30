create table  `aix_distribution_lock`
(
       `lock_name`       VARCHAR(50) not null comment '����'
) comment '�ֲ�ʽ��';
alter  table `aix_distribution_lock`
       add constraint `PK_aix_disock_lock_nameE2BC` primary key (`lock_name`);


       insert into aix_distribution_lock(lock_name) values('AixScheduleTaskLock');


create table  aix_schedule_task_info
(
       id                INT primary key auto_increment not null comment '����',
       task_group        VARCHAR(50) comment '������ ������Ҫ������չ',
       task_status       INT default 0 not null comment '״̬ 0=���� 1=����',
       task_name         VARCHAR(50) not null comment '��������',
       task_desc         VARCHAR(200) comment '��������',
       cron              VARCHAR(50) not null comment '��ʱ���ʽ',
       task_content      VARCHAR(500) not null comment '����',
       last_execute_time BIGINT default 0 not null comment '�ϴ�ִ��ʱ��',
       next_execute_time BIGINT default 0 not null comment '�´�ִ��ʱ��',
       max_retry_count   INT default 0 not null comment '������Դ��� 0=������',
       creator_id        VARCHAR(50) not null comment '�����˱��',
       create_time       TIMESTAMP default current_timestamp not null comment '��������',
       modifier_id       VARCHAR(50) not null comment '�޸��˱��',
       modify_time       TIMESTAMP default current_timestamp not null comment '�޸�����'
) comment '��ʱ����';

create table  aix_schedule_task_log
(
       id                INT primary key auto_increment not null comment '����',
       schedule_task_id  INT not null comment '��ʱ����id',
       retry_count       INT default 0 not null comment '���Դ���',
       trigger_code      INT default 0 not null comment '����code 0=��ʼ��  2=ִ�гɹ� 9=ִ��ʧ��  ',
       trigger_message   VARCHAR(500) comment '������Ϣ',
       trigger_time      DATETIME comment '����ʱ��',
       result_code       INT not null comment '���code 0=��ʼ��  2=ִ�гɹ� 9=ִ��ʧ��  ',
       result_message    VARCHAR(500) comment '�����Ϣ',
       result_time       DATETIME comment '���ʱ��',
       alarm_status      INT default 0 not null comment '�澯״̬ �澯״̬��0-Ĭ�ϡ�-1-����״̬��1-����澯��2-�澯�ɹ���9-�澯ʧ��',
       create_time       TIMESTAMP default current_timestamp not null comment '��������',
       modify_time       TIMESTAMP default current_timestamp not null comment '�޸�����'
) comment '��ʱ����log';