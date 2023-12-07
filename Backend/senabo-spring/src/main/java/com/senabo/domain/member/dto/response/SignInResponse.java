package com.senabo.domain.member.dto.response;

import com.senabo.config.security.jwt.TokenInfo;
import com.senabo.domain.member.entity.Member;
import com.senabo.domain.member.entity.Sex;
import com.senabo.domain.member.entity.Species;
import lombok.Builder;
import lombok.Getter;

import java.math.BigDecimal;
import java.time.LocalDateTime;

@Builder
public record SignInResponse(
        Long id,
        String dogName,
        String email,
        Species species,
        Sex sex,
        BigDecimal houseLatitude,
        BigDecimal houseLongitude,
        Integer affection,
        Integer stressLevel,
        Integer totalTime,
        LocalDateTime exitTime,
        LocalDateTime enterTime,
        LocalDateTime createTime,
        LocalDateTime updateTime,
        TokenInfo token,
        boolean isMember
) {
    public static SignInResponse emptyMember (boolean isMember){
        return SignInResponse.builder()
                .id(null)
                .dogName(null)
                .email(null)
                .species(null)
                .sex(null)
                .houseLatitude(null)
                .houseLongitude(null)
                .affection(null)
                .stressLevel(null)
                .totalTime(null)
                .exitTime(null)
                .enterTime(null)
                .createTime(null)
                .updateTime(null)
                .token(null)
                .isMember(isMember)
                .build();
    }
    public static SignInResponse from(Member member, TokenInfo tokenInfo, boolean isMember){
        return SignInResponse.builder()
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
                .isMember(isMember)
                .build();
    }
}