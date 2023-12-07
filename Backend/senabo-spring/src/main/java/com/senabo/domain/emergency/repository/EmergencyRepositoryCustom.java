package com.senabo.domain.emergency.repository;

import com.senabo.domain.emergency.entity.Emergency;
import com.senabo.domain.emergency.entity.EmergencyType;
import com.senabo.domain.member.entity.Member;

import java.time.LocalDateTime;
import java.util.List;
import java.util.Optional;

public interface EmergencyRepositoryCustom {
    Optional<Emergency> findLatestEmergency(Member member);
    List<Emergency> findLastWeekEmergency(Member member);
    List<Emergency> findByTypeToday(Member member, EmergencyType type);
    Optional<Emergency> findUnsolvedEmergency(Member member, EmergencyType type);
}
