package com.senabo.domain.member.dto.response;

import com.senabo.config.security.jwt.TokenInfo;
import com.senabo.domain.member.entity.Member;
import com.senabo.domain.member.entity.Sex;
import com.senabo.domain.member.entity.Species;
import lombok.Builder;

import java.math.BigDecimal;
import java.time.LocalDateTime;

@Builder
public record SignUpResponse(
        Long id,
        String dogName,
        String email,
        Species species,
        Sex sex,
        int affection,
        int stressLevel,
        BigDecimal houseLatitude,
        BigDecimal houseLongitude,
        int totalTime,
        LocalDateTime exitTime,
        LocalDateTime enterTime,
        LocalDateTime createTime,
        LocalDateTime updateTime,
        TokenInfo token
) {
    public static SignUpResponse from(Member member, TokenInfo tokenInfo){
        return SignUpResponse.builder()
                .id(member.getId())
                .dogName(member.getDogName())
                .email(member.getEmail())
                .species(member.getSpecies())
                .sex(member.getSex())
                .houseLatitude(member.getHouseLatitude())
                .houseLongitude(member.getHouseLongitude())
                .affection(member.getAffection())
                .stressLevel(member.getStressLevel())
                .totalTime(member.getTotalTime())
                .exitTime(member.getExitTime())
                .enterTime(member.getEnterTime())
                .createTime(member.getCreateTime())
                .updateTime(member.getUpdateTime())
                .token(tokenInfo)
                .build();
    }
}
