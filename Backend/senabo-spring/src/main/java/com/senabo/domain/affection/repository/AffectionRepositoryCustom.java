package com.senabo.domain.affection.repository;

import com.senabo.domain.affection.dto.response.AffectionResponse;
import com.senabo.domain.affection.entity.Affection;
import com.senabo.domain.member.entity.Member;

import java.time.LocalDateTime;
import java.util.List;
import java.util.Optional;

public interface AffectionRepositoryCustom {
    List<Affection> findByMemberId(Member memberId);
    Optional<Affection> findLatestDataByMemberId(Member member);
    List<Affection> findAffectionWeek(Member member, LocalDateTime endTime, LocalDateTime startTime);

}
