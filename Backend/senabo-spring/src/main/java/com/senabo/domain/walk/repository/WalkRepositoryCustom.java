package com.senabo.domain.walk.repository;

import com.senabo.domain.member.entity.Member;
import com.senabo.domain.walk.entity.Walk;

import java.time.LocalDateTime;
import java.util.List;
import java.util.Optional;

public interface WalkRepositoryCustom{
    List<Walk> findByMemberId(Member member);
    Optional<Walk> findLatestData(Member member);
    Optional<Walk> findTodayDataLatest(Member member, LocalDateTime todayStartTime);
    List<Walk> findTodayData(Member member, LocalDateTime startToday);
    List<Walk> findWalkWeek(Member member, LocalDateTime endTime, LocalDateTime startTime);
}
