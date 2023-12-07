package com.senabo.domain.communication.dto.response;

import com.senabo.common.ActivityType;
import com.senabo.domain.communication.entity.Communication;
import lombok.Builder;

import java.time.LocalDateTime;
@Builder
public record CommunicationResponse(
        Long id,
        Long memberId,
        ActivityType type,
        LocalDateTime createTime,
        LocalDateTime updateTime
) {
    public static CommunicationResponse from(Communication communication) {
        return CommunicationResponse.builder()
                .id(communication.getId())
                .memberId(communication.getMemberId().getId())
                .type(communication.getType())
                .createTime(communication.getCreateTime())
                .updateTime(communication.getUpdateTime())
                .build();
    }
}
