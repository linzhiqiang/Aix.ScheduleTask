create table  aix_distribution_lock
(
       lock_name         VARCHAR(50) not null 	/*����*/
);
alter  table aix_distribution_lock
       add constraint PK_aix_disock_lock_nameE2BC primary key (lock_name);
EXEC sp_addextendedproperty 'MS_Description', '�ֲ�ʽ��', 'user', dbo, 'table', aix_distribution_lock, NULL, NULL;
EXEC sp_addextendedproperty 'MS_Description', '����', 'user', dbo, 'table', aix_distribution_lock, 'column', lock_name;


insert into aix_distribution_lock(lock_name) values('AixScheduleTaskLock');


create table  aix_schedule_task_info
(
       id                INT identity(1, 1) not null 	/*����*/,
       task_group        VARCHAR(50) 	/*������ ������Ҫ������չ*/,
       task_status       INT default 0 not null 	/*״̬ 0=���� 1=����*/,
       task_name         VARCHAR(50) not null 	/*��������*/,
       task_desc         VARCHAR(200) 	/*��������*/,
       cron              VARCHAR(50) not null 	/*��ʱ���ʽ*/,
       task_content      VARCHAR(500) not null 	/*����*/,
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
EXEC sp_addextendedproperty 'MS_Description', '������ ������Ҫ������չ', 'user', dbo, 'table', aix_schedule_task_info, 'column', task_group;
EXEC sp_addextendedproperty 'MS_Description', '״̬ 0=���� 1=����', 'user', dbo, 'table', aix_schedule_task_info, 'column', task_status;
EXEC sp_addextendedproperty 'MS_Description', '��������', 'user', dbo, 'table', aix_schedule_task_info, 'column', task_name;
EXEC sp_addextendedproperty 'MS_Description', '��������', 'user', dbo, 'table', aix_schedule_task_info, 'column', task_desc;
EXEC sp_addextendedproperty 'MS_Description', '��ʱ���ʽ', 'user', dbo, 'table', aix_schedule_task_info, 'column', cron;
EXEC sp_addextendedproperty 'MS_Description', '����', 'user', dbo, 'table', aix_schedule_task_info, 'column', task_content;
EXEC sp_addextendedproperty 'MS_Description', '�ϴ�ִ��ʱ��', 'user', dbo, 'table', aix_schedule_task_info, 'column', last_execute_time;
EXEC sp_addextendedproperty 'MS_Description', '�´�ִ��ʱ��', 'user', dbo, 'table', aix_schedule_task_info, 'column', next_execute_time;
EXEC sp_addextendedproperty 'MS_Description', '������Դ��� 0=������', 'user', dbo, 'table', aix_schedule_task_info, 'column', max_retry_count;
EXEC sp_addextendedproperty 'MS_Description', '�����˱��', 'user', dbo, 'table', aix_schedule_task_info, 'column', creator_id;
EXEC sp_addextendedproperty 'MS_Description', '��������', 'user', dbo, 'table', aix_schedule_task_info, 'column', create_time;
EXEC sp_addextendedproperty 'MS_Description', '�޸��˱��', 'user', dbo, 'table', aix_schedule_task_info, 'column', modifier_id;
EXEC sp_addextendedproperty 'MS_Description', '�޸�����', 'user', dbo, 'table', aix_schedule_task_info, 'column', modify_time;




create table  aix_schedule_task_log
(
       id                INT identity(1, 1) not null 	/*����*/,
       schedule_task_id  INT not null 	/*��ʱ����id*/,
       retry_count       INT default 0 not null 	/*���Դ���*/,
       trigger_code      INT default 0 not null 	/*����code 0=��ʼ��  2=ִ�гɹ� 9=ִ��ʧ��  */,
       trigger_message   VARCHAR(500) 	/*������Ϣ*/,
       trigger_time      DATETIME 	/*����ʱ��*/,
       result_code       INT not null 	/*���code 0=��ʼ��  2=ִ�гɹ� 9=ִ��ʧ��  */,
       result_message    VARCHAR(500) 	/*�����Ϣ*/,
       result_time       DATETIME 	/*���ʱ��*/,
       alarm_status      INT default 0 not null 	/*�澯״̬ �澯״̬��0-Ĭ�ϡ�-1-����״̬��1-����澯��2-�澯�ɹ���9-�澯ʧ��*/,
       create_time       DATETIME default getdate() not null 	/*��������*/,
       modify_time       DATETIME default getdate() not null 	/*�޸�����*/
);
alter  table aix_schedule_task_log
       add constraint PK_aix_schlog_idA4D9 primary key (id);
EXEC sp_addextendedproperty 'MS_Description', '��ʱ����log', 'user', dbo, 'table', aix_schedule_task_log, NULL, NULL;
EXEC sp_addextendedproperty 'MS_Description', '����', 'user', dbo, 'table', aix_schedule_task_log, 'column', id;
EXEC sp_addextendedproperty 'MS_Description', '��ʱ����id', 'user', dbo, 'table', aix_schedule_task_log, 'column', schedule_task_id;
EXEC sp_addextendedproperty 'MS_Description', '���Դ���', 'user', dbo, 'table', aix_schedule_task_log, 'column', retry_count;
EXEC sp_addextendedproperty 'MS_Description', '����code 0=��ʼ��  2=ִ�гɹ� 9=ִ��ʧ��  ', 'user', dbo, 'table', aix_schedule_task_log, 'column', trigger_code;
EXEC sp_addextendedproperty 'MS_Description', '������Ϣ', 'user', dbo, 'table', aix_schedule_task_log, 'column', trigger_message;
EXEC sp_addextendedproperty 'MS_Description', '����ʱ��', 'user', dbo, 'table', aix_schedule_task_log, 'column', trigger_time;
EXEC sp_addextendedproperty 'MS_Description', '���code 0=��ʼ��  2=ִ�гɹ� 9=ִ��ʧ��  ', 'user', dbo, 'table', aix_schedule_task_log, 'column', result_code;
EXEC sp_addextendedproperty 'MS_Description', '�����Ϣ', 'user', dbo, 'table', aix_schedule_task_log, 'column', result_message;
EXEC sp_addextendedproperty 'MS_Description', '���ʱ��', 'user', dbo, 'table', aix_schedule_task_log, 'column', result_time;
EXEC sp_addextendedproperty 'MS_Description', '�澯״̬ �澯״̬��0-Ĭ�ϡ�-1-����״̬��1-����澯��2-�澯�ɹ���9-�澯ʧ��', 'user', dbo, 'table', aix_schedule_task_log, 'column', alarm_status;
EXEC sp_addextendedproperty 'MS_Description', '��������', 'user', dbo, 'table', aix_schedule_task_log, 'column', create_time;
EXEC sp_addextendedproperty 'MS_Description', '�޸�����', 'user', dbo, 'table', aix_schedule_task_log, 'column', modify_time;