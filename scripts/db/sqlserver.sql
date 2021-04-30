create table  aix_distribution_lock
(
       lock_name         VARCHAR(50) not null 	/*主键*/
);
alter  table aix_distribution_lock
       add constraint PK_aix_disock_lock_nameE2BC primary key (lock_name);
EXEC sp_addextendedproperty 'MS_Description', '分布式锁', 'user', dbo, 'table', aix_distribution_lock, NULL, NULL;
EXEC sp_addextendedproperty 'MS_Description', '主键', 'user', dbo, 'table', aix_distribution_lock, 'column', lock_name;


insert into aix_distribution_lock(lock_name) values('AixScheduleTaskLock');


create table  aix_schedule_task_info
(
       id                INT identity(1, 1) not null 	/*主键*/,
       task_group        VARCHAR(50) 	/*所属组 根据需要进行扩展*/,
       task_status       INT default 0 not null 	/*状态 0=禁用 1=启动*/,
       task_name         VARCHAR(50) not null 	/*任务名称*/,
       task_desc         VARCHAR(200) 	/*任务描述*/,
       cron              VARCHAR(50) not null 	/*定时表达式*/,
       task_content      VARCHAR(500) not null 	/*内容*/,
       last_execute_time BIGINT default 0 not null 	/*上次执行时间*/,
       next_execute_time BIGINT default 0 not null 	/*下次执行时间*/,
       max_retry_count   INT default 0 not null 	/*最大重试次数 0=不重试*/,
       creator_id        VARCHAR(50) not null 	/*创建人编号*/,
       create_time       DATETIME default getdate() not null 	/*创建日期*/,
       modifier_id       VARCHAR(50) not null 	/*修改人编号*/,
       modify_time       DATETIME default getdate() not null 	/*修改日期*/
);
alter  table aix_schedule_task_info
       add constraint PK_aix_schnfo_id5BDF primary key (id);
EXEC sp_addextendedproperty 'MS_Description', '定时任务', 'user', dbo, 'table', aix_schedule_task_info, NULL, NULL;
EXEC sp_addextendedproperty 'MS_Description', '主键', 'user', dbo, 'table', aix_schedule_task_info, 'column', id;
EXEC sp_addextendedproperty 'MS_Description', '所属组 根据需要进行扩展', 'user', dbo, 'table', aix_schedule_task_info, 'column', task_group;
EXEC sp_addextendedproperty 'MS_Description', '状态 0=禁用 1=启动', 'user', dbo, 'table', aix_schedule_task_info, 'column', task_status;
EXEC sp_addextendedproperty 'MS_Description', '任务名称', 'user', dbo, 'table', aix_schedule_task_info, 'column', task_name;
EXEC sp_addextendedproperty 'MS_Description', '任务描述', 'user', dbo, 'table', aix_schedule_task_info, 'column', task_desc;
EXEC sp_addextendedproperty 'MS_Description', '定时表达式', 'user', dbo, 'table', aix_schedule_task_info, 'column', cron;
EXEC sp_addextendedproperty 'MS_Description', '内容', 'user', dbo, 'table', aix_schedule_task_info, 'column', task_content;
EXEC sp_addextendedproperty 'MS_Description', '上次执行时间', 'user', dbo, 'table', aix_schedule_task_info, 'column', last_execute_time;
EXEC sp_addextendedproperty 'MS_Description', '下次执行时间', 'user', dbo, 'table', aix_schedule_task_info, 'column', next_execute_time;
EXEC sp_addextendedproperty 'MS_Description', '最大重试次数 0=不重试', 'user', dbo, 'table', aix_schedule_task_info, 'column', max_retry_count;
EXEC sp_addextendedproperty 'MS_Description', '创建人编号', 'user', dbo, 'table', aix_schedule_task_info, 'column', creator_id;
EXEC sp_addextendedproperty 'MS_Description', '创建日期', 'user', dbo, 'table', aix_schedule_task_info, 'column', create_time;
EXEC sp_addextendedproperty 'MS_Description', '修改人编号', 'user', dbo, 'table', aix_schedule_task_info, 'column', modifier_id;
EXEC sp_addextendedproperty 'MS_Description', '修改日期', 'user', dbo, 'table', aix_schedule_task_info, 'column', modify_time;




create table  aix_schedule_task_log
(
       id                INT identity(1, 1) not null 	/*主键*/,
       schedule_task_id  INT not null 	/*定时任务id*/,
       retry_count       INT default 0 not null 	/*重试次数*/,
       trigger_code      INT default 0 not null 	/*调度code 0=初始化  2=执行成功 9=执行失败  */,
       trigger_message   VARCHAR(500) 	/*调度信息*/,
       trigger_time      DATETIME 	/*调度时间*/,
       result_code       INT not null 	/*结果code 0=初始化  2=执行成功 9=执行失败  */,
       result_message    VARCHAR(500) 	/*结果信息*/,
       result_time       DATETIME 	/*结果时间*/,
       alarm_status      INT default 0 not null 	/*告警状态 告警状态：0-默认、-1-锁定状态、1-无需告警、2-告警成功、9-告警失败*/,
       create_time       DATETIME default getdate() not null 	/*创建日期*/,
       modify_time       DATETIME default getdate() not null 	/*修改日期*/
);
alter  table aix_schedule_task_log
       add constraint PK_aix_schlog_idA4D9 primary key (id);
EXEC sp_addextendedproperty 'MS_Description', '定时任务log', 'user', dbo, 'table', aix_schedule_task_log, NULL, NULL;
EXEC sp_addextendedproperty 'MS_Description', '主键', 'user', dbo, 'table', aix_schedule_task_log, 'column', id;
EXEC sp_addextendedproperty 'MS_Description', '定时任务id', 'user', dbo, 'table', aix_schedule_task_log, 'column', schedule_task_id;
EXEC sp_addextendedproperty 'MS_Description', '重试次数', 'user', dbo, 'table', aix_schedule_task_log, 'column', retry_count;
EXEC sp_addextendedproperty 'MS_Description', '调度code 0=初始化  2=执行成功 9=执行失败  ', 'user', dbo, 'table', aix_schedule_task_log, 'column', trigger_code;
EXEC sp_addextendedproperty 'MS_Description', '调度信息', 'user', dbo, 'table', aix_schedule_task_log, 'column', trigger_message;
EXEC sp_addextendedproperty 'MS_Description', '调度时间', 'user', dbo, 'table', aix_schedule_task_log, 'column', trigger_time;
EXEC sp_addextendedproperty 'MS_Description', '结果code 0=初始化  2=执行成功 9=执行失败  ', 'user', dbo, 'table', aix_schedule_task_log, 'column', result_code;
EXEC sp_addextendedproperty 'MS_Description', '结果信息', 'user', dbo, 'table', aix_schedule_task_log, 'column', result_message;
EXEC sp_addextendedproperty 'MS_Description', '结果时间', 'user', dbo, 'table', aix_schedule_task_log, 'column', result_time;
EXEC sp_addextendedproperty 'MS_Description', '告警状态 告警状态：0-默认、-1-锁定状态、1-无需告警、2-告警成功、9-告警失败', 'user', dbo, 'table', aix_schedule_task_log, 'column', alarm_status;
EXEC sp_addextendedproperty 'MS_Description', '创建日期', 'user', dbo, 'table', aix_schedule_task_log, 'column', create_time;
EXEC sp_addextendedproperty 'MS_Description', '修改日期', 'user', dbo, 'table', aix_schedule_task_log, 'column', modify_time;