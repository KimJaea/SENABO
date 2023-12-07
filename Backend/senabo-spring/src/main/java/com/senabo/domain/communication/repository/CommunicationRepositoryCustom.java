package com.senabo.domain.communication.repository;

import com.senabo.domain.communication.entity.Communication;
import com.senabo.domain.member.entity.Member;

import java.time.LocalDateTime;
import java.util.List;



public interface CommunicationRepositoryCustom {
    List<Communication> findByMemberId(Member memberId);

    List<Communication> findCommunicationWeek(Member member, LocalDateTime endTime, LocalDateTime startTime);
    Long countCommunicationWeek(Member member, LocalDateTime lastStart);
}
