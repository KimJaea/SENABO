package com.senabo.domain.member.dto.response;

import com.senabo.domain.member.entity.Member;
import com.senabo.domain.member.entity.Sex;
import com.senabo.domain.member.entity.Species;
import lombok.Builder;

import java.math.BigDecimal;
import java.time.LocalDateTime;

@Builder
public record MemberResponse(
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
        String deviceToken,
        LocalDateTime exitTime,
        LocalDateTime enterTime,
        LocalDateTime createTime,
        LocalDateTime updateTime
) {
    public static MemberResponse from(Member member) {
        return MemberResponse.builder()
                .id(member.getId())
                .dogName(member.getDogName())
                .email(member.getEmail())
                .species(member.getSpecies())
                .sex(member.getSex())
                .houseLatitude(member.getHouseLatitude())
                .houseLongitude(member.getHouseLongitude())
                .deviceToken(member.getDeviceToken())
                .affection(member.getAffection())
                .stressLevel(member.getStressLevel())
                .totalTime(member.getTotalTime())
                .exitTime(member.getExitTime())
                .enterTime(member.getEnterTime())
                .createTime(member.getCreateTime())
                .updateTime(member.getUpdateTime())
                .build();
    }
}
