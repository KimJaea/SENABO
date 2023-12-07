package com.senabo.domain.member.dto.request;

import com.senabo.domain.member.entity.Sex;
import com.senabo.domain.member.entity.Species;
import io.swagger.v3.oas.annotations.media.Schema;
import jakarta.validation.constraints.NotNull;

import java.math.BigDecimal;

public record UpdateDeviceTokenRequest(
        @Schema(description = "기기 토큰")
        String deviceToken
) {

}