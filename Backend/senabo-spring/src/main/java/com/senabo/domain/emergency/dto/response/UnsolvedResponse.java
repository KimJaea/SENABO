package com.senabo.domain.emergency.dto.response;

import com.senabo.domain.emergency.entity.Emergency;
import com.senabo.domain.emergency.entity.EmergencyType;
import lombok.Builder;

import java.time.LocalDateTime;

@Builder
public record UnsolvedResponse(
        Long id,
        Long memberId,
        EmergencyType type,
        boolean solved,
        LocalDateTime createTime,
        LocalDateTime updateTime
) {
    public static UnsolvedResponse from(Emergency emergency) {
        return UnsolvedResponse.builder()
                .id(emergency.getId())
                .memberId(emergency.getMemberId().getId())
                .type(emergency.getType())
                .solved(emergency.isSolved())
                .createTime(emergency.getCreateTime())
                .updateTime(emergency.getUpdateTime())
                .build();
    }
}
