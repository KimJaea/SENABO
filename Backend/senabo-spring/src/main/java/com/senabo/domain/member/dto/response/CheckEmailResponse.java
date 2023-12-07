package com.senabo.domain.member.dto.response;

import lombok.Builder;

@Builder
public record CheckEmailResponse(
        boolean duplicateYn
) {
    public static CheckEmailResponse from(boolean duplicateYn){
        return CheckEmailResponse.builder()
                .duplicateYn(duplicateYn)
                .build();
    }
}
