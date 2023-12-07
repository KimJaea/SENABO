package com.senabo.domain.walk.dto.response;

import com.senabo.domain.walk.entity.Walk;
import lombok.Builder;

import java.time.LocalDateTime;

@Builder
public record WalkResponse(
        Long id,
        Long memberId,
        Double distance,
        LocalDateTime startTime,
        LocalDateTime endTime,
        LocalDateTime createTime,
        LocalDateTime updateTime
) {
    public static WalkResponse from(Walk walk){
        return WalkResponse.builder()
                .id(walk.getId())
                .memberId(walk.getMemberId().getId())
                .startTime(walk.getStartTime())
                .endTime(walk.getEndTime())
                .distance(walk.getDistance())
                .createTime(walk.getCreateTime())
                .updateTime(walk.getUpdateTime())
                .build();
    }
}
