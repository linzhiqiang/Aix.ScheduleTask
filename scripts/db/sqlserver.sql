create table  aix_distribution_lock
(
       lock_name         VARCHAR(50) not null 	/*主键*/
);
alter  table aix_distribution_lock
       add constraint PK_aix_disock_lock_nameE2BC primary key (lock_name);
EXEC sp_addextendedproperty 'MS_Description', '分布式锁', 'user', dbo, 'table', aix_distribution_lock, NULL, NULL;
EXEC sp_addextendedproperty 'MS_Description', '主键', 'user', dbo, 'table', aix_distribution_lock, 'column', lock_name;


insert into aix_distribution_lock(lock_name) values('ScheduleTaskLock');



create table  aix_schedule_task_info
(
       id                INT identity(1, 1) not null 	/*主键*/,
       executor          VARCHAR(50) 	/*执行器 根据需要进行扩展*/,
       status            TINYINT default 0 not null 	/*状态 0=禁用 1=启动*/,
       task_name         VARCHAR(50) not null 	/*任务名称*/,
       task_desc         VARCHAR(200) 	/*任务描述*/,
       cron              VARCHAR(50) not null 	/*定时表达式*/,
       executor_param    VARCHAR(500) 	/*执行参数*/,
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
EXEC sp_addextendedproperty 'MS_Description', '执行器 根据需要进行扩展', 'user', dbo, 'table', aix_schedule_task_info, 'column', executor;
EXEC sp_addextendedproperty 'MS_Description', '状态 0=禁用 1=启动', 'user', dbo, 'table', aix_schedule_task_info, 'column', status;
EXEC sp_addextendedproperty 'MS_Description', '任务名称', 'user', dbo, 'table', aix_schedule_task_info, 'column', task_name;
EXEC sp_addextendedproperty 'MS_Description', '任务描述', 'user', dbo, 'table', aix_schedule_task_info, 'column', task_desc;
EXEC sp_addextendedproperty 'MS_Description', '定时表达式', 'user', dbo, 'table', aix_schedule_task_info, 'column', cron;
EXEC sp_addextendedproperty 'MS_Description', '执行参数', 'user', dbo, 'table', aix_schedule_task_info, 'column', executor_param;
EXEC sp_addextendedproperty 'MS_Description', '上次执行时间', 'user', dbo, 'table', aix_schedule_task_info, 'column', last_execute_time;
EXEC sp_addextendedproperty 'MS_Description', '下次执行时间', 'user', dbo, 'table', aix_schedule_task_info, 'column', next_execute_time;
EXEC sp_addextendedproperty 'MS_Description', '最大重试次数 0=不重试', 'user', dbo, 'table', aix_schedule_task_info, 'column', max_retry_count;
EXEC sp_addextendedproperty 'MS_Description', '创建人编号', 'user', dbo, 'table', aix_schedule_task_info, 'column', creator_id;
EXEC sp_addextendedproperty 'MS_Description', '创建日期', 'user', dbo, 'table', aix_schedule_task_info, 'column', create_time;
EXEC sp_addextendedproperty 'MS_Description', '修改人编号', 'user', dbo, 'table', aix_schedule_task_info, 'column', modifier_id;
EXEC sp_addextendedproperty 'MS_Description', '修改日期', 'user', dbo, 'table', aix_schedule_task_info, 'column', modify_time;
