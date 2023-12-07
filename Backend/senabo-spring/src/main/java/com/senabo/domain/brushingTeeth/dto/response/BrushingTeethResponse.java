package com.senabo.domain.brushingTeeth.dto.response;

import com.senabo.domain.brushingTeeth.entity.BrushingTeeth;
import lombok.Builder;

import java.time.LocalDateTime;

@Builder
public record BrushingTeethResponse(
        Long id,
        Long memberId,
        LocalDateTime createTime,
        LocalDateTime updateTime
) {
    public static BrushingTeethResponse from(BrushingTeeth brushingTeeth) {
        return BrushingTeethResponse.builder()
                .id(brushingTeeth.getId())
                .memberId(brushingTeeth.getMemberId().getId())
                .createTime(brushingTeeth.getCreateTime())
                .updateTime(brushingTeeth.getUpdateTime())
                .build();
    }

}
