create table  aix_distribution_lock
(
       lock_name         VARCHAR(50) not null 	/*����*/
);
alter  table aix_distribution_lock
       add constraint PK_aix_disock_lock_nameE2BC primary key (lock_name);
EXEC sp_addextendedproperty 'MS_Description', '�ֲ�ʽ��', 'user', dbo, 'table', aix_distribution_lock, NULL, NULL;
EXEC sp_addextendedproperty 'MS_Description', '����', 'user', dbo, 'table', aix_distribution_lock, 'column', lock_name;


insert into aix_distribution_lock(lock_name) values('ScheduleTaskLock');



create table  aix_schedule_task_info
(
       id                INT identity(1, 1) not null 	/*����*/,
       executor          VARCHAR(50) 	/*ִ���� ������Ҫ������չ*/,
       status            TINYINT default 0 not null 	/*״̬ 0=���� 1=����*/,
       task_name         VARCHAR(50) not null 	/*��������*/,
       task_desc         VARCHAR(200) 	/*��������*/,
       cron              VARCHAR(50) not null 	/*��ʱ���ʽ*/,
       executor_param    VARCHAR(500) 	/*ִ�в���*/,
       last_execute_time BIGINT default 0 not null 	/*�ϴ�ִ��ʱ��*/,
       next_execute_time BIGINT default 0 not null 	/*�´�ִ��ʱ��*/,
       max_retry_count   INT default 0 not null 	/*������Դ��� 0=������*/,
       creator_id        VARCHAR(50) not null 	/*�����˱��*/,
       create_time       DATETIME default getdate() not null 	/*��������*/,
       modifier_id       VARCHAR(50) not null 	/*�޸��˱��*/,
       modify_time       DATETIME default getdate() not null 	/*�޸�����*/
);
alter  table aix_schedule_task_info
       add constraint PK_aix_schnfo_id5BDF primary key (id);
EXEC sp_addextendedproperty 'MS_Description', '��ʱ����', 'user', dbo, 'table', aix_schedule_task_info, NULL, NULL;
EXEC sp_addextendedproperty 'MS_Description', '����', 'user', dbo, 'table', aix_schedule_task_info, 'column', id;
EXEC sp_addextendedproperty 'MS_Description', 'ִ���� ������Ҫ������չ', 'user', dbo, 'table', aix_schedule_task_info, 'column', executor;
EXEC sp_addextendedproperty 'MS_Description', '״̬ 0=���� 1=����', 'user', dbo, 'table', aix_schedule_task_info, 'column', status;
EXEC sp_addextendedproperty 'MS_Description', '��������', 'user', dbo, 'table', aix_schedule_task_info, 'column', task_name;
EXEC sp_addextendedproperty 'MS_Description', '��������', 'user', dbo, 'table', aix_schedule_task_info, 'column', task_desc;
EXEC sp_addextendedproperty 'MS_Description', '��ʱ���ʽ', 'user', dbo, 'table', aix_schedule_task_info, 'column', cron;
EXEC sp_addextendedproperty 'MS_Description', 'ִ�в���', 'user', dbo, 'table', aix_schedule_task_info, 'column', executor_param;
EXEC sp_addextendedproperty 'MS_Description', '�ϴ�ִ��ʱ��', 'user', dbo, 'table', aix_schedule_task_info, 'column', last_execute_time;
EXEC sp_addextendedproperty 'MS_Description', '�´�ִ��ʱ��', 'user', dbo, 'table', aix_schedule_task_info, 'column', next_execute_time;
EXEC sp_addextendedproperty 'MS_Description', '������Դ��� 0=������', 'user', dbo, 'table', aix_schedule_task_info, 'column', max_retry_count;
EXEC sp_addextendedproperty 'MS_Description', '�����˱��', 'user', dbo, 'table', aix_schedule_task_info, 'column', creator_id;
EXEC sp_addextendedproperty 'MS_Description', '��������', 'user', dbo, 'table', aix_schedule_task_info, 'column', create_time;
EXEC sp_addextendedproperty 'MS_Description', '�޸��˱��', 'user', dbo, 'table', aix_schedule_task_info, 'column', modifier_id;
EXEC sp_addextendedproperty 'MS_Description', '�޸�����', 'user', dbo, 'table', aix_schedule_task_info, 'column', modify_time;
