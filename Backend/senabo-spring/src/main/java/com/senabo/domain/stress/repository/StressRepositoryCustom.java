package com.senabo.domain.stress.repository;

import com.senabo.domain.member.entity.Member;
import com.senabo.domain.stress.entity.Stress;
import com.senabo.domain.stress.entity.StressType;
import org.springframework.data.jpa.repository.Query;

import java.time.LocalDateTime;
import java.util.List;
import java.util.Optional;


public interface StressRepositoryCustom  {
    List<Stress> findByMemberId(Member member);

    List<Stress> findLastWeekData(Member member, LocalDateTime lastStart, StressType type);

    Long countLastWeekData(Member member, LocalDateTime lastStart, StressType type);

    Optional<Stress> findLatestData(Member member);

    List<Stress> findStressWeek(Member member, LocalDateTime endTime, LocalDateTime startTime);
}
