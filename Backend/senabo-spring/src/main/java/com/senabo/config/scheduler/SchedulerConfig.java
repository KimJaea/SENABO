package com.senabo.config.scheduler;

import lombok.extern.slf4j.Slf4j;
import org.springframework.context.annotation.Configuration;
import org.springframework.scheduling.annotation.SchedulingConfigurer;
import org.springframework.scheduling.concurrent.ThreadPoolTaskScheduler;
import org.springframework.scheduling.config.ScheduledTaskRegistrar;

@Slf4j
@Configuration
public class SchedulerConfig implements SchedulingConfigurer {

    @Override
    public void configureTasks(ScheduledTaskRegistrar scheduledTaskRegistrar){
        ThreadPoolTaskScheduler threadPoolTaskScheduler = new ThreadPoolTaskScheduler();

                int poolSize = Runtime.getRuntime().availableProcessors();
                threadPoolTaskScheduler.setPoolSize(poolSize);
                threadPoolTaskScheduler.setThreadNamePrefix("scheduled-task-pool");
                threadPoolTaskScheduler.initialize();

                scheduledTaskRegistrar.setTaskScheduler(threadPoolTaskScheduler);
                log.info("\n");
                log.info("======================================");
                log.info("current thread : " + Thread.currentThread().getName());
                log.info("======================================");
                log.info("\n");

    }

}
