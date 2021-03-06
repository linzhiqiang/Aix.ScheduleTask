create table  `aix_distribution_lock`
(
       `lock_name`       VARCHAR(50) not null comment '主键'
) comment '分布式锁';
alter  table `aix_distribution_lock`
       add constraint `PK_aix_disock_lock_nameE2BC` primary key (`lock_name`);


       insert into aix_distribution_lock(lock_name) values('AixScheduleTaskLock');


create table  aix_schedule_task_info
(
       id                INT primary key auto_increment not null comment '主键',
       task_group        VARCHAR(50) comment '所属组 根据需要进行扩展',
       task_status       INT default 0 not null comment '状态 0=禁用 1=启动',
       task_name         VARCHAR(50) not null comment '任务名称',
       task_desc         VARCHAR(200) comment '任务描述',
       cron              VARCHAR(50) not null comment '定时表达式',
       task_content      VARCHAR(500) not null comment '内容',
       last_execute_time BIGINT default 0 not null comment '上次执行时间',
       next_execute_time BIGINT default 0 not null comment '下次执行时间',
       max_retry_count   INT default 0 not null comment '最大重试次数 0=不重试',
       creator_id        VARCHAR(50) not null comment '创建人编号',
       create_time       TIMESTAMP default current_timestamp not null comment '创建日期',
       modifier_id       VARCHAR(50) not null comment '修改人编号',
       modify_time       TIMESTAMP default current_timestamp not null comment '修改日期'
) comment '定时任务';

create table  aix_schedule_task_log
(
       id                INT primary key auto_increment not null comment '主键',
       schedule_task_id  INT not null comment '定时任务id',
       retry_count       INT default 0 not null comment '重试次数',
       trigger_code      INT default 0 not null comment '调度code 0=初始化  2=执行成功 9=执行失败  ',
       trigger_message   VARCHAR(500) comment '调度信息',
       trigger_time      DATETIME comment '调度时间',
       result_code       INT not null comment '结果code 0=初始化  2=执行成功 9=执行失败  ',
       result_message    VARCHAR(500) comment '结果信息',
       result_time       DATETIME comment '结果时间',
       alarm_status      INT default 0 not null comment '告警状态 告警状态：0-默认、-1-锁定状态、1-无需告警、2-告警成功、9-告警失败',
       create_time       TIMESTAMP default current_timestamp not null comment '创建日期',
       modify_time       TIMESTAMP default current_timestamp not null comment '修改日期'
) comment '定时任务log';