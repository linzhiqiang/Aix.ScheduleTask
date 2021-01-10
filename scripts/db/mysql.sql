create table  `aix_distribution_lock`
(
       `lock_name`       VARCHAR(50) not null comment '主键'
) comment '分布式锁';
alter  table `aix_distribution_lock`
       add constraint `PK_aix_disock_lock_nameE2BC` primary key (`lock_name`);


       insert into aix_distribution_lock(lock_name) values('AixScheduleTaskLock');


create table  `aix_schedule_task_info`
(
       `id`              INT auto_increment primary key not null comment '主键',
       `task_group`      VARCHAR(50) comment '所属组 根据需要进行扩展',
       `status`          TINYINT default 0 not null comment '状态 0=禁用 1=启动',
       `task_name`       VARCHAR(50) not null comment '任务名称',
       `task_desc`       VARCHAR(200) comment '任务描述',
       `cron`            VARCHAR(50) not null comment '定时表达式',
       `task_content`    VARCHAR(500) not null comment '内容',
       `last_execute_time` BIGINT default 0 not null comment '上次执行时间',
       `next_execute_time` BIGINT default 0 not null comment '下次执行时间',
       `max_retry_count` INT default 0 not null comment '最大重试次数 0=不重试',
       `creator_id`      VARCHAR(50) not null comment '创建人编号',
       `create_time`     DATETIME default now() not null comment '创建日期',
       `modifier_id`     VARCHAR(50) not null comment '修改人编号',
       `modify_time`     DATETIME default now() not null comment '修改日期'
) comment '定时任务';