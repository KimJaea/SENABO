package com.senabo.domain.member.service;


import com.senabo.exception.message.ExceptionMessage;
import com.senabo.exception.model.TokenNotFoundException;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.data.redis.core.StringRedisTemplate;
import org.springframework.stereotype.Service;

@Slf4j
@Service
@RequiredArgsConstructor
public class RefreshTokenService {
    private final StringRedisTemplate stringRedisTemplate;

    public void setValues(String token, String email) {
        try {
            stringRedisTemplate.opsForValue().set(token, email);
        } catch (Exception e) {
            log.info("Redis 서버 오류");
            throw new IllegalArgumentException();
        }
    }

    public String getValues(String token) {
        try {
            return stringRedisTemplate.opsForValue().get(token);
        } catch (RuntimeException e) {
            log.info("Redis get error");
            throw new TokenNotFoundException(ExceptionMessage.TOKEN_NOT_FOUND);
        }
    }

    public void delValues(String token) {
        try {
            stringRedisTemplate.delete(token);
        } catch (RuntimeException e) {
            log.info("Redis delete error");
            throw new TokenNotFoundException(ExceptionMessage.TOKEN_NOT_FOUND);
        }
    }
}
