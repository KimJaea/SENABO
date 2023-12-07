package com.senabo.domain.affection.dto.response;

import com.senabo.common.ActivityType;
import com.senabo.domain.affection.entity.Affection;
import lombok.Builder;

import java.time.LocalDateTime;

@Builder
public record AffectionResponse(
        Long id,
        Long memberId,
        ActivityType type,
        int changeAmount,
        int score,
        LocalDateTime createTime,
        LocalDateTime updateTime
) {
    public static AffectionResponse from(Affection affection){
        return AffectionResponse.builder()
                .id(affection.getId())
                .memberId(affection.getMemberId().getId())
                .type(affection.getType())
                .changeAmount(affection.getChangeAmount())
                .score(affection.getScore())
                .createTime(affection.getCreateTime())
                .updateTime(affection.getUpdateTime())
                .build();
    }
}

