package com.senabo.domain.report.dto.response;

import com.senabo.domain.report.entity.Report;

public record SimpleReportResponse(
        Long id,
        int week,
        int endStressScore
) {
    public static SimpleReportResponse from(Report report) {
        return new SimpleReportResponse(
                report.getId(),
                report.getWeek(),
                report.getEndStressScore()
        );
    }
}