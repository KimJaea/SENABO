package com.senabo.domain.feed.dto.response;

import com.senabo.domain.feed.entity.Feed;
import lombok.Builder;

import java.time.LocalDateTime;

@Builder
public record FeedResponse(
        Long id,
        Long memberId,
        Boolean cleanYn,
        LocalDateTime createTime,
        LocalDateTime updateTime

) {
    public static FeedResponse from(Feed feed){
        return FeedResponse.builder()
                .id(feed.getId())
                .memberId(feed.getMemberId().getId())
                .cleanYn(feed.getCleanYn())
                .createTime(feed.getCreateTime())
                .updateTime(feed.getUpdateTime())
                .build();
    }
}
