package com.senabo.domain.report.dto.request;

import java.time.LocalDateTime;

public record UpdateTotalTimeRequest(
        LocalDateTime startTime,
        LocalDateTime endTime

) {
}
