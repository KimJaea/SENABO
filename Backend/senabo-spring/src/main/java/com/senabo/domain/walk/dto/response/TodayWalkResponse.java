package com.senabo.domain.walk.dto.response;

import lombok.Builder;

@Builder
public record TodayWalkResponse(
        int todayTotalWalkTime,
        double todayTotalWalkDistance
) {
    public static TodayWalkResponse from (int todayTotalWalkTime, double todayTotalWalkDistance){
        return TodayWalkResponse.builder()
                .todayTotalWalkTime(todayTotalWalkTime)
                .todayTotalWalkDistance(todayTotalWalkDistance)
                .build();
    }
}