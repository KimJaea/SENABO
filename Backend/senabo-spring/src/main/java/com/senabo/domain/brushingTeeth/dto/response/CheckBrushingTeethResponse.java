package com.senabo.domain.brushingTeeth.dto.response;

import lombok.Builder;

@Builder
public record CheckBrushingTeethResponse(
        boolean possibleYn
) {
    public static CheckBrushingTeethResponse from(boolean possibleYn){
        return CheckBrushingTeethResponse.builder()
                .possibleYn(possibleYn)
                .build();
    }
}
