package com.senabo.config.firebase;

import lombok.extern.slf4j.Slf4j;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;
import org.springframework.scheduling.annotation.EnableAsync;
import org.springframework.scheduling.concurrent.ThreadPoolTaskExecutor;

@Slf4j
@Configuration
@EnableAsync
public class AsyncConfig {

    @Bean(name = "fcmTaskExecutor")
    public ThreadPoolTaskExecutor fcmTaskExecutor() {
        ThreadPoolTaskExecutor executor = new ThreadPoolTaskExecutor();
        int poolSize = Runtime.getRuntime().availableProcessors();
        log.info("======================================");
        log.info("processors count: {}", poolSize);
        log.info("======================================");
        executor.setThreadNamePrefix("FCM-Executor-"); // Thread 이름 설정
        executor.setCorePoolSize(poolSize); // 기본 스레드 수
        executor.setMaxPoolSize(poolSize * 2); // 최대 스레드 수
        executor.setQueueCapacity(50); // 최대 큐 수
        executor.setKeepAliveSeconds(60); // maxPoolSize로 인해 덤으로 더 돌아다니는 튜브는 60초 후에 수거해서 정리
        executor.initialize(); // 초기화 후 반환
        return executor;
    }
}
