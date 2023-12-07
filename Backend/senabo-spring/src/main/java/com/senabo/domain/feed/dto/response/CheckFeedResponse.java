package com.senabo.domain.feed.dto.response;

import com.senabo.domain.feed.entity.Feed;
import lombok.Builder;

import java.time.LocalDateTime;

@Builder
public record CheckFeedResponse(
        boolean possibleYn,
        LocalDateTime lastFeedDateTime,
        LocalDateTime nowDateTime
) {
    public static CheckFeedResponse from(boolean possibleYn, LocalDateTime lastFeedDateTime, LocalDateTime nowDateTime){
        return CheckFeedResponse.builder()
                .possibleYn(possibleYn)
                .lastFeedDateTime(lastFeedDateTime)
                .nowDateTime(nowDateTime)
                .build();
    }
}
