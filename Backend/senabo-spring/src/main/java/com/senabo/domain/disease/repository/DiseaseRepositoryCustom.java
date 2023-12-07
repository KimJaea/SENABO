package com.senabo.domain.disease.repository;

import com.senabo.domain.disease.entity.Disease;
import com.senabo.domain.member.entity.Member;

import java.time.LocalDateTime;
import java.util.List;


public interface DiseaseRepositoryCustom {
    List<Disease> findByMemberId(Member memberId);
    List<Disease> findLastWeekData(Member memberId, LocalDateTime lastStart);
    Long countLastWeekData(Member memberId, LocalDateTime lastStart);
    List<Disease> findDiseaseWeek(Member member, LocalDateTime endTime, LocalDateTime startTime);
}
