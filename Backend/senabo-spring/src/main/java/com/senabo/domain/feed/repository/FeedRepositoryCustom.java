package com.senabo.domain.feed.repository;

import com.senabo.domain.feed.entity.Feed;
import com.senabo.domain.member.entity.Member;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;
import org.springframework.stereotype.Repository;

import java.time.LocalDateTime;
import java.util.List;
import java.util.Optional;


public interface FeedRepositoryCustom {
    List<Feed> findByMemberId(Member member);
    Optional<Feed> findLatestData(Member member);
    List<Feed> findFeedWeek(Member member, LocalDateTime endTime, LocalDateTime startTime);
}
