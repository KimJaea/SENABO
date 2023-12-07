package com.senabo.domain.member.dto.request;

import com.senabo.domain.member.entity.Sex;
import com.senabo.domain.member.entity.Species;
import io.swagger.v3.oas.annotations.media.Schema;
import jakarta.validation.constraints.NotNull;

import java.math.BigDecimal;

public record UpdateLocateRequest(

        @Schema(description = "위도", nullable = true, example = "0")
        BigDecimal houseLatitude,
        @Schema(description = "경도", nullable = true, example = "0")
        BigDecimal houseLongitude
) {

}